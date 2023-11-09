using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DreamscapeUtil;

namespace DreamscapeUtil
{
    public class MusicFadeoutTrigger : MonoBehaviour
    {
        [NonEmpty]
        public string cueName;

        [Tooltip("delay between call to TriggerCue and start of fade out")]
        public double delay = 0.0;
        [Tooltip("fade out time in seconds")]
        public float fadeOutTime = 0.0f;
        [Tooltip("set to a positive value to match start of fade out to beat of current track")]
        public int beatInterval = -1;
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
            if (beatInterval > 0)
            {
                MusicManager.Instance.FadeOutCueOnBeat(cueName, beatInterval, fadeOutTime, delay);
            }
            else if (delay > 0)
            {
                MusicManager.Instance.FadeOutCue(cueName, fadeOutTime, delay);
            }
            else
            {
                MusicManager.Instance.FadeOutCue(cueName, fadeOutTime);
            }
        }

    }
}
