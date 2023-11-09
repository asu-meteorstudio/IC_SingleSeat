using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TemplePearl : MonoBehaviour {

    public Color startColor = Color.black;
    [ColorUsageAttribute(true, true, 0f, 40f, 0.125f, 3f)]
    public Color maxColor = Color.white;
    [ColorUsageAttribute(true, true, 0f, 40f, 0.125f, 3f)]
    public Color minColor = Color.white;

    public Light pearlLight;
    public float minLightRange = 2.0f;
    public float maxLightRange = 10.0f;

    //public AudioSource audioSource;

    public bool startOn = true;

    private Material m;
    private float startTime = -100.0f;
    private bool lightOn = false;

    private float turnOnTime = 4.0f;
    private float period = 2.5f;

    //TODO temp hack
    //int frameNo = 0;

	void Start () {
        pearlLight.range = 0.0f;
        m = GetComponent<Renderer>().material;

        m.SetColor("_EmissionColor", startColor);

        if (startOn)
        {
            lightOn = true;
            startTime = -100.0f;
            //audioSource.enabled = true;
        }
	}
	
	public void TurnOn()
    {
        Debug.Log("Turning Pearl On");
        if(!lightOn)
        {
            lightOn = true;
            startTime = Time.time;
            //audioSource.enabled = true;
        }
    }

    public void TurnOff()
    {
        Debug.Log("Turning Pearl Off");
        if(lightOn)
        {
            lightOn = false;
            startTime = Time.time;
            //audioSource.enabled = false;
        }
    }

    private void Update()
    {

        float t = Time.time - startTime;
        //TODO temp hack
        //float t = frameNo / 24.0f + 100.0f;


        float y;
        if (lightOn)
        {
            y = Mathf.Clamp(t / (turnOnTime), 0.0f, 1.0f);
        }
        else
        {
            y = 1.0f - Mathf.Clamp(t / (turnOnTime), 0.0f, 1.0f);
        }

        float x = 0.5f + 0.5f*Mathf.Sin(2.0f * Mathf.PI * Time.time / period);

        pearlLight.range = Mathf.Lerp(0.0f, Mathf.Lerp(minLightRange, maxLightRange, x), y);

        m.SetColor("_EmissionColor", Color.Lerp(startColor, Color.Lerp(minColor, maxColor, x), y));

        //frameNo++;
    }

}
