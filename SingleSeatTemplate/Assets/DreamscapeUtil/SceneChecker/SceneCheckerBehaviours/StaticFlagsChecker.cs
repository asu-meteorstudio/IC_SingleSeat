using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
namespace DreamscapeUtil
{

    //copy of built in StaticEditorFlags enum with duplicate entry commented out
    //(this was causing bugs in editor code to display bitmask field)
    public enum StaticEditorFlags_copy
    {
        //ContributeGI = 1,
        LightmapStatic = 1,
        OccluderStatic = 2,
        BatchingStatic = 4,
        NavigationStatic = 8,
        OccludeeStatic = 16,
        OffMeshLinkGeneration = 32,
        ReflectionProbeStatic = 64
    }

    /// <summary>
    /// Generates warnings about game objects with the wrong static flags
    /// </summary>
    public class StaticFlagsChecker : MonoBehaviour, ISceneCheckerBehaviour
    {
#if UNITY_EDITOR
        [BitMaskAttribute(typeof(StaticEditorFlags_copy))]
        public StaticEditorFlags staticFlags;
        public bool recursive = true;

        [EasyButtons.Button]
        public void ApplyStaticFlags()
        {
            if (recursive)
            {
                SceneCheckerIterator it = SceneCheckerIterator.IterHierarchy(gameObject);
                while (it.MoveNext())
                {
                    GameObject go = it.Current;
                    if (go != gameObject && go.GetComponent<StaticFlagsChecker>())
                    {
                        it.SkipChildren();
                    }
                    else if (GameObjectUtility.GetStaticEditorFlags(go) != staticFlags)
                    {
                        GameObjectUtility.SetStaticEditorFlags(go, staticFlags);
                        PrefabUtility.RecordPrefabInstancePropertyModifications(go);
                        EditorUtility.SetDirty(go);
                    }
                }
            }
            else
            {
                GameObjectUtility.SetStaticEditorFlags(gameObject, staticFlags);
            }
        }

        private bool FlagCompare(StaticEditorFlags flags1, StaticEditorFlags flags2)
        {
            return (((uint) flags1) & (128 - 1)) == (((uint) flags2) & (128 - 1));
        }

        public int CheckForErrors()
        {
            if (recursive)
            {
                //return CheckForErrors_recursive(this.transform);
                int numErrors = 0;
                SceneCheckerIterator it = SceneCheckerIterator.IterHierarchy(gameObject);
                while (it.MoveNext())
                {
                    GameObject go = it.Current;
                    if (go != gameObject && go.GetComponent<StaticFlagsChecker>())
                    {
                        it.SkipChildren();
                    }
                    //else if(GameObjectUtility.GetStaticEditorFlags(go) != staticFlags)
                    else if(!FlagCompare(GameObjectUtility.GetStaticEditorFlags(go), staticFlags))
                    {
                        numErrors++;
                        Debug.LogWarningFormat(go, "'{0}' has the wrong static flags", go.name);
                        it.SkipChildren();
                    }
                }

                return numErrors;
            }
            else
            {
                if (!FlagCompare(GameObjectUtility.GetStaticEditorFlags(gameObject), staticFlags))
                {
                    Debug.LogWarningFormat(gameObject, "'{0}' has the wrong static flags", gameObject.name);
                    return 1;
                }
                else
                {
                    return 0;
                }
            }
        }
#else
    public int CheckForErrors()
    {
        return 0;
    }

#endif

    }
}
