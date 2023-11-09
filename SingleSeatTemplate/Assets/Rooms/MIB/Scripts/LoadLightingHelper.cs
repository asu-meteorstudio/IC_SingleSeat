//#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using UnityEditor;
using UnityEngine.SceneManagement;
//using UnityEditor.SceneManagement;

public class LoadLightingHelper : MonoBehaviour 

{
	
	//public SceneAsset LightingHelperScene;

	public bool debug;
	[HideInInspector]public string name;
    public SceneReference lightLoaderScene;
	//public string name;
	[HideInInspector]public bool loaded = false;




	void Start () 
	{
		


//		if (LightingHelperScene == null) 
//		{
//			print ("No Lighting Helper Scene Specified");
//		} 
//		else 
//		{
//			name = LightingHelperScene.name;
//
//		}

	}




	void Update () 
	{

		if (lightLoaderScene.SceneName != "") 
		{
            
            if (SceneManager.GetActiveScene () == this.gameObject.scene && loaded == false) {
                UnityEngine.Debug.Log("Loading through scene reference");
                StartCoroutine(LoadHelper (lightLoaderScene.SceneName));
			}


			if (SceneManager.GetActiveScene () != this.gameObject.scene && loaded == true) {
				StartCoroutine(UnloadHelper (lightLoaderScene.SceneName));
			}

		}
        else if (name != null)
        {
            
            if (SceneManager.GetActiveScene() == this.gameObject.scene && loaded == false)
            {
                UnityEngine.Debug.Log("Loading through name");
                StartCoroutine(LoadHelper(name));
            }


            if (SceneManager.GetActiveScene() != this.gameObject.scene && loaded == true)
            {
                StartCoroutine(UnloadHelper(name));
            }
        }

	}


	IEnumerator LoadHelper(string loadName)
	{
        loaded = true;
        Application.backgroundLoadingPriority = ThreadPriority.Low;
        yield return SceneManager.LoadSceneAsync(loadName, LoadSceneMode.Additive);
        Application.backgroundLoadingPriority = ThreadPriority.Normal;
        
		Debug (debug);

	}

    IEnumerator UnloadHelper(string unloadName)
	{
        loaded = false;
        Application.backgroundLoadingPriority = ThreadPriority.Low;
        yield return SceneManager.UnloadSceneAsync (unloadName);
        Application.backgroundLoadingPriority = ThreadPriority.Normal;
        
		Debug (debug);
	}




	void Debug(bool debug)
	{
		if (debug == true) 
		{
			

			if (loaded == true) 
			{
				print ("Helper Scene: " + name + " was loaded");
			}

			if (loaded != true) 
			{
				print("Helper Scene: " + name + " was unloaded");
			}

			if (SceneManager.GetActiveScene () == this.gameObject.scene && loaded == false) 
			{
				print ("Something went wrong, this scene: (" + this.gameObject.scene.name + ") is the active scene, but Lighting Helper Scene: (" + name + ") was not loaded");
			}

			print ("Lighting Helper Active Scene:  " + SceneManager.GetActiveScene ().name);
		}
	}



}
//#endif