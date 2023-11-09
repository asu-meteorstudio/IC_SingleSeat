#if DSUTIL_ARTANIM_COMMON_IN_PROJECT

using Artanim;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace DreamscapeUtil
{

    [Description("Checks for various project settings as required by the artanim SDK")]
    public class ProjectSettingsCheck : ISceneCheckerGlobalProjectCheck
    {

        private static readonly string SCENE_TYPE = ".unity";
        private static readonly string SDK_MAIN_SCENE = "Scenes/Main Scene";
        private static readonly string[] REQUIRED_SDK_SCENES = new string[]
        {
            SDK_MAIN_SCENE,
            "Scenes/Emergency Scene/Emergency Scene",
            "Scenes/Main Menu/Main Menu Scene",
            "Scenes/Experience Controller/Experience Controller Scene",
            "Scenes/Observer/Experience Observer Scene",
            "Scenes/Construct/Construct Scene",
        };

        public int CheckForErrors()
        {
            int numErrors = 0;

            if(EditorUserBuildSettings.selectedBuildTargetGroup != BuildTargetGroup.Standalone ||
               EditorUserBuildSettings.selectedStandaloneTarget != BuildTarget.StandaloneWindows64)
            {
                Debug.LogWarning("Build settings must have Target Platform='Windows', Architecture='x86_64'");
                numErrors++;
            }

            /*var copyPDBValue = EditorUserBuildSettings.GetPlatformSettings(BuildTargetGroup.Standalone.ToString(), "CopyPDBFiles");
            bool copyPDBBool = false;
            bool.TryParse(copyPDBValue, out copyPDBBool);
            if (!copyPDBBool)
            {
                Debug.LogWarning("'Copy PDB files' should be enabled in Build Settings");
                numErrors++;
            }*/

#if UNITY_2018_1_OR_NEWER
            if(!PlayerSettings.allowUnsafeCode)
            {
                Debug.LogWarning("'Allow unsafe code' must be enabled in Player Settings");
                numErrors++;
            }

#endif

            if (!PlayerSettings.runInBackground)
            {
                Debug.LogWarning("'Run in Background' must be enabled in Player Settings");
                numErrors++;
            }

            if(PlayerSettings.displayResolutionDialog != ResolutionDialogSetting.Disabled)
            {
                Debug.LogWarning("'Display Resolution Dialog' should be disabled in Player Settings");
                numErrors++;
            }

            if (PlayerSettings.SplashScreen.show)
            {
                Debug.LogWarning("Splash screen should be disabled in Player Settings");
                numErrors++;
            }

#if !UNITY_2019_1_OR_NEWER
            if (!PlayerSettings.virtualRealitySupported)
            {
                Debug.LogWarning("'Virtual Reality Supported' must be enabled in the Player Settings");
                numErrors++;
            }
            else
            {
                string[] vrSdks = PlayerSettings.GetVirtualRealitySDKs(BuildTargetGroup.Standalone);
                if(vrSdks.Length < 1 || vrSdks[0] != "Oculus")
                {
                    Debug.LogWarning("Oculus must be first in list of Virtual Reality SDKs in Player Settings");
                    numErrors++;
                }
                if(vrSdks.Length < 2 || vrSdks[1] != "OpenVR")
                {
                    Debug.LogWarning("OpenVR must be second in list of Virtual Reality SDKs in Player Settings");
                    numErrors++;
                }
            }
#endif

            var mainFolder = EditorUtils.GetSDKAssetFolder();
            if (!string.IsNullOrEmpty(mainFolder))
            {
                //Check scenes
                var buildScenes = new List<EditorBuildSettingsScene>(EditorBuildSettings.scenes);
                foreach (var sdkScene in REQUIRED_SDK_SCENES)
                {
                    var buildScene = buildScenes.FirstOrDefault(s => s.path == string.Concat(mainFolder, sdkScene, SCENE_TYPE));
                    if (buildScene == null)
                    {
                        Debug.LogWarningFormat("SDK scene '{0}' is missing from the build settings", sdkScene);
                        numErrors++;
                    }
                    else if (!buildScene.enabled)
                    {
                        Debug.LogWarningFormat("SDK scene '{0}' is disabled in the build settings", sdkScene);
                        numErrors++;
                    }
                }

                //Check first scene
                if (buildScenes.Count > 0)
                {
                    if (buildScenes[0].path != string.Concat(mainFolder, SDK_MAIN_SCENE, SCENE_TYPE))
                    {
                        Debug.LogWarning("SDK main scene must be listed first in the build settings");
                        numErrors++;
                    }
                }
            }

            if(!File.Exists(Path.Combine(Application.streamingAssetsPath, ConfigService.EXPERIENCE_CONFIG_NAME)))
            {
                Debug.LogWarning("Missing SDK experience config in streaming assets folder");
                numErrors++;
            }

            if(ResourceUtils.LoadResources<ExperienceSettingsSO>(ExperienceSettingsSO.EXPERIENCE_SETTINGS_RESOURCE) == null)
            {
                Debug.LogWarning("Missing SDK experience settings in Resources");
                numErrors++;
            }

            return numErrors;
        }

    }
}

#endif