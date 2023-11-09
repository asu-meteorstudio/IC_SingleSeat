#if DSUTIL_ARTANIM_COMMON_IN_PROJECT
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Artanim;
using Artanim.Location.Network;

namespace DreamscapeUtil
{
    /// <summary>
    /// Triggers a network activated when an experience scene is finished loading on all components
    /// </summary>
    [RequireComponent(typeof(NetworkActivated))]
    public class SceneLoadTrigger : MonoBehaviour
    {

        private void OnEnable()
        {
            if (GameController.Instance && NetworkInterface.Instance.IsServer)
            {
                GameController.Instance.OnSceneLoadedInSession += OnSceneLoad;
            }

        }

        private void OnDisable()
        {
            if (GameController.Instance && NetworkInterface.Instance.IsServer)
            {
                GameController.Instance.OnSceneLoadedInSession -= OnSceneLoad;
            }
        }

        private void OnSceneLoad(string[] sceneNames, bool sceneLoadTimedOut)
        {
            GetComponent<NetworkActivated>().Activate();
            if (GameController.Instance)
            {
                GameController.Instance.OnSceneLoadedInSession -= OnSceneLoad;
            }
        }
    }
}
#endif
