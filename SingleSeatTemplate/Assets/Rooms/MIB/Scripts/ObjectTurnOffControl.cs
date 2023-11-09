using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectTurnOffControl : MonoBehaviour
{
    public HeadquartersSubstationControl.ObjectTurnOffState whenToTurnOff;
    private HeadquartersSubstationControl subwayManager;
	// Use this for initialization
	void Start () {
        subwayManager = GameObject.FindObjectOfType<HeadquartersSubstationControl>();
        subwayManager.AddToHeadquartersTurnOffList(this);
	}
	
}
