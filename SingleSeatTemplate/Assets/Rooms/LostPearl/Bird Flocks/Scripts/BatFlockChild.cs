using UnityEngine;
using Artanim;
using Artanim.Location.Network;

public class BatFlockChild:FlockChild
{
    void Awake()
    {
        Debug.Log("Searching for flock Controller in bat flock controller");
        GameObject go = GameObject.FindGameObjectWithTag("BatSpawner");
        _spawner = go.GetComponent<FlockController>();
    }


}