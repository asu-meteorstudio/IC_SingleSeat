using DreamscapeUtil;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Controls lerping of hand poses via SimpleHandPoseAnimator and IK via HandGrabIK(optional) based on interactions with triggers
/// </summary>
public class HandPoseController : MonoBehaviour
{
    public enum EHand
    {
        Left,
        Right
    }

    [NonNull]
    public SimpleHandPoseAnimator handAnimator;

    public HandGrabIK handGrabIK;
    public Transform middleFinger;

    public EHand hand;

    //list of triggers that the hand is currently overlapping
    private List<IHandPoseTrigger> activeTriggers = new List<IHandPoseTrigger>(10);
    private string currentPose = "default";

    private float defaultTimer = 0.0f;

    // Update is called once per frame
    void LateUpdate()
    {
        string targetPose = "default";
        int highestPriority = int.MinValue;
        Transform grabTarget = null;

        //remove inactive/deleted triggers
        for(int i = 0; i < activeTriggers.Count; i++)
        {
            MonoBehaviour b = activeTriggers[i] as MonoBehaviour;
            if(b == null || !b.isActiveAndEnabled)
            {
                activeTriggers.RemoveAt(i);
                i--;
            }
            /*else
            {
                
                if(b && !b.isActiveAndEnabled)
                {
                    activeTriggers.RemoveAt(i);
                    i--;
                }
            }*/
        }

        //find the active trigger with the highest priority
        foreach(IHandPoseTrigger trigger in activeTriggers)
        {
            Transform handTarget, middleFingerTarget;
            trigger.GetGrabTarget(this.hand, out handTarget, out middleFingerTarget);

            //if there is a grab target, make sure we are also within the tolerances for that target
            if(handTarget && middleFingerTarget &&
               (trigger.TargetRadius < Vector3.Distance(middleFingerTarget.position, middleFinger.position) ||
                Quaternion.Angle(handTarget.rotation, transform.rotation) > trigger.AngleTolerance))
            {
                continue;
            }
            if(trigger.Priority >= highestPriority)
            {
                highestPriority = trigger.Priority;
                targetPose = trigger.TargetPose;
                grabTarget = handTarget;
            }
        }

        if(targetPose != "default")
        {
            defaultTimer = 0.0f;
        }

        if (targetPose != currentPose)
        {
            //TODO need less kludgy solution for this with fewer special cases
            if (targetPose != "default" || currentPose == "joystick" || currentPose == "grip_pointer")
            {
                handAnimator.LerpToPose(targetPose);
                currentPose = targetPose;
            }
            else
            {
                //only switch back to default pose after a small delay
                //this prevents rapidly switching back and forth when on the boundary
                //of a trigger
                defaultTimer += Time.deltaTime;
                if(defaultTimer > 0.5f)
                {
                    handAnimator.LerpToPose(targetPose);
                    currentPose = targetPose;
                }
            }
        }

        if (handGrabIK)
        {
            handGrabIK.SetHandTarget(grabTarget);
        }

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<IHandPoseTrigger>(out IHandPoseTrigger trigger))
        {
            MonoBehaviour b = trigger as MonoBehaviour;
            if (b && b.isActiveAndEnabled)
            {
                if (handAnimator.HasPose(trigger.TargetPose))
                {
                    activeTriggers.Add(trigger);
                }
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.TryGetComponent<IHandPoseTrigger>(out IHandPoseTrigger trigger))
        {
            activeTriggers.Remove(trigger);
        }
    }
}
