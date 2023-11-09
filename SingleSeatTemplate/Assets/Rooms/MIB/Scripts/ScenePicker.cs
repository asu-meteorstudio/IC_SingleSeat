using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using Artanim;
using Artanim.Location.Messages;
using System.Linq;

public class ScenePicker : SingletonBehaviour<ScenePicker>
{
    public MultiSceneSetup AllScenesSetup;

    public UnityEvent onAllScenesLoaded;
    private Dictionary<string, DreamscapeSceneControl> sceneMapping = new Dictionary<string, DreamscapeSceneControl>();
    private Coroutine CoroutineFadeOut;
    private Coroutine CoroutineFadeIn;

    [SerializeField]
    [Tooltip("We add a small delay before loading in all the scenes as SceneController is waiting for the Construct to finish loading," +
        "and starting async scene loading right away seemed to cause locking issues.")]
    int startupFrameDelay = 5;

    [SerializeField] bool logMessages;
    [SerializeField] bool logWarnings;

    [SerializeField]
    bool logErrors;

    Dictionary<string, Scene> nameToSceneTable = new Dictionary<string, Scene>();

    private void Start()
    {
#if UNITY_EDITOR
        if (DevelopmentMode.CurrentMode == EDevelopmentMode.Standalone)
        {
            //
            Log("Standalone mode. Pick which scene setup to run", LogType.Log);
            if (GameController.Instance)
            { GameController.Instance.SetReadyForSession(); }
        }
        else
        {
            StartCoroutine(LoadSceneRoutine(AllScenesSetup));
        }
#else
        StartCoroutine(LoadSceneRoutine(AllScenesSetup));
#endif
    }

    public void TransitionScenesWithFade(string sceneToUnload, string sceneToLoad, Transition transitionType, float fadeOutSpeed, float fadeInSpeed)
    {
        StartCoroutine(FadedSceneTransition(sceneToUnload, sceneToLoad, transitionType, fadeOutSpeed, fadeInSpeed));
    }

    public IEnumerator FadeOut(Transition transitionType, float speed)
    {
        CoroutineFadeOut = StartCoroutine(FadeOutAsync(transitionType, speed));
        yield return CoroutineFadeOut;
    }

    public void FadeInExternal(float newSpeed)
    {
        StartCoroutine(FadeIn(newSpeed));
    }

    public IEnumerator FadeIn(float speed)
    {
        CoroutineFadeIn = StartCoroutine(FadeInAsync(speed));
        yield return CoroutineFadeIn;
    }

    public void ActivateSceneLoadedTrigger(string activatedScene)
    {
        Log("Calling ActivateSceneLoadTrigger on: " + activatedScene, LogType.Log);
        var currentRef = nameToSceneTable[activatedScene];
        //SceneManager.SetActiveScene(currentRef);
        SceneLoadTrigger sceneLoadedTrigger = null;
        foreach (GameObject go in currentRef.GetRootGameObjects())
        {
            SceneLoadTrigger temp = go.GetComponent<SceneLoadTrigger>();
            if (temp)
                sceneLoadedTrigger = temp;
        }
        if (sceneLoadedTrigger != null)
        {
            sceneLoadedTrigger.networkActivated.Activate();
            Log("Found Scene Load trigger and calling activate!", LogType.Log);
        }
        else
        {
            Log("Couldn't find a scene load trigger!", LogType.Error);
        }
    }

    private IEnumerator FadedSceneTransition(string unloadScene, string loadScene, Transition transition, float fadeOut, float fadeIn)
    {
        CoroutineFadeOut = StartCoroutine(FadeOutAsync(transition, fadeOut));
        yield return CoroutineFadeOut;

        DeActivateSceneObjects(unloadScene);
        ActivateSceneObjects(loadScene);
        //Look for scene load trigger if needed
        var currentRef = nameToSceneTable[loadScene];
        SceneManager.SetActiveScene(currentRef);
        SceneLoadTrigger sceneLoadedTrigger = null;
        foreach (GameObject go in currentRef.GetRootGameObjects())
        {
            SceneLoadTrigger temp = go.GetComponent<SceneLoadTrigger>();
            if (temp)
                sceneLoadedTrigger = temp;
        }
        if (sceneLoadedTrigger != null)
        { sceneLoadedTrigger.networkActivated.Activate(); }
        else
        { Log("Couldn't find a scene load trigger!", LogType.Error); }
        CoroutineFadeIn = StartCoroutine(FadeInAsync(fadeIn));
        yield return CoroutineFadeIn;
        
        Log("Scene Transition Finished. Unloaded: " + unloadScene + ". Loaded: " + loadScene, LogType.Log);
    }

