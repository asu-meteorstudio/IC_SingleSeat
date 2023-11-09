#if DSUTIL_ARTANIM_COMMON_IN_PROJECT

using Artanim;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace DreamscapeUtil
{
    [Description("Checks for multiple SceneSetup scripts and invalid additive scenes to load")]
    public class SceneSetupCheck : ISceneCheckerGlobalSceneCheck
    {
        public int CheckForErrors(SceneCheckerContext context)
        {
            int numErrors = 0;

            bool foundSceneSetup = false;

            SceneCheckerIterator it = SceneCheckerIterator.IterContext(context);



            while (it.MoveNext())
            {
                GameObject go = it.Current;

                if (go.TryGetComponent<SceneSetup>(out SceneSetup setup))
                {
                    if (foundSceneSetup)
                    {
                        numErrors++;
                        Debug.LogWarning("More than one SceneSetup script", setup);
                    }
                    if (setup.AdditiveScenesToLoad != null)
                    {
                        foreach (string sceneName in setup.AdditiveScenesToLoad)
                        {
                            if (!IsSceneInBuildSettings(sceneName))
                            {
                                numErrors++;
                                Debug.LogWarningFormat(setup, "Scene '{0}' is not in the build settings", sceneName);
                            }
                        }
                    }


                    foundSceneSetup = true;
                }

            }

            return numErrors;
        }

        private static bool IsSceneInBuildSettings(string sceneName)
        {
            foreach(EditorBuildSettingsScene scene in EditorBuildSettings.scenes)
            {
                if(scene.path == sceneName)
                {
                    return true;
                }
                string path = scene.path;
                //strip out "Assets/" at beginning of path and .unity at end to get path as it appears in build settings window
                path = path.Split(new char[] { '/', '\\' }, 2)[1];
                path = path.Substring(0, path.LastIndexOf('.'));
                if (path == sceneName)
                {
                    return true;
                }

                path = path.Substring(path.LastIndexOf('/') + 1);
                if(path == sceneName)
                {
                    return true;
                }
            }
            return false;
        }
    }
}

#endif
