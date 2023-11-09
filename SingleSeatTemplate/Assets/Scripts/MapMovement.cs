using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
//using Unity.VisualScripting;
using UnityEngine;

public class MapMovement : MonoBehaviour
{
    public Transform tracked;
    public Transform indicator;
    public Transform map;
    public Transform localZero;
    public double scale = 10; //map to minimap scale
    void Start()
    {
        //Vector2 mapCenter = map.position;
        //indicator.localPosition = new Vector3(0, 0, 0);
    }
    void Update()
    {
        float x = tracked.position.x - localZero.position.x;
        float z = tracked.position.z - localZero.position.z;
        //Debug.Log("X:" + x + "\nZ:" + z + "\n");
        indicator.localPosition = new Vector3(-x / (float)scale, 0, -z / (float)scale);
    }
}
