using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace DreamscapeUtil
{
    [Description("Checks for references to deleted scripts")]
    public class MissingScriptCheck : ISceneCheckerGlobalSceneCheck
    {
        public int CheckForErrors(SceneCheckerContext context)
        {
            int numErrors = 0;

            SceneCheckerIterator it = SceneCheckerIterator.IterContext(context);

            while (it.MoveNext())
            {
                foreach(Behaviour b in it.Current.GetComponents<Behaviour>())
                {
                    if (!b)
                    {
                        numErrors++;
                        Debug.LogWarningFormat(it.Current, "Gameobject '{0}' has a reference to a deleted script", it.Current.name);
                    }
                }
            }

            return numErrors;
        }
    }
}
