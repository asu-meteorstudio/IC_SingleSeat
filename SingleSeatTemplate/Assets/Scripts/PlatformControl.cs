using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformControl : MonoBehaviour
{

    public Animator animator;
    public bool state; //true = running false == stopped

    // Start is called before the first frame update
    void Start()
    {
        state = true;
    }

    public void StartStopPlatform()
    {
        if (state)
        {
            state = false;
            animator.speed = 0;
        }
        else
        {
            state = true;
            animator.speed = 1;
        }
    }
}
