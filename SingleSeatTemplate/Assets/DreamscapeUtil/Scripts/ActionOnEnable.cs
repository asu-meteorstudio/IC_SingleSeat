using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace DreamscapeUtil
{
    /// <summary>
    /// Onvokes a unity event on enable
    /// </summary>
    public class ActionOnEnable : MonoBehaviour
    {
        public enum EMode
        {
            WorkOnce,
            WorkOnEveryEnable
        }
        [Tooltip("Actions will be invoked when script is enabled")]
        public UnityEvent action;
        [Tooltip("Choose whether to invoke actions just once or every time OnEnable is called")]
        public EMode mode = EMode.WorkOnce;

        private bool triggered = false;

        private void OnEnable()
        {
            if (mode == EMode.WorkOnEveryEnable || !triggered)
            {
                triggered = true;
                action.Invoke();
            }
        }
    }
}
