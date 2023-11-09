using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Artanim;
public class FollowGlobalMocapOffset : MonoBehaviour
{
    private Transform globalMocapOffset;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (!globalMocapOffset)
        {
            globalMocapOffset = GlobalMocapOffset.Instance.transform;
        }
        else
        {
            transform.position = globalMocapOffset.position;
            transform.rotation = globalMocapOffset.rotation;
        }
    }
}
