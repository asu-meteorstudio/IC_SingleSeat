using UnityEngine;
using System.Collections.Generic;
using Artanim;
using UnityEngine.Timeline;
using UnityEngine.Playables;
using System.Linq;

[ExecuteInEditMode]
public class TimelineManager : SingletonBehaviour<TimelineManager>
{
    [SerializeField]
    List<PlayableController> controllers = new List<PlayableController>();

    Dictionary<string, PlayableController> idsToControllersTable = new Dictionary<string, PlayableController>();
    Dictionary<PlayableController, List<ControlTrack>> controllersToControlTracksTable = new Dictionary<PlayableController, List<ControlTrack>>();
    Dictionary<PlayableController, PlayableController> childToParentTable = new Dictionary<PlayableController, PlayableController>();

    [SerializeField]
    bool logMessages;

    void Awake()
    {
        ResetData();
    }

    public void Register(PlayableController controller)
    {
        if(logMessages) Debug.LogFormat("Registering {0} on {1}", controller.Id, controller.name);
        //if (controllers.Contains(controller))
        //    return;

        controllers.Add(controller);
        PlayableController tmp;
        if (idsToControllersTable.TryGetValue(controller.Id, out tmp))
        {
            idsToControllersTable[controller.Id] = controller;
        }
        else
        {
            idsToControllersTable.Add(controller.Id, controller);
        }

        var controllables = new List<ControlTrack>(); 
        controllersToControlTracksTable.Add(controller, controllables);

        foreach (var binding in controller.PlayableDirector.playableAsset.outputs)
        {
            if (binding.sourceObject is ControlTrack)
            {
                controllables.Add((ControlTrack)binding.sourceObject);
            }
        }

        foreach(var c in controllers)
        {
            if(c == null)
            { continue; }
            TryLinkDependencies(c);
        }
    }

    public void Deregister(PlayableController controller)
    {
        controllers.Remove(controller);
        idsToControllersTable.Remove(controller.Id);
        controllersToControlTracksTable.Remove(controller);
        childToParentTable.Remove(controller);
    }

    [ContextMenu("Reset Data")]
    public void ResetData()
    {
        var masterController = controllers.Where(pc => pc != null).FirstOrDefault(pc => pc.Id.Equals("MasterController"));
        if(masterController != null)
        {
            foreach (var binding in masterController.PlayableDirector.playableAsset.outputs)
            {
                if (binding.sourceObject is ControlTrack)
                {
                    var controlTrack = (ControlTrack)binding.sourceObject;
                    foreach (var clip in controlTrack.GetClips())
                    {
                        var playableClip = (ControlPlayableAsset)clip.asset;
                        masterController.PlayableDirector.SetReferenceValue(playableClip.sourceGameObject.exposedName, null);
                    }
                }
            }
        }

        controllers.Clear();
        idsToControllersTable.Clear();
        controllersToControlTracksTable.Clear();
        childToParentTable.Clear();
    }

#if UNITY_EDITOR
    [ContextMenu("Try Link Dependencies")]
    public void TryLinkDependenciesEditor()
    {
        ResetData();

        var scenes = new UnityEngine.SceneManagement.Scene[UnityEngine.SceneManagement.SceneManager.sceneCount];
        for (var i = 0; i < scenes.Length; i++)
        {
            scenes[i] = UnityEngine.SceneManagement.SceneManager.GetSceneAt(i);
        }
        foreach (var scene in scenes)
        {
            if (!scene.isLoaded)
                continue;

            foreach (var go in scene.GetRootGameObjects())
            {
                var controllers = go.GetComponentsInChildren<PlayableController>(true);
                foreach (var controller in controllers)
                    Register(controller);
            }
        }
    }
#endif

    public void TryLinkDependencies(PlayableController controller)
    {
        List<ControlTrack> controlTracks;
        if (!controllersToControlTracksTable.TryGetValue(controller, out controlTracks))
            return;

        foreach(var controlTrack in controlTracks)
        {
            PlayableController childController;
            if(idsToControllersTable.TryGetValue(controlTrack.name, out childController))
            {
                PlayableController tmp;
                if(childToParentTable.TryGetValue(childController, out tmp))
                {
                    childToParentTable[childController] = controller;
                }
                else
                {
                    childToParentTable.Add(childController, controller);
                }
                foreach (var clip in controlTrack.GetClips())
                {
                    var playableClip = (ControlPlayableAsset)clip.asset;
                    controller.PlayableDirector.SetReferenceValue(playableClip.sourceGameObject.exposedName, childController.gameObject);
                    if (logMessages) Debug.LogFormat("Linked {0} to {1} on clip {2}", childController.gameObject.name, controlTrack.name, clip.displayName);
                }
            }
            //controller.PlayableDirector.RebuildGraph();
        }
        var time = controller.PlayableDirector.time;
        controller.PlayableDirector.RebuildGraph();
        GoToTime(controller, (float)time);
    }

