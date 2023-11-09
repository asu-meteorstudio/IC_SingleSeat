using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

/// <summary>
/// Zeros out the transforms of selected transform while modifying it's parent transform to keep this object in the same
/// place in world space
/// </summary>
static class ZeroChildTransform
{
    [MenuItem("Dreamscape/Zero Child Transform")]
    [MenuItem("CONTEXT/Transform/Zero Child Transform")]
    [MenuItem("GameObject/Dreamscape/Zero Child Transform", false, 0)]
    static void ZeroTransform()
    {
        if (!Selection.activeTransform)
        {
            return;
        }
        if (!Selection.activeTransform.parent)
        {
            return;
        }

        Transform t = Selection.activeTransform;
        Transform p = Selection.activeTransform.parent;



        Undo.RecordObject(Selection.activeTransform, "Zero Child Transform");
        Undo.RecordObject(Selection.activeTransform.parent, "Zero Child Transform");

        Vector3 pos = t.position;

        p.localScale = new Vector3(p.localScale.x * t.localScale.x, p.localScale.y * t.localScale.y, p.localScale.z * t.localScale.z);
        p.localRotation *= t.localRotation;
        p.position = pos;

        t.localPosition = Vector3.zero;
        t.localRotation = Quaternion.identity;
        t.localScale = Vector3.one;
        PrefabUtility.RecordPrefabInstancePropertyModifications(Selection.activeTransform.parent);
        PrefabUtility.RecordPrefabInstancePropertyModifications(Selection.activeTransform);
    }
}
