using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class ReplaceWithPrefabWindow : EditorWindow
{
    static GameObject prefab;
    static bool useTransformScale = false;

    [MenuItem("Dreamscape/Replace Selection With Prefab Instances...")]
    static void Init()
    {
        ReplaceWithPrefabWindow window = (ReplaceWithPrefabWindow)GetWindow<ReplaceWithPrefabWindow>("Replace Selection With Prefab");

        window.Show();
    }

    private void OnGUI()
    {
        EditorGUILayout.Space();
        EditorGUILayout.SelectableLabel("Select a prefab below, then select objects in the scene and press Replace. All selected objects will be replaced" +
            " with an instance of the selected prefab", EditorStyles.wordWrappedLabel);

        EditorGUILayout.Space();

        EditorGUILayout.BeginVertical(GUILayout.ExpandHeight(false));
        prefab = (GameObject)EditorGUILayout.ObjectField("Prefab", prefab, typeof(GameObject), false);
        GUIContent c = new GUIContent("Use Transform Scale", "If checked, maintain local scale from transforms being replaced. Otherwise, use the scale from the prefab");
        useTransformScale = EditorGUILayout.ToggleLeft(c, useTransformScale);
        EditorGUILayout.Space();

        if(prefab == null || UnityEditor.Selection.gameObjects.Length == 0)
        {
            GUI.enabled = false;
        }
        else
        {
            if (!CanReplace())
            {
                EditorGUILayout.HelpBox("One or more objects in the selection is a descendant of another object in the selection", MessageType.Info);
                GUI.enabled = false;
            }
        }
        EditorGUILayout.EndVertical();
        if (GUILayout.Button("Replace"))
        {
            ReplaceSelectionWithPrefab();
        }

        GUI.enabled = true;
    }

    private bool CanReplace()
    {
        HashSet<GameObject> selection = new HashSet<GameObject>(UnityEditor.Selection.gameObjects);

        //confirm that not objects in the selection are descendants of another object in the selection
        foreach (GameObject go in selection)
        {
            Transform t = go.transform.parent;
            while (t != null)
            {
                if (selection.Contains(t.gameObject))
                {
                    return false;
                }
                t = t.parent;
            }
        }

        return true;
    }

    private void ReplaceSelectionWithPrefab()
    {

        int i = 0;
        foreach(GameObject go in UnityEditor.Selection.gameObjects)
        {
#if UNITY_2018_1_OR_NEWER
            GameObject newObj = PrefabUtility.InstantiatePrefab(prefab, go.transform.parent) as GameObject;
#else
            GameObject newObj = PrefabUtility.InstantiatePrefab(prefab, go.scene) as GameObject;
            newObj.transform.parent = go.transform.parent;
#endif
            newObj.transform.localPosition = go.transform.localPosition;
            newObj.transform.localRotation = go.transform.localRotation;
            if (useTransformScale)
            {
                newObj.transform.localScale = go.transform.localScale;
            }
            if(i > 0)
            {
                newObj.name += string.Format(" ({0})", i);
            }

            Undo.RegisterCreatedObjectUndo(newObj, "Replace Selection with Prefab");

            Undo.DestroyObjectImmediate(go);

            i++;
        }
    }
}
