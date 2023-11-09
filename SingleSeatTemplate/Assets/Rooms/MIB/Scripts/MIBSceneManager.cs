using System.Collections.Generic;
using UnityEngine;
using UniRx;
using System.Linq;

//[CreateAssetMenu(menuName = "Scriptable Reference/MIBSceneManager")]
//public class MIBSceneManager : ReactiveObject
public class MIBSceneManager : ReactiveBehaviour
{
    [Header("Main Scenes")]
    public SceneReference ConstructScene;
    public SceneReference IntroScene;
    public SceneReference GalaxariumScene;
    public SceneReference PlatformScene;
    //public SceneReference LondonWorldScene;
    public SceneReference NYSubstationScene;
    public SceneReference LondonSubstationScene;
    public SceneReference HeadquartersScene;
    public SceneReference HeadquartersToGalaxariumScene;
    public SceneReference GalaxariumToLondonScene;
    public SceneReference OceanFloorScene;
    public SceneReference ScootersGameplayScene;
    public SceneReference ZarthanHomeScene;
    public SceneReference ConstructEndingScene;

    [Header("Planet Splines")]
    public SceneReference LondonSpline;
    public SceneReference MoonSpline;
    public SceneReference FaceSpline;
    public SceneReference IceSpline;

    [Header("Planet Portals")]
    public SceneReference LondonPortalSpline;
    public SceneReference MoonPortalSpline;
    public SceneReference FacePortalSpline;
    public SceneReference IcePortalSpline;

    [ReadOnly] public HashSet<SceneReference> PlanetScenes;
    [ReadOnly] public HashSet<SceneReference> PortalScenes;
    [ReadOnly] public List<SceneReference> PortalPlanetScenes;
    //public Dictionary<SceneReference, SceneReference> FromToSceneTable;
    //Dictionary<string, int> sceneToIndices = new Dictionary<string, int>();

    ScenePicker scenePicker { get { return ScenePicker.Instance; } }
    //ScooterGameplayControl scooterController { get { return ScooterGameplayControl.Instance; } }

    //public MIBEventStreams MIBEventStreams;

    public override void OnEnable()
    {
        base.OnEnable();
    //protected override void OnBegin()
    //{
    //    base.OnBegin();

        //HACK + TODO -> put these in an array and just increment???
        //EventManager.OnEvent<LevelFinishedEvent>().Subscribe(evt =>
        //{
        //    if(evt.SceneReference.SceneName.Equals(LondonSpline.SceneName))
        //    {
        //        StartScene(LondonPortalSpline);
        //    }
        //    else if(evt.SceneReference.SceneName.Equals(LondonPortalSpline.SceneName))
        //    {
        //        StartScene(MoonSpline);
        //    }
        //    else if (evt.SceneReference.SceneName.Equals(MoonSpline.SceneName))
        //    {
        //        StartScene(MoonPortalSpline);
        //    }
        //    else if (evt.SceneReference.SceneName.Equals(MoonPortalSpline.SceneName))
        //    {
        //        StartScene(FaceSpline);
        //    }
        //    else if (evt.SceneReference.SceneName.Equals(FaceSpline.SceneName))
        //    {
        //        StartScene(FacePortalSpline);
        //    }
        //    else if (evt.SceneReference.SceneName.Equals(FacePortalSpline.SceneName))
        //    {
        //        StartScene(IceSpline);
        //    }
        //    else if (evt.SceneReference.SceneName.Equals(IceSpline.SceneName))
        //    {
        //        StartScene(ZarthanHomeScene);
        //    }
        //    else if (evt.SceneReference.SceneName.Equals(ZarthanHomeScene.SceneName))
        //    {
        //        StartScene(ConstructEndingScene);
        //    }
        //}).AddTo(Disposer);

        PlanetScenes = new HashSet<SceneReference>(new SceneReference[] { LondonSpline, MoonSpline, FaceSpline, IceSpline });
        PortalScenes = new HashSet<SceneReference>(new SceneReference[] { LondonPortalSpline, MoonPortalSpline, FacePortalSpline, IcePortalSpline });
        PortalPlanetScenes = new List<SceneReference> { LondonSpline, LondonPortalSpline, MoonSpline, MoonPortalSpline, FaceSpline, FacePortalSpline, IceSpline, ZarthanHomeScene, ConstructEndingScene };
        //for(var i = 0; i < PortalPlanetScenes.Count; i++)
        //{
        //    sceneToIndices.Add(PortalPlanetScenes[i].SceneName, i);
        //}
        //FromToSceneTable = new Dictionary<SceneReference, SceneReference>
        //{
        //    { LondonSpline, LondonPortalSpline },
        //    { LondonPortalSpline, MoonSpline },
        //    { MoonSpline, MoonPortalSpline },
        //    { MoonPortalSpline, FaceSpline },
        //    { FaceSpline, FacePortalSpline },
        //    { FacePortalSpline, IceSpline },
        //    { IceSpline , ZarthanHomeScene },
        //    { ZarthanHomeScene, ConstructEndingScene }
        //};

        //MIBEventStreams.SessionStartedStream.Subscribe(_ =>
        //{

        //}).AddTo(Disposer);
    }

