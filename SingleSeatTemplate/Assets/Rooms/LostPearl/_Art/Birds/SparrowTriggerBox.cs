using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SparrowTriggerBox : MonoBehaviour {

    public Animator anim;


    private void Start()
    {
        //terrible hack around bug in dreamteck spline system that is causing spline to be evaluated in world space instead of local
        transform.parent.position = transform.parent.position + new Vector3(0.0001f, 0.0f, 0.0f);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("SceneTriggerCollider"))
        {
            anim.SetTrigger("takeoff");
        }
    }
}
