using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

/// <summary>
/// Zeros out the transforms of selected transform while keeping child transforms in place in world space
/// </summary>
static class ZeroParentTransform
{
    [MenuItem("Dreamscape/Zero Parent Transform")]
    [MenuItem("CONTEXT/Transform/Zero Parent Transform")]
    [MenuItem("GameObject/Dreamscape/Zero Parent Transform", false, 0)]
    static void ZeroTransforms()
    {
        if (!Selection.activeTransform)
        {
            return;
        }

        Undo.RecordObject(Selection.activeTransform, "Zero Parent Transform");
        List<Transform> children = new List<Transform>();
        List<Vector3> positions = new List<Vector3>();
        List<Quaternion> rotations = new List<Quaternion>();
        for(int i = 0; i < Selection.activeTransform.childCount; i++)
        {
            Transform t = Selection.activeTransform.GetChild(i);
            children.Add(t);
            positions.Add(t.position);
            rotations.Add(t.rotation);
            Undo.RecordObject(Selection.activeTransform.GetChild(i), "Zero Parent Transform");
        }

        Selection.activeTransform.localPosition = Vector3.zero;
        Selection.activeTransform.localRotation = Quaternion.identity;
        Vector3 parentScale = Selection.activeTransform.localScale;
        Selection.activeTransform.localScale = Vector3.one;
        PrefabUtility.RecordPrefabInstancePropertyModifications(Selection.activeTransform);
        for (int i = 0; i < children.Count; i++)
        {
            Transform t = children[i];
            t.position = positions[i];
            t.rotation = rotations[i];
            Vector3 s = /*t.localRotation * */parentScale;
            t.localScale = new Vector3(t.localScale.x * s.x, t.localScale.y * s.y, t.localScale.z * s.z);
            PrefabUtility.RecordPrefabInstancePropertyModifications(t);
        }
    }
}
