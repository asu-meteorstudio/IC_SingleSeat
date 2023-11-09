using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using Artanim;
using Artanim.Location.Network;
using System.Linq;

public class ConstructManager : DreamscapeSceneControl
{
    #region Singleton Behaviour
    private static ConstructManager _instance;
    public static bool HasInstance
    {
        get
        {
            return _instance != null;
        }
    }
    public static ConstructManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<ConstructManager>();
            }
            return _instance;
        }

        protected set
        {
            _instance = value;
        }
    }
    #endregion

    public enum StartScene
    {
        DEFAULT,
        ICEPLANET,
        FACEPLANET,
        MOON,
        LONDONTOWER,
        NYSUBSTATION,
        SCOOTERS,
        GALAXARIUM,
        LONDONHQ,
        ZARTHANHOME,
        INTRO,
        LONDONPORTAL,
        FACEPORTAL,
        ICEPORTAL,
    }

    public StartScene sceneToStartIn;

    public bool showEveryoneFromStart;
    public Suit[] suits;
    private Animator[] handScannerAnims;
    private TextTyper[] alerts;
    public Animator sceneAnim;
    private Suit mainPlayerSuit;

    //HACk + TODO -> make this generic
    [Header("Scene Loading Triggers")]
    public ClientToServerTrigger readyToSwapScenes;
    public NetworkActivated swapScenesNetActivated;
    public ClientToServerTrigger ReadyForLondonHQTrigger;
    public NetworkActivated LondonHQNetActivated;

    public Transform playerOffsetSource;
    public Transform scooterOffsetPos;
    public GameObject constructLight;
    public LoadLightingHelper constructLightLoadHelper;

    public Transform NYOffsetRef;

    public PlayableController ConstructController;
    public PlayableController MasterController;
    public LightProbeAnchorTransfer lightProbeTransfer;
    //public PlayableController IntroController;

    //[Header("Galaxy Hologram Animations")]
    //public GalaxyScaleBlendTreeController galaxyBlendTree;

    private ScenePicker scenePicker { get { return ScenePicker.Instance; } }
    private GameController gameController { get { return GameController.Instance; } }
    private GameplayManager gameplayManager { get { return GameplayManager.Instance; } }
    private AvatarOffsetController avatarOffsetController { get { return AvatarOffsetController.Instance; } }
    public MIBSceneManager MIBSceneManager;
    private HeadquartersSubstationControl headquartersSubstationControl { get { return HeadquartersSubstationControl.Instance; } }
    //private ScooterGameplayControl scooterGameplayControl { get { return ScooterGameplayControl.Instance; } }
    //private GalaxariumControl galaxariumControl { get { return GalaxariumControl.Instance; } }

    private int numPlayersReadyToSwapScenes;
    private bool experienceStarted;
    private bool calibratedTrigger;

    void Start()
    {
        _instance = this;

        ////#if UNITY_EDITOR
        //        var datas = Resources.LoadAll<ManagedObject>("ScriptableReferences");
        //        foreach (var data in datas)
        //        { data.ForceBegin(); }
        ////#endif
        ///
        //Shader.WarmupAllShaders();

        if (gameController)
        {
            string startScene = ConfigService.Instance.ExperienceConfig.GetPropertyString("STARTSCENE");
            if (startScene != null)
            {
                switch (startScene)
                {
                    case ("DEFAULT"):
                        sceneToStartIn = StartScene.DEFAULT;
                        break;
                    case ("ICEPLANET"):
                        sceneToStartIn = StartScene.ICEPLANET;
                        break;
                    case ("FACEPLANET"):
                        sceneToStartIn = StartScene.FACEPLANET;
                        break;
                    case ("MOON"):
                        sceneToStartIn = StartScene.MOON;
                        break;
                    case ("LONDONTOWER"):
                        sceneToStartIn = StartScene.LONDONTOWER;
                        break;
                    case ("NYSUBSTATION"):
                        sceneToStartIn = StartScene.NYSUBSTATION;
                        break;
                    case ("SCOOTERS"):
                        sceneToStartIn = StartScene.SCOOTERS;
                        break;
                }
            }
        }
    }

    public void TurnOnBiospheres()
    {
        if (NetworkInterface.Instance.IsClient)
        {
            int playerCount = gameController.RuntimePlayers.Count > 0 ? GameController.Instance.RuntimePlayers.Count : 1;
            Debug.Log("Playercount: " + playerCount);
            for (int i = 0; i < playerCount; i++)
            {
                suits[i].nameAnim.gameObject.SetActive(true);
            }
        }
    }

    public void StartExperience()
    {
        if(!experienceStarted)
        {
            experienceStarted = true;

            //warm up the playable graphs -> there's noticeable slowdown when preparing the first frame, especially in 2017...
            //...so we warm up the graph at the start of the experience
            //ConstructController.PlayableDirector.RebuildGraph();
            //ConstructController.Play();
            //ConstructController.Pause();
            //MasterController.PlayableDirector.RebuildGraph();
            MasterController.Play();
            MasterController.Pause();

            //constructLight.SetActive(false);
            switch (sceneToStartIn)
            {
                case (StartScene.DEFAULT):
                    StartSuitTransition();
                    break;
                case (StartScene.ICEPLANET):
                    StartCoroutine(StartAtScooterSection(MIBSceneManager.IceSpline, MIBSceneManager.FacePortalSpline));
                    break;
                case (StartScene.FACEPLANET):
                    StartCoroutine(StartAtScooterSection(MIBSceneManager.FaceSpline, MIBSceneManager.LondonPortalSpline));
                    break;
                case (StartScene.MOON):
                    StartCoroutine(StartAtScooterSection(MIBSceneManager.MoonSpline, MIBSceneManager.LondonPortalSpline));
                    break;
                case (StartScene.LONDONTOWER):
                    StartCoroutine(StartAtScooterSection(MIBSceneManager.LondonSpline, MIBSceneManager.GalaxariumScene));
                    break;
                case (StartScene.ZARTHANHOME):
                    StartCoroutine(StartAtScooterSection(MIBSceneManager.ZarthanHomeScene, MIBSceneManager.IcePortalSpline));
                    break;
                case (StartScene.NYSUBSTATION):
                    StartCoroutine(StartAtNYSubstation());
                    break;
                case (StartScene.SCOOTERS):
                    StartCoroutine(StartAtScooters());
                    break;
                case (StartScene.GALAXARIUM):
                    StartCoroutine(StartAtGalaxarium());
                    break;
                case (StartScene.LONDONHQ):
                    StartCoroutine(StartAtLondonHQ());
                    break;
                case (StartScene.INTRO):
                    StartCoroutine(StartAtIntro());
                    break;
                case (StartScene.LONDONPORTAL):
                    StartCoroutine(StartAtScooterSection(MIBSceneManager.LondonPortalSpline, MIBSceneManager.LondonSpline));
                    break;
                case (StartScene.FACEPORTAL):
                    StartCoroutine(StartAtScooterSection(MIBSceneManager.FacePortalSpline, MIBSceneManager.FaceSpline));
                    break;
                case (StartScene.ICEPORTAL):
                    StartCoroutine(StartAtScooterSection(MIBSceneManager.IcePortalSpline, MIBSceneManager.IceSpline));
                    break;
            }
        }
    }

    public IEnumerator StartAtScooterSection(SceneReference sceneToStart, SceneReference sceneToFinish)
    {
        //Debug.Log("STARTING AT PLANET SCENE");

        yield return StartCoroutine(scenePicker.FadeOut(Artanim.Location.Messages.Transition.FadeBlack, 2.5f));

        var avatarOffset = FindObjectsOfType<AvatarOffset>().Where(ao => ao.ObjectId.Equals("WeaponRoom")).FirstOrDefault();
        foreach (var player in gameController.RuntimePlayers)
        {
            //avatarOffsetController.RegisterAvatarOffset(player.AvatarController.PlayerId, avatarOffset, true, AvatarOffsetController.ESyncMode.Unsynced);
            player.AvatarOffset.position = avatarOffset.transform.position;
        }

        //scooterGameplayControl.scooterAssignment.DoAssign();

        playerOffsetSource.position = scooterOffsetPos.position;

        scenePicker.DeActivateSceneObjects(SceneManager.GetActiveScene().name);

        //scooterGameplayControl.ToggleScooterVisuals(true);
        //scooterGameplayControl.ToggleScooterEngines(true);
        //scooterGameplayControl.EnableScooterRFPods();
        //scooterGameplayControl.EnableScooterZarthans();
        //scooterGameplayControl.PlayScooterEngineAudio(true);

        yield return StartCoroutine(scenePicker.FadeIn(2.5f));

        //TODO -> move this over to MiBSceneManager
        //World.WorldName worldName = World.WorldName.London;
        //SceneReference portalScene = null;
        //if (sceneToStart.SceneName.Equals(MIBSceneManager.LondonSpline.SceneName)) { portalScene = MIBSceneManager.GalaxariumScene; worldName = World.WorldName.London; }
        //else if (sceneToStart.SceneName.Equals(MIBSceneManager.MoonSpline.SceneName)) { portalScene = MIBSceneManager.LondonPortalSpline; worldName = World.WorldName.Moon; }
        ////else if (planetScene.SceneName.Equals(mibSceneManager.FaceSpline.SceneName)) { portalScene = mibSceneManager.MoonPortalSpline; worldName = World.WorldName.FacePlanet; }
        //else if (sceneToStart.SceneName.Equals(MIBSceneManager.FaceSpline.SceneName)) { portalScene = MIBSceneManager.LondonPortalSpline; worldName = World.WorldName.FacePlanet; }
        //else if (sceneToStart.SceneName.Equals(MIBSceneManager.IceSpline.SceneName)) { portalScene = MIBSceneManager.FacePortalSpline; worldName = World.WorldName.IcePlanet; }
        //else if (sceneToStart.SceneName.Equals(MIBSceneManager.ZarthanHomeScene.SceneName)) { portalScene = MIBSceneManager.IceSpline; worldName = World.WorldName.RoyalFamily; }

        //scooterGameplayControl.SwitchToPlanet(worldName);
        //scooterGameplayControl.scooterAssignment.DoAssign();

        //Debug.LogFormat("PORTAL SCENE NULL: {0} PLANET SCENE {1}", portalScene == null, planetScene.SceneName);
        //if(portalScene != null)
        //{
        //    MIBSceneManager.FinishScene(portalScene);
        //}
        //else
        //{
        //    MIBSceneManager.StartScene(sceneToStart);
        //}

        MIBSceneManager.FinishScene(sceneToFinish);
        MIBSceneManager.StartScene(sceneToStart);

        if (sceneToStart.SceneName.Equals(MIBSceneManager.LondonSpline.SceneName))
        {
            //yield return new WaitForSeconds(1f); 
            //EventManager.Publish(new ScootersRisenEvent());
        }
    }

    public IEnumerator StartAtNYSubstation()
    {
        //yield return StartCoroutine(scenePicker.FadeOut(Artanim.Location.Messages.Transition.FadeBlack, 5));
        readyToSwapScenes.Trigger();
        //HACK + TODO -> cache these or open up the TimelineManager to dig into these
        //...also use proper marker tracks / clips
        //var controller = FindObjectsOfType<PlayableController>().FirstOrDefault(pc => pc.Id.Equals("PlatformController (0)"));
        //controller.GoToTime(17f);
        MasterController.GoToTime(48f);
        MasterController.Play();
        yield return null;
    }

    public IEnumerator StartAtLondonHQ()
    {
        yield return StartCoroutine(scenePicker.FadeOut(Artanim.Location.Messages.Transition.FadeBlack, 1f));

        MasterController.GoToTime(120f); //london subway
        //MasterController.GoToTime(170f); //start of elevator rise
        MasterController.Play();

        MIBSceneManager.StartScene(MIBSceneManager.LondonSubstationScene);

        //allow for some time to ensure the scene has been turned on
        yield return new WaitForSeconds(1f);
        headquartersSubstationControl.FrankAnimator.gameObject.SetActive(true);

        ReadyForLondonHQTrigger.Trigger();
    }

    public void OnReadyForLondonHQTrigger()
    {
        if (!NetworkInterface.Instance.IsServer)
            return;

        numPlayersReadyToSwapScenes++;
        if (numPlayersReadyToSwapScenes == gameController.RuntimePlayers.Count)
        {
            LondonHQNetActivated.Activate();
            Debug.Log("Activating LondonHQ Trigger");
        }
    }

    public void OnLondonHQNetActivated()
    {
        var offset = FindObjectsOfType<AvatarOffset>().Where(ao => ao.ObjectId.Equals("LogoPlatformOffset")).FirstOrDefault();
        foreach (var player in gameController.RuntimePlayers)
        {
            avatarOffsetController.RegisterAvatarOffset(player.AvatarController.PlayerId, offset, true, AvatarOffsetController.ESyncMode.Unsynced);
        }

        //mibSceneManager.StartScene(mibSceneManager.LondonSubstationScene);
        scenePicker.FadeInExternal(0.25f);
    }

    [ContextMenu("StartIntro")]
    public void StartIntro()
    {
        StartCoroutine(StartAtIntro());
    }

    public IEnumerator StartAtIntro()
    {
        //yield return StartCoroutine(scenePicker.FadeOut(Artanim.Location.Messages.Transition.FadeBlack, 2.5f));

        if(DevelopmentMode.CurrentMode == EDevelopmentMode.Standalone)
        {
            AvatarController ac = GameController.Instance.RuntimePlayers[0].AvatarController;
            ac.GetComponent<MIBAvatarDisplayController>().SwitchToStandaloneAvatar();
        }

        scenePicker.ActivateSceneObjects(MIBSceneManager.IntroScene.SceneName);
        scenePicker.SetActiveScene(MIBSceneManager.IntroScene.SceneName);
        scenePicker.StartScene(MIBSceneManager.IntroScene.SceneName);

        //StartCoroutine(scenePicker.FadeIn(2.5f));

        //if (NetworkInterface.Instance.IsClient)
        //{ AudioManager.Instance.PlayTempScore(); }

        yield return null;
    }

    public IEnumerator StartAtScooters()
    {
        yield return StartCoroutine(scenePicker.FadeOut(Artanim.Location.Messages.Transition.FadeBlack, 2.5f));
        //activate Galaxarium
        scenePicker.ActivateSceneObjects(MIBSceneManager.GalaxariumScene.SceneName);
        //Set offset to galaxarium offset
        playerOffsetSource.position = scooterOffsetPos.position;
        playerOffsetSource.gameObject.SetActive(false);
        playerOffsetSource.gameObject.SetActive(true);
        scenePicker.DeActivateSceneObjects(MIBSceneManager.ConstructScene.SceneName);
        //Activate ScootersGameplay
        scenePicker.ActivateSceneObjects(MIBSceneManager.ScootersGameplayScene.SceneName);
        //scooterGameplayControl.ToggleScooterVisuals(true);
        //scooterGameplayControl.ToggleScooterTubes(true);
        //Everything loaded
        yield return StartCoroutine(scenePicker.FadeIn(2.5f));
        //Activate scooter trigger after 5 seconds
        yield return new WaitForSeconds(5);
        //HACK + TODO -> add support for markers, so we don't have to worry about updating this either here or in a config file whenever the playable graph plays...
        //...then we can useour extension methods or events, ie graph.GoToMarker("MARKER_NAME")
        //galaxariumControl.PlayableController.GoToTime(81f);
        //galaxariumControl.PlayableController.Play();
        MIBSceneManager.StartScene(MIBSceneManager.GalaxariumScene);
    }

    public IEnumerator StartAtGalaxarium()
    {
        yield return StartCoroutine(scenePicker.FadeOut(Artanim.Location.Messages.Transition.FadeBlack, 2.5f));
        //activate Galaxarium
        scenePicker.ActivateSceneObjects(MIBSceneManager.GalaxariumScene.SceneName);
        //Set offset to galaxarium offset
        playerOffsetSource.position = scooterOffsetPos.position;
        playerOffsetSource.gameObject.SetActive(false);
        playerOffsetSource.gameObject.SetActive(true);
        //Activate ScootersGameplay
        scenePicker.ActivateSceneObjects(MIBSceneManager.ScootersGameplayScene.SceneName);
        //scooterGameplayControl.ToggleScooterVisuals(true);
        //scooterGameplayControl.ToggleScooterTubes(true);
        //Everything loaded
        yield return StartCoroutine(scenePicker.FadeIn(2.5f));
        //Activate scooter trigger after 5 seconds
        yield return new WaitForSeconds(5f);
        //galaxariumControl.PlayableController.GoToTime(19f);
        //galaxariumControl.ArrivedAtGalaxarium();
        MIBSceneManager.StartScene(MIBSceneManager.GalaxariumScene);
    }

    public void MoveLightProbesToNY()
    {
        lightProbeTransfer.TransferAnchorPoints(true);
    }

    public void StartSuitTransition()
    {
        sceneAnim.SetTrigger("RailsFade");

        lightProbeTransfer.LockPlayerAnchorPoints();
        ConstructController.Play();
        

        //if (NetworkInterface.Instance.IsClient)
        //{ AudioManager.Instance.PlayTempScore(); }

        //EventManager.Publish(new SessionStartedEvent());
    }

    public void TurnOnAllGlasses()
    {
        gameplayManager.TurnOnAllGlasses();
    }

    public void LoadNextScene()
    {
        if (gameController)
        { gameController.LoadGameScene(MIBSceneManager.PlatformScene.SceneName, Artanim.Location.Messages.Transition.FadeWhite); }
    }

    public void FadeOut(float speed)
    {
        //Debug.Log("starting fade out " + speed);
        StartCoroutine(FadeOutRoutine(speed));
    }

    public IEnumerator FadeOutRoutine(float speed)
    {
        //Todo: take out fade out and remove the trigger here since there's no sync needed anymore
        yield return null;//StartCoroutine(scenePicker.FadeOut(Artanim.Location.Messages.Transition.FadeBlack, speed));
        readyToSwapScenes.Trigger();
    }

    public void StartNYTransition()
    {
        //Keeping the server sync here
        readyToSwapScenes.Trigger();
    }

    public void RegisterSceneSwapReady()
    {
        if(NetworkInterface.Instance.IsServer)
        {
            numPlayersReadyToSwapScenes++;
            if (numPlayersReadyToSwapScenes == gameController.RuntimePlayers.Count)
            {
                swapScenesNetActivated.Activate();
                //Debug.Log("Activating scene swap network trigger");
            }
        }
    }

    public void SwapScenes()
    {
        //Teleport to NY offset
        playerOffsetSource.position = NYOffsetRef.position;
        playerOffsetSource.rotation = NYOffsetRef.rotation;
        //mibSceneManager.StartScene(mibSceneManager.NYSubstationScene);

        //ExperienceTiming.Instance.RegisterSectionEnd("Suit Intro");
        SessionTiming.Instance.RegisterSectionEnd("Intro Sequence and Mission Briefing");
        //scenePicker.FadeInExternal(.25f);
    }

    public void LoadConstructLighting()
    {
        Debug.Log("LoadingConstructLighting");
        constructLightLoadHelper.enabled = true;
    }
}