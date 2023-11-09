using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;
using System;
using System.Reflection;
using UnityEditor.SceneManagement;
using UnityEditor.Build;
using UnityEngine.Events;
using System.Linq;
using UnityEditor.Experimental.SceneManagement;

namespace DreamscapeUtil
{
    [InitializeOnLoad]
    public class SceneChecker
    {

        static SceneChecker()
        {
            BuildPlayerWindow.RegisterBuildPlayerHandler(BuildPlayer);
        }

        static List<ISceneCheckerGlobalSceneCheck> GetEnabledGlobalSceneChecks()
        {
            List<ISceneCheckerGlobalSceneCheck> checks = new List<ISceneCheckerGlobalSceneCheck>();

            Type type = typeof(ISceneCheckerGlobalSceneCheck);
            IEnumerable<Type> types = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(s => s.GetTypes())
                .Where(p => type.IsAssignableFrom(p));

            foreach(Type t in types)
            {
                if (t != type)
                {
                    if (!SceneCheckerSettings.Instance.disabledSceneChecks.Contains(t.Name))
                    {
                        checks.Add((ISceneCheckerGlobalSceneCheck)Activator.CreateInstance(t));
                    }
                }
            }

            return checks;
        }

        static List<ISceneCheckerGlobalProjectCheck> GetEnabledGlobalProjectChecks()
        {
            List<ISceneCheckerGlobalProjectCheck> checks = new List<ISceneCheckerGlobalProjectCheck>();

            Type type = typeof(ISceneCheckerGlobalProjectCheck);
            IEnumerable<Type> types = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(s => s.GetTypes())
                .Where(p => type.IsAssignableFrom(p));

            foreach (Type t in types)
            {
                if (t != type)
                {
                    if (!SceneCheckerSettings.Instance.disabledProjectChecks.Contains(t.Name))
                    {
                        checks.Add((ISceneCheckerGlobalProjectCheck)Activator.CreateInstance(t));
                    }
                }
            }

            return checks;
        }

        static void BuildPlayer(BuildPlayerOptions options)
        {
            if (EditorPrefs.GetBool("Dreamscape/SceneChecker/AutoCheckOnBuild", true))
            {
                string msg;
                if (!CheckAllScenesInBuild(out msg))
                {
                    msg += "\n\nBuild Anyway?";
                    if (!EditorUtility.DisplayDialog("Scene Checker", msg, "Yes", "No"))
                    {
                        return;
                    }
                }
            }
            BuildPlayerWindow.DefaultBuildMethods.BuildPlayer(options);
        }

        /*[MenuItem("Dreamscape/Scene Checker/Check Active Scene")]
        static void CheckCurrentScene()
        {
            Scene scene = EditorSceneManager.GetActiveScene();

            int numErrors = CheckScene(scene);
            
            if(numErrors > 0)
            {
                Debug.LogWarningFormat("{0} errors found in scene '{1}'", numErrors, scene.name);
            }
            else
            {
                Debug.LogFormat("No errors found in scene '{0}'", scene.name);
            }

        }*/

        [MenuItem("Dreamscape/Scene Checker/Check Open Scenes")]
        static void CheckOpenScenes()
        {
            Dictionary<string, int> errorsByScene = new Dictionary<string, int>();
            int numTotalErrors = 0;

            HashSet<GameObject> prefabDependencies = new HashSet<GameObject>();

            if (PrefabStageUtility.GetCurrentPrefabStage() != null)
            {
                SceneCheckerContext context = SceneCheckerContext.FromCurrentPrefabStage();
                int numErrors = CheckContext(context, out HashSet<GameObject> dependencies);
                foreach(GameObject prefab in dependencies)
                {
                    prefabDependencies.Add(prefab);
                }

                if(numErrors > 0)
                {
                    errorsByScene[context.Name] = numErrors;
                    numTotalErrors += numErrors;
                }
            }
            else
            {
                for (int i = 0; i < EditorSceneManager.sceneCount; i++)
                {
                    Scene scene = EditorSceneManager.GetSceneAt(i);
                    if (scene.isLoaded)
                    {
                        int numErrors = CheckContext(SceneCheckerContext.FromScene(scene), out HashSet<GameObject> dependencies);
                        foreach (GameObject prefab in dependencies)
                        {
                            prefabDependencies.Add(prefab);
                        }
                        if (numErrors > 0)
                        {
                            errorsByScene[scene.name] = numErrors;
                            numTotalErrors += numErrors;
                        }
                    }

                }
            }

            foreach(GameObject go in prefabDependencies)
            {
                int numErrors = CheckDependency(go, new List<GameObject>());
                if (numErrors > 0)
                {
                    errorsByScene[go.name] = numErrors;
                    numTotalErrors += numErrors;
                }
            }

            if(errorsByScene.Count == 0)
            {
                Debug.Log("No errors found in open scenes.");
            }
            else
            {
                Debug.LogWarningFormat("Found {0} errors in {1} scene(s):", numTotalErrors, errorsByScene.Count);
                foreach(string scene in errorsByScene.Keys)
                {
                    Debug.LogWarningFormat("'{0}': {1} error(s)", scene, errorsByScene[scene]);
                }
            }

        }

