using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Artanim;
using UnityEngine.SceneManagement;
using Artanim.Location.Network;
using UnityEngine.Audio;
//using Artanim.Tools;


public class GameplayManager : SingletonBehaviour<GameplayManager>
{
    public enum GameState
    {
        Start, AtFirstStation, LeavingFirstStation,
        AtSecondStation, MIBPlatformSwitch, HeadingIntoHeadquarters,
        AtGalaxarium, ElevatorArriving, AtHeadquarters, AtOceanStart,
        InTunnelToGalaxarium, OceanToLondonStation, RiseOutOfGalaxarium,
        AtVictoriaTower, AtNurlene, TransitionToGalaxarium,
    };
    //Other Comments

    /// <summary>
    /// Plastic Changes
    /// </summary>

    public GameState m_gameState;
    private const string HEADQUARTERS_ANIM_TAG = "HeadquartersAnimations";
    private MIBAvatarDisplayController[] playerDisplayControllers;
    //[Header("Subway Logic")]
    //public string ON_SUBWAY_TRIGGER_ID = "OnSubwayTrigger";
    //private Collider onSubwayTrigger;
    //public string OFF_SUBWAY_TRIGGER_ID = "OffSubwayTrigger";
    //private Collider offSubwayTrigger;
    //public string ON_PLATFORM_TRIGGER_ID = "PlatformTrigger";
    //private Collider onPlatformTrigger;

    //public enum ObjectTurnOffState { EnteringHeadquarters}
    private List<GameObject> turnOff_EnteringHeadquarters;
    private GameplayManager.GameState carState;
    //public Animator subwayParentAnim;
    //public Animator subwayVehicleAnim;

    [Header("Scene Transitions")]
    public WorldLightingSettings londonLightSettings;

    //Scene Managers
    //private ScooterGameplayControl scooterGameplayControl { get { return ScooterGameplayControl.Instance; } }
    //private LondonControl londonControl { get { return LondonControl.Instance; } }
    private HeadquartersSubstationControl headquartersSubstationControl { get { return HeadquartersSubstationControl.Instance; } }
    private ConstructManager constructManager { get { return ConstructManager.Instance; } }
    private MIBSceneManager mibSceneManager { get { return ConstructManager.Instance.MIBSceneManager; } }
    private ScenePicker scenePicker { get { return ScenePicker.Instance; } }
    private static NetworkInterface networkInterface { get { return NetworkInterface.Instance; } }
    private static GameController gameController { get { return GameController.Instance; } }
    //private ScooterMotionTest scooterTest;

    [Header("Scene Setups")]
    public MultiSceneSetup AllScenesSetup;
    public MultiSceneSetup ConstructSetup;
    public MultiSceneSetup ConstructToNYSetup;
    public MultiSceneSetup IntroSetup;
    public MultiSceneSetup IntroToLondonHQSetup;
    public MultiSceneSetup NYToGalaxariumSetup;
    public MultiSceneSetup NYSetup;
    public MultiSceneSetup GalaxariumSetup;
    public MultiSceneSetup ScootersSetup;
    public MultiSceneSetup LondonToGalaxariumSetup;
    public MultiSceneSetup LondonPlanetSetup;
    public MultiSceneSetup MoonSetup;
    public MultiSceneSetup FacePlanetSetup;
    public MultiSceneSetup IcePlanetSetup;
    public MultiSceneSetup ZarthanHomeSetup;
    public MultiSceneSetup ScreenCaptureSetup;

    [Header("Mixer Snapshots")]
    public AudioMixerSnapshot defaultMixerSnapshot;
    public AudioMixerSnapshot noMusicMixerSnapshot;

    [Header("TimelineControl")]
    public TimelineControl debugTimelineControl;

    public static bool IsServerOnly()
    {
        return networkInterface != null && networkInterface.IsServer && !networkInterface.IsClient;
    }

    private void Awake()
    {
        turnOff_EnteringHeadquarters = new List<GameObject>();
    }

