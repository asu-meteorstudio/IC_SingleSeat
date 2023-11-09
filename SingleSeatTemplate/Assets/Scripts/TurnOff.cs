using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TurnOff : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        foreach(MeshRenderer rend in GetComponentsInChildren<MeshRenderer>())
        {
            rend.enabled = false;
        }

        foreach(SphereCollider col in GetComponentsInChildren<SphereCollider>())
        {
            col.enabled = false;
        }

        foreach(Text t in GetComponentsInChildren<Text>())
        {
            t.enabled = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
