using DreamscapeUtil;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DreamscapeUtil
{
    /// <summary>
    /// Script to control music playback during experience.
    /// 
    /// Can be used to play music across scene transition, layer music clips on top of each other, cross fade between
    /// music clips, and seamlessly transition between clips with sample-accurate timing.
    /// 
    /// Attach this script to a game object with a DontDestroyOnLoad behavior in the construct scene so it is available through experience.
    /// Populate the list of cues with a separate audio source for each audio cue and give each a unique name for it to be triggered by.
    /// </summary>
    public class MusicManager : MonoBehaviour, ISceneCheckerBehaviour
    {
        [System.Serializable]
        public struct MusicCue
        {
            public string name;
            public AudioSource source;
        }

        private static MusicManager _inst = null;

        [NonNull]
        public MusicCue[] cues;

        private double referenceTime = 0.0;
        private double bpm = 93.0;

        public static MusicManager Instance
        {
            get
            {
                return _inst;
            }
        }

        void Awake()
        {
            if (_inst)
            {
                Debug.LogError("More than one Music Manager!");
            }

            _inst = this;
            referenceTime = AudioSettings.dspTime;
        }

        /// <summary>
        /// Starts the specified cue
        /// 
        /// note: this does not stop/fade out any cues which are currently playing
        /// </summary>
        /// <param name="name">name of the music cue to start</param>
        /// <param name="fadeTime">optional fade in time in seconds when starting the audio clip</param>
        public void StartCue(string name, float fadeTime = 0.0f)
        {
            if(GetCueByName(name, out MusicCue cue))
            {
                if (cue.source.isPlaying)
                {
                    Debug.LogErrorFormat("Music Cue '{0}' is already playing", name);
                    return;
                }
                if (fadeTime > 0)
                {
                    cue.source.volume = 0.0f;
                    StartCoroutine(FadeIn(cue.source, fadeTime));
                }
                else
                {
                    cue.source.volume = 1.0f;
                }

                cue.source.Play();
            }
        }


        /// <summary>
        /// Schedules a cue to be started after a specified delay
        /// 
        /// note: this does not stop/fade out any cues which are currently playing
        /// </summary>
        /// <param name="name">name of the music cue to start</param>
        /// <param name="delay">delay in seconds before starting the clip</param>
        /// <param name="fadeTime">optional fade in time in seconds</param>
        public void StartCueScheduled(string name, double delay, float fadeTime = 0.0f)
        {
            if(GetCueByName(name, out MusicCue cue))
            {
                if (cue.source.isPlaying)
                {
                    Debug.LogErrorFormat("Music Cue '{0}' is already playing", name);
                    return;
                }
                StartCueAtDspTime(cue.source, AudioSettings.dspTime + delay);
            }
        }

        private void StartCueAtDspTime(AudioSource src, double dspTime, float fadeTime = 0.0f)
        {
            if (fadeTime > 0)
            {
                src.volume = 0.0f;
                StartCoroutine(FadeIn(src, fadeTime, (float)(dspTime - AudioSettings.dspTime)));
            }
            else
            {
                src.volume = 1.0f;
            }
            src.PlayScheduled(dspTime);
        }

        /// <summary>
        /// Starts a music cue in rhythm with the currently playing music track. The new clip will be delayed a small amount so that
        /// it lines up with the previous track as specified.
        /// 
        /// For this to work properly, you must call SetRefTime to establish the reference point(generally the time when the previously playing clip starts)
        /// and SetBpm to set the beats per minute of the previously playing clip.
        /// 
        /// As an example, if the bpm is set to 60(i.e. 1 per second), and StartCueOnBeat is called exactly 22.7 seconds after the last time that SetRefTime was called with a beat interval of 2,
        /// then the new clip will be delayed exactly 1.3 seconds so that it starts at an even multiple of 2 seconds from the reference time that was set.
        /// 
        /// </summary>
        /// <param name="name">name of the music cue to start</param>
        /// <param name="beatInterval">specifies how the new clip is allowed to line up with the currently playing clip
        /// - a value of 1 means it will start on the next beat, a value of 2 means it can only start on every other beat, 4 means every 4 beats, etc.</param>
        /// <param name="delay">optional delay before starting the clip - the additional delay to make the clip match the beat of the previous clip is calculated after applying this delay</param>
        /// <param name="fadeTime">optional fade in time</param>
        public void StartCueOnBeat(string name, int beatInterval, double delay = 0.0, float fadeTime = 0.0f)
        {
            double timeInterval = (60.0 / bpm) * beatInterval;

            double targetTime = AudioSettings.dspTime - referenceTime + delay;
            double mod = targetTime % timeInterval;

            double beatTime = referenceTime + targetTime + timeInterval - mod;
            if(GetCueByName(name, out MusicCue cue))
            {
                if (cue.source.isPlaying)
                {
                    Debug.LogErrorFormat("Music Cue '{0}' is already playing", name);
                    return;
                }
                StartCueAtDspTime(cue.source, beatTime, fadeTime);
            }
        }

        /// <summary>
        /// Fades out a currently playing music clip
        /// </summary>
        /// <param name="name">name of the music cue to fade out</param>
        /// <param name="fadeTime">duration of fade in seconds</param>
        /// <param name="delay">optional delay before starting the fade out</param>
        public void FadeOutCue(string name, float fadeTime = 1.0f, double delay = 0.0)
        {
            if(GetCueByName(name, out MusicCue cue))
            {
                if (fadeTime > 0)
                {
                    StartCoroutine(FadeOut(cue.source, fadeTime, (float)delay));
                }
                else
                {
                    cue.source.SetScheduledEndTime(AudioSettings.dspTime + delay);
                }
            }
        }

        /// <summary>
        /// Fades out a cue according to the rhythm of the currently playing track. Generally used in conjuction with StartCueOnBeat
        /// to crossfade between clips, or seamlessly stop a clip and start a new one
        /// </summary>
        /// <param name="name">name of cue to fade out</param>
        /// <param name="beatInterval">specifies timing of fade out - see description in StartCueOnBeat</param>
        /// <param name="fadeTime">duration of fade out</param>
        /// <param name="delay">additional delay before starting fade</param>
        public void FadeOutCueOnBeat(string name, int beatInterval, float fadeTime = 1.0f, double delay = 0.0)
        {
            double timeInterval = (60.0 / bpm) * beatInterval;

            double targetTime = AudioSettings.dspTime - referenceTime + delay;
            double mod = targetTime % timeInterval;

            double beatTime = targetTime + timeInterval - mod;

            if(GetCueByName(name, out MusicCue cue))
            {
                if (fadeTime > 0)
                {
                    StartCoroutine(FadeOut(cue.source, fadeTime, (float)(referenceTime - AudioSettings.dspTime + beatTime)));
                }
                else
                {
                    cue.source.SetScheduledEndTime(referenceTime + beatTime);
                }
            }
        }

        /// <summary>
        /// Fades out all currently playing music clips
        /// </summary>
        /// <param name="fadeTime">fade duration in seconds</param>
        public void FadeOutAll(float fadeTime = 1.0f)
        {
            foreach (MusicCue cue in cues)
            {
                if (cue.source.isPlaying)
                {
                    StartCoroutine(FadeOut(cue.source, fadeTime));
                }
            }
        }

        /// <summary>
        /// Sets the beats per minutes used by StartCueOnBeat and FadeOutCueOnBeat
        /// </summary>
        /// <param name="newBpm">beats per minue</param>
        public void SetBpm(double newBpm)
        {
            bpm = newBpm;
        }

        /// <summary>
        /// Sets the reference time(start of the track) used by StartCueOnBeat and FadeOutCueOnBeat
        /// </summary>
        /// <param name="delay">offset from current time to use a reference point</param>
        public void SetRefTime(double delay = 0.0)
        {
            referenceTime = AudioSettings.dspTime + delay;
        }

        /// <summary>
        /// returns true if the specified clip is currently playing
        /// </summary>
        /// <param name="cueName"></param>
        /// <returns></returns>
        public bool IsCuePlaying(string cueName)
        {
            if(GetCueByName(cueName, out MusicCue cue))
            {
                return cue.source.isPlaying;
            }
            else
            {
                return false;
            }
        }

        private IEnumerator FadeIn(AudioSource source, float fadeTime, float initialDelay = 0.0f)
        {
            if (initialDelay > 0.0f)
            {
                yield return new WaitForSeconds(initialDelay);
            }

            float t = 0;
            while (t <= fadeTime)
            {
                t += Time.deltaTime;

                source.volume = Mathf.Clamp01(t / fadeTime);
                yield return null;
            }
        }

        private IEnumerator FadeOut(AudioSource source, float fadeTime, float initialDelay = 0.0f)
        {
            if (initialDelay > 0.0f)
            {
                yield return new WaitForSeconds(initialDelay);
            }


            float startVolume = source.volume;

            float t = 0;
            while (t <= fadeTime)
            {
                t += Time.deltaTime;

                source.volume = Mathf.Clamp01(1.0f - t / fadeTime);
                yield return null;
            }

            source.Stop();
        }

        private bool GetCueByName(string name, out MusicCue cue)
        {
            foreach(MusicCue c in cues)
            {
                if(c.name == name)
                {
                    cue = c;
                    return true;
                }
            }
            Debug.LogErrorFormat("No Music Cue with name: '{0}'", name);
            cue =  default(MusicCue);
            return false;
        }

        public int CheckForErrors()
        {
            int numErrors = 0;

            for (int i = 0; i < cues.Length; i++)
            {
                MusicCue cue = cues[i];
                if (string.IsNullOrEmpty(cue.name))
                {
                    numErrors++;
                    Debug.LogWarning("Music Cue must have name", this);
                }
                if (cue.source == null)
                {
                    numErrors++;
                    Debug.LogWarningFormat(this, "Music Cue '{0}' audio source is null", cue.name);
                }

                for (int j = 0; j < i; j++)
                {
                    if (cues[j].name == cue.name)
                    {
                        numErrors++;
                        Debug.LogWarningFormat(this, "Duplicate music cue name '{0}'", cue.name);
                    }
                }

            }

            return numErrors;
        }
    }
}
