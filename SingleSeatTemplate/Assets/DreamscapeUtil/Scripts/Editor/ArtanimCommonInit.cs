using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;

//checks for artanim common folder and automatically sets up scripting define symbol if it exists

[InitializeOnLoad]
public class ArtanimCommonInit : MonoBehaviour
{
    static ArtanimCommonInit()
    {
#if !DSUTIL_ARTANIM_COMMON_IN_PROJECT

        if (AssetDatabase.IsValidFolder("Assets/ArtanimCommon"))
        {

            string[] defines = PlayerSettings.GetScriptingDefineSymbolsForGroup(BuildTargetGroup.Standalone).Split(';');
            if (!defines.Contains("DSUTIL_ARTANIM_COMMON_IN_PROJECT"))
            {

                List<string> definesList = new List<string>(defines);
                definesList.Add("DSUTIL_ARTANIM_COMMON_IN_PROJECT");
                PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.Standalone, string.Join(";", definesList));
            }
        }
#endif
    }
}
