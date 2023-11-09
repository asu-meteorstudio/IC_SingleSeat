#if UNITY_2018_1_OR_NEWER

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.SceneManagement;

namespace DreamscapeUtil
{
    [Description("Checks for constraints that are inactive or have null references")]
    public class ConstraintCheck : ISceneCheckerGlobalSceneCheck
    {
        public int CheckForErrors(SceneCheckerContext context)
        {
            int numErrors = 0;

            SceneCheckerIterator it = SceneCheckerIterator.IterContext(context);

            while (it.MoveNext())
            {
                if (it.Current.TryGetComponent<IConstraint>(out _ ))    //optimization - most gameobjects won't have constraints
                {
                    foreach (IConstraint c in it.Current.GetComponents<IConstraint>())
                    {
                        if (!c.constraintActive)
                        {
                            numErrors++;
                            Debug.LogWarningFormat(it.Current, "Constraint not active - {0}", it.Current.name);
                        }
                        for (int i = 0; i < c.sourceCount; i++)
                        {
                            if (c.GetSource(i).sourceTransform == null)
                            {
                                numErrors++;
                                Debug.LogWarningFormat(it.Current, "Null reference in Constraint - {0}", it.Current.name);
                            }
                        }
                    }
                }
            }

            return numErrors;
        }
    }
}

#endif