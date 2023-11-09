using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;

[ExecuteInEditMode]
#endif
public class NYC_TrainBakeSetup : MonoBehaviour {


	public GameObject BakeLocation_NYC;
	public GameObject BakeLocation_HYP;
	public GameObject NYC_Train;
	public GameObject Hyper_Train;

	private bool didIt = false;
	public bool BakeTrain = false;

	void Start () {
		
	}

#if UNITY_EDITOR
    void OnRenderObject () {
		
		if (NYC_Train != null && Hyper_Train != null) {



			if (Lightmapping.isRunning == true && didIt == false && BakeTrain == true) {
				SetBakePos();
			} 

			if (Lightmapping.isRunning != true && BakeTrain)
			{
				ResetBakePos();
			}

		}
		

	}

#endif


    void SetBakePos(){


		NYC_Train.transform.position = BakeLocation_NYC.transform.position;
		Hyper_Train.transform.position = BakeLocation_HYP.transform.position;
		didIt = true;
	}


	void ResetBakePos(){


		NYC_Train.transform.localPosition = new Vector3 (0.0f, 0.0f, 0.0f);
		Hyper_Train.transform.localPosition = new Vector3 (0.0f, 0.0f, 0.0f);
		didIt = false;
	}

}
