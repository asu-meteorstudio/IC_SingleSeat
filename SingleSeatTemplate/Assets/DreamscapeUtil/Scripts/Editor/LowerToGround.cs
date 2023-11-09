using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

public class AlignToGroundWindow : EditorWindow
{
    private enum RotationMode
    {
        DontChange,
        AlignToGroundNormal,
        AlignToGroundNormalAndRandomize
    }

    static RotationMode rotationMode = RotationMode.DontChange;
    static int layerMask;
    static float raycastHeight = 100.0f;
    static float groundOffset = 0.0f;

    [MenuItem("Dreamscape/Align Selection With Ground...")]
    static void Init()
    {
        AlignToGroundWindow window = GetWindow<AlignToGroundWindow>("Align Selection to Ground");

        layerMask = LayerMask.GetMask("Ground");

        layerMask = EditorPrefs.GetInt("Dreamscape/AlignToGround/LayerMask", layerMask);
        raycastHeight = EditorPrefs.GetFloat("Dreamscape/AlignToGround/RaycastHeight", raycastHeight);
        groundOffset = EditorPrefs.GetFloat("Dreamscape/AlignToGround/GroundOffset", groundOffset);
        rotationMode = (RotationMode) EditorPrefs.GetInt("Dreamscape/AlignToGround/RotationMode", (int)rotationMode);


        window.Show();
    }

    private void OnGUI()
    {

        EditorGUILayout.BeginVertical(GUILayout.ExpandHeight(false));

        EditorGUILayout.Space();
        rotationMode= (RotationMode) EditorGUILayout.EnumPopup("Rotation Mode", rotationMode);

        LayerMask tempMask = EditorGUILayout.MaskField("Ground Layer Mask", InternalEditorUtility.LayerMaskToConcatenatedLayersMask(layerMask), InternalEditorUtility.layers);
        layerMask = InternalEditorUtility.ConcatenatedLayersMaskToLayerMask(tempMask);

        raycastHeight = EditorGUILayout.FloatField("Raycast Height", raycastHeight);
        groundOffset = EditorGUILayout.FloatField("Ground Offset", groundOffset);

        EditorGUILayout.EndVertical();

        EditorGUILayout.Space();
        if (GUILayout.Button("Align To Ground"))
        {
            AlignSelectionWithGround();

            EditorPrefs.SetInt("Dreamscape/AlignToGround/LayerMask", layerMask);
            EditorPrefs.SetFloat("Dreamscape/AlignToGround/RaycastHeight", raycastHeight);
            EditorPrefs.SetFloat("Dreamscape/AlignToGround/GroundOffset", groundOffset);
            EditorPrefs.SetInt("Dreamscape/AlignToGround/RotationMode", (int)rotationMode);
        }

    }

    private void AlignSelectionWithGround()
    {
        foreach (GameObject go in UnityEditor.Selection.gameObjects)
        {
            Transform t = go.transform;

            Undo.RecordObject(t, "Align Selection with Ground");

            if(Physics.Raycast(t.position + Vector3.up * raycastHeight, Vector3.down, out RaycastHit hitInfo,  raycastHeight * 2, layerMask, QueryTriggerInteraction.Ignore))
            {
                t.position = hitInfo.point + Vector3.up * groundOffset;

                if (rotationMode == RotationMode.AlignToGroundNormal)
                {
                    t.rotation *= Quaternion.FromToRotation(Vector3.up, t.InverseTransformDirection(hitInfo.normal));
                }
                else if(rotationMode == RotationMode.AlignToGroundNormalAndRandomize)
                {
                    t.rotation *= Quaternion.FromToRotation(Vector3.up, t.InverseTransformDirection(hitInfo.normal));
                    t.rotation *= Quaternion.AngleAxis(Random.Range(-180.0f, 180.0f), Vector3.up);
                }
            }

            PrefabUtility.RecordPrefabInstancePropertyModifications(t);
        }
    }
}
