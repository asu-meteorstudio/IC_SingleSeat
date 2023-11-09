using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace DreamscapeUtil
{
    [System.Serializable]
    public struct EventMapping{
        public string eventName;
        public UnityEvent action;
    }

    /// <summary>
    /// Simple script to invoke arbitrary UnityEvents from an animation event.
    /// 
    /// To use, when setting up the animation event, use "AnimEvent" as the function name with a unique event name as the string parameter.
    /// Then add an entry in the events field in this script with the same name
    /// </summary>
    public class AnimEventHelper : MonoBehaviour, ISceneCheckerBehaviour
    {
        public EventMapping[] events;

       
        void AnimEvent(string name)
        {
            foreach (EventMapping evt in events)
            {
                if (name == evt.eventName)
                {
                    evt.action.Invoke();
                    return;
                }
            }

            Debug.LogWarningFormat("AnimEventHelper - No event with name '{0}'", name);
        }

        public int CheckForErrors()
        {
            int numErrors = 0;

            for(int i = 0; i < events.Length; i++)
            {
                if (string.IsNullOrEmpty(events[i].eventName))
                {
                    Debug.LogWarning("Event Name cannot be null", this);
                    numErrors++;
                }
                else
                {
                    for(int j = 0; j < i; j++)
                    {
                        if(events[i].eventName == events[j].eventName)
                        {
                            Debug.LogWarningFormat(this, "Duplicate event name: '{0}'", events[i].eventName);
                            numErrors++;
                        }
                    }
                }

            }

            return numErrors;
        }

    }
}
