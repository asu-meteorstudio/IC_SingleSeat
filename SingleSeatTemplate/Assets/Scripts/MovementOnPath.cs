using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;


public class MovementOnPath : MonoBehaviour
{


    public Transform mobile;
    public float rangeOfMotion = 1f;
    public UnityEvent action;
    public ParticleSystem partSystem;

    [SerializeField]GameObject tempRobot;

    Transform[] toSiteWaypoints;
    [SerializeField]GameObject toSite;
    Transform[] atSiteWaypoints;
    [SerializeField]GameObject atSite;
    Transform[] toCampWaypoints;
    [SerializeField]GameObject toCamp;

    [HideInInspector] Transform[] waypoints;
 
    public float speed;
    public float turnSpeed = 3;
     
    public static int waypointIndex = 0;

    public float idleThreshold = 5.0f;
    private float idleTimer = 0.0f;
    private bool isIdle = false;

    public string destination = "toPumpSite";

    public static bool switch1Scan = false;
    public static bool pump1Scan = false;
    public static bool switch2Scan = false;
    public static bool switch3Scan = false;
    public static bool pump3RobotSequence = false;

    public bool lockMovement = false;

    private float leeway = 2f;

    public TMPro.TMP_Text idleMessage;

    private void Start()
    {
       /// mobile.GetComponent<Rigidbody>().useGravity = true;
        // Get waypoints at each section of the map -- there are 3 sections: toSite, Site, and toCamp. Each is a GameObject in the scene that has waypoints childed underneath them.
        toSiteWaypoints = new Transform[toSite.transform.childCount];
        atSiteWaypoints = new Transform[atSite.transform.childCount];
        toCampWaypoints = new Transform[toCamp.transform.childCount];

        // cycle through waypoints in each array
        for (int i = 0; i < toSite.transform.childCount; i++) // toSite
        {
            Transform t = toSite.transform.GetChild(i);
            int index = parseWaypointForIndex(t.name);
            toSiteWaypoints[index - 1] = t;
        }
        for (int i = 0; i < atSite.transform.childCount; i++) // site
        {
            Transform t = atSite.transform.GetChild(i);
            int index = parseWaypointForIndex(t.name);
            atSiteWaypoints[index - 1] = t;
        }
        for (int i = 0; i < toCamp.transform.childCount; i++) // toCamp
        {
            Transform t = toCamp.transform.GetChild(i);
            int index = parseWaypointForIndex(t.name);
            toCampWaypoints[index - 1] = t;
        }
        PumpScanBehavior.enabled = false;
        waypoints = toSiteWaypoints;
        //setTestingSettings();
    }

    int parseWaypointForIndex(string name) // Takes xxx (n), returns n
    {
        char[] arr = name.ToCharArray();
        for (int i = 0; i < arr.Length; i++)
        {
            if (arr[i].CompareTo(' ') == 0)
            {
                i += 2;
                string ret = "";
                while (arr[i] != ')')
                {
                    ret += arr[i];
                    i++;
                }
                return int.Parse(ret);
            }
        }
        return -1;
    }

