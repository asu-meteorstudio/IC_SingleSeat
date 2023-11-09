using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseLook : MonoBehaviour
{
    Vector2 rotation = Vector2.zero;
    public float sensitivity = 2.0f;
    public GameObject player;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        rotation.y += Input.GetAxis("Mouse X") * sensitivity;
        rotation.x -= Input.GetAxis("Mouse Y") * sensitivity;
        rotation.x = Mathf.Clamp(rotation.x, -90, 90);

        player.transform.localRotation = Quaternion.Euler(0, rotation.y, 0);
        transform.localRotation = Quaternion.Euler(rotation.x, 0, 0);
    }
}
