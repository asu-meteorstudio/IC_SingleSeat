using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace DreamscapeUtil
{
    [Description("Checks for missing references to mesh assets")]
    public class MissingMeshCheck : ISceneCheckerGlobalSceneCheck
    {
        public int CheckForErrors(SceneCheckerContext context)
        {
            int numErrors = 0;

            SceneCheckerIterator it = SceneCheckerIterator.IterContext(context);
            while (it.MoveNext())
            {
                if(it.Current.TryGetComponent<MeshFilter>(out MeshFilter mf) && mf.sharedMesh == null)
                {
                    //expection for TextMeshPro which normally has a null reference on MeshFilter component
                    if (!it.Current.TryGetComponent<TMPro.TextMeshPro>(out _))
                    {
                        numErrors++;
                        Debug.LogWarningFormat(mf, "Missing mesh on MeshFilter component - {0}", mf.name);
                    }
                }

                if(it.Current.TryGetComponent<MeshCollider>(out MeshCollider mc) && mc.sharedMesh == null)
                {
                    numErrors++;
                    Debug.LogWarningFormat(mc, "Missing mesh on MeshCollider component - {0}", mc.name);
                }

                if(it.Current.TryGetComponent<SkinnedMeshRenderer>(out SkinnedMeshRenderer smr) && smr.sharedMesh == null)
                {
                    numErrors++;
                    Debug.LogWarningFormat(smr, "Missing mesh on SkinnedMeshRenderer component - {0}", smr.name);
                }
            }

            return numErrors;
        }
    }
}