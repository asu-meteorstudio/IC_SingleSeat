using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DreamscapeUtil
{

    //
    // Summary:
    //     ///
    //     Checks that objects in the scene are correctly enabled or disabled
    //     ///
    public class EnabledChecker : MonoBehaviour, ISceneCheckerBehaviour
    {
        [Tooltip("List of objects which should be set to active before building")]
        public GameObject[] activeObjects;
        [Tooltip("List of objects which should be set to inactive before building")]
        public GameObject[] inactiveObjects;

        [Tooltip("List of components which should be enabled before building")]
        public Behaviour[] enabledComponents;
        [Tooltip("List of Components which should be disabled before building")]
        public Behaviour[] disabledComponents;

        public int CheckForErrors()
        {
            int numErrors = 0;

            if (activeObjects != null)
            {
                foreach (GameObject go in activeObjects)
                {
                    if (!go)
                    {
                        numErrors++;
                        Debug.LogWarning("Null reference in active objects list", this);
                    }
                    else if (!go.activeSelf)
                    {
                        numErrors++;
                        Debug.LogWarningFormat(go, "GameObject '{0}' should be active.", go.name);
                    }
                }
            }

            if (inactiveObjects != null)
            {
                foreach (GameObject go in inactiveObjects)
                {
                    if (!go)
                    {
                        numErrors++;
                        Debug.LogWarning("Null reference in inactive objects list", this);
                    }
                    else if (go.activeSelf)
                    {
                        numErrors++;
                        Debug.LogWarningFormat(go, "GameObject '{0}' should be inactive.", go.name);
                    }
                }
            }

            if (enabledComponents != null)
            {
                foreach (Behaviour co in enabledComponents)
                {
                    if (!co)
                    {
                        numErrors++;
                        Debug.LogWarning("Null reference in enabled components list", this);
                    }
                    else if (!co.enabled)
                    {
                        numErrors++;
                        Debug.LogWarningFormat(co, "Component '{0}({1})' should be enabled.", co.GetType().Name, co.gameObject.name);
                    }
                }
            }


            if (disabledComponents != null)
            {
                foreach (Behaviour co in disabledComponents)
                {
                    if (!co)
                    {
                        numErrors++;
                        Debug.LogWarning("Null reference in disabled components list", this);
                    }
                    else if (co.enabled)
                    {
                        numErrors++;
                        Debug.LogWarningFormat(co, "Component '{0}({1})' should be disabled.", co.GetType().Name, co.gameObject.name);
                    }
                }
            }

            return numErrors;
        }
    }
}
