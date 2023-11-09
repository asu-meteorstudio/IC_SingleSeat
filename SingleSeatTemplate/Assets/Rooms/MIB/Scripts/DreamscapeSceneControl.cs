using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Events;
using SubjectNerd.Utilities;
using System.Collections;
using System;
using UnityEngine.Serialization;

//TODO -> give this a None / Default value?
[System.Serializable]
public enum SceneControlTag
{
    MeshRenderer,
    SkinnedMeshRenderer,
    Terrain,
    ParticleSystem,
    Light,
    Collider,
    Animator,
}

[ExecuteInEditMode]
public class DreamscapeSceneControl : ReactiveBehaviour
{
    [Serializable]
    public class RootObjectContainer
    {
        public GameObject RootObject;
        [Tooltip("Toggle the root object along with child components when enabling and disabling the scene")]
        public bool ToggleRootGameObject;
        [Tooltip("Toggle the components in the children of the root object when enabling and disabling the scene")]
        [EnumMask]
        public SceneControlTag ChildComponentsToToggle;

        public MeshRenderer[] MeshRenderers;
        public SkinnedMeshRenderer[] SkinnedMeshRenderers;
        public Terrain[] Terrains;
        public ParticleSystem[] ParticleSystems;
        public Light[] Lights;
        public Collider[] Colliders;
        public Animator[] Animators;
        //public Behaviour[] Behaviours;

        RootObjectContainer()
        {
            ToggleRootGameObject = true;
            ChildComponentsToToggle = ~(SceneControlTag)(0);
        }
    }

    [Reorderable]
    public List<RootObjectContainer> RootObjects = new List<RootObjectContainer>();

    [Reorderable]
    public List<Behaviour> Behaviours = new List<Behaviour>();

    [Tooltip("Sets the number of items to toggle on or off per frame. Set to < 1 to disable batching.")]
    public int BatchSize;
    int currentObject;

    [Header("Events")]
    public UnityEvent OnSceneTurnOn;
    public UnityEvent OnSceneTurnOff;

    public UnityEvent OnSceneStarted;
    public UnityEvent OnSceneEnded;

    IEnumerator toggleSequence;
    Coroutine toggleRoutine;

    protected virtual void Awake()
    {
        //if (GameplayManager.IsServerOnly())
        //    return;

        SetupReferences();
    }

    Dictionary<Type, SceneControlTag> typeToTagTable = new Dictionary<Type, SceneControlTag>
    {
        { typeof(MeshRenderer), SceneControlTag.MeshRenderer },
        { typeof(SkinnedMeshRenderer), SceneControlTag.SkinnedMeshRenderer },
        { typeof(Terrain), SceneControlTag.Terrain },
        { typeof(ParticleSystem), SceneControlTag.ParticleSystem },
        { typeof(Light), SceneControlTag.Light },
        { typeof(Collider), SceneControlTag.Collider },
        { typeof(Animator), SceneControlTag.Animator },
    };

    [ContextMenu("Setup References")]
    protected virtual void SetupReferences()
    {
        //if (GameplayManager.IsServerOnly())
        //    return;

        foreach(var ro in RootObjects)
        {
            ro.MeshRenderers = ro.ChildComponentsToToggle.Contains(SceneControlTag.MeshRenderer) ? GetComponentArray<MeshRenderer>(ro.RootObject) : null;
            ro.SkinnedMeshRenderers = ro.ChildComponentsToToggle.Contains(SceneControlTag.SkinnedMeshRenderer) ? GetComponentArray<SkinnedMeshRenderer>(ro.RootObject) : null;
            ro.Terrains = ro.ChildComponentsToToggle.Contains(SceneControlTag.Terrain) ? GetComponentArray<Terrain>(ro.RootObject) : null;
            ro.ParticleSystems = ro.ChildComponentsToToggle.Contains(SceneControlTag.ParticleSystem) ? GetComponentArray<ParticleSystem>(ro.RootObject) : null;
            ro.Lights = ro.ChildComponentsToToggle.Contains(SceneControlTag.Light) ? GetComponentArray<Light>(ro.RootObject) : null;
            ro.Colliders = ro.ChildComponentsToToggle.Contains(SceneControlTag.Collider) ? GetComponentArray<Collider>(ro.RootObject) : null;
            ro.Animators = ro.ChildComponentsToToggle.Contains(SceneControlTag.Animator) ? GetComponentArray<Animator>(ro.RootObject) : null;
        }
    }

    protected T[] GetComponentArray<T>(GameObject go) where T : Component
    {
        SceneControlTag tag;
        typeToTagTable.TryGetValue(typeof(T), out tag);
        return go.GetComponentsInChildren<T>().Where(x =>
        {
            var ignore = x.GetComponent<IgnoredBySceneControl>();
            return ignore == null || !ignore.ComponentsToIgnore.Contains(tag);
        }).ToArray();
    }

