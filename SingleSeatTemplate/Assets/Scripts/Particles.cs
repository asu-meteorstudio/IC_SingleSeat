using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Particles : MonoBehaviour
{
    private Transform objectTransform;
    private ParticleSystem particleSystem;
    private int emissions = 0;
    private float emissionRateFactor;

    private void Start()
    {
        objectTransform = transform;
        particleSystem = GetComponent<ParticleSystem>();
        
    }

    private void Update()
    {
        Vector3 parentPosition = transform.parent.position;
        //Vector3 objectPosition = objectTransform.position;
        if (parentPosition.x < 700)
        {

            //change ratefactor if you want to make it appear faster 
            emissionRateFactor = 2.0f;
            float emissionsRate = Mathf.Lerp(0f, 35f, parentPosition.x / (700f * emissionRateFactor));
            var emissionModule = particleSystem.emission;
            emissionModule.rateOverTime = emissionsRate;
        }

        //Debug.Log("Object Position: " + objectPosition);

    }
}
