using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowTable : MonoBehaviour
{
    public Transform Target_Table;
    private float YOffset = -0.74295f;
    private float XOffset = -0.7345f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 AdjustedRight = Vector3.ProjectOnPlane(Target_Table.up, Vector3.up);

        transform.right = AdjustedRight;
        transform.localPosition = Target_Table.localPosition + new Vector3(0, YOffset) + transform.localRotation * new Vector3(XOffset, 0);
    }
}
