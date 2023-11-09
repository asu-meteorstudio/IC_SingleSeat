using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Hydroform;

public class VC_CameraManager : MonoBehaviour
{
    public List<string> underwaterScenes;
    public Shader undewaterFilterShader;

    private string currentSceneName = null;
    private HydroMultiCamComp hydroMultiCamComp = null;
    private UnderwaterFilter underwaterFilter = null;

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    public IEnumerator AddHydroformComponents()
    {
        // If Hydroform components already exist on camera, destoy them
        if (hydroMultiCamComp != null)
        {
            Destroy(hydroMultiCamComp);
            //hydroMultiCamComp = null;
        }
        if (underwaterFilter != null)
        {
            Destroy(underwaterFilter);
            //underwaterFilter = null;
        }

        yield return null;
    
        // If the loaded scene is a scene requiring Hydroform components, add them to the camera
        if (underwaterScenes.Contains(currentSceneName))
        {
            Debug.Log(currentSceneName + " loaded. Adding Hydroform camera effects.");

            //if (hydroMultiCamComp == null)
            //{
            hydroMultiCamComp = gameObject.AddComponent<HydroMultiCamComp>();
            //}

            //if (underwaterFilter == null)
            //{
            underwaterFilter = gameObject.AddComponent<UnderwaterFilter>();
            if (undewaterFilterShader != null)
                underwaterFilter.Shader = undewaterFilterShader;
            else
                Debug.LogError("No underwaterFilter shader assigned to the VC_CameraManager component");
            //}
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        currentSceneName = scene.name;

        StartCoroutine("AddHydroformComponents");
    }
}
