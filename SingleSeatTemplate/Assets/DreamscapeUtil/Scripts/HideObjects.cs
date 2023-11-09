#if DSUTIL_ARTANIM_COMMON_IN_PROJECT
using Artanim;
#endif
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace DreamscapeUtil
{

    /// <summary>
    /// Attribute that can be added to a monobehaviour class so that it gets disabled by the HideObjects script
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class DisableOnHide : Attribute
    {

    }

    /// <summary>
    /// Used to toggle visibility of groups of objects while keeping the game objects active.
    /// 
    /// Can be used as a faster alternative to disabling a game object or in situations where you
    /// want to disable some components of a game object and its children but not others, for example,
    /// to stop a group of objects from being rendered while leaving their colliders enabled.
    /// 
    /// An instance of this script nested under another instance behaves analogously to changing the
    /// active state of nested game objects. That is, a child HideObjects instance will always be hidden
    /// if one of its ancestors is hidden.
    /// 
    /// </summary>
    public class HideObjects : MonoBehaviour
    {
        public enum EOnAwakeBehavior
        {
            DontHide,
            AlwaysHide,
            HideInSession
        }

        public enum EColliderBehavior
        {
            DontDisable,
            DisableCollidersOnly,
            DisableTriggersOnly,
            DisableAll
        }

        //can be used to hide automatically on start
        [FormerlySerializedAs("onAwakeMode")]
        public EOnAwakeBehavior onStartMode = EOnAwakeBehavior.DontHide;

        /////settings for which components get disabled by this script/////////////////////

        //all renderers(static and skinned meshes)
        public bool turnOffRenderers = true;
        //includes lights and reflection probes
        public bool turnOffLightingObjects = true;
        //includes audio sources and reverb zones
        public bool turnOffAudioObjects = true;
        //animators
        public bool turnOffAnimators = true;
        //colliders (can choose all colliders, only triggers, or only non-triggers
        public EColliderBehavior turnOffColliders = EColliderBehavior.DisableAll;
        //particle system
        public bool turnOffParticles = true;
        //disables any script with a DisableOnHide attribute
        public bool turnOffDisableOnHideBehaviours = true;

        //TODO add support for audio objects

        private List<Renderer> renderers = new List<Renderer>();
        private List<Terrain> terrains = new List<Terrain>();
        private List<Light> lights = new List<Light>();
        private List<ReflectionProbe> reflectionProbes = new List<ReflectionProbe>();
        private List<AudioSource> audioSources = new List<AudioSource>();
        private List<AudioReverbZone> audioReverbZones = new List<AudioReverbZone>();
        private List<Animator> animators = new List<Animator>();
        private List<Collider> colliders = new List<Collider>();
        private List<ParticleSystem> particleSystems = new List<ParticleSystem>();
        private List<MonoBehaviour> disableOnHideBehaviours = new List<MonoBehaviour>();

        private bool hidden = false;
        private HideObjects parent = null;
        private List<HideObjects> children = new List<HideObjects>();

        // Use this for initialization
        void Awake()
        {
            PopulateComponents();
        }

        private void Start()
        {
            if (onStartMode == EOnAwakeBehavior.AlwaysHide
#if DSUTIL_ARTANIM_COMMON_IN_PROJECT
                || (onStartMode == EOnAwakeBehavior.HideInSession && GameController.Instance != null)
#endif
                )
            {
                Hide();
            }
        }

        private void PopulateComponents()
        {
            Transform current = transform;


            while (current)
            {
                if (turnOffRenderers)
                {
                    Renderer[] renderersArray = current.GetComponents<Renderer>();
                    foreach (Renderer r in renderersArray)
                    {
                        if (r.enabled)
                        {
                            renderers.Add(r);
                        }
                    }
                    Terrain[] terrainsArray = current.GetComponents<Terrain>();
                    foreach(Terrain t in terrainsArray)
                    {
                        if(t.enabled)
                        {
                            terrains.Add(t);
                        }
                    }
                }

                if (turnOffLightingObjects)
                {
                    Light[] lightsArray = current.GetComponents<Light>();
                    foreach (Light l in lightsArray)
                    {
                        if (l.enabled)
                        {
                            lights.Add(l);
                        }
                    }


                    ReflectionProbe[] reflectionProbeArray = current.GetComponents<ReflectionProbe>();
                    foreach (ReflectionProbe p in reflectionProbeArray)
                    {
                        if (p.enabled)
                        {
                            reflectionProbes.Add(p);
                        }
                    }
                }

                if(turnOffAudioObjects)
                {
                    AudioSource[] audioSourceArray = current.GetComponents<AudioSource>();
                    foreach(AudioSource a in audioSourceArray)
                    {
                        if(a.enabled)
                        {
                            audioSources.Add(a);
                        }
                    }

                    AudioReverbZone[] audioReverbZoneArray = current.GetComponents<AudioReverbZone>();
                    foreach(AudioReverbZone a in audioReverbZoneArray)
                    {
                        if(a.enabled)
                        {
                            audioReverbZones.Add(a);
                        }
                    }
                }

                if (turnOffAnimators)
                {
                    Animator[] animatorArray = current.GetComponents<Animator>();
                    foreach (Animator a in animatorArray)
                    {
                        if (a.enabled)
                        {
                            animators.Add(a);
                        }
                    }
                }

                if(turnOffColliders != EColliderBehavior.DontDisable)
                {
                    foreach(Collider c in current.GetComponents<Collider>())
                    {
                        if (c.isTrigger)
                        {
                            if (turnOffColliders != EColliderBehavior.DisableCollidersOnly)
                            {
                                colliders.Add(c);
                            }
                        }
                        else
                        {
                            if(turnOffColliders != EColliderBehavior.DisableTriggersOnly)
                            {
                                colliders.Add(c);
                            }
                        }
                    }
                }

                if (turnOffParticles)
                {
                    ParticleSystem[] particleSystemArray = current.GetComponents<ParticleSystem>();
                    foreach (ParticleSystem p in particleSystemArray)
                    {
                        if (p.main.playOnAwake && p.main.loop)
                        {
                            particleSystems.Add(p);
                        }
                    }
                }

                if (turnOffDisableOnHideBehaviours)
                {
                    foreach (MonoBehaviour b in current.GetComponents<MonoBehaviour>())
                    {
                        if (b && b.enabled && b.GetType().GetCustomAttributes(typeof(DisableOnHide), true).Length > 0)
                        {
                            disableOnHideBehaviours.Add(b);
                        }
                    }
                }

                current = GetNext(current);
                while(current && current.GetComponent<HideObjects>())
                {
                    HideObjects child = current.GetComponent<HideObjects>();
                    children.Add(child);
                    child.parent = this;

                    current = GetNext(current, true);
                }

            }
        }

        private Transform GetNext(Transform current, bool skipChildren = false)
        {
            if (!skipChildren && current.childCount > 0)
            {
                return current.GetChild(0);
            }
            else
            {
                Transform t = current;
                while(t != transform && t.parent && t.GetSiblingIndex() >= t.parent.childCount - 1)
                {
                    t = t.parent;
                }

                if(t == transform)
                {
                    return null;
                }
                else if(t.parent && t.GetSiblingIndex() < t.parent.childCount - 1)
                {
                    return t.parent.GetChild(t.GetSiblingIndex() + 1);
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// Hides/disables components in children as specified in inspector
        /// </summary>
        public void Hide()
        {
            if (!hidden)
            {
                hidden = true;

                if (!ParentHidden())
                {
                    SetVisibility(false);
                }
            }

        }

        private bool ParentHidden()
        {
            HideObjects h = this;
            while (h.parent)
            {
                if (h.parent.hidden)
                {
                    return true;
                }
                h = h.parent;
            }

            return false;
        }

        private void SetVisibility(bool visible)
        {
            foreach (Renderer r in renderers)
            {
                if (r)
                    r.enabled = visible;
            }
            foreach(Terrain t in terrains)
            {
                if (t)
                    t.enabled = visible;
            }

            foreach (Light l in lights)
            {
                if (l)
                    l.enabled = visible;
            }
            foreach (ReflectionProbe p in reflectionProbes)
            {
                if (p)
                    p.enabled = visible;
            }

            foreach(AudioSource a in audioSources)
            {
                if (a)
                {
                    a.enabled = visible;
                }
            }
            foreach(AudioReverbZone a in audioReverbZones)
            {
                if (a)
                {
                    a.enabled = visible;
                }
            }

            foreach (Animator a in animators)
            {
                if (a)
                    a.enabled = visible;
            }

            foreach(Collider c in colliders)
            {
                if (c)
                    c.enabled = visible;
            }

            foreach (ParticleSystem p in particleSystems)
            {
                if (p)
                {
                    if (visible)
                    {
                        p.Play();
                    }
                    else
                    {
                        p.Stop();
                    }
                }
            }

            foreach (Behaviour b in disableOnHideBehaviours)
            {
                if (b)
                    b.enabled = visible;
            }

            foreach(HideObjects child in children)
            {
                if (child && !child.hidden)
                {
                    child.SetVisibility(visible);
                }
            }
        }

        /// <summary>
        /// Shows/enables components in children as specified in inspector
        /// </summary>
        public void Show()
        {
            if (hidden)
            {
                hidden = false;

                if (!ParentHidden())
                {
                    SetVisibility(true);
                }

            }
        }

        /// <summary>
        /// Returns true if Hide was called on this component specifically
        /// </summary>
        public bool IsHiddenSelf()
        {
            return hidden;
        }

        /// <summary>
        /// Returns true if Hide was called on this component or a parent
        /// </summary>
        public bool IsHiddenInHierarchy()
        {
            return hidden || ParentHidden();
        }
    }
}
