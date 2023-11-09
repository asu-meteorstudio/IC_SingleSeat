using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DreamscapeUtil
{
    public class SceneReferenceUniquenessCheck : ISceneCheckerGlobalSceneCheck
    {
        public int CheckForErrors(SceneCheckerContext context)
        {
            HashSet<string> ids = new HashSet<string>();

            int numErrors = 0;

            SceneCheckerIterator it = SceneCheckerIterator.IterContext(context);
            while (it.MoveNext())
            {
                if(it.Current.TryGetComponent<SceneReference>(out SceneReference reference))
                {
                    if (!ids.Add(reference.id))
                    {
                        Debug.LogWarningFormat(reference, "Multiple SceneReference instances with id '{0}'", reference.id);
                        numErrors++;
                    }
                }
            }


            return numErrors;
        }
    }
}
