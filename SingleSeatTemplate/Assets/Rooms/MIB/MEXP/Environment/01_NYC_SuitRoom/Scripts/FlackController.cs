using UnityEngine;

public class FlackController : MonoBehaviour
{
	[SerializeField] ParticleSystem ps;
	[SerializeField] [Range(0f, 3f)] private float emissionStartDelayMin = 0.0f;
	[SerializeField] [Range(0f, 3f)] private float emissionStartDelayMax = 3.0f;
	[SerializeField] [Range(0f, 3f)] private float emissionRateMin = 0.1f;
	[SerializeField] [Range(0f, 3f)] private float emissionRateMax = 0.3f;
	[SerializeField] private bool isGizmoOn = true;

	private const string breathTag = "Breath";
	private ParticleSystem.EmissionModule emitterMod;
	private ParticleSystem.MainModule mainMod;
	private ParticleSystem.ShapeModule shapeMod;
	private ParticleSystem.MinMaxCurve rateOverTimeDefault;
	private ParticleSystem.MinMaxCurve rateOverTimeUpdated;

	private void OnEnable()
	{
		mainMod = ps.main;
		emitterMod = ps.emission;
		shapeMod = ps.shape;

		mainMod.startDelay = Random.Range(emissionStartDelayMin, emissionStartDelayMax);
		rateOverTimeDefault.mode = ParticleSystemCurveMode.TwoConstants;
		rateOverTimeUpdated.mode = ParticleSystemCurveMode.TwoConstants;
		rateOverTimeDefault.constantMax = Random.Range(emissionRateMin, emissionRateMax);
		rateOverTimeUpdated = rateOverTimeDefault;
		emitterMod.rateOverTime = rateOverTimeDefault;

		shapeMod.scale = transform.localScale;
	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.CompareTag(breathTag))
		{
			rateOverTimeUpdated = 0;
			emitterMod.rateOverTime = rateOverTimeUpdated;
		}
	}

	private void OnTriggerExit(Collider other)
	{
		if (other.CompareTag(breathTag))
		{
			rateOverTimeUpdated = rateOverTimeDefault;
			emitterMod.rateOverTime = rateOverTimeUpdated;
		}
	}

	private void OnDrawGizmosSelected()
	{
		if (isGizmoOn)
		{
			Gizmos.color = Color.white;
			Gizmos.matrix = transform.localToWorldMatrix;
			Gizmos.DrawWireCube(Vector3.zero, Vector3.one); 
		}
	}

	private void OnDrawGizmos()
	{
		if (isGizmoOn)
		{
			Gizmos.color = Color.green;
			Gizmos.matrix = transform.localToWorldMatrix;
			Gizmos.DrawWireCube(Vector3.zero, Vector3.one); 
		}
	}
}
