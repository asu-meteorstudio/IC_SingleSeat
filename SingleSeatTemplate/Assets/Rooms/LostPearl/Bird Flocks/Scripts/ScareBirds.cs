using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScareBirds : MonoBehaviour {

    public LandingSpotController landingSpotController;
    public Animator[] yellowOrigami;
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            landingSpotController.ScareAll(0.0f, 0.1f);
            yellowOrigami = GetComponentsInChildren<Animator>();
            foreach (Animator ori in yellowOrigami)
            {
                ori.SetTrigger("Go");
            }
        }
	}
}