    public void SetActiveScene(string sceneName)
    {
        Scene scene;
        if(nameToSceneTable.TryGetValue(sceneName, out scene))
        {
            if (scene.IsValid())
            { SceneManager.SetActiveScene(scene); }
        }
        else
        {
            Debug.LogWarningFormat("UNABLE TO SET SCENE {0} AS THE ACTIVE SCENE", sceneName);
        }
    }

    public void ReplaceCurrentActiveScene(string sceneToReplaceWith)
    {
        DeActivateSceneObjects(SceneManager.GetActiveScene().name);
        ActivateSceneObjects(sceneToReplaceWith);
        var currentRef = nameToSceneTable[sceneToReplaceWith];
        SceneManager.SetActiveScene(currentRef);
        SceneLoadTrigger sceneLoadedTrigger = null;
        foreach (GameObject go in currentRef.GetRootGameObjects())
        {
            SceneLoadTrigger temp = go.GetComponent<SceneLoadTrigger>();
            if (temp)
                sceneLoadedTrigger = temp;
        }
        if (sceneLoadedTrigger != null)
        { sceneLoadedTrigger.networkActivated.Activate(); }
        else
        { Log("Couldn't find a scene load trigger!", LogType.Error); }
    }

    public void ActivateSceneObjects(string scene)
    {
        DreamscapeSceneControl sceneControl;
        if(sceneMapping.TryGetValue(scene, out sceneControl))
        {
            Log(string.Format("Scene Picker: Turning on scene geo: {0}", scene), LogType.Log);
            sceneControl.SceneTurnOn();
        }
        else
        {
            Log(string.Format("Trying to turn on {0}, but it's not in the current scene mapping!", scene), LogType.Warning);
        }
    }

    public void DeActivateSceneObjects(string scene)
    {
        if (sceneMapping == null)
            Log("sceeneMapping was null!", LogType.Log);
        if (scene == null)
            Log("Scene name is null!", LogType.Log);

        DreamscapeSceneControl sceneControl;
        if (sceneMapping.TryGetValue(scene, out sceneControl))
        {
            Log("Scene Picker: Turning off scene geo: " + scene, LogType.Log);
            sceneControl.SceneTurnOff();
        }
        else
        {
            Log(string.Format("Trying to turn off {0}, but it's not in the current scene mapping!", scene), LogType.Warning);
        }
    }

    public void StartScene(string scene)
    {
        DreamscapeSceneControl sceneControl;
        if (sceneMapping.TryGetValue(scene, out sceneControl))
        {
            sceneControl.StartScene();
            Log(string.Format("STARTING SCENE {0}", scene), LogType.Log);
        }
        else
        {
            Log(string.Format("Trying start {0}, but it's not in the current scene mapping!", scene), LogType.Warning);
        }
    }

