using DreamscapeUtil;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DreamscapeUtil
{
    /// <summary>
    /// Warns about any renderers that are not parented under one of the specified game objects
    /// </summary>
    public class StrayGeometryChecker : MonoBehaviour, ISceneCheckerBehaviour
    {
        [NonNull]
        public List<GameObject> rootObjects = new List<GameObject>();

        [Tooltip("If checked, ignore renderers that are disabled or inactive in the hierarchy")]
        public bool ignoreDisabledRenderers = false;

        public int CheckForErrors()
        {
            if(rootObjects == null || rootObjects.Count == 0)
            {
                Debug.LogWarningFormat(this, "Root Objects cannot be empty - {0}", this.name);
                return 1;
            }
            else
            {
                int numErrors = 0;

                SceneCheckerIterator it = SceneCheckerIterator.IterScene(gameObject.scene);

                while (it.MoveNext())
                {
                    if (rootObjects.Contains(it.Current) || (ignoreDisabledRenderers && !it.Current.activeSelf))
                    {
                        it.SkipChildren();
                    }
                    else
                    {
                        Renderer rend = it.Current.GetComponent<Renderer>();
                        if (rend && (rend.enabled || !ignoreDisabledRenderers))
                        {
                            numErrors++;
                            Debug.LogWarningFormat(it.Current, "StrayGeometryChecker found renderer outside of allowed root game objects - {0}", it.Current.name);
                        }
                    }
                }

                return numErrors;
            }
        }
    }
}
