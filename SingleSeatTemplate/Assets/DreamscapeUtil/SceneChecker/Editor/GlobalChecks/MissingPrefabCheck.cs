using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace DreamscapeUtil
{
    [Description("Checks for references to deleted prefabs")]
    public class MissingPrefabCheck : ISceneCheckerGlobalSceneCheck
    {
        public int CheckForErrors(SceneCheckerContext context)
        {
            int numErrors = 0;

            SceneCheckerIterator it = SceneCheckerIterator.IterContext(context);


            while (it.MoveNext())
            {
#if UNITY_2018_1_OR_NEWER
                //Note - for some reason the first check doesn't work on nested prefabs, falling back on just checking for (Missing Prefab) in object name instead
                //(the second check doesn't work if the top-level prefab was deleted, so both are necessary at the moment)
                if(
                    (PrefabUtility.IsAnyPrefabInstanceRoot(it.Current) && PrefabUtility.GetPrefabAssetType(it.Current) == PrefabAssetType.MissingAsset)
                    || it.Current.name.Contains("(Missing Prefab)")
                    )
#else
                if(PrefabUtility.GetPrefabType(it.Current) == PrefabType.MissingPrefabInstance)
#endif
                {
                    numErrors++;
                    Debug.LogWarningFormat(it.Current, "Missing Prefab - {0}", it.Current.name);
                }
            }

            return numErrors;
        }
    }
}
