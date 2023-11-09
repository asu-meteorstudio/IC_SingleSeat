using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class InvokeOnCollision : MonoBehaviour
{
    public UnityEvent eventToTrigger;
    // Start is called before the first frame update
    void Start()
    {
        
    }
    private void OnTriggerEnter(Collider other)
    {
        eventToTrigger.Invoke();
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
