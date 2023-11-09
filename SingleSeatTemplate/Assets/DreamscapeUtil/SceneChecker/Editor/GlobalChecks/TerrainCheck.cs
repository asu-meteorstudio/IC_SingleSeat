using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DreamscapeUtil
{
    [Description("Checks for missing references to terrain assets")]
    public class TerrainCheck : ISceneCheckerGlobalSceneCheck
    {
        public int CheckForErrors(SceneCheckerContext context)
        {
            int numErrors = 0;

            SceneCheckerIterator it = SceneCheckerIterator.IterContext(context);
            while (it.MoveNext())
            {
                if (it.Current.TryGetComponent<Terrain>(out Terrain t) && t.terrainData == null)
                {
                    numErrors++;
                    Debug.LogWarningFormat(t, "Missing terrain data on Terrain component - {0}", t.name);
                }

                if (it.Current.TryGetComponent<TerrainCollider>(out TerrainCollider tc) && tc.terrainData == null)
                {
                    numErrors++;
                    Debug.LogWarningFormat(tc, "Missing terrain data on TerrainCollider component - {0}", tc.name);
                }

            }

            return numErrors;
        }
    }
}
