using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.AnimatedValues;
using UnityEngine;

namespace DreamscapeUtil
{
    public class SceneCheckerSettingsIMGUIRegister : MonoBehaviour
    {
        private static bool expandGlobalSceneChecks = true;
        private static bool expandGlobalProjectChecks = true;

#if UNITY_2018_1_OR_NEWER
        [SettingsProvider]
        public static SettingsProvider CreateSceneCheckerSettingsProvider()
        {
            SettingsProvider provider = new SettingsProvider("Project/SceneChecker", SettingsScope.Project)
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
            bool changed = false;
            EditorGUI.BeginChangeCheck();
            SceneCheckerSettings settings = SceneCheckerSettings.Instance;
            settings.checkInactiveObjects = EditorGUILayout.BeginToggleGroup("Check Inactive Objects", settings.checkInactiveObjects);
            EditorGUILayout.EndToggleGroup();
            settings.checkDisabledBehaviours = EditorGUILayout.BeginToggleGroup("Check Disabled Behaviours", settings.checkDisabledBehaviours);
            EditorGUILayout.EndToggleGroup();
            settings.checkEditorOnlyObjects = EditorGUILayout.BeginToggleGroup("Check Editor Only Objects", settings.checkEditorOnlyObjects);
            EditorGUILayout.EndToggleGroup();
            settings.ignoreAllObjectsWithDestroyInSession = EditorGUILayout.BeginToggleGroup("Ignore All Objects With Destroy In Session Component", settings.ignoreAllObjectsWithDestroyInSession);
            EditorGUILayout.EndToggleGroup();
            if (EditorGUI.EndChangeCheck())
            {
                changed = true;
            }

            string[] defines =  PlayerSettings.GetScriptingDefineSymbolsForGroup(BuildTargetGroup.Standalone).Split(';');
            bool enableBakeryExtensions = defines.Contains("SCENE_CHECKER_BAKERY_EXTENSIONS");
            EditorGUI.BeginChangeCheck();

            enableBakeryExtensions = EditorGUILayout.BeginToggleGroup(
                new GUIContent("Enable Bakery Extensions", "only check if Bakery lightmap package is imported in the project"), enableBakeryExtensions);
            EditorGUILayout.EndToggleGroup();
            if (EditorGUI.EndChangeCheck())
            {
                List<string> definesList = new List<string>(defines);

                if (enableBakeryExtensions)
                {
                    definesList.Add("SCENE_CHECKER_BAKERY_EXTENSIONS");
                }
                else
                {
                    definesList.Remove("SCENE_CHECKER_BAKERY_EXTENSIONS");
                }
                PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.Standalone, string.Join(";", definesList));
            }

            EditorGUI.BeginChangeCheck();
            GUILayout.BeginVertical(new GUIContent("Exclude Scenes", "Scenes to exclude from check all scenes in build"),
                "window", GUILayout.ExpandHeight(false), GUILayout.MaxHeight(80), GUILayout.MaxWidth(500));
            for(int i = 0; i < settings.ignoreScenes.Count; i++)
            {
                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button("-", GUILayout.ExpandWidth(false)))
                {
                    settings.ignoreScenes.RemoveAt(i);
                    i--;
                }
                else
                {
                    settings.ignoreScenes[i] = EditorGUILayout.TextField(settings.ignoreScenes[i]);
                }
                EditorGUILayout.EndHorizontal();
            }
            if (GUILayout.Button("+", GUILayout.ExpandWidth(false)))
            {
                settings.ignoreScenes.Add("");
            }
            GUILayout.EndVertical();

            GUILayout.BeginVertical(new GUIContent("Additional Scenes", "Additional scenes to check when checking all scenes in build"),
                "window", GUILayout.ExpandHeight(false), GUILayout.MaxHeight(80), GUILayout.MaxWidth(500));
            for (int i = 0; i < settings.additionalScenes.Count; i++)
            {
                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button("-", GUILayout.ExpandWidth(false)))
                {
                    settings.additionalScenes.RemoveAt(i);
                    i--;
                }
                else
                {
                    settings.additionalScenes[i] = EditorGUILayout.TextField(settings.additionalScenes[i]);
                }
                EditorGUILayout.EndHorizontal();
            }
            if (GUILayout.Button("+", GUILayout.ExpandWidth(false)))
            {
                settings.additionalScenes.Add("");
            }
            GUILayout.EndVertical();



#if UNITY_2018_1_OR_NEWER
            expandGlobalSceneChecks = EditorGUILayout.BeginFoldoutHeaderGroup(expandGlobalSceneChecks, "Global Scene Checks");

