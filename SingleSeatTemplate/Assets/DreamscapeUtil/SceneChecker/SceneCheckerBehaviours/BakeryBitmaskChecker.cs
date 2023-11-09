#if SCENE_CHECKER_BAKERY_EXTENSIONS

using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace DreamscapeUtil
{
#if UNITY_EDITOR
    [CustomEditor(typeof(BakeryBitmaskChecker))]
    [CanEditMultipleObjects]
    public class BakerBitmaskCheckerInspector : UnityEditor.Editor
    {
        static string[] selStrings = new string[] {"0","1","2","3","4","5","6","7","8","9","10","11","12","13","14","15","16",
                                                "17","18","19","20","21","22","23","24","25","26","27","28","29","30"};

        public override void OnInspectorGUI()
        {
            BakeryBitmaskChecker c = targets[0] as BakeryBitmaskChecker;

            EditorGUI.showMixedValue = false;
            for(int i = 1; i < targets.Length; i++)
            {
                if((targets[i] as BakeryBitmaskChecker).bitmask != c.bitmask)
                {
                    EditorGUI.showMixedValue = true;
                }
            }

            EditorGUI.BeginChangeCheck();
            int newVal = EditorGUILayout.MaskField("Bitmask", c.bitmask, selStrings);
            if (EditorGUI.EndChangeCheck())
            {
                for (int i = 0; i < targets.Length; i++)
                {
                    (targets[i] as BakeryBitmaskChecker).bitmask = newVal;
                    PrefabUtility.RecordPrefabInstancePropertyModifications(targets[i]);
                    EditorUtility.SetDirty(targets[i]);
                }
            }

            EditorGUI.showMixedValue = false;

            EditorGUILayout.PropertyField(serializedObject.FindProperty("recursive"));
        }
    }
#endif


    public class BakeryBitmaskChecker : MonoBehaviour, ISceneCheckerBehaviour
    {
        public int bitmask;
        public bool recursive = true;

        public int CheckForErrors()
        {
            BakeryPointLight point = GetComponent<BakeryPointLight>();
            if(point && point.bitmask != bitmask)
            {
                Debug.LogWarningFormat(this, "Bakery light has the wrong bitmask - {0}", this.name);
                return 1;
            }
            BakeryDirectLight direct = GetComponent<BakeryDirectLight>();
            if(direct && direct.bitmask != bitmask)
            {
                Debug.LogWarningFormat(this, "Bakery light has the wrong bitmask - {0}", this.name);
                return 1;
            }
            BakerySkyLight sky = GetComponent<BakerySkyLight>();
            if (sky && sky.bitmask != bitmask)
            {
                Debug.LogWarningFormat(this, "Bakery light has the wrong bitmask - {0}", this.name);
                return 1;
            }

            if (recursive)
            {
                int numErrors = 0;

                SceneCheckerIterator it = SceneCheckerIterator.IterHierarchy(gameObject);
                while (it.MoveNext())
                {
                    GameObject go = it.Current;
                    if (go != gameObject && go.GetComponent<BakeryBitmaskChecker>())
                    {
                        it.SkipChildren();
                    }
                    else
                    {
                        BakeryPointLight bpl = go.GetComponent<BakeryPointLight>();
                        if (bpl && bpl.bitmask != bitmask)
                        {
                            Debug.LogWarningFormat(go, "Bakery light has the wrong bitmask - {0}", go.name);
                            numErrors++;
                        }
                        BakeryDirectLight bdl = go.GetComponent<BakeryDirectLight>();
                        if (bdl && bdl.bitmask != bitmask)
                        {
                            Debug.LogWarningFormat(go, "Bakery light has the wrong bitmask - {0}", go.name);
                            numErrors++;
                        }
                        BakeryDirectLight bal = go.GetComponent<BakeryDirectLight>();
                        if (bal && bal.bitmask != bitmask)
                        {
                            Debug.LogWarningFormat(go, "Bakery light has the wrong bitmask - {0}", go.name);
                            numErrors++;
                        }
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

#endif
