using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Artanim.SeatManagement
{ 
    public class AvatarChaperoneController : MonoBehaviour
    {
        public GameObject _avatarChaperonePrefab;
        private GameObject _avatarChaperone;

        void OnDestroy()
        {
            Destroy(_avatarChaperone);
        }

        public AvatarChaperone InstantiateAvatarChaperone(Transform transform)
        {
            if (_avatarChaperonePrefab)
            {
                _avatarChaperone = Instantiate(_avatarChaperonePrefab, transform);
                if (_avatarChaperone)
                {
                    _avatarChaperone.GetComponent<AvatarChaperone>().SourceAvatarController = GetComponent<AvatarController>();
                    _avatarChaperone.name = "AvatarChaperone-" + GetComponent<AvatarController>().PlayerId;

                    return _avatarChaperone.GetComponent<AvatarChaperone>();
                }
            }

            return null;
        }
    }
}
