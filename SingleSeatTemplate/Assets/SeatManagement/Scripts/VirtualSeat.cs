using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Artanim.SeatManagement
{
    // Represent a virtual seat
    public class VirtualSeat : Seat
    {
        public GameObject chairVisualsPrefab;

        private GameObject chairVisualsInstance = null;
        
        // Start is called before the first frame update
        void Awake ()
        {
            _isAvailable = true;

            // Set unique name to be "SceneName-SeatName"
            _uniqueIdentifier = "V-" + gameObject.scene.name + "-" + gameObject.name;
        }

        // Update is called once per frame
        void Update()
        {
            if (chairVisualsInstance == null)
            {
                if (chairVisualsPrefab)
                {
                    chairVisualsInstance = Instantiate(chairVisualsPrefab, transform);
                    chairVisualsInstance.SetActive(false);
                }
            }
            else
            {
                chairVisualsInstance.SetActive(false);
            }
        }
    }
}
