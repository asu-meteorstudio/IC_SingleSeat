using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Artanim;
using UnityEngine.Animations;

public class ObserverCamera : MonoBehaviour
{
    private Transform Target;
    private PositionConstraint PC;
    private RotationConstraint RC;

    // Start is called before the first frame update
    void Start()
    {
        GameController.Instance.OnSceneLoadedInSession += Instance_OnSceneLoadedInSession;
    }

    private void Instance_OnSceneLoadedInSession(string[] sceneNames, bool sceneLoadTimedOut)
    {
        Target = null;
        GameObject go = GameObject.FindGameObjectWithTag("ObserverAnchor");

        if (go)
        {
            //Debug.Log("Setting the Observer Anchor");
            Target = go.transform;
            transform.parent = go.transform;
            transform.localPosition = Vector3.zero;
            transform.forward = Target.forward;
        }
        else
        {
            Debug.LogError("Couldn't find Observer Anchor");
        }
    }

    void FixedUpdate()
    {
        if (Target)
        {
            //transform.forward = Target.forward;

            if (Input.GetKey(KeyCode.UpArrow))
            {
                transform.position += transform.up * 0.01f;
            }

            if (Input.GetKey(KeyCode.DownArrow))
            {
                transform.position += transform.up * -0.01f;
            }

            if (Input.GetKey(KeyCode.LeftArrow))
            {
                transform.Rotate(new Vector3(0, -1.0f, 0));
            }

            if (Input.GetKey(KeyCode.RightArrow))
            {
                transform.Rotate(new Vector3(0, 1.0f, 0));
            }

            if (Input.GetKey(KeyCode.W))
            {
                transform.position += transform.forward * 0.01f;
            }

            if (Input.GetKey(KeyCode.S))
            {
                transform.position += transform.forward * -0.01f;
            }

            if (Input.GetKey(KeyCode.A))
            {
                transform.position += transform.right * -0.01f;
            }

            if (Input.GetKey(KeyCode.D))
            {
                transform.position += transform.right * 0.01f;
            }

            if (Input.GetKey(KeyCode.Q))
            {
                transform.Rotate(new Vector3(1.0f, 0f, 0));
            }

            if (Input.GetKey(KeyCode.E))
            {
                transform.Rotate(new Vector3(-1.0f, 0f, 0));
            }

            if (Input.GetKey(KeyCode.F))
            {
                transform.forward = (-transform.position + Target.position).normalized;
            }

            if (Input.GetKey(KeyCode.R))
            {
                transform.position = Target.position;
                transform.forward = Target.forward;
            }
        }
    }

    private void OnDestroy()
    {
        GameController.Instance.OnSceneLoadedInSession -= Instance_OnSceneLoadedInSession;
    }
}