        private static int CheckDependency(GameObject go, List<GameObject> visited)
        {
            visited.Add(go);

            /*Stage stage = StageUtility.GetCurrentStage();
            AssetDatabase.OpenAsset(go);
            int numErrors = 0;
            HashSet<GameObject> prefabDeps;
            try
            {
                SceneCheckerContext context = SceneCheckerContext.FromCurrentPrefabStage();
                numErrors = CheckContext(context, out prefabDeps);
            }
            finally
            {
                StageUtility.GoToStage(stage, false);
            }

            if(prefabDeps != null)
            {
                foreach(GameObject prefab in prefabDeps)
                {
                    if (!visited.Contains(prefab))
                    {
                        numErrors += CheckDependency(prefab, visited);
                    }
                }
            }*/

            return 0;
        }

        [MenuItem("Dreamscape/Scene Checker/Check All Scenes in Build")]
        static void CheckSceneInBuildMenuItem()
        {
            string msg;
            CheckAllScenesInBuild(out msg);
        }

        [MenuItem("Dreamscape/Scene Checker/Perform Global Project Checks")]
        static void PerformGlobalProjectChecks()
        {
            int numErrors = 0;
            foreach(ISceneCheckerGlobalProjectCheck check in GetEnabledGlobalProjectChecks())
            {
                numErrors += check.CheckForErrors();
            }

            if(numErrors == 0)
            {
                Debug.Log("No errors found by global project checks");
            }
            else
            {
                Debug.LogWarningFormat("{0} errors found by global project checks", numErrors);
            }
        }

        static IEnumerable<string> GetScenesInBuild()
        {
            HashSet<string> scenes = new HashSet<string>();
            foreach (EditorBuildSettingsScene buildSettingsScene in EditorBuildSettings.scenes)
            {
                if (!buildSettingsScene.enabled)
                {
                    continue;
                }
                bool exclude = false;
                foreach (string pattern in SceneCheckerSettings.Instance.ignoreScenes)
                {
                    if(pattern.Length < 1)
                    {
                        continue;
                    }
                    if (buildSettingsScene.path.StartsWith(pattern))
                    {
                        exclude = true;
                        continue;
                    }
                }
                if (exclude) continue;

                scenes.Add(buildSettingsScene.path);
            }


            foreach(string pattern in SceneCheckerSettings.Instance.additionalScenes)
            {
                foreach (string guid in AssetDatabase.FindAssets("t:Scene"))
                {
                    string path = AssetDatabase.GUIDToAssetPath(guid);
                    if (path.StartsWith(pattern))
                    {
                        scenes.Add(path);
                    }
                }
            }

            return scenes;
        }

