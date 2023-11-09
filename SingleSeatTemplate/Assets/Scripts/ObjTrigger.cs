using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjTrigger : MonoBehaviour
{
    public GameObject obj;
    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("SnowMobile"))
        {
            obj.SetActive(true);
        }
    }
}
