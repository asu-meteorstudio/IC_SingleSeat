using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if DSUTIL_ARTANIM_COMMON_IN_PROJECT
using Artanim;
#endif

namespace DreamscapeUtil
{
    /// <summary>
    /// Destroys the object on enable when running in a session within the artanim sdk.
    /// This includes normal builds running in the pod and standalone mode.
    /// 
    /// Add this script to preview cameras or other test objects that might
    /// conflict with the sdk in normal operation.
    /// </summary>
    public class DestroyInSession : MonoBehaviour
    {
        private void OnEnable()
        {
#if DSUTIL_ARTANIM_COMMON_IN_PROJECT
            if (GameController.Instance)
            {
                Destroy(gameObject);
            }
#endif
        }
    }
}

