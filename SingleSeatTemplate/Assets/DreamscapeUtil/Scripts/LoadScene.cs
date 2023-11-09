#if DSUTIL_ARTANIM_COMMON_IN_PROJECT
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Artanim;
using UnityEngine.SceneManagement;

namespace DreamscapeUtil
{
    /// <summary>
    /// Utility script to help load an experience scene in the artanim SDK, e.g. from a timeline or animation event
    /// </summary>
    public class LoadScene : MonoBehaviour, ISceneCheckerBehaviour
    {
        [NonEmpty]
        [Tooltip("Name of next scene to load")]
        public string sceneName;
        [Tooltip("Type of transition to use")]
        public Artanim.Location.Messages.Transition transition = Artanim.Location.Messages.Transition.FadeBlack;
        [Tooltip("If checked, change the fade speed on the VRCameraFader first")]
        public bool changeFadeSpeed = false;
        [Tooltip("New fade speed when fading out of this scene")]
        public float fadeSpeed = 1.0f;

        /// <summary>
        /// Starts the scene load process
        /// </summary>
        public void DoLoadScene()
        {
            if (changeFadeSpeed)
            {
                VRCameraFader fader = MainCameraController.Instance.ActiveCamera.GetComponent<VRCameraFader>();
                fader.FadeSpeed = fadeSpeed;
            }
            GameController.Instance.LoadGameScene(sceneName, transition);
        }

        public int CheckForErrors()
        {
            List<string> scenesInBuild = new List<string>();
            for (int i = 0; i < SceneManager.sceneCountInBuildSettings; i++)
            {
                string scenePath = SceneUtility.GetScenePathByBuildIndex(i);
                int lastSlash = scenePath.LastIndexOf("/");
                string n = scenePath.Substring(lastSlash + 1, scenePath.LastIndexOf(".") - lastSlash - 1);
                if (n == sceneName)
                {
                    return 0;
                }
            }

            Debug.LogWarningFormat(this, "Scene with name '{0}' not found in build settings.", sceneName);
            return 1;
        }
    }
}
#endif
