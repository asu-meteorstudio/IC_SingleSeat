using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Artanim;
using UnityEngine.Playables;
using System.Linq;
using UniRx;

public class HeadquartersSubstationControl : DreamscapeSceneControl
{
    #region Singleton Behaviour
    private static HeadquartersSubstationControl _instance;
    public static bool HasInstance
    {
        get
        {
            return _instance != null;
        }
    }
    public static HeadquartersSubstationControl Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<HeadquartersSubstationControl>();
            }
            return _instance;
        }

        protected set
        {
            _instance = value;
        }
    }
    #endregion

    private const string HEADQUARTERS_ANIM_TAG = "HeadquartersAnimations";
    [Header("Subway Logic")]
    public string ON_SUBWAY_TRIGGER_ID = "OnSubwayTrigger";
    //private Collider onSubwayTrigger;
    public string OFF_SUBWAY_TRIGGER_ID = "OffSubwayTrigger";
    //private Collider offSubwayTrigger;
    public string ON_PLATFORM_TRIGGER_ID = "PlatformTrigger";
    private Collider onPlatformTrigger;
    public GameObject subwayLeverTrackedRigid;

    public enum ObjectTurnOffState { EnteringHeadquarters }
    private List<GameObject> turnOff_EnteringHeadquarters;

    public GameObject platformButton;
    //public Animator subwayVehicleAnim;
    public Transform mibPlatform;

    [Header("World Light Settings")]
    public WorldLightingSettings oceanFloorLighting;
    public WorldLightingSettings londonSubstationLighting;

    public PlayableController PlayableController;
    public Animator FrankAnimator;

    public GameObject subwayRunningLights;

    ScenePicker scenePicker { get { return ScenePicker.Instance; } }
    MIBSceneManager mibSceneManager { get { return ConstructManager.Instance.MIBSceneManager; } }

    protected override void Awake()
    {
        base.Awake();

        turnOff_EnteringHeadquarters = new List<GameObject>();
        subwayLeverTrackedRigid.SetActive(false);
    }

    void Start ()
    {
        foreach (AvatarArea trigger in GameObject.FindObjectsOfType<AvatarArea>())
        {
            if (trigger.ObjectId.Equals(ON_SUBWAY_TRIGGER_ID))
            { /*onSubwayTrigger = trigger.GetComponent<Collider>();*/ }
            else if (trigger.ObjectId.Equals(OFF_SUBWAY_TRIGGER_ID))
            { /*offSubwayTrigger = trigger.GetComponent<Collider>();*/ }
            else if (trigger.ObjectId.Equals(ON_PLATFORM_TRIGGER_ID))
                onPlatformTrigger = trigger.GetComponent<Collider>();
        }
    }

    public override void OnEnable()
    {
        base.OnEnable();

        EventManager.OnEvent<FrankFinishedNYEvent>().Subscribe(_ =>
        {
            FrankAnimator.gameObject.SetActive(true);
            FrankAnimator.Play(FrankStates.SubwayIdle);
        }).AddTo(Disposer);
    }

    public override void SceneTurnOn()
    {
        base.SceneTurnOn();

        //FrankAnimator.gameObject.SetActive(false);

        //PlayableController.GoToTime(0f);
        //PlayableController.Play();
    }

    public override void SceneTurnOff()
    {
        base.SceneTurnOff();

        //FrankAnimator.gameObject.SetActive(false);
    }

    public void AtFirstStation()
    {
        //subwayVehicleAnim.SetTrigger("OpenRegularDoor");
        //onSubwayTrigger.enabled = true;
        subwayLeverTrackedRigid.SetActive(true);
    }

    public void AtOceanStart()
    {
        //oceanFloorLighting.SwapLightSettings();
    }
    

    public void AtSecondStation()
    {
        //NY_Substation.SetActive(false);
        //subwayVehicleAnim.SetTrigger("OpenSecondDoor");
        //offSubwayTrigger.enabled = true;
    }

    public void DetachMIBPlatform()
    {
        //mibPlatform.SetParent(mainGeoAndLighting.transform, true);
    }

    public void MIBPlatformSwitch()
    {
        onPlatformTrigger.enabled = true;
    }

    public void TurnOnButton()
    {
        platformButton.SetActive(true);
    }

    public void HeadingIntoHeadquarters()
    {
        //Headquarters.SetActive(true);
        //StartCoroutine(TurnOnHeadquartersAnimations());
    }

    public void AddToHeadquartersTurnOffList(ObjectTurnOffControl turnOff)
    {
        switch (turnOff.whenToTurnOff)
        {
            case ObjectTurnOffState.EnteringHeadquarters:
                turnOff_EnteringHeadquarters.Add(turnOff.gameObject);
                break;
        }
    }

    public void TransitioningToGalaxarium()
    {
        EventManager.Publish(new TransitioningToGalaxariumEvent());
    }

    private IEnumerator TurnOnHeadquartersAnimations()
    {
        foreach (GameObject go in turnOff_EnteringHeadquarters)
            go.SetActive(false);
        Transform animations = GameObject.FindGameObjectWithTag(HEADQUARTERS_ANIM_TAG).transform;
        foreach (Transform transform in animations)
        {
            transform.gameObject.SetActive(true);
            yield return new WaitForSeconds(.5f);
            Debug.Log("");
        }
    }

    /*public void ToggleAvatarWarp(bool isTarget)
    {
        //var avatarControllers = GameController.Instance.RuntimePlayers.Select(rp => rp.AvatarController);
        var avatarControllers = FindObjectsOfType<AvatarController>();
        foreach (var ac in avatarControllers)
        {
            var shaderSwapper = ac.GetComponent<ShaderSwapper>();
            var vertController = ac.GetComponent<VertexDisplacementController>();
            if (shaderSwapper && vertController)
            {
                shaderSwapper.SwapShaders(isTarget);
                if (isTarget)
                {
                    vertController.Speed = 20f;
                    vertController.Scale = 0.005f;
                }
            }
        }
    }*/

    public void ActivateLondonStation()
    {
        scenePicker.ActivateSceneObjects(mibSceneManager.LondonSubstationScene.SceneName);
    }

    public void SwitchToLondonStationLightSettings()
    {
        scenePicker.SetActiveScene(mibSceneManager.LondonSubstationScene.SceneName);
    }

    public void StartFrankSubtitles()
    {
        StartCoroutine(FrankSubtitles());
    }

    public void PlayFrankArchieSubtitle()
    {
        Artanim.SubtitleController.Instance.ShowSubtitle("033_FrankItsAlrightArchie", true, 2.24f, false, false, false);
    }

    IEnumerator FrankSubtitles()
    {
        Artanim.SubtitleController.Instance.ShowSubtitle("043_FrankGoodLuckRookies", true, 2.5f, false, false, false);
        yield return new WaitForSeconds(2.5f);
        Artanim.SubtitleController.Instance.ShowSubtitle("044_FrankThatWeDontTake", true, 5f, false, false, false);
    }

    public void ToggleRunningLights(bool state)
    {
        Debug.Log("Toggle running lights. State: " + state);
        subwayRunningLights.SetActive(state);
    }
}

public class TransitioningToGalaxariumEvent { }
