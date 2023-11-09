using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Artanim.Location.Network;

namespace Artanim.SeatManagement
{
    public class PhysicalSeatLayout : SeatLayout
    {
        public string _podId;

        private PodChaperoneManager podChaperoneManager;

        private void Start()
        {
            if (NetworkInterface.Instance.IsClient)
            {
                FollowGlobalMocapOffset followGlobalMocapOFfset = GetComponent<FollowGlobalMocapOffset>();
                if (followGlobalMocapOFfset != null)
                    Destroy(followGlobalMocapOFfset);

                DontDestroyOnLoadBehaviour destroyOnLoadBehaviour = GetComponent<DontDestroyOnLoadBehaviour>();
                if (destroyOnLoadBehaviour != null)
                    Destroy(destroyOnLoadBehaviour);
            }

            podChaperoneManager = GetComponent<PodChaperoneManager>();
            if (podChaperoneManager != null)
            {
                // Pass the reference to the PodChaperoneManager to the individual seats
                foreach (PhysicalSeat pSeat in _availableSeats)
                    pSeat._podChaperoneManager = podChaperoneManager;
            }
            else
                Debug.LogError("Could not find a PodChaperoneManager on " + name);
        }

        public void UpdatePhysicalSeatUniqueIdentifier(string podId)
        {
            foreach(PhysicalSeat pSeat in _availableSeats)
            { 
                pSeat._uniqueIdentifier = "P-" + podId + "-" + pSeat.gameObject.name;
                pSeat._podId = podId;
            }
        }
    }
}
