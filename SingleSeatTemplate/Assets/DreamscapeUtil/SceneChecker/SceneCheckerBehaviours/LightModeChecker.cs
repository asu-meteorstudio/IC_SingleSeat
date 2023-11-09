using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DreamscapeUtil
{
    public enum LightMode
    {
        Mixed = 1,
        Baked = 2,
        Realtime = 4
    }

    /// <summary>
    /// Generates warning about lights with the wrong mode(realtime, mixed or baked)
    /// </summary>
    public class LightModeChecker : MonoBehaviour, ISceneCheckerBehaviour
    {
        public LightMode mode = LightMode.Realtime;
        public bool recursive = true;
        public int CheckForErrors()
        {
#if UNITY_EDITOR
            if (recursive)
            {
                int numErrors = 0;
                SceneCheckerIterator it = SceneCheckerIterator.IterHierarchy(gameObject);

                while (it.MoveNext())
                {
                    GameObject go = it.Current;
                    LightModeChecker c = go.GetComponent<LightModeChecker>();
                    if (c && go != gameObject)
                    {
                        it.SkipChildren();
                    }
                    else
                    {
                        Light l = go.GetComponent<Light>();
                        if (l && (int)l.lightmapBakeType != (int)mode)
                        {
                            numErrors++;
                            Debug.LogWarningFormat(l, "Light '{0}' should be set to {1}", l.name, mode.ToString());
                        }
                    }
                }

                return numErrors;
            }
            else
            {
                Light l = GetComponent<Light>();
                if(l && (int)l.lightmapBakeType != (int)mode)
                {
                    Debug.LogWarningFormat(l, "Light '{0}' should be set to {1}", l.name, mode.ToString());
                    return 1;
                }
                else
                {
                    return 0;
                }
            }
#else
            return 0;
#endif
        }
    }
}