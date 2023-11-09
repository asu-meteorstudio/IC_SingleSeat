using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeasureDistance : MonoBehaviour
{
    public Transform obj1;
    public Transform obj2;
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("obj1:"+obj1.position);
        Debug.Log("obj2:"+obj2.position);
        Debug.Log("Distance:" + Mathf.Sqrt(Mathf.Pow(obj2.position.x - obj1.position.x, 2) + Mathf.Pow(obj2.position.z - obj1.position.z, 2)));
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
