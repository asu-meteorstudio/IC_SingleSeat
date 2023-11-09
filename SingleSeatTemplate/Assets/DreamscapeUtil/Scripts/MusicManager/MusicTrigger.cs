using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DreamscapeUtil;

namespace DreamscapeUtil
{
    public class MusicTrigger : MonoBehaviour, ISceneCheckerBehaviour
    {
        [NonEmpty]
        public string cueName;
        [Tooltip("delay between call to TriggerCue and music track starting")]
        public double delay = 0.0;
        [Tooltip("fade in time in seconds")]
        public float fadeInTime = 0.0f;
        [Tooltip("set to a positive value to match start of new music track to beat of current track")]
        public int beatInterval = -1;
        [Tooltip("if checked, sets reference time used by beat interval when starting this track")]
        public bool setReferenceTime = false;
        [Tooltip("if set to a positive value, changes the beats per minute used by beat interval for starting subsequent cues")]
        public float newBpm = -1.0f;
        [Tooltip("if checked, automatically trigger this cue on start")]
        public bool triggerOnStart = false;

        public void Start()
        {
            if (triggerOnStart)
            {
                TriggerCue();
            }
        }

        [EasyButtons.Button(EasyButtons.ButtonMode.EnabledInPlayMode)]
        public void TriggerCue()
        {
            if (setReferenceTime)
            {
                MusicManager.Instance.SetRefTime(delay);
            }
            if (newBpm > 0.0f)
            {
                MusicManager.Instance.SetBpm(newBpm);
            }
            if (beatInterval > 0)
            {
                MusicManager.Instance.StartCueOnBeat(cueName, beatInterval, delay, fadeInTime);
            }
            else if (delay > 0)
            {
                MusicManager.Instance.StartCueScheduled(cueName, delay, fadeInTime);
            }
            else
            {
                MusicManager.Instance.StartCue(cueName, fadeInTime);
            }
        }

        public int CheckForErrors()
        {
            if(setReferenceTime && delay <= 0)
            {
                Debug.LogWarningFormat(this, "{0} - Must use a delay when setting reference time for it to work properly", this.name);
                return 1;
            }
            else
            {
                return 0;
            }
        }
    }
}
