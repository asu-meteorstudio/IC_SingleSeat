using Artanim;
using Artanim.Location.Network;
using DreamscapeUtil;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace DreamscapeUtil
{
    /// <summary>
    /// Adds ability to play sections of a timeline and easily jump to set points on a timeline.
    /// 
    /// To use, add one or more SimpleTimelineMarkers(with unique id fields) to a timeline and add this script to the 
    /// same GameObject as the playable director. Then use JumpToMarker to set the current playback time to the specified
    /// marker or PlayToMarker to play from the current time and pause when the specified marker is reached
    /// </summary>
    public class TimelineController : MonoBehaviour, ISceneCheckerBehaviour
    {

        protected PlayableDirector director;

        protected TrackAsset markerTrack;

        protected double stopTime = -1.0;
        protected string stopMarker = "";
        protected string startMarker = "";

        protected delegate void PauseHandler(string marker);
        protected delegate void JumpHandler(string marker);
        protected event PauseHandler OnPause;
        protected event JumpHandler OnJump;

        protected virtual void Awake()
        {
            director = GetComponent<PlayableDirector>();
            TimelineAsset timeline = director.playableAsset as TimelineAsset;
            if (timeline)
            {
                markerTrack = timeline.markerTrack;
            }
        }

        // Update is called once per frame
        protected virtual void Update()
        {
            if (stopTime >= 0 && director.time >= stopTime)
            {
                director.Pause();
                stopTime = -1.0f;
                OnPause?.Invoke(stopMarker);
            }
        }

        /// <summary>
        /// Causes the playable director to begin playing from the current timestamp and pause at the specified marker
        /// 
        /// Note: playback will not stop exactly on the marker specified, but instead will be stopped as soon as the current
        /// time is greater than or equal to the specified time. As such, the actual time it stops may be as much as Time.deltaTime past
        /// the specfied time. Be aware that this may cause issues if, for example, there is a timeline signal or activation clip a
        /// small fraction of a second beyond the point you are expecting to stop at
        /// </summary>
        /// <param name="endMarkerId">id of the SimpleTimelineMarker to pause at</param>
        public virtual void PlayToMarker(string endMarkerId)
        {
            stopTime = GetMarkerTimestamp(endMarkerId);
            stopMarker = endMarkerId;
            director.Play();
        }

        /// <summary>
        /// Causes the playable director to play from the current timestamp and continue to the end of the timeline
        /// 
        /// This is the same as calling Play directly on the PlayableDirector except that it will ensure the playable director does
        /// not pause due to a previous call to PlayToMarker.
        /// </summary>
        public virtual void PlayToEnd()
        {
            stopTime = -1.0f;
            director.Play();
        }

        /// <summary>
        /// Causes the playable director to jump to the specified marker on the timeline and pauses playback if it was previously playing.
        /// </summary>
        /// <param name="startMarkerId">id of the SimpleTimelineMarker to jump to</param>
        public virtual void JumpToMarker(string startMarkerId)
        {
            double startTime = GetMarkerTimestamp(startMarkerId);
            if (startTime >= 0.0)
            {
                director.time = startTime;

                OnJump?.Invoke(startMarkerId);
            }
            stopTime = -1.0;
            if (director.state == PlayState.Playing)
            {
                director.Pause();
            }
        }

        protected double GetMarkerTimestamp(string markerId)
        {
            if (markerTrack)
            {
                for (int i = 0; i < markerTrack.GetMarkerCount(); i++)
                {
                    IMarker marker = markerTrack.GetMarker(i);
                    if (marker is SimpleTimelineMarker)
                    {
                        SimpleTimelineMarker m = marker as SimpleTimelineMarker;

                        if (m.ID == markerId)
                        {
                            return m.time;
                        }
                    }

                }

                Debug.LogErrorFormat("No timeline marker with ID '{0}'", markerId);
                return -1;
            }
            else
            {
                Debug.LogErrorFormat("Timeline not assigned or has no marker track - {0}", gameObject.name);
                return -1.0f;
            }
        }

        public virtual int CheckForErrors()
        {
            int numErrors = 0;

            if (!TryGetComponent<PlayableDirector>(out _))
            {
                Debug.LogWarningFormat(this, "TimelineController requries a PlayableDirector component - {0}", this.name);
                numErrors++;
            }

            return numErrors;
        }
    }
}
