using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Stores transform data for the hand hierarchy - for use with SimpleHandPoseAnimator
/// </summary>
public class HandPose : ScriptableObject
{
    public Vector3[] positions;
    public Quaternion[] rotations;
}
