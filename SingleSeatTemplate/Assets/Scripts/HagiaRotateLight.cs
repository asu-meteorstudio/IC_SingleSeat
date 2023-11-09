using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HagiaRotateLight : MonoBehaviour
{
    public float MaxAngle;
    public float delta;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Mathf.Abs(transform.eulerAngles.z) > MaxAngle)
        {
            delta *= -1;
        }
        transform.Rotate(transform.forward, delta);
    }
}