    public void Play(PlayableController controller)
    {
        if (logMessages) Debug.Log("Playing " + controller.gameObject.name);

        controller.ShouldContinue = false;

        //if (!controller.PlayableDirector.playableGraph.IsPlaying())
        //    controller.PlayableDirector.Play();

        controller.PlayableDirector.Play();
        controller.PlayableDirector.playableGraph.GetRootPlayable(0).SetSpeed(1);

        PlayableController parent;
        if(childToParentTable.TryGetValue(controller, out parent))
        {
            Play(parent);
        }
    }

    public void Pause(PlayableController controller)
    {
        if (logMessages) Debug.Log("Pausing " + controller.gameObject.name);

        if(controller.ShouldContinue)
        {
            if (logMessages) Debug.Log("Continuing on " + controller.gameObject.name);
            controller.ShouldContinue = false;
        }
        else
        {
            controller.PlayableDirector.playableGraph.GetRootPlayable(0).SetSpeed(0);
        }

        PlayableController parent;
        if (childToParentTable.TryGetValue(controller, out parent))
        {
            Pause(parent);
        }
    }

    public void Pause(PlayableController controller, TimelineClip clip)
    {
        if (logMessages) Debug.Log("Pausing " + controller.gameObject.name);

        if (controller.ShouldContinue)
        {
            if (logMessages) Debug.Log("Continuing on " + controller.gameObject.name);
            controller.ShouldContinue = false;
            return;
        }

        controller.PlayableDirector.playableGraph.GetRootPlayable(0).SetSpeed(0);
        controller.PlayableDirector.time = clip.start;

        PlayableController parent;
        if (childToParentTable.TryGetValue(controller, out parent))
        {
            Pause(parent, clip);
        }
    }

    public void GoToTime(PlayableController controller, float time)
    {
        if (logMessages) Debug.LogFormat("Going to time {0} on {1}", time, controller.gameObject.name);

        var deltaTime = time - controller.PlayableDirector.time;
        controller.PlayableDirector.time = time;

        PlayableController parent;
        if (childToParentTable.TryGetValue(controller, out parent))
        {
            //HACK + TODO -> triple check that this actually works...
            GoToTime(parent, (float)parent.PlayableDirector.time + (float)deltaTime);
        }
    }

    public void Continue(PlayableController controller)
    {
        PlayableController parent;
        childToParentTable.TryGetValue(controller, out parent);

        if (!controller.PlayableDirector.playableGraph.IsValid() ||
           (controller.PlayableDirector.playableGraph.IsValid() && !controller.PlayableDirector.playableGraph.IsPlaying()) ||
           (controller.PlayableDirector.playableGraph.IsValid() && controller.PlayableDirector.playableGraph.GetRootPlayable(0).GetSpeed() == 0))
        {
            if(parent != null && ShouldContinue(parent))
            {
                if (logMessages) Debug.Log("Setting continue on " + controller.gameObject.name);
                controller.ShouldContinue = true;
                Continue(parent);
            }
            else
            {
                if (logMessages) Debug.Log("Continuing on " + controller.gameObject.name);
                Play(controller);
            }
        }
        else
        {
            if (logMessages) Debug.Log("Setting continue on " + controller.gameObject.name);
            controller.ShouldContinue = true;

            if(parent != null)
            {
                Continue(parent);
            }
        }
    }

    bool ShouldContinue(PlayableController controller)
    {
        if (!controller.PlayableDirector.playableGraph.IsValid() ||
           (controller.PlayableDirector.playableGraph.IsValid() && !controller.PlayableDirector.playableGraph.IsPlaying()) ||
           (controller.PlayableDirector.playableGraph.IsValid() && controller.PlayableDirector.playableGraph.GetRootPlayable(0).GetSpeed() == 0))
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    void OnDestroy()
    {
        if (Instance == this)
        { Instance = null; }
    }
}