        public static bool CheckScenes(IEnumerable<string> scenes, out string msg)
        {
            bool canceled = false;

            msg = "";
            if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
            {
                msg = "Scene Check Skipped";
                return false;
            }

            UnityEditor.SceneManagement.SceneSetup[] sceneSetup = EditorSceneManager.GetSceneManagerSetup();

            Dictionary<string, int> errorsByScene = new Dictionary<string, int>();
            int numTotalErrors = 0;

            Debug.Log("Performing Global Project Checks...");
            foreach (ISceneCheckerGlobalProjectCheck projectCheck in GetEnabledGlobalProjectChecks())
            {
                numTotalErrors += projectCheck.CheckForErrors();
            }


            HashSet<GameObject> prefabDependencies = new HashSet<GameObject>();

            

            int i = 0;
            int numScenes = scenes.Count<string>();
            foreach (string scenePath in scenes)
            {
                bool exclude = false;
                foreach (string pattern in SceneCheckerSettings.Instance.ignoreScenes)
                {
                    if (pattern.Length < 1)
                    {
                        continue;
                    }
                    if (scenePath.StartsWith(pattern))
                    {
                        exclude = true;
                        continue;
                    }
                }
                if (exclude) continue;

                string progressMsg = string.Format("Checking Scene {0}/{1}: '{2}'...", i + 1, numScenes, scenePath);
                if(EditorUtility.DisplayCancelableProgressBar(string.Format("Scene Checker - Scene {0}/{1}", i + 1, numScenes), progressMsg, (float)i / (float)numScenes))
                {
                    canceled = true;
                    break;
                }

                i++;

                EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Single);

                Scene scene = EditorSceneManager.GetActiveScene();

                int numErrors = CheckContext(SceneCheckerContext.FromScene(scene), out HashSet<GameObject> dependencies);
                foreach (GameObject prefab in dependencies)
                {
                    prefabDependencies.Add(prefab);
                }
                if (numErrors > 0)
                {
                    errorsByScene[scene.name] = numErrors;
                    numTotalErrors += numErrors;
                }

            }

            foreach (GameObject go in prefabDependencies)
            {
                int numErrors = CheckDependency(go, new List<GameObject>());
                if (numErrors > 0)
                {
                    errorsByScene[go.name] = numErrors;
                    numTotalErrors += numErrors;
                }
            }

            if (canceled)
            {
                Debug.LogWarning("Scene Check Canceled in Progress");
            }

            EditorUtility.ClearProgressBar();

            if (numTotalErrors == 0)
            {
                Debug.Log("No errors found in scenes.");
                if (canceled)
                {
                    msg = "Scene Check Canceled in Progress";
                }
            }
            else
            {
                msg = string.Format("Found {0} errors in {1} scene(s):", numTotalErrors, errorsByScene.Count);

                msg += "\n\n";
                foreach (string scene in errorsByScene.Keys)
                {
                    msg += string.Format("'{0}': {1} error(s)", scene, errorsByScene[scene]);
                }

                Debug.LogWarningFormat("Found {0} errors in {1} scene(s):", numTotalErrors, errorsByScene.Count);
                foreach (string scene in errorsByScene.Keys)
                {
                    Debug.LogWarningFormat("'{0}': {1} error(s)", scene, errorsByScene[scene]);
                }
            }

            if (sceneSetup.Length > 0)
            {
                EditorSceneManager.RestoreSceneManagerSetup(sceneSetup);
            }
            else
            {
                EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects, NewSceneMode.Single);
            }



