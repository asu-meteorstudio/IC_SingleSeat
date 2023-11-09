using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class WorldLightingSettings : MonoBehaviour
{
    [Header("Environment")]
    public Material skyboxMaterial;
    //public Light sunSource;
    public AmbientMode ambientMode;
    public Color ambientSkyColor;
    public Color ambientEquatorColor;
    public Color ambientGroundColor;
    [Header("Fog Settings")]
    public bool fogEnabled;
    public Color fogColor;
    public FogMode fogMode;
    public float fogDensity;
    public float fogStart;
    public float fogEnd;

	public void SwapLightSettings()
    {
        RenderSettings.skybox = skyboxMaterial;
        //RenderSettings.sun = sunSource;
        RenderSettings.ambientMode = ambientMode;
        RenderSettings.ambientLight = ambientSkyColor;
        RenderSettings.ambientEquatorColor = ambientEquatorColor;
        RenderSettings.ambientGroundColor = ambientGroundColor;

        RenderSettings.fog = fogEnabled;
        RenderSettings.fogColor = fogColor;
        RenderSettings.fogMode = fogMode;

        switch (fogMode)
        {
            case (FogMode.Exponential):
                RenderSettings.fogDensity = fogDensity;
                break;
            case (FogMode.Linear):
                RenderSettings.fogStartDistance = fogStart;
                RenderSettings.fogEndDistance = fogEnd;
                break;
        }
    }
	
	
}
