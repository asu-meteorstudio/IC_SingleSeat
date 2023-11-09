using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Artanim;
using Artanim.Location.Network;
using System;
using UnityEngine.SceneManagement;

namespace Artanim.SeatManagement
{
    public class GroupManager : ClientSideBehaviour
    {
        public VirtualSeatLayout VSlayout;
        public Group group1;
        public Group group2;
        private const string SEAT_MAPPING = "SeatMapping";
        private int players;
        private int playerscount;
        public GameObject[] Buttons;
        public GameObject BluRails;

        [Header("Options")]
        public bool GroupsEnabled;

        private GROUP m_group = GROUP.NONE;
        private enum GROUP { GROUP_1, GROUP_2, NONE, PROFESSOR }
        public void OnEnable()
        {
            if(NetworkInterface.Instance.IsClient)
            {
                if (!VSlayout)
                {
                    VSlayout = this.GetComponentInChildren<VirtualSeatLayout>();
                }

                players = GameController.Instance.RuntimePlayers.Count;
                playerscount = 0;

                GameSessionController.Instance.OnValueUpdated += OnGameSessionValueUpdated;

                if (!GroupsEnabled && GameController.Instance.CurrentPlayer.Player.Avatar.Contains("prof"))
                {
                    foreach (GameObject go in Buttons)
                    {
                        go.SetActive(false);
                    }
                }

                //BluRails.SetActive(GameController.Instance.CurrentPlayer.Player.Avatar.Contains("prof"));
            }            
        }



        public void OnDestroy()
        {
            if (GameSessionController.Instance)
                GameSessionController.Instance.OnValueUpdated -= OnGameSessionValueUpdated;

            if (SceneManager.GetActiveScene().name.Contains("Greek"))
            {
                foreach(RuntimePlayer player in GameController.Instance.RuntimePlayers)
                {
                    if(!player.Player.Avatar.Contains("prof"))
                    {
                        foreach (MeshRenderer rend in player.AvatarController.GetComponentsInChildren<MeshRenderer>())
                        {
                            rend.enabled = false;
                        }
                    }                    
                }                
            }
        }

        public void Update()
        {

        }

        public void ToggleGroup1()
        {
            if (NetworkInterface.Instance.IsClient && GameController.Instance.CurrentPlayer.Player.Avatar.Contains("prof") && GroupsEnabled)
            {
                foreach (Guid id in group1.PlayerIDs)
                {
                    RuntimePlayer player = GameController.Instance.GetPlayerByPlayerId(id);
                    if (player != null)
                    {
                        foreach (MeshRenderer rend in player.AvatarController.GetComponentsInChildren<MeshRenderer>())
                        {
                            rend.enabled = true;
                        }

                        foreach (SkinnedMeshRenderer rend in player.AvatarController.GetComponentsInChildren<SkinnedMeshRenderer>())
                        {
                            rend.enabled = true;
                        }
                    }
                }

                foreach (Guid id in group2.PlayerIDs)
                {
                    RuntimePlayer player = GameController.Instance.GetPlayerByPlayerId(id);
                    if (player != null)
                    {
                        foreach (MeshRenderer rend in player.AvatarController.GetComponentsInChildren<MeshRenderer>())
                        {
                            rend.enabled = false;
                        }

                        foreach (SkinnedMeshRenderer rend in player.AvatarController.GetComponentsInChildren<SkinnedMeshRenderer>())
                        {
                            rend.enabled = false;
                        }
                    }
                }
            }
        }

        public void ToggleGroup2()
        {
            if (NetworkInterface.Instance.IsClient && GameController.Instance.CurrentPlayer.Player.Avatar.Contains("prof") && GroupsEnabled)
            {
                foreach (Guid id in group2.PlayerIDs)
                {
                    RuntimePlayer player = GameController.Instance.GetPlayerByPlayerId(id);
                    if (player != null)
                    {
                        foreach (MeshRenderer rend in player.AvatarController.GetComponentsInChildren<MeshRenderer>())
                        {
                            rend.enabled = true;
                        }

                        foreach (SkinnedMeshRenderer rend in player.AvatarController.GetComponentsInChildren<SkinnedMeshRenderer>())
                        {
                            rend.enabled = true;
                        }
                    }
                }

                foreach (Guid id in group1.PlayerIDs)
                {
                    RuntimePlayer player = GameController.Instance.GetPlayerByPlayerId(id);
                    if (player != null)
                    {
                        foreach (MeshRenderer rend in player.AvatarController.GetComponentsInChildren<MeshRenderer>())
                        {
                            rend.enabled = false;
                        }

                        foreach (SkinnedMeshRenderer rend in player.AvatarController.GetComponentsInChildren<SkinnedMeshRenderer>())
                        {
                            rend.enabled = false;
                        }
                    }
                }
            }
        }

