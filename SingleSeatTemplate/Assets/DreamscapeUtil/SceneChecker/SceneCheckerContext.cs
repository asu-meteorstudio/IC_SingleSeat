using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
#if UNITY_EDITOR
using UnityEditor.SceneManagement;
using UnityEditor.Experimental.SceneManagement;
#endif

namespace DreamscapeUtil
{
    /// <summary>
    /// Represents a unit to be checked for errors. Either a scene or a prefab root
    /// </summary>
    public class SceneCheckerContext
    {
        private Scene scene;
        private GameObject prefabRoot;

        private SceneCheckerContext()
        {

        }
#if UNITY_EDITOR
        public static SceneCheckerContext FromCurrentPrefabStage()
        {
            PrefabStage stage = PrefabStageUtility.GetCurrentPrefabStage();
            if(stage != null)
            {
                SceneCheckerContext c = new SceneCheckerContext();
                c.prefabRoot = stage.prefabContentsRoot;

                return c;
            }
            else
            {
                return null;
            }

        }

        public static SceneCheckerContext FromScene(Scene s)
        {
            SceneCheckerContext c = new SceneCheckerContext();
            c.scene = s;

            return c;
        }

#endif

        public string Name
        {
            get
            {
                if(scene.IsValid())
                {
                    return scene.name;
                }
                else if(prefabRoot != null)
                {
                    return prefabRoot.name;
                }
                else
                {
                    return "<Invalid Context>";
                }
            }
        }

        /// <summary>
        /// The scene that this context represents(null if this is a prefab editing context)
        /// </summary>
        public Scene Scene
        {
            get
            {
                return scene;
            }
        }

        /// <summary>
        /// The root gameobject for a prefab editing context(null if this is not a prefab editing context)
        /// </summary>
        public GameObject PrefabRoot
        {
            get
            {
                return prefabRoot;
            }
        }


    }
}
