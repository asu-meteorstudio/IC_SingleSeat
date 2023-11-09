using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine;

public class PumpScanBehavior : MonoBehaviour
{
    public UnityEvent action;
    public static bool enabled = false;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(enabled)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                string hitObject = hit.collider.name;
                Debug.Log(hitObject);
                switch (hitObject)
                {
                    case "Pump (1)":
                        if(MovementOnPath.waypointIndex==2)
                        {
                            Debug.Log("Pump 1 Scanned");
                            MovementOnPath.switch1Scan = true;
                            action.Invoke();
                        }
                        break;
                    case "PumpSwitch (1)":
                        if(MovementOnPath.waypointIndex==2)
                        {
                            if (MovementOnPath.switch1Scan)
                            {
                                Debug.Log("Switch 1 Scanned");
                                MovementOnPath.pump1Scan = true;
                                action.Invoke();
                            }
                        }
                        break;
                    case "Pump (2)":
                        if (MovementOnPath.waypointIndex == 3)
                        {
                            Debug.Log("Pump 2 Scanned");
                            MovementOnPath.switch2Scan = true;
                            action.Invoke();
                        }
                        break;
                        /*
                    case "Pump (3)":
                        Debug.Log("Pump 3 Scanned");
                        MovementOnPath.switch3Scan = true;
                        action.Invoke();
                        break;*/
                    case "ProbeCanister":
                        if(MovementOnPath.waypointIndex==5)
                        {
                            MovementOnPath.pump3RobotSequence = true;
                            action.Invoke();
                        }
                        break;
                    default:
                        break;
                }
            }
        }
    }
    void advanceScript()
    {
        action.Invoke();
    }
}
