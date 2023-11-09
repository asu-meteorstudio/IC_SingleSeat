using System.Collections;
using UnityEngine;

public class GalaxyScaleBlendTreeController : MonoBehaviour
{
    [SerializeField] private AnimationCurve galaxyCurve;
    [SerializeField] private AnimationCurve planetCurve;
    [SerializeField] private float elapsedTime = 5f;

    private Animator animator;
    private int galaxyParamID;
    private int planetParamID;
    private IEnumerator scaleCoroutine;
    private Coroutine startScaleCoroutine;
    private bool isBlending = false;
    private float galaxyAnimCurvePointer;
    private float planetAnimCurvePointer;
    private float startElapsedTime;

    private void Start()
    {
        animator = GetComponent<Animator>();
        galaxyParamID = Animator.StringToHash("GalaxyScaleBlend");
        planetParamID = Animator.StringToHash("PlanetScaleBlend");
        startElapsedTime = elapsedTime;
        scaleCoroutine = ScaleGalaxy();
    }

    public void StartScale()
    {
        isBlending = true;
        elapsedTime = startElapsedTime;
        galaxyAnimCurvePointer = 0;

        if (startScaleCoroutine == null)
        {
            startScaleCoroutine = StartCoroutine(scaleCoroutine);
        }
    }

    IEnumerator ScaleGalaxy()
    {
        while (true)
        {
            if (isBlending && elapsedTime > 0)
            {
                elapsedTime -= Time.deltaTime;
                galaxyAnimCurvePointer = galaxyCurve.Evaluate(Mathf.InverseLerp(startElapsedTime, 0, elapsedTime));
                planetAnimCurvePointer = planetCurve.Evaluate(Mathf.InverseLerp(startElapsedTime, 0, elapsedTime));
                animator.SetFloat(galaxyParamID, galaxyAnimCurvePointer);
                animator.SetFloat(planetParamID, planetAnimCurvePointer);

                if (elapsedTime <= 0)
                {
                    isBlending = false;
                }
            }
            yield return null;
        }
    }


    private void OnDestroy()
    {
        if (startScaleCoroutine != null)
        {
            startScaleCoroutine = null;
            StopCoroutine(startScaleCoroutine);
        }
    }
}
