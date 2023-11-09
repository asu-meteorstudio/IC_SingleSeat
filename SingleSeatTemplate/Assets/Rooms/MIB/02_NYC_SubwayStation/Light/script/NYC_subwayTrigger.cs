using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class NYC_subwayTrigger : MonoBehaviour {


	/// <summary>
	/// Attach this script to a box collider and size it to your moving platform (train car or elevator etc..) 
	/// when an object with a rigid-body enters the trigger its probe-anchor on the mesh-renderer will be given a dummy object that samples lighting from your original location
	/// since probe lighting cannot be moved this samples lighitng and reflections from wherever you baked the platform lighting.
	/// the vehicle variable should be populated with teh object that moves (train car, elevator etc.)
	/// Reflection source should be the reflection probe that is parented to the moving platform
	/// </summary>
	private BoxCollider trigger;
	public bool enter = true;
	public bool exit = true;
	public bool showLightingOrigin = false;
	private GameObject anchor;
	private bool startAnchor = false;
	private Renderer charRenderer;
	private GameObject character;
	public GameObject vehicle;
	public ReflectionProbe reflectionSource;

	// Use this for initialization
	void Start () {

		trigger = gameObject.GetComponent<BoxCollider>();
		trigger.isTrigger = true;



		if (showLightingOrigin == true)
		{
			anchor = GameObject.CreatePrimitive (PrimitiveType.Sphere);
			anchor.transform.localScale = new Vector3 (0.1f, 0.1f, 0.1f);
		}
		else 
		{
			anchor = new GameObject ();
		}

		anchor.gameObject.transform.position = new Vector3(0, 0, 0);



	}
	
	// Update is called once per frame
	void Update () {

		if (startAnchor == true)
		{
			StartCoroutine (UpdateAnchorPosition ());
		}
		
	}

	private void OnTriggerEnter(Collider other)
	{



		if (enter)
		{
			Debug.Log("entered"); 

			charRenderer = other.GetComponent<MeshRenderer> ();
			charRenderer.probeAnchor = anchor.transform;
			startAnchor = true;
			character = other.gameObject;
			ReflectionProbe probeCopy = Instantiate<ReflectionProbe> (reflectionSource);
			probeCopy.bakedTexture = reflectionSource.bakedTexture;
			probeCopy.boxProjection = false;
			probeCopy.importance = 10;

			charRenderer.reflectionProbeUsage = ReflectionProbeUsage.Simple;

		}
	}


	private void OnTriggerExit(Collider other)
	{
		if (exit)
		{
			Debug.Log("exited"); 

			startAnchor = false;

		}
	}

	IEnumerator UpdateAnchorPosition()
	{
		anchor.gameObject.transform.position = character.transform.position - vehicle.transform.position;

		yield return new WaitForEndOfFrame ();

	}
}