    public IEnumerator LoadSceneRoutine(MultiSceneSetup sceneSetup)
    {
        //yield return new WaitForSeconds(1f);
        for (var i = 0; i < startupFrameDelay; i++)
            yield return null;

        var scenesToLoad = sceneSetup.Setups.Select(s => s.SceneReference.SceneName).ToList();

        sceneMapping.Clear();

        if(logErrors)
        {
            foreach (string scene in scenesToLoad)
            { Log(scene, LogType.Log); }
        }

        foreach (string scene in scenesToLoad)
        {
            if(logErrors)
            {
                if (sceneMapping.ContainsKey(scene))
                {
                    Log(string.Format("SCENE MAPPING ALREADY CONTAINS KEY {0}", scene), LogType.Warning);
                }
                else
                {
                    Log(string.Format("ADDING SCENE {0} TO SCENE MAPPING", scene), LogType.Log);
                }
            }

            sceneMapping.Add(scene, null);
        }

        Application.backgroundLoadingPriority = ThreadPriority.High;
        foreach (string scene in scenesToLoad)
        {
            if (!ConfigService.Instance.ExperienceSettings.ConstructSceneName.Equals(scene))
            {
                //Log(string.Format("MAPPING SCENE CONTROL {0}", scene), LogType.Message);
                yield return StartCoroutine(LoadSceneAsync(scene));
                Scene currentRef = SceneManager.GetSceneByName(scene);
                nameToSceneTable.Add(scene, currentRef);
                foreach (GameObject go in currentRef.GetRootGameObjects())
                {
                    if (go.CompareTag("SceneControl"))
                    {
                        var sceneControl = go.GetComponent<DreamscapeSceneControl>();
                        sceneMapping[scene] = sceneControl;
                        //yield return sceneControl.ToggleScene(true);
                        //SceneManager.SetActiveScene(currentRef);
                        //yield return null;
                        //yield return sceneControl.ToggleScene(false);
                        //yield return null;
                        DeActivateSceneObjects(scene);
                    }
                }
            }
            else
            {
                yield return null;

                //Log(string.Format("MAPPING CONSTRUCT SCENE {0}", scene), LogType.Message);
                //Scene constructScene = SceneManager.GetActiveScene();
                Scene constructScene = SceneManager.GetSceneByName(scene);
                nameToSceneTable.Add(scene, constructScene);
                foreach (GameObject go in constructScene.GetRootGameObjects())
                {
                    if(go.CompareTag("SceneControl"))
                    {
                        sceneMapping[constructScene.name] = go.GetComponent<DreamscapeSceneControl>();
                    }
                }
            }


            if (sceneMapping[scene] == null && !scene.Contains("_lightLoader"))
            { Log("Couldn't find a scene root gameobject for scene: " + scene, LogType.Error); }
        }
        ActivateSceneObjects(ConfigService.Instance.ExperienceSettings.ConstructSceneName);
        Application.backgroundLoadingPriority = ThreadPriority.Normal;
        Log("All scenes loaded!", LogType.Log, true);
        onAllScenesLoaded.Invoke();
        if (GameController.Instance)
        { GameController.Instance.SetReadyForSession(); }

#if !UNITY_EDITOR
        if (DevelopmentMode.CurrentMode == EDevelopmentMode.Standalone)
        {
            string devMode = ConfigService.Instance.ExperienceConfig.GetPropertyString("DEVMODE");
        if(devMode != null)
        {
            switch (devMode)
            {
                case "DEFAULT":
                        GameplayManager.Instance.StartExperienceDefault();
                    break;
                case "CONSTRUCTTONY":
                        GameplayManager.Instance.StartConstructToNY();
                    break;
                case "NYTOFULL":
                        GameplayManager.Instance.StartNYFull();
                    break;
                case "CONSTRUCTONLY":
                        GameplayManager.Instance.StartConstruct();
                    break;
                case "NYTOGALAXARIUM":
                        GameplayManager.Instance.StartNYToGalaxarium();
                    break;
                case "NYSUBWAYONLY":
                        GameplayManager.Instance.StartNYOnly();
                    break;
                default:
                    break;
            }
        }
        }
#endif
    }

    private VRCameraFader GetCurrentVRCamFader()
    {
        if (MainCameraController.Instance)
            return MainCameraController.Instance.ActiveCamera.GetComponent<VRCameraFader>();
        else
            return null;
    }

    private IEnumerator FadeOutAsync(Transition transition, float transitionSpeed, string customTransitionName = null)
    {
        var cameraFader = GetCurrentVRCamFader();
        if (cameraFader != null)
        {
            cameraFader.FadeSpeed = transitionSpeed;
            yield return cameraFader.DoFadeAsync(transition, customTransitionName: customTransitionName);
        }
        CoroutineFadeOut = null;
    }

    private IEnumerator FadeInAsync(float transitionSpeed)
    {
        var cameraFader = GetCurrentVRCamFader();
        if (cameraFader != null)
        {
            cameraFader.FadeSpeed = transitionSpeed;
            yield return cameraFader.DoFadeInAsync();
        }
        CoroutineFadeIn = null;
    }

    public void LoadScene(string sceneName)
    {
        StartCoroutine(LoadSceneAsync(sceneName));
    }

    public void UnloadScene(string sceneName)
    {
        StartCoroutine(UnloadSceneAsync(sceneName));
    }

    void Log(string message, LogType logType, bool forceLog = false)
    {
        if(logType == LogType.Log && (logMessages || forceLog))
        {
            Debug.Log(message);
        }
        else if(logType == LogType.Warning && (logWarnings || forceLog))
        {
            Debug.LogWarning(message);
        }
        else if(logType == LogType.Error && (logErrors || forceLog))
        {
            Debug.LogError(message);
        }
    }

#region Internal
    private IEnumerator LoadSceneAsync(string scene)
    {
        float time = Time.time;
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(scene, LoadSceneMode.Additive);
        while (!asyncLoad.isDone)
            yield return null;
        Log("Loading " + scene + " took " + (Time.time - time) + " seconds.", LogType.Log);
    }

    public IEnumerator UnloadSceneAsync(string scene)
    {
        float time = Time.time;
        Application.backgroundLoadingPriority = ThreadPriority.Low;
        AsyncOperation asyncUnload = SceneManager.UnloadSceneAsync(scene);
        while (!asyncUnload.isDone)
            yield return null;
        Application.backgroundLoadingPriority = ThreadPriority.Normal;
        Log("Unloading " + scene + " took " + (Time.time - time) + " seconds.", LogType.Log);
    }
#endregion
}

//public enum LogType
//{
//    Message,
//    Warning,
//    Error
//}