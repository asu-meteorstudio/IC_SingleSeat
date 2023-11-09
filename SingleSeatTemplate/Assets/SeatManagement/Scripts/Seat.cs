using UnityEngine;

namespace Artanim.SeatManagement
{
    public class Seat : MonoBehaviour
    {
        public bool _isAvailable = true;
        public GameObject _chaperoneVisualRoot;
        public string _uniqueIdentifier = "";

        void Awake()
        {
            SetChaperoneVisibility(false);
        }

        // Show or hide the chaperone geometry
        public void SetChaperoneVisibility(bool state)
        {
            if (_chaperoneVisualRoot) _chaperoneVisualRoot.SetActive(state);
        }
    }
}