            return errorsByScene.Count == 0 && !canceled;
        }

        static bool CheckAllScenesInBuild(out string msg)
        {
            return CheckScenes(GetScenesInBuild(), out msg);
        }

        static int CheckContext(SceneCheckerContext context, out HashSet<GameObject> prefabDependencies)
        {
            prefabDependencies = new HashSet<GameObject>();
            try
            {
                Debug.LogFormat("Checking for errors in '{0}'...", context.Name);
                int numErrors = 0;

                SceneCheckerIterator it = SceneCheckerIterator.IterContext(context);
                while (it.MoveNext())
                {
                    GameObject go = it.Current;
                    foreach(MonoBehaviour b in go.GetComponents<MonoBehaviour>())
                    {
                        if (!b)
                        {
                            continue;
                        }
                        if(!b.enabled && !SceneCheckerSettings.Instance.checkDisabledBehaviours)
                        {
                            continue;
                        }
                        numErrors += CheckObject(b, out List<GameObject> prefabs);
                        if (prefabs != null)
                        {
                            foreach(GameObject prefab in prefabs)
                            {
                                prefabDependencies.Add(prefab);
                            }
                        }
                    }

                    foreach(ISceneCheckerBehaviour checker in go.GetComponents<ISceneCheckerBehaviour>())
                    {
                        MonoBehaviour b = (MonoBehaviour)checker;
                        if(!b.enabled && SceneCheckerSettings.Instance.checkDisabledBehaviours)
                        {
                            continue;
                        }
                        try
                        {
                            numErrors += checker.CheckForErrors();
                        }
                        catch (Exception e)
                        {
                            numErrors++;
                            Debug.LogErrorFormat((UnityEngine.Object)checker, "Unhandled exception while checking for errors ({0}):", checker.GetType().Name);
                            Debug.LogException(e);
                        }

                    }


                }


                foreach (ISceneCheckerGlobalSceneCheck c in GetEnabledGlobalSceneChecks())
                {
                    try
                    {
                        numErrors += c.CheckForErrors(context);
                    }
                    catch (Exception e)
                    {
                        numErrors++;
                        Debug.LogErrorFormat("Unhandled exception while checking for errors ({0}):", c.GetType().Name);
                        Debug.LogException(e);
                    }
                }

                /*if(numErrors == 0)
                {
                    Debug.LogFormat("No errors found in '{0}'", context.Name);
                }
                else
                {
                    Debug.LogWarningFormat("Found {0} errors in '{1}'", numErrors, context.Name);
                }*/


                return numErrors;
            }
            catch (Exception e)
            {
                Debug.LogError("Unhandled exception while checking for errors:");
                Debug.LogException(e);
                return 1;
            }
        }

        /*static int CheckScene(Scene scene)
        {
            try
            {
                Debug.LogFormat("Checking for errors in scene '{0}'...", scene.name);
                int numErrors = 0;

                bool checkInactiveObjects = SceneCheckerSettings.Instance.checkInactiveObjects;
                bool checkDisabledBehaviours = SceneCheckerSettings.Instance.checkDisabledBehaviours;
                bool checkEditorOnlyObjects = SceneCheckerSettings.Instance.checkEditorOnlyObjects;

                foreach (GameObject go in scene.GetRootGameObjects())
                {

                    foreach (MonoBehaviour behaviour in go.GetComponentsInChildren<MonoBehaviour>(true))
                    {
                        if (!behaviour)
                        {
                            continue;
                        }
                        if (!behaviour.gameObject.activeInHierarchy && !checkInactiveObjects)
                        {
                            continue;
                        }
                        if (!behaviour.enabled && !checkDisabledBehaviours)
                        {
                            continue;
                        }
                        if (ShouldIgnore(behaviour.gameObject) || (!checkEditorOnlyObjects && IsEditorOnly(behaviour.gameObject)))
                        {
                            continue;
                        }

                        numErrors += CheckBehaviour(behaviour);
                    }

                    foreach (ISceneCheckerBehaviour checker in go.GetComponentsInChildren<ISceneCheckerBehaviour>(checkInactiveObjects))
                    {
                        MonoBehaviour behaviour = (MonoBehaviour)checker;
                        if (behaviour.GetComponent<SceneCheckerIgnore>())
                        {
                            continue;
                        }
                        foreach(SceneCheckerIgnore ignore in behaviour.GetComponentsInParent<SceneCheckerIgnore>())
                        {
                            if (ignore.ignoreChildren)
                            {
                                continue;
                            }
                        }
                        if (SceneCheckerSettings.Instance.ignoreAllObjectsWithDestroyInSession)
                        {
                            if (behaviour.GetComponentInParent<DestroyInSession>())
                            {
                                continue;
                            }
                        }

                        if(!checkInactiveObjects && behaviour.gameObject.activeInHierarchy)
                        {
                            continue;
                        }
                        if(!checkEditorOnlyObjects && IsEditorOnly(behaviour.gameObject))
                        {
                            continue;
                        }

                        try
                        {
                            numErrors += checker.CheckForErrors();
                        }
                        catch (Exception e)
                        {
                            numErrors++;
                            Debug.LogErrorFormat((UnityEngine.Object)checker, "Unhandled exception while checking for errors ({0}):", checker.GetType().Name);
                            Debug.LogException(e);
                        }
                    }

                }

                foreach(ISceneCheckerGlobalSceneCheck c in GetEnabledGlobalSceneChecks())
                {
                    try
                    {
                        numErrors += c.CheckForErrors(scene);
                    }
                    catch(Exception e)
                    {
                        numErrors++;
                        Debug.LogErrorFormat("Unhandled exception while checking for errors ({0}):", c.GetType().Name);
                        Debug.LogException(e);
                    }
                }

                if (numErrors == 0)
                {
                    Debug.LogFormat("No errors found in scene '{0}'...", scene.name);
                }
                return numErrors;
            }
            catch(Exception e)
            {
                Debug.LogError("Unhandled exception while checking for errors:");
                Debug.LogException(e);
                return 1;
            }
        }*/

        static bool IsEditorOnly(GameObject go)
        {
            if(go.transform.parent && IsEditorOnly(go.transform.parent.gameObject))
            {
                return true;
            }

            return go.CompareTag("EditorOnly");
        }

        static bool ShouldIgnore(GameObject go)
        {
            if (go.GetComponent<SceneCheckerIgnore>())
            {
                return true;
            }
            if (SceneCheckerSettings.Instance.ignoreAllObjectsWithDestroyInSession &&go.GetComponent<DestroyInSession>()){
                return true;
            }
            else
            {
                return ShouldIgnore_parent(go);
            }
        }

        static bool ShouldIgnore_parent(GameObject go)
        {
            if(go.transform.parent && ShouldIgnore_parent(go.transform.parent.gameObject))
            {
                return true;
            }
            if (SceneCheckerSettings.Instance.ignoreAllObjectsWithDestroyInSession && go.GetComponent<DestroyInSession>())
            {
                return true;
            }
            SceneCheckerIgnore ignore = go.GetComponent<SceneCheckerIgnore>();
            if(ignore && ignore.ignoreChildren)
            {
                return true;
            }
            return false;
        }


        static int CheckObject(UnityEngine.Object obj, out List<GameObject> prefabDependencies)
        {
            int numErrors = 0;
            prefabDependencies = null;

            Type t = obj.GetType();
            foreach (FieldInfo fieldInfo in t.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))
            {
                foreach(ISceneCheckerAttribute attr in fieldInfo.GetCustomAttributes(typeof(ISceneCheckerAttribute), true))
                {
                    if(!attr.CheckField(obj, fieldInfo))
                    {
                        numErrors++;
                    }
                }
                if(typeof(ScriptableObject).IsAssignableFrom(fieldInfo.FieldType))
                {
                    ScriptableObject sobj = fieldInfo.GetValue(obj) as ScriptableObject;
                    if (sobj)
                    {
                        numErrors += CheckObject(sobj, out List<GameObject> prefabDeps);
                        if(prefabDeps != null)
                        {
                            foreach(GameObject prefab in prefabDeps)
                            {
                                prefabDependencies.Add(prefab);
                            }
                        }
                    }
                }
                //find any prefabs referenced by this behaviour so that they can be checked too
                //exception for scene setup script so we don't get warnings about camera prefabs
                if (fieldInfo.FieldType == typeof(GameObject) && !(obj is Artanim.SceneSetup))
                {
                    var value = fieldInfo.GetValue(obj);
                    GameObject go = value as GameObject;
                    if (go != null && go.scene.rootCount == 0) {   //if this is a prefab reference and not a reference to a game object in the scene
                        if(prefabDependencies == null)
                        {
                            prefabDependencies = new List<GameObject>();
                        }
                        prefabDependencies.Add(go);
                    }
                }
                //for array-like objects check for nested prefab dependencies or scriptable object references
                else if (typeof(IList).IsAssignableFrom(fieldInfo.FieldType))
                {
                    var value = fieldInfo.GetValue(obj);
                    IList list = value as IList;
                    if (list != null)
                    {
                        foreach(var elem in list)
                        {
                            if(elem is GameObject)
                            {
                                GameObject go = elem as GameObject;
                                if (go != null && go.scene.rootCount == 0) //if this is a prefab reference and not a reference to a game object in the scene
                                {   
                                    if (prefabDependencies == null)
                                    {
                                        prefabDependencies = new List<GameObject>();
                                    }
                                    prefabDependencies.Add(go);
                                }
                            }
                            else if(elem is ScriptableObject)
                            {
                                ScriptableObject sobj = elem as ScriptableObject;
                                if(sobj != null)
                                {
                                    numErrors += CheckObject(sobj, out List<GameObject> prefabDeps);
                                    if (prefabDeps != null)
                                    {
                                        if(prefabDependencies == null)
                                        {
                                            prefabDependencies = new List<GameObject>();
                                        }
                                        foreach (GameObject prefab in prefabDeps)
                                        {
                                            prefabDependencies.Add(prefab);
                                        }
                                    }
                                }
                            }
                            else if(elem != null)
                            {
                                //at this point we can assume this is a list or some other type other than GameObject or ScriptableObject
                                //break so we don't waste time iterating through a long list of some other type
                                break;
                            }
                        }
                    }
                }
            }

            return numErrors;
        }

    }
}
