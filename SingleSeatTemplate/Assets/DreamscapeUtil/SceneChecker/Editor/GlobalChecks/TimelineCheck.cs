using Artanim;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;
using UnityEngine.Timeline;

namespace DreamscapeUtil
{
    [Description("Checks for missing references in timelines")]
    public class TimelineCheck : ISceneCheckerGlobalSceneCheck
    {
        public int CheckForErrors(SceneCheckerContext context)
        {
            int numErrors = 0;

            SceneCheckerIterator it = SceneCheckerIterator.IterContext(context);

            while (it.MoveNext())
            {
                if (it.Current.TryGetComponent<PlayableDirector>(out PlayableDirector director))
                {
                    PlayableAsset asset = director.playableAsset;
                    TimelineAsset timeline = (TimelineAsset)asset;
                    if (timeline)
                    {
                        //start at one to skip over marker track
                        for(int i = 1; i < timeline.outputTrackCount; i++)
                        {
                            TrackAsset track = timeline.GetOutputTrack(i);
#if UNITY_2018_1_OR_NEWER
                            if (!track.mutedInHierarchy)
#else
                            if(!track.muted)
#endif
                            {
                                foreach (PlayableBinding binding in track.outputs)
                                {
#if UNITY_2018_1_OR_NEWER
                                    if (binding.outputTargetType != null && !director.GetGenericBinding(binding.sourceObject) && binding.outputTargetType != typeof(BaseSubtitleDisplayer))
#else
                                    if(binding.streamType != null)
#endif
                                    {
                                        Debug.LogWarningFormat(director, "Missing binding for timeline track '{0}'", track.name);
                                        numErrors++;
                                    }

                                    //for animation tracks, check for missing animation clips
#if UNITY_2018_1_OR_NEWER
                                    if (binding.outputTargetType == typeof(Animator))
#else
                                    if(binding.streamType == DataStreamType.Animation)
#endif
                                    {
                                        foreach (TimelineClip clip in track.GetClips())
                                        {
                                            if (!clip.animationClip)
                                            {
                                                Debug.LogWarningFormat(director, "Missing animation clip on timeline track '{0}'", track.name);
                                                numErrors++;
                                            }
                                        }
                                    }
                                    //for audio tracks, check for missing audio clips
#if UNITY_2018_1_OR_NEWER
                                    else if (binding.outputTargetType == typeof(AudioSource))
#else
                                    else if(binding.streamType == DataStreamType.Audio)
#endif
                                    {
                                        foreach (TimelineClip clip in track.GetClips())
                                        {
                                            AudioPlayableAsset clipAsset = (AudioPlayableAsset) clip.asset;
                                            if (clipAsset)
                                            {
                                                if (clipAsset.clip == null)
                                                {
                                                    Debug.LogWarningFormat(director, "Missing audio clip on timeline track '{0}'", track.name);
                                                    numErrors++;
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        numErrors++;
                        Debug.LogWarningFormat(director, "Timeline asset is null - {0}", director.name);
                    }
                }
                
            }

            return numErrors;
        }
    }
}
