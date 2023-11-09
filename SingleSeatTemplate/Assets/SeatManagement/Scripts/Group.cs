using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Artanim.SeatManagement
{
    public class Group : MonoBehaviour
    {
        public List<VirtualSeat> VirtualSeats;
        public List<Guid> PlayerIDs;

        public void Awake()
        {
            PlayerIDs = new List<Guid>();
        }
    }
}
