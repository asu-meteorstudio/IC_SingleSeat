using Artanim.Location.SharedData;
using Artanim.Location.Data;
using Artanim.Location.Network;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Artanim.SeatManagement
{
    // Represent a physical seat
    public class PhysicalSeat : Seat
    {        
        public Guid _assignedPlayerId = Guid.Empty;
        public GameObject _seatChaperone = null;

        private Collider _collider;
        private AvatarChaperoneTrigger _avatarChaperoneTrigger;

        public PodChaperoneManager _podChaperoneManager;

        private bool _isServer; 
        public string _podId;

        // Start is called before the first frame update
        void Start()
        {
            _isServer = NetworkInterface.Instance.IsServer;
            
            if (GameController.Instance)
            {
                GameController.Instance.OnSessionStarted += Instance_OnSessionStarted;
                GameController.Instance.OnLeftSession += Instance_OnLeftSession; // NOTE: PROBABLY NOT NEEDED
            }

            // Retrieve collider and disable it
            _collider = GetComponent<Collider>();
            if (_collider)
                _collider.enabled = false; 
            else
                Debug.LogError("Could not find a box collider on " + name);

            // Retrieve AvatarChaperoneTrigger and disable it
            _avatarChaperoneTrigger = GetComponent<AvatarChaperoneTrigger>();
            if (_avatarChaperoneTrigger)
                _avatarChaperoneTrigger.enabled = false;
            else
                Debug.LogError("Could not find an AvatarChaperoneTrigger on " + name);
        }

        public void Instance_OnSessionStarted()
        {
            Debug.Log("[" + Time.frameCount + "] - Session started: activating physical seats");
            EnableBehaviors();
        }

        public void Instance_OnLeftSession()
        {
            Debug.Log("[" + Time.frameCount + "] Session left: disabling physical seats");
            DisableBehaviors();
        }
        public void EnableBehaviors()
        {
            if (_collider && _isServer) _collider.enabled = true;
            if (_avatarChaperoneTrigger) _avatarChaperoneTrigger.enabled = true;
        }

        public void DisableBehaviors()
        {
            if (_collider && _isServer) _collider.enabled = false;
            if (_avatarChaperoneTrigger) _avatarChaperoneTrigger.enabled = false;
        }

        public void OnHeadEnter(AvatarController avatar)
        {
            Debug.Log("[" + Time.frameCount + "] - OnHeadEnter - " + gameObject.name);

            // if physical seat it free, take it
            // Decision should probably be taken upward
            if (_isServer)
            { 
                if (_isAvailable && !SeatManager.Instance.IsPlayerAssignedToPhysicalSeat(avatar.PlayerId) && !avatar.Player.Avatar.Contains("prof"))
                {
                    _isAvailable = false;
                    _assignedPlayerId = avatar.PlayerId;

                    //Notify seat manager
                    SeatManager.Instance.OnPhysicalSeatAssigned(this, _assignedPlayerId);
                }
            }
            /*else
            { 
                // If this is the seat we are assigned to, update chaperone visualization
                if (_assignedPlayerId == GameController.Instance.CurrentPlayerId)
                { 
                    if (_assignedPlayerId == avatar.PlayerId)
                    {
                        // We are back in the seat that was assigned to us, hide our chaperone
                        Debug.Log("[" + Time.frameCount + "] " + avatar.PlayerId + " returned to his assigned seat. Hiding chaperone.");
                        if (_podChaperoneManager != null)
                            _podChaperoneManager.SetChaperoneVisibility(false);
                        else
                            Debug.LogError(name + " doesn't have a PodChaperoneManager assigned");
                    }
                    else
                    {
                        // Another player is entering our seat area, show his avatar chaperone
                        Debug.Log("[" + Time.frameCount + "] " + avatar.PlayerId + " entered our assigned seat. Showing his avatar chaperone.");
                        if (_podChaperoneManager != null)
                            _podChaperoneManager.SetAvatarChaperoneVisibility(true, avatar.PlayerId);
                        else
                            Debug.LogError(name + " doesn't have a PodChaperoneManager assigned");
                    }
                }
            }*/
        }

        public void OnHeadExit(AvatarController avatar)
        {
            Debug.Log(Time.frameCount + " - OnHeadExit - " + gameObject.name);

            if (!_isAvailable && NetworkInterface.Instance.IsClient && !avatar.Player.Avatar.Contains("prof"))   
            {
                // If this is the seat we are assigned to, update chaperone visualization
                /*if (_assignedPlayerId == GameController.Instance.CurrentPlayerId)
                {
                    if (_assignedPlayerId == avatar.PlayerId)
                    {
                        // We are leaving the seat that is assigned to us, show chaperone
                        Debug.Log("[" + Time.frameCount + "] " + avatar.PlayerId + " left his assigned seat. Showing chaperone.");
                        if (_podChaperoneManager != null)
                            _podChaperoneManager.SetChaperoneVisibility(true);
                        else
                            Debug.LogError(name + " doesn't have a PodChaperoneManager assigned");
                    }
                    else
                    {
                        // Another player is leaving our seat area, hide his avatar chaperone
                        Debug.Log("[" + Time.frameCount + "] " + avatar.PlayerId + " has left our assigned seat. Hiding his avatar chaperone.");
                        if (_podChaperoneManager != null)
                            _podChaperoneManager.SetAvatarChaperoneVisibility(false, avatar.PlayerId);
                        else
                            Debug.LogError(name + " doesn't have a PodChaperoneManager assigned");
                    }
                }*/
            }
        }
    }
}