    [ContextMenu("Clear References")]
    protected virtual void ClearReferences()
    {
        foreach (var ro in RootObjects)
        {
            ro.MeshRenderers = null;
            ro.SkinnedMeshRenderers = null;
            ro.ParticleSystems = null;
            ro.Lights = null;
            ro.Colliders = null;
        }
    }

#if UNITY_EDITOR
    void Update()
    {
        if (!Application.isPlaying && toggleSequence != null)
        { toggleSequence.MoveNext(); }
    }

    [ContextMenu("Toggle On")]
    void ToggleOnEditor()
    {
        SetupReferences();
        SceneTurnOn();
    }

    [ContextMenu("Toggle Off")]
    void ToggleOffEditor()
    {
        SetupReferences();
        SceneTurnOff();
    }
#endif

    public virtual void SceneTurnOn()
    {
        if(toggleRoutine != null)
        { StopCoroutine(toggleRoutine); }

        toggleSequence = ToggleScene(true);
        toggleRoutine = StartCoroutine(toggleSequence);

        if(OnSceneTurnOn != null)
        { OnSceneTurnOn.Invoke(); }
    }

    public virtual void SceneTurnOff()
    {
        if (toggleRoutine != null)
        { StopCoroutine(toggleRoutine); }

        toggleSequence = ToggleScene(false);
        toggleRoutine = StartCoroutine(toggleSequence);

        if (OnSceneTurnOff != null)
        { OnSceneTurnOff.Invoke(); }
    }

    public virtual void StartScene()
    {
        if (OnSceneStarted != null)
        { OnSceneStarted.Invoke(); }
    }

    public IEnumerator ToggleScene(bool isOn)
    {
        //if (GameplayManager.IsServerOnly())
        //    return;

        currentObject = 0;

        foreach (var ro in RootObjects)
        {
            if(ro.ToggleRootGameObject)
            { ro.RootObject.SetActive(isOn); }

            if (ro.MeshRenderers != null && ro.MeshRenderers.Length > 0)
            {
                foreach (var o in ro.MeshRenderers)
                {
                    o.enabled = isOn;
                    if (ShouldBatch())
                    { yield return null; }
                }
            }

            if (ro.SkinnedMeshRenderers != null && ro.SkinnedMeshRenderers.Length > 0)
            {
                foreach (var o in ro.SkinnedMeshRenderers)
                {
                    o.enabled = isOn;
                    if (ShouldBatch())
                    { yield return null; }
                }
            }

            if (ro.Terrains != null && ro.Terrains.Length > 0)
            {
                foreach (var o in ro.Terrains)
                {
                    o.enabled = isOn;
                    if (ShouldBatch())
                    { yield return null; }
                }
            }

            if (ro.ParticleSystems != null && ro.ParticleSystems.Length > 0)
            {
                foreach (var o in ro.ParticleSystems)
                {
                    //ps.gameObject.SetActive(isOn && ToggleGameObjects);
                    if (isOn && o.main.playOnAwake)
                    { o.Play(); }
                    else if (!isOn)
                    { o.Stop(); }

                    if (ShouldBatch())
                    { yield return null; }
                }
            }

            if (ro.Lights != null && ro.Lights.Length > 0)
            {
                foreach (var o in ro.Lights)
                {
                    o.enabled = isOn;
                    if (ShouldBatch())
                    { yield return null; }
                }
            }

            if (ro.Colliders != null && ro.Colliders.Length > 0)
            {
                foreach (var o in ro.Colliders)
                {
                    o.enabled = isOn;
                    if (ShouldBatch())
                    { yield return null; }
                }
            }

            if (ro.Animators != null && ro.Animators.Length > 0)
            {
                foreach (var o in ro.Animators)
                {
                    o.enabled = isOn;
                    if (ShouldBatch())
                    { yield return null; }
                }
            }

            if (Behaviours != null && Behaviours.Count > 0)
            {
                foreach (var o in Behaviours)
                {
                    o.enabled = isOn;
                    if (ShouldBatch())
                    { yield return null; }
                }
            }

            if (ShouldBatch())
            { yield return null; }

            //if (ro.ToggleRootGameObject)
            //{ ro.RootObject.SetActive(isOn); }
        }
    }

    bool ShouldBatch()
    {
        //Debug.LogFormat("SHOULD BATCH {0}", this.name);

        if (BatchSize > 0)
        {
            currentObject += 1;
            if (currentObject > BatchSize)
            {
                currentObject = 0;
                //Debug.LogFormat("BATCHING {0}", this.name);
                return true;
            }
            else
            {
                return false;
            }
        }
        else
        {
            return false;
        }
    }
}
