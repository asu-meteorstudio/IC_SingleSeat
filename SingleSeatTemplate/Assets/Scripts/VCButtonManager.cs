using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Artanim.Location.Data;
using Artanim.Location.Network;
using Artanim.Location.Messages;
using Artanim.Location.SharedData;
using System;

namespace Artanim
{ 
    public class VCButtonManager : MonoBehaviour
    {
        public bool HideIfNotProfessor = true;
        public GameObject Chaperone;

        private bool isProfessor()
        {
            //var playerComponent = SharedDataUtils.FindLocationComponent<LocationComponentWithSession>(playerId);
            if (NetworkInterface.Instance.IsClient)
            {
                    return GameController.Instance.CurrentPlayer.Player.Avatar.Contains("prof");
            }

            return false;
        }

        // Start is called before the first frame update
        void Start()
        {
            // Only hide buttons if we are a client
            if (NetworkInterface.Instance.IsClient)
            { 
                // Hide the buttons unless we are the professor, or if we are in standalone mode
                if (HideIfNotProfessor)
                { 
                    if (!isProfessor() && DevelopmentMode.CurrentMode != EDevelopmentMode.Standalone)
                    {
                        Debug.Log("Hidding buttons for player: " + GameController.Instance.CurrentPlayerId);
                        foreach (var avatarTrigger in GetComponentsInChildren<AvatarTrigger>())
                        {
                            avatarTrigger.gameObject.SetActive(false);
                        }

                        Chaperone.SetActive(false);
                    }
                }
            }
        }
    }
}