    public SceneReference TryGetSceneReference(string sceneName)
    {
        return PlanetScenes.FirstOrDefault(sr => sr.SceneName.Equals(sceneName)) ?? PortalScenes.FirstOrDefault(sr => sr.SceneName.Equals(sceneName));
    }

    public void FinishScene(SceneReference sceneReference)
    {
        EventManager.Publish(new SceneFinishedEvent(sceneReference));
        if (sceneReference.SceneName.Equals(GalaxariumScene.SceneName))
        {
            StartScene(LondonSpline);
        }
        else if (sceneReference.SceneName.Equals(LondonSpline.SceneName))
        {
            StartScene(LondonPortalSpline);
        }
        else if (sceneReference.SceneName.Equals(LondonPortalSpline.SceneName))
        {
            //StartScene(MoonSpline);
            StartScene(FaceSpline);
        }
        else if (sceneReference.SceneName.Equals(MoonSpline.SceneName))
        {
            StartScene(MoonPortalSpline);
        }
        else if (sceneReference.SceneName.Equals(MoonPortalSpline.SceneName))
        {
            StartScene(FaceSpline);
        }
        else if (sceneReference.SceneName.Equals(FaceSpline.SceneName))
        {
            StartScene(FacePortalSpline);
        }
        else if (sceneReference.SceneName.Equals(FacePortalSpline.SceneName))
        {
            StartScene(IceSpline);
        }
        else if (sceneReference.SceneName.Equals(IceSpline.SceneName))
        {
            StartScene(IcePortalSpline);
        }
        else if (sceneReference.SceneName.Equals(IcePortalSpline.SceneName))
        {
            StartScene(ZarthanHomeScene);
        }
        else if (sceneReference.SceneName.Equals(ZarthanHomeScene.SceneName))
        {
            StartScene(ConstructEndingScene);
        }
    }