    private void Start()
    {
        //if (DevelopmentMode.CurrentMode == EDevelopmentMode.Standalone && GameController.Instance)
        //{ GameController.Instance.SetReadyForSession(); }
    }

    private void OnEnable()
    {
        if(gameController)
        {
            gameController.OnLeftSession += Instance_OnLeftSession;
        }
    }

    private void OnDisable()
    {
        if(gameController)
        {
            gameController.OnLeftSession -= Instance_OnLeftSession;
        }
    }

    private void Instance_OnLeftSession()
    {
        #if !UNITY_EDITOR
        Application.Quit();
        #endif
    }

    

#region Construct
    public void StartIfStandalone()
    {
        //if (DevelopmentMode.CurrentMode == EDevelopmentMode.Standalone)
        //    Invoke("StartExperienceDefault", 10);
    }

    public void TurnOnAllGlasses()
    {
        List<RuntimePlayer> runtimePlayers = gameController.RuntimePlayers;
        playerDisplayControllers = new MIBAvatarDisplayController[runtimePlayers.Count];
        for(int i = 0; i < playerDisplayControllers.Length; ++i)
        {
            playerDisplayControllers[i] = runtimePlayers[i].AvatarController.GetComponent<MIBAvatarDisplayController>();
        }
        if (playerDisplayControllers != null)
        {
            foreach (MIBAvatarDisplayController controller in playerDisplayControllers)
            {
                if (controller)
                    controller.TurnOffCasualRoot();
            }
        }
    }

#if UNITY_EDITOR
    [ContextMenu("Start Experience (Default)")]
#endif
    public void StartExperienceDefault()
    {
        StartCoroutine(StartSession(AllScenesSetup, ConstructManager.StartScene.DEFAULT));
    }

#if UNITY_EDITOR
    [ContextMenu("Start Construct Only")]
#endif
    public void StartConstruct()
    {
        StartCoroutine(StartSession(ConstructSetup, ConstructManager.StartScene.DEFAULT));
    }

#if UNITY_EDITOR
    [ContextMenu("Start Construct To NY")]
#endif
    public void StartConstructToNY()
    {
        StartCoroutine(StartSession(ConstructToNYSetup, ConstructManager.StartScene.DEFAULT));
    }

#if UNITY_EDITOR
    [ContextMenu("Start Intro Only")]
#endif
    public void StartIntro()
    {
        StartCoroutine(StartSession(IntroSetup, ConstructManager.StartScene.INTRO));
    }

#if UNITY_EDITOR
    [ContextMenu("Start Intro To London HQ")]
#endif
    public void StartIntroToLondonHQ()
    {
        StartCoroutine(StartSession(IntroToLondonHQSetup, ConstructManager.StartScene.INTRO));
    }

#if UNITY_EDITOR
    [ContextMenu("Start NY (Full)")]
#endif
    public void StartNYFull()
    {
        StartCoroutine(StartSession(AllScenesSetup, ConstructManager.StartScene.NYSUBSTATION));
    }

#if UNITY_EDITOR
    [ContextMenu("Start NY To Galaxarium")]
#endif
    public void StartNYToGalaxarium()
    {
        StartCoroutine(StartSession(NYToGalaxariumSetup, ConstructManager.StartScene.NYSUBSTATION));
    }

#if UNITY_EDITOR
    [ContextMenu("Start NY (Subway Only)")]
#endif
    public void StartNYOnly()
    {
        StartCoroutine(StartSession(NYSetup, ConstructManager.StartScene.NYSUBSTATION));
    }

#if UNITY_EDITOR
    [ContextMenu("Start London To Galaxarium")]
#endif
    public void StartLondonToGalaxarium()
    {
        StartCoroutine(StartSession(LondonToGalaxariumSetup, ConstructManager.StartScene.LONDONHQ));
    }

#if UNITY_EDITOR
    [ContextMenu("Start Scooters")]
#endif
    public void StartScooters()
    {
        StartCoroutine(StartSession(ScootersSetup, ConstructManager.StartScene.SCOOTERS));
    }

#if UNITY_EDITOR
    [ContextMenu("Start Galaxarium")]
#endif
    public void StartGalaxarium()
    {
        StartCoroutine(StartSession(GalaxariumSetup, ConstructManager.StartScene.GALAXARIUM));
    }

#if UNITY_EDITOR
    [ContextMenu("Start London Tower")]
#endif
    public void StartLondonTower()
    {
        StartCoroutine(StartSession(LondonPlanetSetup, ConstructManager.StartScene.LONDONTOWER));
    }

#if UNITY_EDITOR
    [ContextMenu("Start London Portal")]
#endif
    public void StartLondonPortal()
    {
        StartCoroutine(StartSession(FacePlanetSetup, ConstructManager.StartScene.LONDONPORTAL));
    }

//#if UNITY_EDITOR
//    [ContextMenu("Start Moon")]
//#endif
//    public void StartMoon()
//    {
//        StartCoroutine(StartSession(MoonSetup, ConstructManager.StartScene.MOON));
//    }

#if UNITY_EDITOR
    [ContextMenu("Start Face Planet")]
#endif
    public void StartFacePlanet()
    {
        StartCoroutine(StartSession(FacePlanetSetup, ConstructManager.StartScene.FACEPLANET));
    }

#if UNITY_EDITOR
    [ContextMenu("Start Face Portal")]
#endif
    public void StartFacePortal()
    {
        StartCoroutine(StartSession(IcePlanetSetup, ConstructManager.StartScene.FACEPORTAL));
    }

#if UNITY_EDITOR
    [ContextMenu("Start Ice Planet")]
#endif
    public void StartIcePlanet()
    {
        StartCoroutine(StartSession(IcePlanetSetup, ConstructManager.StartScene.ICEPLANET));
    }

#if UNITY_EDITOR
    [ContextMenu("Start Ice Portal")]
#endif
    public void StartIcePortal()
    {
        StartCoroutine(StartSession(ZarthanHomeSetup, ConstructManager.StartScene.ICEPORTAL));
    }

#if UNITY_EDITOR
    [ContextMenu("Start Zarthan Home")]
#endif
    public void StartZarthanHome()
    {
        StartCoroutine(StartSession(ZarthanHomeSetup, ConstructManager.StartScene.ZARTHANHOME));
    }

