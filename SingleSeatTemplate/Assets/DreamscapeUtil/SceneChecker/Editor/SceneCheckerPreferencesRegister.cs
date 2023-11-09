using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace DreamscapeUtil
{

    public class SceneCheckerPreferencesRegister : MonoBehaviour
    {
#if UNITY_2018_1_OR_NEWER
        [SettingsProvider]
        public static SettingsProvider CreateSceneCheckerSettingsProvider()
        {
            SettingsProvider provider = new SettingsProvider("Project/SceneChecker", SettingsScope.User)
            {
                label = "Scene Checker",
                guiHandler = (searchContext) =>
                {
                    GuiFunc();
                },
                keywords = new HashSet<string>(new[] { "Scene", "Checker" })
            };

            return provider;
        }
#endif
        public static void GuiFunc()
        {
            bool checkOnBuild = EditorPrefs.GetBool("Dreamscape/SceneChecker/AutoCheckOnBuild", true);

            checkOnBuild = EditorGUILayout.BeginToggleGroup("Check for errors before building", checkOnBuild);
            EditorGUILayout.EndToggleGroup();

            EditorPrefs.SetBool("Dreamscape/SceneChecker/AutoCheckOnBuild", checkOnBuild);
        }
    }

    #if !UNITY_2018_1_OR_NEWER
    public class SceneCheckerPreferencesWindow : EditorWindow
    {
        [MenuItem("Dreamscape/Scene Checker/User Preferences...")]
        static void Init()
        {
            SceneCheckerPreferencesWindow window = (SceneCheckerPreferencesWindow)EditorWindow.GetWindow(typeof(SceneCheckerPreferencesWindow), true, "Scene Checker Preferences");
        }


        private void OnGUI()
        {
            SceneCheckerPreferencesRegister.GuiFunc();
        }
    }
    #endif

}
