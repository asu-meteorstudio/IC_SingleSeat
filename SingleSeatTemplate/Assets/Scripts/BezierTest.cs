using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class BezierTest : MonoBehaviour
{
    float proportion = 0f;

    public Transform WaypointsGameObj;
    public Vector3[] waypoints;

    private void Start()
    {
        addWaypoints();
    }
    void addWaypoints()
    {
        waypoints = new Vector3[WaypointsGameObj.childCount];
        for(int i=0;i<WaypointsGameObj.childCount;i++)
        {
            waypoints[int.Parse(WaypointsGameObj.GetChild(i).name)-1]=WaypointsGameObj.GetChild(i).position;
        }
    }

    void Update()
    {
       if (proportion < 1)
        {
            Vector3 BezierPt = applyBezier(waypoints)[0];

            Vector3 displacementVector = (BezierPt - transform.position);
            Quaternion targetRotation = Quaternion.LookRotation(displacementVector.normalized);

            proportion += 0.1f*Time.deltaTime;
            //transform.LookAt(applyBezier(waypoints)[0]);
            transform.position = BezierPt;
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime);

            transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, 0);
        }
    }

    //Recursive Bezier algorithm
    public Vector3[] applyBezier(Vector3[] t)
    {
        if(t.Length==1)
        {
            return t;
        }

        Vector3[] nextDepth = new Vector3[t.Length-1];
        for(int i=0;i<nextDepth.Length;i++)
        {
            nextDepth[i] = BezierPoint(t[i], t[i+1]);
        }
        return applyBezier(nextDepth);
    }

    public Vector3 BezierPoint(Vector3 p1, Vector3 p2)
    {
        return p1+(p2 - p1) * proportion;
    }

    public void segmentedBezier(int segments) //In progress
    {
        //for(int i=0;i<)
    }
}