using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace DreamscapeUtil
{
    /// <summary>
    /// Invokes a UnityEvent after a specified delay
    /// </summary>
    public class DelayedAction : MonoBehaviour
    {
        public UnityEvent action;
        [Tooltip("Delay in seconds")]
        public float delay = 1.0f;
        [Tooltip("If checked, start the timer automatically on start")]
        public bool triggerOnStart = false;

        private float triggerTime = -1.0f;

        // Start is called before the first frame update
        void Start()
        {
            if (triggerOnStart)
            {
                TriggerDelayedAction();
            }
            else
            {
                if(triggerTime < 0.0f)
                    this.enabled = false;
            }
        }

        private void Update()
        {
            if(triggerTime >= 0 && Time.time >= triggerTime)
            {
                triggerTime = -1.0f;
                this.enabled = false;
                action.Invoke();
            }
        }

        /// <summary>
        /// Starts the timer so that the action will be invoked after the specified delay from when this function was called.
        /// 
        /// Calling this function again before the action is invoked, will reset the timer and the action will still only be
        /// invoked once
        /// </summary>
        [EasyButtons.Button(EasyButtons.ButtonMode.EnabledInPlayMode)]
        public void TriggerDelayedAction()
        {
            triggerTime = Time.time + delay;
            this.enabled = true;
        }

        /// <summary>
        /// Cancels the previously scheduled timer to invoke the action
        /// </summary>
        [EasyButtons.Button(EasyButtons.ButtonMode.EnabledInPlayMode)]
        public void CancelDelayedAction()
        {
            triggerTime = -1.0f;
            this.enabled = false;
        }
    }
}
