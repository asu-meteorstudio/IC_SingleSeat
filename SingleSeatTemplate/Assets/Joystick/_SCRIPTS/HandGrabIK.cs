using Artanim;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Adjusts Arm IK so that hand positon/rotations lines up with target
/// 
/// Assumes avatar uses the Character Creator AR rig
/// 
/// Assumes that animation data from IK server has already been applied before the
/// LateUpdate from this script is run. You may need to change the script execution order
/// to make sure this script runs after all of the SDK code to work properly.
/// Using this on an avatar that is not being updated with data from the IK server or some
/// other source of animation data may produce unpredictable results.
/// </summary>
public class HandGrabIK : MonoBehaviour
{
    public Transform hand;
    public Transform foreArm;
    public Transform arm;
    public Transform foreArmTwist;
    //controls how quickly the adjustment gets applied when setting a target
    public float lerpSpeed = 4.0f;

    //the current transform hand is locked to
    private Transform currentTarget = null;
    //used when easing weight down to zero when moving away from target
    private Transform lastTarget = null;
    private float weight = 0.0f;

    private float upperArmLength;
    private float lowerArmLength;

    private const int maxIterations = 50;
    private const float epsilon = .001f;

    private AvatarController ac;
    private float prevTime = -1.0f;

    private void Start()
    {
        upperArmLength = Vector3.Distance(arm.position, foreArm.position);
        lowerArmLength = Vector3.Distance(foreArm.position, hand.position);

        ac = GetComponentInParent<AvatarController>();
    }

    public void SetHandTarget(Transform target)
    {
        
        if (target != null)
        {
            //can only start to lerp to a target if weight is zero or we are in the middle of lerping weight down from
            //the same target(cannot lerp from one target to another for now)
            if (weight == 0.0f || target == lastTarget)
            {
                lastTarget = target;
                currentTarget = target;
            }
        }
        else
        {
            currentTarget = null;
        }
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (!ac)
        {
            return;
        }

        float deltaTime;
        if(prevTime < 0)
        {
            deltaTime = Time.deltaTime;
        }
        else
        {
            deltaTime = Time.time - prevTime;
        }
        prevTime = Time.time;


        upperArmLength = Vector3.Distance(arm.position, foreArm.position);
        lowerArmLength = Vector3.Distance(foreArm.position, hand.position);

        if (currentTarget && Vector3.Distance(lastTarget.position, arm.position) < (upperArmLength + lowerArmLength))
        {
            weight = Mathf.Min(1.0f, weight + Time.deltaTime * lerpSpeed);
        }
        else
        {
            weight = Mathf.Max(0.0f, weight - Time.deltaTime * lerpSpeed);
        }


        if (lastTarget)
        {
            ComputeBonePositions(out Vector3 foreArmPos, out Vector3 handPos);

            //Debug.DrawLine(arm.position, foreArmPos, Color.red);
            //Debug.DrawLine(foreArmPos, handPos, Color.blue);

            Quaternion origArmRotation = arm.localRotation;
            Quaternion origForearmRotation = foreArm.localRotation;
            Quaternion origHandRotation = hand.localRotation;
            Quaternion origForearmTwist = foreArmTwist.localRotation;

            //rotate upper arm so that elbow ends up in correct place
            arm.rotation = Quaternion.FromToRotation(Vector3.up, (foreArmPos - arm.position).normalized);


            //rotate the upper arm about it's axis so that the elbow rotation in the next step is along the correct axis
            Vector3 v = Vector3.ProjectOnPlane((handPos - foreArmPos), arm.up);
            float theta = Vector3.SignedAngle(arm.forward, v, arm.up);
            arm.Rotate(Vector3.up, theta, Space.Self);

            //apply elbow rotation to get hand in correct place
            foreArm.rotation = Quaternion.FromToRotation(Vector3.up, (handPos - foreArmPos).normalized);

            //rotate forearm about it's y-axis so that it's x-axis stays aligned with upper arm's x-axis(prevents strange rotation about y-axis)
            Vector3 u = Vector3.ProjectOnPlane(arm.right, foreArm.up);
            float theta2 = Vector3.SignedAngle(foreArm.right, u, foreArm.up);
            foreArm.Rotate(Vector3.up, theta2, Space.Self);
            
            //rotate hand to target orientation
            hand.rotation = lastTarget.rotation;

            //rotate forearm twist about it's y-axis so that it's rotation is about halfway betwen rotation of forearm and hand
            foreArmTwist.localRotation = Quaternion.identity;
            Vector3 w = Vector3.ProjectOnPlane(hand.right, foreArm.up);
            float theta3 = Vector3.SignedAngle(foreArmTwist.right, w, foreArm.up);
            foreArmTwist.Rotate(Vector3.up, theta3 * 0.5f, Space.Self);


            //lerp all rotations between original unmodified rotations and target rotations based on current weight
            arm.localRotation = Quaternion.Lerp(origArmRotation, arm.localRotation, weight);
            foreArm.localRotation = Quaternion.Lerp(origForearmRotation, foreArm.localRotation, weight);
            hand.localRotation = Quaternion.Lerp(origHandRotation, hand.localRotation, weight);
            foreArmTwist.localRotation = Quaternion.Lerp(origForearmTwist, foreArmTwist.localRotation, weight);

        }

    }

    private void ComputeBonePositions(out Vector3 foreArmPos, out Vector3 handPos)
    {
        //simple special case of FABRIK IK algorithm with only three points - arm(shoulder), forearm(elbow), and hand

        foreArmPos = foreArm.position;
        handPos = hand.position;
        Vector3 armPos = arm.position;

        int i = 0;
        while(i < maxIterations && Vector3.Distance(handPos, lastTarget.position) > epsilon)
        {
            //forward step
            foreArmPos = lastTarget.position + (foreArmPos - lastTarget.position).normalized * lowerArmLength;

            //reverse step
            foreArmPos = arm.position + (foreArmPos - arm.position).normalized * upperArmLength;
            handPos = foreArmPos + (lastTarget.position - foreArmPos).normalized * lowerArmLength;

            i++;
        }
    }
}