    public void StartScene(SceneReference sceneReference)
    {
        if (sceneReference == NYSubstationScene)
        {
            scenePicker.DeActivateSceneObjects(ConstructScene.SceneName);
            scenePicker.DeActivateSceneObjects(IntroScene.SceneName);
            scenePicker.ActivateSceneObjects(OceanFloorScene.SceneName);
            scenePicker.ActivateSceneObjects(PlatformScene.SceneName);
            scenePicker.ActivateSceneObjects(NYSubstationScene.SceneName);
            scenePicker.ActivateSceneLoadedTrigger(PlatformScene.SceneName);
            scenePicker.SetActiveScene(NYSubstationScene.SceneName);
        }
        else if (sceneReference == OceanFloorScene)
        {
            scenePicker.DeActivateSceneObjects(NYSubstationScene.SceneName);
            //scenePicker.ActivateSceneObjects(LondonSubstationScene.SceneName);
            //scenePicker.SetActiveScene(oceanFloorName.SceneName);
        }
        else if (sceneReference == LondonSubstationScene)
        {
            scenePicker.DeActivateSceneObjects(OceanFloorScene.SceneName);
            //scenePicker.UnloadScene(NYSubstationScene.SceneName);
            scenePicker.DeActivateSceneObjects(NYSubstationScene.SceneName);

            scenePicker.ActivateSceneObjects(PlatformScene.SceneName);
            scenePicker.ActivateSceneObjects(HeadquartersScene.SceneName);
            scenePicker.ActivateSceneObjects(HeadquartersToGalaxariumScene.SceneName);

            //scenePicker.SetActiveScene(LondonSubstationScene.SceneName);
        }
        else if (sceneReference == HeadquartersScene)
        {
            scenePicker.SetActiveScene(HeadquartersScene.SceneName);
            scenePicker.ActivateSceneObjects(HeadquartersScene.SceneName);
            scenePicker.ActivateSceneObjects(HeadquartersToGalaxariumScene.SceneName);
        }
        else if (sceneReference == GalaxariumScene)
        {
            scenePicker.DeActivateSceneObjects(LondonSubstationScene.SceneName);
            scenePicker.DeActivateSceneObjects(HeadquartersScene.SceneName);

            //scooterController.ToggleScooterVisuals(true);
            //scooterController.ToggleScooterTubes(true);

            scenePicker.ActivateSceneObjects(GalaxariumScene.SceneName);
            scenePicker.ActivateSceneObjects(GalaxariumToLondonScene.SceneName);
            scenePicker.ActivateSceneObjects(ScootersGameplayScene.SceneName);
            scenePicker.SetActiveScene(GalaxariumScene.SceneName);
        }
        else if (sceneReference == LondonSpline)
        {
            scenePicker.DeActivateSceneObjects(PlatformScene.SceneName);
            scenePicker.DeActivateSceneObjects(GalaxariumScene.SceneName);
            //scenePicker.DeActivateSceneObjects(HeadquartersToGalaxariumScene.SceneName);

            scenePicker.ActivateSceneObjects(LondonSpline.SceneName);
            scenePicker.ActivateSceneObjects(LondonPortalSpline.SceneName);

            scenePicker.SetActiveScene(LondonSpline.SceneName);
            scenePicker.StartScene(LondonSpline.SceneName);
        }
        else if (sceneReference == LondonPortalSpline)
        {
            scenePicker.DeActivateSceneObjects(HeadquartersToGalaxariumScene.SceneName);
            scenePicker.DeActivateSceneObjects(LondonSpline.SceneName);
            //scooterController.ToggleScooterTubes(false);

            scenePicker.ActivateSceneObjects(LondonPortalSpline.SceneName);

            scenePicker.SetActiveScene(LondonPortalSpline.SceneName);
            scenePicker.StartScene(LondonPortalSpline.SceneName);
        }
        else if (sceneReference == MoonSpline)
        {
            scenePicker.DeActivateSceneObjects(LondonPortalSpline.SceneName);

            scenePicker.ActivateSceneObjects(MoonSpline.SceneName);
            scenePicker.ActivateSceneObjects(MoonPortalSpline.SceneName);

            scenePicker.SetActiveScene(MoonSpline.SceneName);
            scenePicker.StartScene(MoonSpline.SceneName);
        }
        else if (sceneReference == MoonPortalSpline)
        {
            scenePicker.DeActivateSceneObjects(MoonSpline.SceneName);

            scenePicker.ActivateSceneObjects(MoonPortalSpline.SceneName);
            //scenePicker.ActivateSceneObjects(FaceSpline.SceneName);

            scenePicker.SetActiveScene(MoonPortalSpline.SceneName);
            //scenePicker.SetActiveScene(FaceSpline.SceneName);
        }
        else if (sceneReference == FaceSpline)
        {
            scenePicker.DeActivateSceneObjects(LondonPortalSpline.SceneName);

            scenePicker.ActivateSceneObjects(FaceSpline.SceneName);
            scenePicker.ActivateSceneObjects(FacePortalSpline.SceneName);

            scenePicker.SetActiveScene(FaceSpline.SceneName);
            scenePicker.StartScene(FaceSpline.SceneName);
        }
        else if (sceneReference == FacePortalSpline)
        {
            scenePicker.DeActivateSceneObjects(LondonPortalSpline.SceneName);
            scenePicker.DeActivateSceneObjects(FaceSpline.SceneName);

            scenePicker.ActivateSceneObjects(FacePortalSpline.SceneName);

            scenePicker.SetActiveScene(FacePortalSpline.SceneName);
            scenePicker.StartScene(FacePortalSpline.SceneName);
        }
        else if (sceneReference == IceSpline)
        {
            scenePicker.DeActivateSceneObjects(FacePortalSpline.SceneName);

            scenePicker.ActivateSceneObjects(IceSpline.SceneName);
            scenePicker.ActivateSceneObjects(IcePortalSpline.SceneName);

            scenePicker.SetActiveScene(IceSpline.SceneName);
            scenePicker.StartScene(IceSpline.SceneName);
        }
        else if (sceneReference == IcePortalSpline)
        {
            scenePicker.DeActivateSceneObjects(FacePortalSpline.SceneName);
            scenePicker.DeActivateSceneObjects(IceSpline.SceneName);

            scenePicker.ActivateSceneObjects(IcePortalSpline.SceneName);

            scenePicker.SetActiveScene(IcePortalSpline.SceneName);
            scenePicker.StartScene(IcePortalSpline.SceneName);
        }
        else if (sceneReference == ZarthanHomeScene)
        {
            scenePicker.DeActivateSceneObjects(IcePortalSpline.SceneName);

            scenePicker.ActivateSceneObjects(ZarthanHomeScene.SceneName);

            scenePicker.SetActiveScene(ZarthanHomeScene.SceneName);
            scenePicker.StartScene(ZarthanHomeScene.SceneName);
        }
        else if (sceneReference == ConstructEndingScene)
        {
            scenePicker.DeActivateSceneObjects(IcePortalSpline.SceneName);
            scenePicker.DeActivateSceneObjects(ZarthanHomeScene.SceneName);

            scenePicker.ActivateSceneObjects(ConstructEndingScene.SceneName);

            scenePicker.SetActiveScene(ConstructEndingScene.SceneName);
            scenePicker.StartScene(ConstructEndingScene.SceneName);
        }

        EventManager.Publish(new SceneStartedEvent(sceneReference));
    }
}