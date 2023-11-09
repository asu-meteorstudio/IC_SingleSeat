using System;
using UnityEngine;

public class MouseOrbit : MonoBehaviour
{

    public Transform Target;
    public float distance = 1.0f;

    public float xSpeed = 10.0f;
    public float ySpeed = 10.0f;

    public float yMinLimit = -60.0f;
    public float yMaxLimit = 60.0f;

    public float x = 0.0f;
    public float y = 0.0f;

    // Use this for initialization
    void Start()
    {
        Vector3 angles = transform.eulerAngles;
        x = angles.x;
        y = angles.y;

    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (Input.GetKey(KeyCode.Mouse0))
        {
            x += Input.GetAxis("Mouse X") * xSpeed;
            y += Input.GetAxis("Mouse Y") * -ySpeed;

            y = ClampAngle(y, yMinLimit, yMaxLimit);

            Quaternion rotation = Quaternion.Euler(y, x, 0);

            transform.rotation = rotation;

            if (Input.GetKey(KeyCode.W))
            {

            }
            if (Input.GetKey(KeyCode.S))
            {

            }
        }
    }

    float ClampAngle(float ag, float min, float max)
    {
        if (ag < -360)
            ag += 360;
        if (ag > 360)
            ag -= 360;
        return Mathf.Clamp(ag, min, max);
    }

}
