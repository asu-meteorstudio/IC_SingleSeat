using DreamscapeUtil;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DreamscapeUtil
{
    /// <summary>
    /// Warns about any lights not parented under one of the specified root game objects
    /// </summary>
    public class StrayLightChecker : MonoBehaviour, ISceneCheckerBehaviour
    {
        [NonNull]
        public List<GameObject> rootObjects = new List<GameObject>();

        [Tooltip("If checked, ignore lights that are disabled or inactive in the hierarchy")]
        public bool ignoreDisabledLights = false;

        public int CheckForErrors()
        {
            if (rootObjects == null || rootObjects.Count == 0)
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
                    if (rootObjects.Contains(it.Current) || (ignoreDisabledLights && !it.Current.activeSelf))
                    {
                        it.SkipChildren();
                    }
                    else
                    {
                        Light light = it.Current.GetComponent<Light>();
                        if (light && (light.enabled || !ignoreDisabledLights))
                        {
                            numErrors++;
                            Debug.LogWarningFormat(it.Current, "StrayLightChecker found light outside of allowed root game objects - {0}", it.Current.name);
                        }
                    }
                }

                return numErrors;
            }
        }
    }
}
