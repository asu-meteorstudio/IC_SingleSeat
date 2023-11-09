using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FollowTracker : MonoBehaviour
{
    public Transform Offset;
    private Vector3 PulsarOffset = new Vector3(-0.7345f, -0.7493f, 0.0f);

    // Start is called before the first frame update
    void Start()
    {
        
    }

    void Update()
    {
        Vector3 ProjectedUp = Vector3.ProjectOnPlane(Offset.up, Vector3.up);

        transform.right = ProjectedUp;
        transform.localPosition = Offset.localPosition + new Vector3(0,PulsarOffset.y) + transform.right * PulsarOffset.x;   
    }
}
