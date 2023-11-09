using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IHandPoseTrigger
{
    string TargetPose
    {
        get;
    }
    int Priority
    {
        get;
    }

    void GetGrabTarget(HandPoseController.EHand hand, out Transform handTarget, out Transform middleFingerTarget);

    float TargetRadius
    {
        get;
    }

    float AngleTolerance
    {
        get;
    }
}

/// <summary>
/// Trigger used by HandPoseController to cause hand to lerp to a specified pose when intersecting the trigger
/// 
/// Optionally, the trigger can also specify a transform to snap the hand to, e.g. for making it grab onto an object
/// When using a grab target, you must also specify the position where the first joint on the middle finger should be when
/// grabbing the target. In this case, the hand will only respond when the actual middle finger is close enough to the 
/// target position instead of whenever it is inside the trigger box 
/// </summary>
public class HandPoseTrigger : MonoBehaviour, IHandPoseTrigger
{
    //name of pose to transition to
    public string targetPose;
    //if a hand is intersecting multiple triggers, the one with the higher priority takes precedence
    public int priority = 0;
    //optional - specifies a transform to snap the hand bone to via HandGrabIK
    public Transform grabTargetRight;
    //when using a grab target - this transform specifies the position of the first joint on the middle finger when grabbing the object
    //it is used as the comparison point for determining if the hand is close enough to snap to the target
    public Transform middleFingerTargetRight;
    public Transform grabTargetLeft;
    //when using a grab target - this transform specifies the position of the first joint on the middle finger when grabbing the object
    //it is used as the comparison point for determining if the hand is close enough to snap to the target
    public Transform middleFingerTargetLeft;
    //maximum distance of the actual middle finger bone to the middle finger target before hand snaps to target - only used when a grab target is specified
    public float targetRadius = .05f;
    //maximum difference between hand rotation and rotation of grab target
    public float angleTolerance = 60.0f;

    public string TargetPose => targetPose;
    public int Priority => priority;
    public float TargetRadius => targetRadius;
    public float AngleTolerance => angleTolerance;

    public void GetGrabTarget(HandPoseController.EHand hand, out Transform handTarget, out Transform middleFingerTarget)
    {
        if(hand == HandPoseController.EHand.Left)
        {
            handTarget = grabTargetLeft;
            middleFingerTarget = middleFingerTargetLeft;
        }
        else
        {
            handTarget = grabTargetRight;
            middleFingerTarget = middleFingerTargetRight;
        }
    }
}