    IEnumerator StartSession(MultiSceneSetup sceneSetup, ConstructManager.StartScene startScene)
    {
        if(NetworkInterface.Instance.IsClient)
        {
            string audioMix = ConfigService.Instance.ExperienceConfig.GetPropertyString("AUDIOMIX");
            if (audioMix != null)
            {
                switch (audioMix)
                {
                    case ("NOMUSIC"):
                        noMusicMixerSnapshot.TransitionTo(0);
                        break;
                    default:
                        defaultMixerSnapshot.TransitionTo(0);
                        break;

                }
            }
        }
#if UNITY_EDITOR
        if (DevelopmentMode.CurrentMode == EDevelopmentMode.Standalone)
        {
            debugTimelineControl.enabled = true;
            yield return StartCoroutine(scenePicker.LoadSceneRoutine(sceneSetup));
        }
#else
        if (DevelopmentMode.CurrentMode == EDevelopmentMode.Standalone)
        {
            debugTimelineControl.enabled = true;
        }
        yield return null;
#endif
        constructManager.sceneToStartIn = startScene;
        if (startScene != ConstructManager.StartScene.DEFAULT && startScene != ConstructManager.StartScene.INTRO)
        {
            //StartExperience();
            //constructManager.StartExperience();
            gameController.RuntimePlayers[0].AvatarController.GetComponent<MIBAvatarDisplayController>().SwitchToStandaloneAvatar();
        }
        constructManager.StartExperience();

    }

    public void LoadLightHelperConstruct()
    {
        ConstructManager.Instance.LoadConstructLighting();
    }
#endregion

#region HeadquartersAndSubstation
    public void ActivateNYScene()
    {
        mibSceneManager.StartScene(mibSceneManager.NYSubstationScene);
    }

