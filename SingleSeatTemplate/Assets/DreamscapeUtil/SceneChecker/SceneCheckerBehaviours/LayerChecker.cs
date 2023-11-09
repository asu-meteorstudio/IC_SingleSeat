using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace DreamscapeUtil
{
    /// <summary>
    /// Generates warnings about game objects on the wrong layer.
    /// 
    /// Can be overridden by another layer checker in descendant game object
    /// </summary>
    public class LayerChecker : MonoBehaviour, ISceneCheckerBehaviour
    {
        [Layer]
        public int layer;
        public bool recursive = true;

#if UNITY_EDITOR
        /// <summary>
        /// Sets the layer of all game objects checked by this component
        /// </summary>
        [EasyButtons.Button]
        public void ApplyLayer()
        {
            if (recursive)
            {
                SceneCheckerIterator it = SceneCheckerIterator.IterHierarchy(gameObject);
                while (it.MoveNext())
                {
                    GameObject go = it.Current;
                    if (go != gameObject && go.GetComponent<LayerChecker>())
                    {
                        it.SkipChildren();
                    }
                    else if (go.layer != layer)
                    {
                        go.layer = layer;
                        UnityEditor.PrefabUtility.RecordPrefabInstancePropertyModifications(go);
                        UnityEditor.EditorUtility.SetDirty(go);
                    }
                }
            }
            else
            {
                gameObject.layer = layer;
            }
        }
#endif

        public int CheckForErrors()
        {
            if (!recursive && gameObject.layer != layer)
            {
                Debug.LogWarningFormat(this, "'{0}' should be on layer '{1}'", gameObject.name, LayerMask.LayerToName(layer));
                return 1;
            }
            else if (recursive)
            {
                int numErrors = 0;

                SceneCheckerIterator it = SceneCheckerIterator.IterHierarchy(gameObject);
                while (it.MoveNext())
                {
                    GameObject go = it.Current;
                    //ignore parts of hierarchy under other layer checker scripts
                    if (go != gameObject && go.GetComponent<LayerChecker>())
                    {
                        it.SkipChildren();
                    }
                    else if(go.layer != layer)
                    {
                        Debug.LogWarningFormat(go, "'{0}' should be on layer '{1}'", go.name, LayerMask.LayerToName(layer));
                        numErrors++;
                        //skip children after finding an error to avoid spamming logs too much when an entire section of hiearchy is wrong
                        it.SkipChildren();
                    }
                }

                return numErrors;
            }
            else
            {
                return 0;
            }
        }
    }
}
