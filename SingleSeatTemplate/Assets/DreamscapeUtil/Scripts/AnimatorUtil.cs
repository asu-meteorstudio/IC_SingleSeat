
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

#if DSUTIL_ARTANIM_COMMON_IN_PROJECT
using Artanim.Location.Network;
using Artanim;
#endif

namespace DreamscapeUtil {

    [System.Serializable]
    public class AnimatorParamInfo
    {
        public string name;
        public AnimatorControllerParameterType type;
        public float floatVal;
        public int intVal;
        public bool boolVal;
    }

    /// <summary>
    /// Used to set animation properties on start.
    /// 
    /// Can set animator parameters of arnimation state on start. Can also be used to randomize
    /// the starting time of a looping animation.
    /// </summary>
    [RequireComponent(typeof(Animator))]
    public class AnimatorUtil : MonoBehaviour, ISceneCheckerBehaviour
    {
        public bool setState = false;
        public string stateName = "";
        public float normalizedTime = 0.0f;
        public bool randomizeTime = false;
        public int layerIndex = 0;
#if DSUTIL_ARTANIM_COMMON_IN_PROJECT
        public bool disableCullingOnServer = true;
#endif

        public List<AnimatorParamInfo> parameters = new List<AnimatorParamInfo>();

        public int CheckForErrors()
        {
            Animator anim = GetComponent<Animator>();
            if (!anim.runtimeAnimatorController)
            {
                Debug.LogWarningFormat(this, "AnimatorUtil script cannot affect animator with no animator controller assigned - {0}", this.name);
                return 1;
            }
            if (setState && string.IsNullOrEmpty(stateName))
            {
                Debug.LogWarningFormat(this, "AnimatorUtil - state name not specified - {0}", this.name);
                return 1;
            }
            //TODO these sometimes incorrectly report errors saying the animator controller is not playing - look into this
            /*else if(layerIndex > anim.layerCount)
            {
                Debug.LogWarningFormat(this, "AnimatorUtil script references animator layer {0}, but controller only has {1} layers - {2}", layerIndex, anim.layerCount, this.name);
                return 1;
            else if(setState && !anim.HasState(layerIndex, Animator.StringToHash(stateName)))
            {
                Debug.LogWarningFormat(this, "AnimatorUtil - Animation State '{0}' not found on layer {1} - {2}", stateName, layerIndex, this.name);
                return 1;
            }*/
            else
            {
                return 0;
            }
        }

        void Start()
        {
            Animator anim = GetComponent<Animator>();

            foreach (AnimatorParamInfo param in parameters)
            {
                if (param.type == AnimatorControllerParameterType.Bool)
                {
                    anim.SetBool(param.name, param.boolVal);
                }
                else if (param.type == AnimatorControllerParameterType.Float)
                {
                    anim.SetFloat(param.name, param.floatVal);
                }
                else if (param.type == AnimatorControllerParameterType.Int)
                {
                    anim.SetInteger(param.name, param.intVal);
                }
                else if (param.type == AnimatorControllerParameterType.Trigger)
                {
                    anim.SetTrigger(param.name);
                }
            }

            int stateHash = setState ? Animator.StringToHash(stateName) : 0;
            anim.Play(stateHash, layerIndex, randomizeTime ? Random.Range(0.0f, 1.0f) : normalizedTime);
#if DSUTIL_ARTANIM_COMMON_IN_PROJECT
            if (disableCullingOnServer)
            {
                if (NetworkInterface.Instance.IsServer && DevelopmentMode.CurrentMode != EDevelopmentMode.Standalone)
                {
                    anim.cullingMode = AnimatorCullingMode.AlwaysAnimate;
                }
            }
#endif
        }
    }
}
