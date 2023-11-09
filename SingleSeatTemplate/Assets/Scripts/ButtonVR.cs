using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ButtonVR : MonoBehaviour
{
    public UnityEvent onActivate;
    public GameObject button;
    public UnityEvent onPress;
    public UnityEvent onRelease;
    public MonoBehaviour script;
    GameObject presser;
    AudioSource sound;
    bool isPressed;

    void Start()
    {
        sound = GetComponent<AudioSource>();
        isPressed = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!isPressed)
        {
            print("hello");
            button.transform.localPosition = new Vector3(0, 0.0003f, 0);
            presser = other.gameObject;
            onPress.Invoke();
            sound.Play();
            isPressed = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject == presser)
        {
            button.transform.localPosition = new Vector3(0, 0.015f, 0);
            onRelease.Invoke();
            isPressed = false;
        }
    }

    public void ActivateScript()
    {
        //script.enabled = true;
        onActivate.Invoke();
    }

    void OnMouseDown()
    {
        //script.enabled = true;
        onActivate.Invoke();
    }
}
