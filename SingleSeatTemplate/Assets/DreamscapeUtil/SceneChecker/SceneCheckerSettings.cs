using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace DreamscapeUtil
{

    public class SceneCheckerSettings : ScriptableObject
    {
        public const string SETTINGS_PATH = "Assets/SceneCheckerSettings.asset";
        [HideInInspector]
        public bool checkInactiveObjects = true;
        [HideInInspector]
        public bool checkDisabledBehaviours = true;
        [HideInInspector]
        public bool checkEditorOnlyObjects = true;
        [HideInInspector]
        public bool ignoreAllObjectsWithDestroyInSession = true;
        [HideInInspector]
        public List<string> disabledSceneChecks = new List<string>();
        [HideInInspector]
        public List<string> disabledProjectChecks = new List<string>();



        [HideInInspector]
        public List<string> additionalScenes = new List<string>();
        [HideInInspector]
        public List<string> ignoreScenes = new List<string>();

        private static SceneCheckerSettings _instance = null;

        public static SceneCheckerSettings Instance
        {
            get
            {
#if UNITY_EDITOR
                if (_instance == null)
                {
                    _instance = AssetDatabase.LoadAssetAtPath<SceneCheckerSettings>(SETTINGS_PATH);
                    if (_instance == null)
                    {
                        _instance = ScriptableObject.CreateInstance<SceneCheckerSettings>();
                        _instance.ignoreScenes.Add("Assets/ArtanimCommon/");
                        AssetDatabase.CreateAsset(_instance, SETTINGS_PATH);
                        AssetDatabase.SaveAssets();
                    }
                }
                return _instance;
#else
            return null;
#endif
            }
        }


    }
}
