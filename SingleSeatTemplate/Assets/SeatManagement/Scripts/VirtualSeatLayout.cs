using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Artanim.SeatManagement
{
    // Represents a layout of virtual seats for a scene
    public class VirtualSeatLayout : SeatLayout
    {
        private void Start()
        {
            // If there is already a SceneSeatLayout, reset seats assignments first 
            if (SeatManager.Instance._currentSceneSeatLayouts != null)
                SeatManager.Instance.ResetSeatAssignments();

            // Register as the current scene seat layout
            SeatManager.Instance._currentSceneSeatLayouts = this;
        }
    }
}
