using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CopyPositionToUV2 : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        SkinnedMeshRenderer smr = GetComponent<SkinnedMeshRenderer>();
        if (smr != null)
        {
            smr.sharedMesh.SetUVs(1, smr.sharedMesh.vertices);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
