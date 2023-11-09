#if ENABLE_INPUT_SYSTEM
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem.XR;
using static UnityEngine.XR.OpenXR.Features.Interactions.MicrosoftMotionControllerProfile;

namespace DreamscapeUtil
{

    /// <summary>
    /// Simple script to control camera movement using VR controllers. Requires new Input System
    /// </summary>
    public class VRFlyCam : MonoBehaviour
    {
        public Transform cameraTransform;

        private float forwardAmt = 0.0f;
        private float rightAmt = 0.0f;
        private float upAmt = 0.0f;
        private float turnAmt = 0.0f;
        private float turnAmtVel = 0.0f;

        private const float MOVEMENT_EASE_RATE = 3.0f;
        private const float TURN_EASE_TIME = 0.35f;

        public float movementSpeed = 10.0f;
        public float turnSpeed = 25.0f;

        // Update is called once per frame
        void Update()
        {
            XRController leftHand = InputSystem.GetDevice<WMRSpatialController>("LeftHand");// XRController.leftHand;
            XRController rightHand = InputSystem.GetDevice<WMRSpatialController>("RightHand");// XRController.rightHand;

            float currForwardAmt = 0.0f;
            float currRightAmt = 0.0f;
            float currUpAmt = 0.0f;
            float currTurnAmt = 0.0f;

            if (leftHand != null)
            {
                currForwardAmt = ProcessDeadzone(leftHand.GetChildControl<AxisControl>("joystick/y").ReadValue(), .1f);
                currRightAmt = ProcessDeadzone(leftHand.GetChildControl<AxisControl>("joystick/x").ReadValue(), .1f);
                currUpAmt = leftHand.GetChildControl<AxisControl>("trigger").ReadValue() - leftHand.GetChildControl<AxisControl>("grip").ReadValue();
            }
            if (rightHand != null)
            {
                currTurnAmt = ProcessDeadzone(rightHand.GetChildControl<AxisControl>("joystick/x").ReadValue(), .1f);
            }

            forwardAmt = Mathf.Lerp(forwardAmt, currForwardAmt, 1.0f - Mathf.Exp(-MOVEMENT_EASE_RATE * Time.deltaTime));
            rightAmt = Mathf.Lerp(rightAmt, currRightAmt, 1.0f - Mathf.Exp(-MOVEMENT_EASE_RATE * Time.deltaTime));
            upAmt = Mathf.Lerp(upAmt, currUpAmt, 1.0f - Mathf.Exp(-MOVEMENT_EASE_RATE * Time.deltaTime));
            turnAmt = Mathf.SmoothDamp(turnAmt, currTurnAmt, ref turnAmtVel, TURN_EASE_TIME, Mathf.Infinity, Time.deltaTime);


            transform.position += (forwardAmt * cameraTransform.forward + rightAmt * cameraTransform.right + upAmt * Vector3.up) * movementSpeed * Time.deltaTime;

            transform.RotateAround(cameraTransform.position, Vector3.up, turnAmt * turnSpeed * Time.deltaTime);
        }

        private float ProcessDeadzone(float input, float deadZoneAmt)
        {
            float absValue = Mathf.Abs(input);
            float sign = Mathf.Sign(input);

            if (absValue < deadZoneAmt)
            {
                return 0.0f;
            }
            else
            {
                return sign * (absValue - deadZoneAmt) / (1.0f - deadZoneAmt);
            }
        }
    }
}
#endif
