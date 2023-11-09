using DreamscapeUtil;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>
/// Animates hand by interpolating between predefined poses.
/// Should be attached to the hand bone of an avatar
/// 
/// Note: Poses are stored as a simple flat list of transform data for each transform in the hierarchy, so any
/// changes to the hierarchy will invalidate previously saved pose data.
/// </summary>
public class SimpleHandPoseAnimator : MonoBehaviour
{
    [System.Serializable]
    public struct NamedPose{
        public string name;
        public HandPose pose;
    }

    public NamedPose[] poses;

    //list of transforms to be controlled by this script
    private Transform[] transforms;

    //time to lerp from one pose to another
    public float lerpDuration = 0.25f;

    //when in transition, this is the pose we are transitioning from
    private HandPose prevHandPose;
    //when in transition, this is the pose we are transitioning to
    private HandPose currHandPose;
    private HandPose nextHandPose = null;

    //lerp progress
    private float t = 1.0f;

    void Awake()
    {
        if(poses.Length == 0)
        {
            Debug.LogWarning("No Poses specified in SimpleHandAnimator. Disabling");
            this.enabled = false;
            return;
        }


        prevHandPose = poses[0].pose;
        currHandPose = poses[0].pose;

        transforms = GetComponentsInChildren<Transform>();

    }

    void LateUpdate()
    {
        t = Mathf.Min(t + Time.deltaTime / lerpDuration, 1.0f);
        if(t == 1.0f)
        {
            prevHandPose = currHandPose;
            if (nextHandPose != null)
            {
                currHandPose = nextHandPose;
                nextHandPose = null;
                t = 0.0f;
            }
        }

        if (t < 1.0f)
        {
            for (int i = 0; i < transforms.Length; i++)
            {
                Transform xform = transforms[i];
                if (xform != transform)
                {
                    xform.localPosition = Vector3.Lerp(prevHandPose.positions[i], currHandPose.positions[i], t);
                    xform.localRotation = Quaternion.Slerp(prevHandPose.rotations[i], currHandPose.rotations[i], t);
                }
            }
        }
        else
        {
            for (int i = 0; i < transforms.Length; i++)
            {
                Transform xform = transforms[i];
                if (xform != transform)
                {
                    xform.localPosition = currHandPose.positions[i];
                    xform.localRotation = currHandPose.rotations[i];
                }
            }
        }
    }

    public void LerpToPose(HandPose pose)
    {
        if(pose.positions.Length != transforms.Length || pose.rotations.Length != transforms.Length)
        {
            Debug.LogError("Hand pose being lerped to does not contain the correct number of transforms for this hand");
            return;
        }

        if(t < 1.0f)
        {
            nextHandPose = pose;
        }
        else
        {
            currHandPose = pose;
            t = 0.0f;
        }
    }

    public bool HasPose(string poseName)
    {
        for (int i = 0; i < poses.Length; i++)
        {
            if (poses[i].name == poseName)
            {
                return true;
            }
        }

        return false;
    }

    public void LerpToPose(string poseName)
    {
        for(int i = 0; i < poses.Length; i++)
        {
            if(poses[i].name == poseName)
            {
                LerpToPose(poses[i].pose);
                return;
            }
        }
        Debug.LogErrorFormat("No pose with name '{0}'", poseName);
    }

#if UNITY_EDITOR
    [EasyButtons.Button(EasyButtons.ButtonMode.DisabledInPlayMode)]
    private void SaveCurrentPose()
    {
        string path = EditorUtility.SaveFilePanelInProject("Save Pose", "NewHandPose", "asset", "");
        if(path.Length != 0)
        {
            HandPose pose = HandPose.CreateInstance<HandPose>();

            Transform[] xforms = GetComponentsInChildren<Transform>();

            pose.positions = new Vector3[xforms.Length];
            pose.rotations = new Quaternion[xforms.Length];

            for (int i = 0; i < xforms.Length; i++)
            {
                pose.positions[i] = xforms[i].localPosition;
                pose.rotations[i] = xforms[i].localRotation;
            }

            AssetDatabase.CreateAsset(pose, path);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            EditorUtility.FocusProjectWindow();
            Selection.activeObject = pose;
        }
    }

    [EasyButtons.Button(EasyButtons.ButtonMode.DisabledInPlayMode)]
    private void LoadPose()
    {
        string path = EditorUtility.OpenFilePanel("Load Pose", Application.dataPath, "asset");
        if (path.Length != 0 && path.StartsWith(Application.dataPath)) ;
        {
            string relPath = "Assets" + path.Substring(Application.dataPath.Length);

            HandPose pose = AssetDatabase.LoadAssetAtPath<HandPose>(relPath);

            if (pose)
            {
                Transform[] xforms = GetComponentsInChildren<Transform>();
                if(xforms.Length != pose.positions.Length)
                {
                    Debug.LogError("Wrong number of transforms for this pose");
                    return;
                }
                for (int i = 0; i < xforms.Length; i++)
                {
                    Transform xform = xforms[i];
                    if (xform != transform)
                    {
                        xform.localPosition = pose.positions[i];
                        xform.localRotation = pose.rotations[i];
                    }
                }
            }
        }
    }
#endif
}