    public void TransferLightProbesToNY()
    {
        constructManager.MoveLightProbesToNY();
    }

    public void SetStateToTransitionToGalaxarium()
    {
        SetCarState(GameState.TransitionToGalaxarium);
    }

    //HACK + TODO making these explicit methods as we are calling them from timeline and there seem to be occasionally issues with parameterized timeline events
    public void SetStateToTunnel()
    {
        SetCarState(GameState.InTunnelToGalaxarium);
    }

    public void SetStateToArrivedAtGalaxarium()
    {
        SetCarState(GameState.AtGalaxarium);
    }

    public void SetCarState(GameplayManager.GameState state)
    {
        carState = state;
        switch (carState)
        {
            case GameplayManager.GameState.AtFirstStation:
                //subwayVehicleAnim.SetTrigger("OpenRegularDoor");
                //onSubwayTrigger.enabled = true;
                headquartersSubstationControl.AtFirstStation();
                break;
            case GameplayManager.GameState.AtOceanStart:
                SessionTiming.Instance.RegisterSectionEnd("NY Subway");
                mibSceneManager.StartScene(mibSceneManager.OceanFloorScene);
                headquartersSubstationControl.AtOceanStart();
                break;
            case GameplayManager.GameState.OceanToLondonStation:
                SessionTiming.Instance.RegisterSectionEnd("Ocean Travel");
                mibSceneManager.StartScene(mibSceneManager.LondonSubstationScene);
                break;
            case GameplayManager.GameState.AtSecondStation:
                //subwayVehicleAnim.SetTrigger("OpenSecondDoor");
                //offSubwayTrigger.enabled = true;
                headquartersSubstationControl.AtSecondStation();
                ScenePicker.Instance.UnloadScene(mibSceneManager.OceanFloorScene);
                break;
            case GameplayManager.GameState.MIBPlatformSwitch:
                //onPlatformTrigger.enabled = true;
                headquartersSubstationControl.MIBPlatformSwitch();
                break;
            case GameplayManager.GameState.HeadingIntoHeadquarters:
                SessionTiming.Instance.RegisterSectionEnd("London Station");
                Debug.Log("Set Active scene to Headquarters");
                mibSceneManager.StartScene(mibSceneManager.HeadquartersScene);
                //headquartersSubwayControl.HeadingIntoHeadquarters();
                break;
            case GameState.AtNurlene:
                //headquartersSubwayControl.DetachMIBPlatform();
                break;
            case GameplayManager.GameState.TransitionToGalaxarium:
                //mibSceneManager.StartScene(mibSceneManager.GalaxariumScene);
                break;
            case GameplayManager.GameState.InTunnelToGalaxarium:
                SessionTiming.Instance.RegisterSectionEnd("Headquarters");
                mibSceneManager.StartScene(mibSceneManager.GalaxariumScene);
                break;
            case GameplayManager.GameState.AtGalaxarium:
                //GalaxariumControl.Instance.ArrivedAtGalaxarium();
                break;
        }
    }
#endregion

#region Galaxarium

    public void StartScooterSection()
    {
        //TODO -> move to event
        StartCoroutine(StartScooterSectionSequence()); 
    }

    public IEnumerator StartScooterSectionSequence()
    {
        Debug.Log("starting scooters");
        mibSceneManager.StartScene(mibSceneManager.GalaxariumToLondonScene);
        //scooterGameplayControl.ScooterRisesAudio();
        yield return new WaitForSeconds(10.75f);
        Debug.Log("starting london");
        mibSceneManager.StartScene(mibSceneManager.LondonSpline);
    }
#endregion

    private IEnumerator StartLondonSceneSwap()
    {
        yield return new WaitForSeconds(1);
        Debug.Log("<color=blue>Unloading headquarters.</color>");
        scenePicker.DeActivateSceneObjects(mibSceneManager.PlatformScene);
        Debug.Log("<color=blue>Headquarters Unloaded. Loading London.</color>");
        scenePicker.ActivateSceneObjects(mibSceneManager.LondonSpline);
        londonLightSettings.SwapLightSettings();
    }
}