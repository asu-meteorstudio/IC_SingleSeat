using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;

public class WaypointTest : MonoBehaviour
{
    public GameObject player;
    /*
    public Transform waypoint1;
    public Transform waypoint2;
    public Transform waypoint3;
    public Transform waypoint4;
    public Transform waypoint5;*/
    public Transform WaypointsGameObj;
    List<Vector3> waypoints = new List<Vector3>();

    public LineRenderer lineRenderer;

    //List<int> waypoints = new List<int>();

    // Start is called before the first frame update
    void Start()
    {
        lineRenderer = gameObject.GetComponent<LineRenderer>();

        for (int i = 0; i < WaypointsGameObj.childCount; i++)
        {
            waypoints.Add(WaypointsGameObj.GetChild(i).position); //Unstable code, has to be in sorted order
        }

        /*
        waypoints.Add(waypoint1.position);
        waypoints.Add(waypoint2.position);
        waypoints.Add(waypoint3.position);
        waypoints.Add(waypoint4.position);
        waypoints.Add(waypoint5.position);*/

        lineRenderer.positionCount = waypoints.Count;

        print(waypoints.Count);
        for(int i=0; i<waypoints.Count; i++)
        {
            lineRenderer.SetPosition(i, waypoints.ElementAt(i));
            print(i + " " + waypoints[i]);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
