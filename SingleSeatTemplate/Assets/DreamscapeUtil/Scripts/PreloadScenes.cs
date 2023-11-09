#if DSUTIL_ARTANIM_COMMON_IN_PROJECT
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Artanim;
using DreamscapeUtil;
using Artanim.Location.Network;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

namespace DreamscapeUtil
{

    /// <summary>
    /// Preloads(and then unloads) a list of scenes in the construct in order to reduce load times
    /// between scenes later in the experience. In order to use this, you must set the
    /// 'Ready for Session Mode' to 'Experience Controlled' in the artanim experience config.
    /// Do not use in combination with ScenePicker or any other scene management tool that
    /// operates while the construct scene is loaded.
    /// </summary>
    public class PreloadScenes : SingletonBehaviour<PreloadScenes>, ISceneCheckerBehaviour
    {

        //private bool hasRun;
        private static bool loaded = false;
        [Tooltip("List of scenes to be preloaded in construct")]
        public string[] scenes;
        [Tooltip("Actions to be performed after all scenes have been preloaded")]
        public UnityEvent OnComplete;

        // Use this for initialization
        void Start()
        {
            //loaded = false;
            if (!loaded)
            {
                Debug.Log("Preloading Scenes");
                StartCoroutine(LoadScenes());
            }
            else
            {
                Debug.Log("Scenes already preloaded");
                GameController.Instance.SetReadyForSession();
            }

            /*if (GameController.Instance)
            {
                GameController.Instance.OnLeftSession += Instance_OnLeftSession;
            }*/
        }

        private void OnDestroy()
        {
            if (GameController.Instance)
            {
                GameController.Instance.OnLeftSession -= Instance_OnLeftSession;
            }
        }

        private void Instance_OnLeftSession()
        {
            if (loaded)
            {
                Debug.Log("Scenes already preloaded");
                GameController.Instance.SetReadyForSession();
            }
            else
            {
                //Debug.LogError("RELOADING SCENES");
                StartCoroutine(LoadScenes());
            }

        }

        private IEnumerator LoadScenes()
        {
            if (!Application.isEditor)
            {
                foreach (string level in scenes)
                {
                    Debug.LogFormat("Preloading Scene '{0}'", level);
                    AsyncOperation op = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(level, UnityEngine.SceneManagement.LoadSceneMode.Additive);
                    //SceneController.Instance.LoadChildScene(level, Artanim.Location.Messages.Transition.None, Artanim.Location.Messages.ELoadSequence.UnloadFirst, true, true);
                    //loaded = false;
                    //SceneController.Instance.OnSceneLoaded += Instance_OnSceneLoaded;
                    while (!op.isDone)
                    {
                        yield return null;
                    }

                    op = UnityEngine.SceneManagement.SceneManager.UnloadSceneAsync(level);

                    while (!op.isDone)
                    {
                        yield return null;
                    }
                }
            }

            OnComplete.Invoke();

            //SceneController.Instance.LoadChildScene("MainConstruct", Artanim.Location.Messages.Transition.None, Artanim.Location.Messages.ELoadSequence.UnloadFirst, false, true);
            //SceneController.Instance.OnSceneLoaded += Instance_OnSceneLoaded;    
            GameController.Instance.SetReadyForSession();
            loaded = true;
        }

        private void Instance_OnSceneLoaded(string sceneName, UnityEngine.SceneManagement.Scene scene, bool isMainScene)
        {
            GameController.Instance.SetReadyForSession();
            SceneController.Instance.OnSceneLoaded -= Instance_OnSceneLoaded;
            loaded = true;
            Debug.Log("Preloading Complete");
        }

        public int CheckForErrors()
        {
            int numErrors = 0;
            foreach (string sceneName in scenes)
            {
                List<string> scenesInBuild = new List<string>();
                for (int i = 0; i < SceneManager.sceneCountInBuildSettings; i++)
                {
                    string scenePath = SceneUtility.GetScenePathByBuildIndex(i);
                    int lastSlash = scenePath.LastIndexOf("/");
                    string n = scenePath.Substring(lastSlash + 1, scenePath.LastIndexOf(".") - lastSlash - 1);
                    if (n == sceneName)
                    {
                        goto Found;
                    }
                }
                numErrors++;
                Debug.LogWarningFormat(this, "Scene with name '{0}' not found in build settings.", sceneName);

                Found:;
            }
            return numErrors;
        }
    }
}
#endif