    private void Update()
    {
        if(GameState.Singleton.introState == false)
        {
            if (waypointIndex >= waypoints.Length)
            {
                switch (destination)
                {
                    case "toPumpSite":

                        destination = "atPumpSite"; //Transition to settings for driving around pump
                        waypointIndex ^= waypointIndex;
                        waypoints = atSiteWaypoints;
                        speed /= 2;
                        break;
                    case "atPumpSite": //Transition to settings for driving back to camp
                        destination = "toCamp";
                        waypointIndex ^= waypointIndex;
                        waypoints = toCampWaypoints;
                        speed *= 2;
                        break;
                    case "toCamp": //Transition end scnee
                        lockMovement = true;
                        waypointIndex ^= waypointIndex;
                        //TRIGGER END SCENE
                        break;
                }
            }
            Vector3 displacement = waypoints[waypointIndex].position - transform.position;

            if (displacement.magnitude < leeway)
            {
                if(destination.CompareTo("toCamp")==0&&waypointIndex==20)
                {
                    action.Invoke();
                }
                if (destination.CompareTo("atPumpSite") == 0)
                {
                    switch (waypointIndex)
                    {
                        case 0:
                            StartCoroutine(RotateTowardsTarget(waypoints[waypointIndex + 1]));
                            waypointIndex++;
                            break;
                        case 2:
                            if (switch1Scan && pump1Scan)
                            {
                                Debug.Log("Switch 1 & Pump 1 have been scanned");
                                lockMovement = false;
                                PumpScanBehavior.enabled = false;
                                leeway = 2f;
                                waypointIndex++;
                            }
                            else
                            {
                                lockMovement = true;
                                PumpScanBehavior.enabled = true;
                                leeway = 3f;
                            }
                            break;
                            
                        case 3:
                            if (switch2Scan)
                            {
                                //Trigger switch 2 scanned dialogue
                                Debug.Log("Switch 2 has been scanned");
                                lockMovement = false;
                                //StartCoroutine(RotateTowardsTarget(waypoints[waypointIndex + 1].transform));
                                //transform.LookAt(waypoints[waypointIndex+1].position);
                                PumpScanBehavior.enabled = false;
                                leeway = 2f;
                                waypointIndex++;
                            }
                            else
                            {
                                lockMovement = true;
                                PumpScanBehavior.enabled = true;
                                leeway = 3f;
                            }
                            break;
                            /*
                        case 4:
                            if (switch3Scan)
                            {
                                //Trigger switch 3 scanned dialogue
                                Debug.Log("Switch 3 has been scanned");
                                lockMovement = false;
                                //StartCoroutine(RotateTowardsTarget(toCampWaypoints[0].transform));
                                //transform.LookAt(toCampWaypoints[0].position);
                                PumpScanBehavior.enabled = false;
                                leeway = 2f;
                                waypointIndex++;
                            }
                            else
                            {
                                lockMovement = true;
                                PumpScanBehavior.enabled = true;
                                leeway = 3f;
                            }
                            break;*/
                        case 5:
                            if (pump3RobotSequence)
                            {
                                //Trigger switch 3 scanned dialogue
                                Debug.Log("Switch 3 has been scanned");
                                lockMovement = false;
                                //StartCoroutine(RotateTowardsTarget(toCampWaypoints[0].transform));
                                //transform.LookAt(toCampWaypoints[0].position);
                                PumpScanBehavior.enabled = false;
                                leeway = 2f;
                                waypointIndex++;
                            }
                            else
                            {
                                lockMovement = true;
                                PumpScanBehavior.enabled = true;
                                leeway = 3f;
                            }
                            break;
                        case 7:
                            StartCoroutine(RotateTowardsTarget(toCampWaypoints[0]));
                            //leeway = 4f;
                            waypointIndex++;
                            break;
                        default:
                            waypointIndex++;
                            break;
                    }
                }
                else
                {
                    waypointIndex++;
                }
            }

            if (!lockMovement)
            {
                float speedScaled = Mathf.Max(Input.GetAxis("Vertical"), 0) * speed;
                float turnSpeedScaled = Mathf.Max(Input.GetAxis("Vertical"), 0) * turnSpeed;

                float xPos = mobile.transform.localPosition.x + Input.GetAxis("Horizontal") * Input.GetAxis("Vertical") * Time.deltaTime;
                xPos = Mathf.Max(Mathf.Min(xPos, rangeOfMotion), -rangeOfMotion);
                mobile.transform.localPosition = new Vector3(xPos, mobile.transform.localPosition.y, mobile.transform.localPosition.z);

                Vector3 pos = transform.position + transform.forward * speedScaled * Time.deltaTime;
                transform.position = pos;

                Quaternion targetRotation = Quaternion.LookRotation(displacement);
                transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, turnSpeedScaled * Time.deltaTime);
            }

            float verticalInput = Input.GetAxis("Vertical");
            float horizontalInput = Input.GetAxis("Horizontal");
            bool isMoving = Mathf.Abs(verticalInput) > 0.01f || Mathf.Abs(horizontalInput) > 0.01f;

            if (isMoving)
            {
                // Start the particle system when moving
                if (!partSystem.isPlaying)
                {
                    partSystem.Play();
                }
            }
            else
            {
                // Stop the particle system when not moving
                if (partSystem.isPlaying)
                {
                    partSystem.Stop();
                }
            }

            // Check for joystick input
            if (Mathf.Abs(verticalInput) < 0.01f)
            {
                idleTimer += Time.deltaTime;
                if (idleTimer >= idleThreshold && !isIdle)
                {
                    isIdle = true;
                    ShowIdleMessage();
                }
            }
            else
            {
                idleTimer = 0.0f;
                if (isIdle)
                {
                    isIdle = false;
                    HideIdleMessage();
                }
            }
        }

        
    }

    private void ShowIdleMessage()
    {
        if (idleMessage != null)
        {
            idleMessage.gameObject.SetActive(true);
        }
    }

    private void HideIdleMessage()
    {
        if (idleMessage != null)
        {
            idleMessage.gameObject.SetActive(false);
        }
    }

    private IEnumerator RotateTowardsTarget(Transform target)
    {
        Vector3 targetDirection = target.position - transform.position;
        float elapsedTime = 0f;

        Quaternion initialRotation = transform.rotation;
        Quaternion targetRotation = Quaternion.LookRotation(targetDirection.normalized);

        //targetRotation *= Quaternion.Euler(0, 90, 0);

        while (elapsedTime < 1f)
        {
            transform.rotation = Quaternion.Slerp(initialRotation, targetRotation, elapsedTime / 1f);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        transform.rotation = targetRotation;
    }
    private void setTestingSettings()
    {
        transform.position = atSiteWaypoints[2].position;
        waypoints = toCampWaypoints;
        transform.LookAt(toCampWaypoints[0].position);
        waypointIndex ^= waypointIndex;
    }
}
