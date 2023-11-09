using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace DreamscapeUtil
{
    [Description("Checks for enabled cameras and audio listeners in scene")]
    public class CameraCheck : ISceneCheckerGlobalSceneCheck
    {
        public int CheckForErrors(SceneCheckerContext context)
        {
            int numErrors = 0;

            SceneCheckerIterator it = SceneCheckerIterator.IterContext(context);

#if !DSUTIL_ARTANIM_COMMON_IN_PROJECT //if not using artanim sdk, just check if there are multiple cameras or audio listeners
            int numCameras = 0;
            int numAudioListeners = 0;
#endif

            while (it.MoveNext())
            {
                GameObject go = it.Current;
                if (go.TryGetComponent<DestroyInSession>(out _ ))
                {
                    it.SkipChildren();
                }
                else
                {
                    if (go.TryGetComponent<Camera>(out Camera c) && c.isActiveAndEnabled && c.targetTexture == null)
                    {
#if DSUTIL_ARTANIM_COMMON_IN_PROJECT
                        numErrors++;
                        Debug.LogWarningFormat(c, "A camera is turned on - {0}", c.gameObject.name);
#else
                        numCameras++;
#endif
                    }

                    if(go.TryGetComponent<AudioListener>(out AudioListener l) && l.isActiveAndEnabled)
                    {
#if DSUTIL_ARTANIM_COMMON_IN_PROJECT
                        numErrors++;
                        Debug.LogWarningFormat(l, "An audio listener is turned on - {0}", l.gameObject.name);
#else
                        numAudioListeners++;
#endif
                    }
                }
            }

#if !DSUTIL_ARTANIM_COMMON_IN_PROJECT
            if(numCameras > 1)
            {
                Debug.LogWarning("More than one camera is enabled");
                numErrors++;
            }

            if(numAudioListeners > 1)
            {
                Debug.LogWarning("More than one audio listener is enabled");
                numErrors++;
            }
#endif


            return numErrors;
        }
    }
}
