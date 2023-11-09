using Artanim.Location.Network;
using Artanim.Location.Data;
using Artanim.Location.SharedData;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Artanim.SeatManagement
{
    public struct PlayerSeatMappingMessage
    {
        public Guid playerId;
        public string virtualSeatName;
        public string physicalSeatName;
        public string podId;
        public Vector3 physicalSeatPosition;
        public Quaternion physicalSeatRotation;
    }

    public class PlayerSeatMapping
    {
        public PhysicalSeat pSeat;
        public VirtualSeat vSeat;
        public AvatarOffset avatarOffset;
        public String podId;

        public PlayerSeatMapping()
        {
            pSeat = null;
            vSeat = null;
            avatarOffset = null;
            podId = null;
        }
    }

    public class SeatManager : SingletonBehaviour<SeatManager>
    {
        private const string SEAT_MAPPING = "SeatMapping";

        public List<GameObject> _podSeatLayoutPrefab = new List<GameObject>();
        public List<GameObject> _podSeatLayoutConstructPrefab = new List<GameObject>();

        public VirtualSeatLayout _currentSceneSeatLayouts;

        public Dictionary<string, PodChaperoneManager> _podChaperoneManagers = new Dictionary<string, PodChaperoneManager>();

        public Dictionary<Guid, PlayerSeatMapping> playerSeatMapping = new Dictionary<Guid, PlayerSeatMapping>();

        private bool _readyToAssignSeat = false;
        private bool _isServer;
        private string _podId;

        public bool _verbose = false;

        private Vector3 trackedposition = Vector3.zero;
        private Quaternion trackedrotation = Quaternion.identity;

        private void Start()
        {
            _isServer = NetworkInterface.Instance.IsServer;
            _podId = SharedDataUtils.GetMyComponent<LocationComponent>().PodId;
            if (_podId == "")
                Debug.LogError("The is no PodId defined for this component.");
        }

        void OnEnable()
        {
            GameController.Instance.OnSceneLoadedInSession += OnSceneLoadedInSession;
            GameController.Instance.OnJoinedSession += OnJoinedSession;
            GameController.Instance.OnLeftSession += OnLeftSession;
            GameController.Instance.OnSessionPlayerJoined += OnSessionPlayerJoined;
            GameController.Instance.OnSessionPlayerLeft += OnSessionPlayerLeft;

            GameSessionController.Instance.OnValueUpdated += OnGameSessionValueUpdated;
        }

        void OnDisable()
        {
            if (GameController.Instance)
            {
                GameController.Instance.OnSceneLoadedInSession -= OnSceneLoadedInSession;
                GameController.Instance.OnSessionPlayerJoined -= OnSessionPlayerJoined;
                GameController.Instance.OnSessionPlayerLeft -= OnSessionPlayerLeft;
                GameController.Instance.OnJoinedSession -= OnJoinedSession;
                GameController.Instance.OnLeftSession -= OnLeftSession;
            }

            if (GameSessionController.Instance)
                GameSessionController.Instance.OnValueUpdated -= OnGameSessionValueUpdated;
        }

        #region ServerSideCode
        void Update()
        {
            // If we are the server, check if a player has been assigned to a physical seat but not yet to a virtual seat
            if (_isServer && _readyToAssignSeat)
            {
                foreach (PlayerSeatMapping seatMapping in playerSeatMapping.Values)
                {
                    if (seatMapping.pSeat != null && seatMapping.vSeat == null)
                        TryAssignSeat(seatMapping);
                }
            }
        }

        private void TryAssignSeat(PlayerSeatMapping seatMapping)
        {
            if (seatMapping.pSeat == null) return;

            if (_verbose) Debug.Log("[" + Time.frameCount + "]Trying to assign virtual seat to player: " + seatMapping.pSeat._assignedPlayerId);
            if (_currentSceneSeatLayouts && GameController.Instance.CurrentSession.Status == Location.Data.ESessionStatus.Started)
            {
                foreach (VirtualSeat vSeat in _currentSceneSeatLayouts._availableSeats)
                {
                    if (vSeat._isAvailable)
                    {
                        if (_verbose) Debug.Log("[" + Time.frameCount + "] - Player [" + seatMapping.pSeat._assignedPlayerId + "] in physical seat [" + seatMapping.pSeat.gameObject.name + "] is assigned to virtual seat [" + vSeat.gameObject.name + "]");

                        // Assign seat
                        seatMapping.vSeat = vSeat;

                        // Register Avatar Offset
                        DoRegisterOffset(seatMapping.pSeat._assignedPlayerId, seatMapping.avatarOffset);

                        // Publish seat assignement to notify clients
                        PublishSeatAssignment(seatMapping.pSeat, seatMapping.vSeat);

                        break;
                    }
                }
            }
        }

        private void PublishSeatAssignment(PhysicalSeat pSeat, VirtualSeat vSeat)
        {
            Guid playerId = pSeat._assignedPlayerId;
            String gameSessionKey = SEAT_MAPPING + playerId.ToString();

            PlayerSeatMappingMessage seatAssignment = GameSessionController.Instance.GetValue<PlayerSeatMappingMessage>(gameSessionKey, new PlayerSeatMappingMessage { playerId = new Guid(), virtualSeatName = "" }); // Really needed?
            seatAssignment.playerId = pSeat._assignedPlayerId;
            seatAssignment.podId = pSeat._podId;
            seatAssignment.virtualSeatName = vSeat._uniqueIdentifier;
            seatAssignment.physicalSeatName = pSeat._uniqueIdentifier;
            seatAssignment.physicalSeatPosition = pSeat.transform.localPosition;
            seatAssignment.physicalSeatRotation = pSeat.transform.localRotation;

            if (_verbose) Debug.Log("[" + Time.frameCount + "] Sending new seat assignment: {" + seatAssignment.playerId + " - " + seatAssignment.virtualSeatName + " - " + seatAssignment.physicalSeatName + "}");

            // [NOTE] For custom values, the value has to be explicitly set when updated. Just updating a property will not work.
            GameSessionController.Instance.SetValue(gameSessionKey, seatAssignment);
        }

        private void DoRegisterOffset(Guid playerId, AvatarOffset avatarOffset)
        {
            if (avatarOffset)
                // To Do: check if playerId is still valid
                AvatarOffsetController.Instance.RegisterAvatarOffset(
                    playerId,
                    avatarOffset,
                    true,
                    AvatarOffsetController.ESyncMode.Unsynced);
        }

        private PhysicalSeat GetPhysicalSeatByUniqueIdentifierAndPodId(string uniqueIdentifier, string podId)
        {
            if (_podChaperoneManagers.ContainsKey(podId))
            {
                foreach (PhysicalSeat pSeat in _podChaperoneManagers[podId]._physicalSeatLayout._availableSeats)
                {
                    if (pSeat._uniqueIdentifier == uniqueIdentifier)
                        return pSeat;
                }

                Debug.LogError("Could not find Physical Seat: " + uniqueIdentifier);
            }
            else
            {
                Debug.LogError("Could not find a PodChaperoneManager for podId: " + podId);
            }

            return null;
        }
        #endregion

        #region ServerSideEvents
        public void OnPhysicalSeatAssigned(PhysicalSeat pSeat, Guid playerId)
        {
            if (_verbose) Debug.Log("[" + Time.frameCount + "] - " + pSeat.gameObject.name + " got assigned to player " + pSeat._assignedPlayerId);

            playerSeatMapping[playerId].pSeat = pSeat;
        }

        public void OnPhysicalSeatReleased(PhysicalSeat pSeat, Guid playerId)
        {
            if (_verbose) Debug.Log("[" + Time.frameCount + "] - Seat: " + pSeat.gameObject.name + " got released by player " + pSeat._assignedPlayerId);

            playerSeatMapping[playerId].pSeat = null;
        }

        public bool IsPlayerAssignedToPhysicalSeat(Guid playerId)
        {
            if (playerSeatMapping.ContainsKey(playerId))
            {
                if (playerSeatMapping[playerId].pSeat != null)
                    return true;
            }

            return false;
        }
        #endregion

        private void OnGameSessionValueUpdated(string key, object value, bool playerValue = false, bool isInitializing = false)
        {
            if (key.Contains(SEAT_MAPPING))
            {
                PlayerSeatMappingMessage seatAssignment = (PlayerSeatMappingMessage)value;
                if (seatAssignment.virtualSeatName != "")
                {
                    if (_verbose) Debug.Log("[" + Time.frameCount + "] - Received new seat assignment: {" + seatAssignment.playerId + " - " + seatAssignment.virtualSeatName + " - " + seatAssignment.physicalSeatName + "}");
                    OnSeatMappingChanged(seatAssignment);
                }

            }
        }

        public void OnSessionPlayerJoined(Session session, Guid playerId)
        {
            if (_verbose) Debug.Log("[" + Time.frameCount + "] - player: " + playerId + " joined the session");
            AddPlayerToSeatManager(playerId);
        }

        public void OnSessionPlayerLeft(Session session, Guid playerId)
        {
            if (_verbose) Debug.Log("[" + Time.frameCount + "] - player: " + playerId + " left the session");
            RemovePlayerFromSeatManager(playerId);
        }

        public void OnJoinedSession(Session session, Guid playerId)
        {
            if (_verbose) Debug.Log("[" + Time.frameCount + "] - component: " + playerId + " joined the session");

            // First add ourselves to the SeatManager
            if (NetworkInterface.Instance.IsClient) AddPlayerToSeatManager(playerId);

            // Then add all the other players that are already in the session
            foreach (RuntimePlayer player in GameController.Instance.RuntimePlayers)
            {
                if (player.AvatarController.PlayerId != playerId)
                    AddPlayerToSeatManager(player.AvatarController.PlayerId);
            }
        }

        public void OnLeftSession()
        {
            if (_verbose) Debug.Log("[" + Time.frameCount + "] - component left the session");
            playerSeatMapping.Clear(); // Maybe not needed as the whole object is going to be destroyed?
        }

        private void OnSceneLoadedInSession(string[] sceneNames, bool sceneloadTimedOut)
        {
            if (_verbose) Debug.Log("[" + Time.frameCount + "] - Scene loaded: " + sceneNames[0]); // per Steve, there should never be more than one scene name as this is no longer suppported
            _readyToAssignSeat = true;
        }

        private void OnSeatMappingChanged(PlayerSeatMappingMessage seatMappingMessage)
        {
            if (playerSeatMapping.ContainsKey(seatMappingMessage.playerId))
            {
                // Retrieve 
                PlayerSeatMapping seatMapping = playerSeatMapping[seatMappingMessage.playerId];

                // Update virtual seat
                VirtualSeat vSeat = GetVirtualSeatByUniqueIdentifier(seatMappingMessage.virtualSeatName);
                if (vSeat == null) Debug.LogError("[" + Time.frameCount + "] could not find Virtual Seat: " + seatMappingMessage.virtualSeatName);
                vSeat._isAvailable = false;

                seatMapping.vSeat = vSeat;

                // Update the physical seats only if we are the server or if this is a physical seat that belong to our pod
                if (_isServer || seatMappingMessage.podId == _podId)
                {
                    PhysicalSeat pSeat = GetPhysicalSeatByUniqueIdentifierAndPodId(seatMappingMessage.physicalSeatName, seatMapping.podId);
                    if (pSeat == null) Debug.LogError("[" + Time.frameCount + "] could not find Physical Seat: " + seatMappingMessage.physicalSeatName);
                    pSeat._isAvailable = false;
                    pSeat._assignedPlayerId = seatMappingMessage.playerId;

                    seatMapping.pSeat = pSeat;
                }

                // Compute transform required to align the physical seat on the virtual seat
                // ** Carefull, computations are in local coordinates. 
                // ** Both virtual and physical seat transforms should not have parent transform different from identity (with the exception of Global Mocap / Avatar Offset)
                Debug.LogError("here-----");               
                seatMapping.avatarOffset.transform.localRotation = vSeat.transform.localRotation * Quaternion.Inverse(seatMappingMessage.physicalSeatRotation);
                seatMapping.avatarOffset.transform.localPosition = vSeat.transform.localPosition - seatMapping.avatarOffset.transform.localRotation * seatMappingMessage.physicalSeatPosition;
            }
            else
            {
                Debug.LogError("There is not SeatMapping for player " + seatMappingMessage.playerId);
            }
        }

        public VirtualSeat GetVirtualSeatByUniqueIdentifier(string uniqueIdentifier)
        {
            foreach (VirtualSeat vSeat in _currentSceneSeatLayouts._availableSeats)
            {
                if (vSeat._uniqueIdentifier == uniqueIdentifier)
                    return vSeat;
            }

            Debug.LogError("Could not find Virtual Seat: " + uniqueIdentifier);
            return null;
        }

        // Return the first prefab that contains the podId in its name
        // NOTE: We can do better
        private GameObject GetPodChaperoneManagerPrefab(string podId)
        {
            foreach (GameObject go in _podSeatLayoutPrefab)
            {
                if (go.name.Contains(podId.ToUpper()) || go.name.Contains(podId.ToLower()))
                    return go;
            }

            if (_verbose) Debug.Log("[" + Time.frameCount + "] - Could not find a PodChaperoneManager prefab for " + podId);

            return null;
        }

        // Return the first prefab that contains the podId in its name
        // NOTE: We can do better
        private GameObject GetPodChaperoneConstructPrefab(string podId)
        {
            if (podId == "")
                return null;

            foreach (GameObject go in _podSeatLayoutConstructPrefab)
            {
                if (go.name.Contains(podId.ToUpper()) || go.name.Contains(podId.ToLower()))
                    return go;
            }

            if (_verbose) Debug.Log("[" + Time.frameCount + "] - Could not find a PodConstruct prefab for " + podId);

            return null;
        }

        public void ResetSeatAssignments()
        {
            Debug.Log("Restting Seat Assignments");
            foreach (PlayerSeatMapping seatMapping in playerSeatMapping.Values)
            {
                seatMapping.vSeat = null;
            }

            _readyToAssignSeat = false;
        }

        public void AddPlayerToSeatManager(Guid playerId)
        {
            if (_verbose) Debug.Log("[" + Time.frameCount + "] - adding player: " + playerId.ToString() + " to SeatManager.");

            var playerComponent = SharedDataUtils.FindLocationComponent<LocationComponentWithSession>(playerId);
            if (playerComponent != null)
            {
                // Player should not have a seat mapping assigned yet, so assign one
                if (!playerSeatMapping.ContainsKey(playerId))
                    playerSeatMapping.Add(playerId, new PlayerSeatMapping());
                else Debug.LogError(playerId + " has already a seat mapping");

                // Add an avatar offset for the player to the playerSeatMapping
                GameObject playerAvatarOffset = new GameObject("AvatarOffset-" + playerId.ToString());
                playerAvatarOffset.transform.parent = transform;

                AvatarOffset avatarOffset = playerAvatarOffset.AddComponent<AvatarOffset>();
                avatarOffset.ObjectId = "AvatarOffset-" + playerId.ToString();

                playerSeatMapping[playerId].avatarOffset = avatarOffset;
                playerSeatMapping[playerId].podId = playerComponent.PodId;

                // If there is already a PodChaperoneManager with this PodId
                if (_podChaperoneManagers.ContainsKey(playerComponent.PodId))
                {
                    if (_verbose) Debug.Log("[" + Time.frameCount + "] - A pod chaperone manager already exist for " + playerComponent.PodId);

                    // add this player to the PodChaperoneManager
                    _podChaperoneManagers[playerComponent.PodId].OnPlayerAdded(GameController.Instance.GetPlayerByPlayerId(playerId), playerAvatarOffset.transform);
                }
                // otherwise,  if we are the server or if this player's PodId is the same as our PodId, 
                else if (_isServer || _podId == playerComponent.PodId)
                {
                    // instantiate the releavant PodChaperoneManager
                    GameObject podChaperoneManagerPrefab = GetPodChaperoneManagerPrefab(playerComponent.PodId);
                    if (podChaperoneManagerPrefab != null)
                    {
                        if (_verbose) Debug.Log("[" + Time.frameCount + "] - A pod chaperone manager doesn't exist for " + playerComponent.PodId + " - Instantiating " + podChaperoneManagerPrefab.name);

                        // If we are the server instantiate directly under the Chaperone Manager, if we are a client instantiate under our avatar offset
                        Transform parent = _isServer ? transform : playerAvatarOffset.transform;
                        PodChaperoneManager PodChaperoneManager = Instantiate(podChaperoneManagerPrefab, parent).GetComponent<PodChaperoneManager>();
                        if (PodChaperoneManager != null)
                        {
                            PodChaperoneManager.SetPodId(playerComponent.PodId);

                            _podChaperoneManagers.Add(playerComponent.PodId, PodChaperoneManager);

                            // and add this player to this PodChaperoneManager
                            _podChaperoneManagers[playerComponent.PodId].OnPlayerAdded(GameController.Instance.GetPlayerByPlayerId(playerId), playerAvatarOffset.transform);
                        }
                        else
                            Debug.LogError("Could not find a PodChaperoneManager in " + podChaperoneManagerPrefab.name);
                    }
                }
                else
                {
                    // We are a client and this player doesn't belong to our Pod. Do not add him to a PodChaperoneManager
                    if (_verbose) Debug.Log("[" + Time.frameCount + "] - No need to add player: " + playerId + " to a PodChaperoneManager since he doesn't belong to our Pod");
                }

                // If we are the main player, instantiate the relevant construct prefab
                if (!_isServer)
                {
                    RuntimePlayer player = GameController.Instance.GetPlayerByPlayerId(playerId);
                    if (player != null)
                    {
                        if (player.IsMainPlayer)
                        {
                            GameObject podConstruct = GetPodChaperoneConstructPrefab(playerComponent.PodId);
                            if (podConstruct != null)
                            {
                                if (_verbose) Debug.Log("[" + Time.frameCount + "] - Instantiating the following Pod Construct: " + podConstruct.name);
                                Instantiate(podConstruct);
                            }
                        }
                    }
                    else
                    {
                        Debug.LogError("Could not find a runtime player with playerId: " + playerId);
                    }
                }
            }
            else
                Debug.LogError("Could not find a component for player: " + playerId);
        }

        public void RemovePlayerFromSeatManager(Guid playerId)
        {
            // Remove player's avatar offset
            Destroy(playerSeatMapping[playerId].avatarOffset.gameObject);

            // Remove player from the playerSeatMapping object
            if (playerSeatMapping.ContainsKey(playerId))
                playerSeatMapping.Remove(playerId);
            else
                Debug.LogError("Could not remove player seat mapping for player " + playerId.ToString());
        }

        private void OnDestroy()
        {
            if (_verbose) Debug.Log("[" + Time.frameCount + "] - OnDestroy called on " + name);
        }
    }
}
