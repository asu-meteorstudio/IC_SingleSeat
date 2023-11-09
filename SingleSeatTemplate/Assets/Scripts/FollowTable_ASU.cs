using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowTable_ASU : MonoBehaviour
{
    public Transform Target_Table;

    //-0.8f for y and -0.75f for X are settings for pod 4
    public float YOffset = -0.8f;
    public float XOffset = -0.75f;
    //private float ZOffset = 1.0f;


    // Start is called before the first frame update
    void Start()
    {
        //transform.forward = Target_Table.right;
    }

    // Update is called once per frame
    void LateUpdate()
    {        
        Vector3 AdjustedForward = Target_Table.forward;
        AdjustedForward.y = 0;

        transform.right = AdjustedForward;


        transform.position = Target_Table.position + new Vector3(0, YOffset) + (transform.right * XOffset);
    }
}
