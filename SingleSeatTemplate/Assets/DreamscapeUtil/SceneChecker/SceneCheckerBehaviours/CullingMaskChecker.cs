using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DreamscapeUtil
{

    /// <summary>
    /// Generates warnings for lights with the wrong culling mask
    /// </summary>
    public class CullingMaskChecker : MonoBehaviour, ISceneCheckerBehaviour
    {
        public LayerMask cullingMask;
        public bool recursive = true;

        public int CheckForErrors()
        {
            Light l = GetComponent<Light>();
            if (l && l.cullingMask != cullingMask)
            {
                Debug.LogWarningFormat(this, "'{0}' has the wrong layer mask", gameObject.name);
                return 1;
            }
            else if (recursive)
            {
                int numErrors = 0;

                SceneCheckerIterator it = SceneCheckerIterator.IterHierarchy(gameObject);
                while (it.MoveNext())
                {
                    GameObject go = it.Current;
                    if(go != gameObject && go.GetComponent<CullingMaskChecker>())
                    {
                        it.SkipChildren();
                    }
                    else
                    {
                        Light light = go.GetComponent<Light>();
                        if (light && light.cullingMask != cullingMask)
                        {
                            Debug.LogWarningFormat(go, "'{0}' has the wrong layer mask", go.name);
                            numErrors++;
                        }
                    }
                }

                return numErrors;
            }
            else
            {
                return 0;
            }
        }

    }
}
