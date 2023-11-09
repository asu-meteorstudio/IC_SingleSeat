using UnityEngine;
using UnityEditor;

/// <summary>
/// Used to copy/paste the transform values of object and all it's descendants - e.g. to match a frame of animation
/// </summary>
static class CopyTransformHierarchy
{
    private static Vector3[] positions;
    private static Quaternion[] rotations;
    private static Vector3[] scales;


    [MenuItem("Dreamscape/Copy Transform Hierarchy")]
    [MenuItem("GameObject/Dreamscape/Copy Transform Hierarchy", false, 0)]
    static void CopyHierarchy()
    {
        Transform t = Selection.activeTransform;

        if (t)
        {
            Transform[] xforms = t.GetComponentsInChildren<Transform>(true);
            positions = new Vector3[xforms.Length];
            rotations = new Quaternion[xforms.Length];
            scales = new Vector3[xforms.Length];

            for(int i = 0; i < xforms.Length; i++)
            {
                positions[i] = xforms[i].localPosition;
                rotations[i] = xforms[i].localRotation;
                scales[i] = xforms[i].localScale;
            }
        }
        else
        {
            EditorUtility.DisplayDialog("Copy Transform Hierarchy", "No Transform selected", "Ok");
        }
    }

    [MenuItem("Dreamscape/Paste Transform Hierarchy")]
    [MenuItem("GameObject/Dreamscape/Paste Transform Hierarchy", false, 0)]
    static void PasteHierarchy()
    {
        Transform t = Selection.activeTransform;
        if (positions != null)
        {
            if (t)
            {
                Transform[] xforms = t.GetComponentsInChildren<Transform>(true);
                if (xforms.Length == positions.Length)
                {
                    Undo.RecordObjects(xforms, "Paste Transform Hierarchy");
                    for (int i = 0; i < xforms.Length; i++)
                    {
                        if (xforms[i] != t)
                        {
                            xforms[i].localPosition = positions[i];
                            xforms[i].localRotation = rotations[i];
                            xforms[i].localScale = scales[i];
                            PrefabUtility.RecordPrefabInstancePropertyModifications(xforms[i]);
                        }
                    }
                }
                else
                {
                    EditorUtility.DisplayDialog("Paste Transform Hierarchy",
                        "Cannot paste transform hierarchy because the number of child transforms does not match the copied object",
                        "Ok");
                }
            }
            else
            {
                EditorUtility.DisplayDialog("Paste Transform Hierarchy", "No Transform selected", "Ok");
            }
        }
        else
        {
            EditorUtility.DisplayDialog("Paste Transform Hierarchy", "Nothing to paste", "Ok");
        }
    }
}