#else
            expandGlobalSceneChecks = EditorGUILayout.Foldout(expandGlobalSceneChecks, "Global Scene Checks");
#endif
            if (expandGlobalSceneChecks)
            {
                EditorGUI.indentLevel++;
                List<ISceneCheckerGlobalSceneCheck> checks = new List<ISceneCheckerGlobalSceneCheck>();

                Type type = typeof(ISceneCheckerGlobalSceneCheck);
                IEnumerable<Type> types = AppDomain.CurrentDomain.GetAssemblies()
                    .SelectMany(s => s.GetTypes())
                    .Where(p => type.IsAssignableFrom(p));

                foreach (Type t in types)
                {
                    if (t != type)
                    {
                        bool enabled = SceneCheckerSettings.Instance.disabledSceneChecks.Contains(t.Name);
                        bool wasEnabled = enabled;

                        string name = ObjectNames.NicifyVariableName(t.Name);
                        string description = "";
                        Description[] d = (Description[])t.GetCustomAttributes(typeof(Description), false);
                        if (d.Length > 0)
                        {
                            description = d[0].description;
                        }
                        enabled = EditorGUILayout.BeginToggleGroup(new GUIContent(name, description), enabled);
                        EditorGUILayout.EndToggleGroup();
                        if (!enabled)
                        {
                            if (!wasEnabled)
                            {
                                SceneCheckerSettings.Instance.disabledSceneChecks.Add(t.Name);
                            }
                        }
                        else
                        {
                            if (wasEnabled)
                            {
                                SceneCheckerSettings.Instance.disabledSceneChecks.Remove(t.Name);
                            }
                        }
                    }
                }
                EditorGUI.indentLevel--;
            }
#if UNITY_2018_1_OR_NEWER
            EditorGUILayout.EndFoldoutHeaderGroup();
#endif

#if UNITY_2018_1_OR_NEWER
            expandGlobalProjectChecks = EditorGUILayout.BeginFoldoutHeaderGroup(expandGlobalProjectChecks, "Global Project Checks");

#else
            expandGlobalProjectChecks = EditorGUILayout.Foldout(expandGlobalProjectChecks, "Global Project Checks");
#endif
            if (expandGlobalProjectChecks)
            {
                EditorGUI.indentLevel++;
                List<ISceneCheckerGlobalProjectCheck> checks = new List<ISceneCheckerGlobalProjectCheck>();

                Type type = typeof(ISceneCheckerGlobalProjectCheck);
                IEnumerable<Type> types = AppDomain.CurrentDomain.GetAssemblies()
                    .SelectMany(s => s.GetTypes())
                    .Where(p => type.IsAssignableFrom(p));

                foreach (Type t in types)
                {
                    if (t != type)
                    {
                        bool enabled = SceneCheckerSettings.Instance.disabledProjectChecks.Contains(t.Name);
                        bool wasEnabled = enabled;

                        string name = ObjectNames.NicifyVariableName(t.Name);
                        string description = "";
                        Description[] d = (Description[])t.GetCustomAttributes(typeof(Description), false);
                        if (d.Length > 0)
                        {
                            description = d[0].description;
                        }
                        enabled = EditorGUILayout.BeginToggleGroup(new GUIContent(name, description), enabled);
                        EditorGUILayout.EndToggleGroup();
                        if (!enabled)
                        {
                            if (!wasEnabled)
                            {
                                SceneCheckerSettings.Instance.disabledProjectChecks.Add(t.Name);
                            }
                        }
                        else
                        {
                            if (wasEnabled)
                            {
                                SceneCheckerSettings.Instance.disabledProjectChecks.Remove(t.Name);
                            }
                        }
                    }
                }
                EditorGUI.indentLevel--;
            }
#if UNITY_2018_1_OR_NEWER
            EditorGUILayout.EndFoldoutHeaderGroup();
#endif

            if (EditorGUI.EndChangeCheck())
            {
                changed = true;
            }

            if (changed)
            {
                EditorUtility.SetDirty(SceneCheckerSettings.Instance);
            }
        }

    }

#if !UNITY_2018_1_OR_NEWER
    public class SceneCheckerSettingsWindow : EditorWindow
    {
        [MenuItem("Dreamscape/Scene Checker/Project Settings...")]
        static void Init()
        {
            SceneCheckerSettingsWindow window = (SceneCheckerSettingsWindow)EditorWindow.GetWindow(typeof(SceneCheckerSettingsWindow), true, "Scene Checker Project Settings");
        }


        private void OnGUI()
        {
            SceneCheckerSettingsIMGUIRegister.GuiFunc();
        }
    }
#endif

}
