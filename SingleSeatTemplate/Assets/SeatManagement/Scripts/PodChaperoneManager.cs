using Artanim.Location.Data;
using Artanim.Location.Network;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Artanim.SeatManagement
{
    public class PodChaperoneManager : MonoBehaviour
    {
        public PhysicalSeatLayout _physicalSeatLayout;
        public bool _verbose = false;

        private Dictionary<Guid, AvatarChaperone> playerAvatarChaperones = new Dictionary<Guid, AvatarChaperone>();
        private bool _isServer;
        private Transform _currentPlayerAvatarOffsetTransform;

        public void SetPodId(string podId)
        {
            if (_physicalSeatLayout != null)
            {
                _physicalSeatLayout._podId = podId;
                _physicalSeatLayout.UpdatePhysicalSeatUniqueIdentifier(podId);
            }
            else
                Debug.LogError("No PhysicalSeatLayout on " + name);
        }

        public void OnPlayerAdded(RuntimePlayer player, Transform playerAvatarOffsetTransform)
        {
            if (player.AvatarController)
            {
                if (player.IsMainPlayer) // This flag can only be true when running on a client
                {
                    if (_verbose) Debug.Log("[" + Time.frameCount + "] - Instantiate Pod Chaperone [Client].");
                    //SeatManager.Instance._podSeatLayout = InstantiatePodChaperone(_podSeatLayoutPrefab, playerAvatarOffsetTransform);
                    _currentPlayerAvatarOffsetTransform = playerAvatarOffsetTransform;
                }

                //Add an avatar chaperone, unless we are a Standalone or AutoSessionJoin player
                if (!player.IsDesktopAvatar && DevelopmentMode.CurrentMode != EDevelopmentMode.Standalone)
                { 
                    var avatarChaperoneController = player.AvatarController.GetComponent<AvatarChaperoneController>();
                    if (avatarChaperoneController)
                    {
                        // If we are the server, instantiate directly under the ChaperonManager transform, otherwise instantiate under our own avatarOffset
                        AvatarChaperone avatarChaperone = null;

                        if (NetworkInterface.Instance.IsServer)
                        {
                            if (_verbose) Debug.Log("[" + Time.frameCount + "] - Instantiate avatar chaperone for player: " + player.AvatarController.PlayerId + " [Server]");
                            avatarChaperone = avatarChaperoneController.InstantiateAvatarChaperone(transform);
                        }
                        else
                        {
                            if (_currentPlayerAvatarOffsetTransform != null)
                            {
                                if (_verbose) Debug.Log("[" + Time.frameCount + "] - Instantiate avatar chaperone for player: " + player.AvatarController.PlayerId + " [Client]");
                                avatarChaperone = avatarChaperoneController.InstantiateAvatarChaperone(_currentPlayerAvatarOffsetTransform);
                            }
                            else
                                Debug.LogError("ChaperoneManager:OnPlayerAdded " + player.AvatarController.PlayerId + " _currentPlayerAvatarOffsetTransform is null. ");
                        }

                        if (avatarChaperone != null)
                        {
                            if (playerAvatarChaperones.ContainsKey(player.AvatarController.PlayerId))
                                playerAvatarChaperones[player.AvatarController.PlayerId] = avatarChaperone;
                            else
                                playerAvatarChaperones.Add(player.AvatarController.PlayerId, avatarChaperone);
                        }
                        else 
                        {
                            Debug.LogError("Could not instantiate avatar chaperone for " + player.AvatarController.gameObject.name);
                        }
                    }
                    else
                    {
                        Debug.LogError("No avatar chaperone controller on " + player.AvatarController.gameObject.name);
                    }
                }
            }
            else
            {
                Debug.LogError("No avatar controller on " + player.AvatarController.gameObject.name);
            }
        }

        public PhysicalSeatLayout InstantiatePodChaperone(GameObject prefabToInstantiate, Transform parent)
        {
            _physicalSeatLayout = Instantiate(prefabToInstantiate, parent).GetComponent<PhysicalSeatLayout>();
            if (_physicalSeatLayout == null)
                Debug.LogError("Could not find a PhysicalSeatLayout in " + prefabToInstantiate.name);

            return _physicalSeatLayout;
        }

        public void SetAvatarChaperoneVisibility(bool visibility, Guid playerId)
        {
            if (playerAvatarChaperones.ContainsKey(playerId))
                playerAvatarChaperones[playerId].SetAvatarVisibility(visibility);
            else
                Debug.LogError("playerAvatarChaperones doesn't contain an entry for player: " + playerId);
        }

        public void SetChaperoneVisibility(bool visibility)
        {
            Color color = _isServer ? Color.red : Color.cyan;
            
            foreach (AvatarChaperone avatarChaperone in playerAvatarChaperones.Values)
            { 
                avatarChaperone.SetAvatarVisibility(visibility);
                avatarChaperone.SetAvatarColor(color);
            }

            _physicalSeatLayout.SetChaperoneVisibility(visibility);
            _physicalSeatLayout.SetSeatLayoutColor(color);
        }
    }
}