        private void OnGameSessionValueUpdated(string key, object value, bool playerValue, bool isInitializing)
        {
            if (key.Contains(SEAT_MAPPING) && NetworkInterface.Instance.IsClient & GroupsEnabled)
            {
                PlayerSeatMappingMessage seatMappingMessage = (PlayerSeatMappingMessage)value;

                if (seatMappingMessage.virtualSeatName == "")
                {
                    Debug.Log("Virtual Seat is blank");
                    return;
                }

                VirtualSeat vSeat = null;
                foreach (VirtualSeat seat in VSlayout._availableSeats)
                {
                    if (seat._uniqueIdentifier.Equals(seatMappingMessage.virtualSeatName))
                    {
                        vSeat = seat;
                        break;
                    }
                }

                if (vSeat != null)
                {
                    playerscount++;

                    if (GameController.Instance.CurrentPlayer.Player.Avatar.Contains("prof"))
                    {
                        m_group = GROUP.PROFESSOR;
                    }
                    else if (GameController.Instance.CurrentPlayer.AvatarController.PlayerId == seatMappingMessage.playerId)
                    {
                        //Determine which group the current client is
                        if (group1.VirtualSeats.Contains(vSeat))
                        {
                            m_group = GROUP.GROUP_1;
                        }
                        else if (group2.VirtualSeats.Contains(vSeat))
                        {
                            m_group = GROUP.GROUP_2;
                        }
                        else
                        {
                            Debug.LogError("Player is neither professor or in group 1 or 2: " + vSeat._uniqueIdentifier);
                            m_group = GROUP.NONE; //this is an error
                        }
                    }

                    //Determine which group the current client is
                    if (group1.VirtualSeats.Contains(vSeat))
                    {
                        group1.PlayerIDs.Add(seatMappingMessage.playerId);
                    }
                    else if (group2.VirtualSeats.Contains(vSeat))
                    {
                        group2.PlayerIDs.Add(seatMappingMessage.playerId);
                    }

                    if (playerscount >= GameController.Instance.RuntimePlayers.Count-1 && m_group != GROUP.NONE) //minus 1 for the professor
                    {
                        switch (m_group)
                        {
                            case GROUP.GROUP_1: //turn off group 2
                                foreach (Guid id in group2.PlayerIDs)
                                {
                                    RuntimePlayer player = GameController.Instance.GetPlayerByPlayerId(id);
                                    if (player != null)
                                    {
                                        foreach (SkinnedMeshRenderer rend in player.AvatarController.GetComponentsInChildren<SkinnedMeshRenderer>())
                                        {
                                            rend.enabled = false;
                                        }

                                        foreach (MeshRenderer mesh in player.AvatarController.GetComponentsInChildren<MeshRenderer>())
                                        {
                                            mesh.enabled = false;
                                        }
                                    }
                                    else
                                    {
                                        Debug.LogError("Couldn't find player " + id);
                                    }
                                }
                                break;

                            case GROUP.GROUP_2: //turn off group 1
                                foreach (Guid id in group1.PlayerIDs)
                                {
                                    RuntimePlayer player = GameController.Instance.GetPlayerByPlayerId(id);
                                    if (player != null)
                                    {
                                        foreach (SkinnedMeshRenderer rend in player.AvatarController.GetComponentsInChildren<SkinnedMeshRenderer>())
                                        {
                                            rend.enabled = false;
                                        }

                                        foreach (MeshRenderer mesh in player.AvatarController.GetComponentsInChildren<MeshRenderer>())
                                        {
                                            mesh.enabled = false;
                                        }
                                    }
                                    else
                                    {
                                        Debug.LogError("Couldn't find player " + id);
                                    }
                                }
                                break;

                            case GROUP.PROFESSOR: //YOU ARE THE PROFESSOR?
                                ToggleGroup1();
                                break;

                            case GROUP.NONE:

                                break;
                        }

                    }
                }
                else
                {
                    Debug.LogError("Couldn't find Virtual Seat");
                }
            } else if(key.Contains(SEAT_MAPPING) && NetworkInterface.Instance.IsClient & !GroupsEnabled)
            {
                PlayerSeatMappingMessage seatMappingMessage = (PlayerSeatMappingMessage)value;

                if (seatMappingMessage.virtualSeatName == "")                
                    return;
                

                //turn on avatars that were turned off
                RuntimePlayer player = GameController.Instance.GetPlayerByPlayerId(seatMappingMessage.playerId);

                if (player != null)
                {
                    foreach(SkinnedMeshRenderer smr in player.AvatarController.GetComponentsInChildren<SkinnedMeshRenderer>())
                    {
                        smr.enabled = true;
                    }

                    if(SceneManager.GetActiveScene().name.Contains("Greek"))
                    {
                        foreach (MeshRenderer rend in player.AvatarController.GetComponentsInChildren<MeshRenderer>())
                        {
                            rend.enabled = false;
                        }
                    } else
                    {
                        foreach (MeshRenderer rend in player.AvatarController.GetComponentsInChildren<MeshRenderer>())
                        {
                            rend.enabled = true;
                        }
                    }
                }
            }
        }
    }
}
