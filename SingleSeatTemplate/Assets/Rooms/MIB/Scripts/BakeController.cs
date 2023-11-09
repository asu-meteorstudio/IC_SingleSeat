
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif
[ExecuteInEditMode]
public class BakeController : MonoBehaviour {

	[Header("Other Scenes that will influence this scene's lighting data")][Space]
#if UNITY_EDITOR
    public SceneAsset[] scenesToBake = new SceneAsset[1];
#endif
    [HideInInspector] public  string[] scenePaths = new string[1];

	[Space(20)][Header("Name of group to search for in each scene")][Space]
	public string[] groupNames = new string[1];
	[Space] public bool saveLightHelperScene = true;


	[HideInInspector] public int b = 0;
	private List<GameObject> tempObjects = new List<GameObject>();

	[Space(20)][Header("debug true will hide temp scenes when done instead of deleting")] public bool debug = false;
	//[Space(20)][Header("Original name of this scene's top group and new temp name for baking")]
	[HideInInspector] public string originalName;
	[HideInInspector] public string newName = "ORIGINAL";

	public GameObject LoadLightingManager;

    private const string SAVEFOLDERPATH = "Assets/_Scenes/Light Loader Scenes/";
    //bool rename = false;
#if UNITY_EDITOR
    Lightmapping.OnCompletedFunction myDelegate;

    // Use this for initialization
    void Start () {
        
	}



    [ContextMenu("LoadScenes")]
    public void LoadScenes()
    {
        if (scenesToBake.Length > 0)
        {
            scenePaths = new string[scenesToBake.Length];
            groupNames = new string[scenesToBake.Length];
            for (int i = 0; i < scenesToBake.Length; i++)
            {
                scenePaths[i] = AssetDatabase.GetAssetOrScenePath(scenesToBake[i]);
                EditorSceneManager.OpenScene(scenePaths[i], OpenSceneMode.Additive);
                if (groupNames[i] == null)
                {
                    GameObject[] sceneObjects = UnityEngine.SceneManagement.SceneManager.GetSceneByPath(scenePaths[i]).GetRootGameObjects();

                    foreach (GameObject sceneObject in sceneObjects)
                    {
                        BakeController controller = sceneObject.GetComponent<BakeController>();
                        if (controller && controller.isActiveAndEnabled)
                        {
                            groupNames[i] = controller.gameObject.name;
                        }
                            
                    }
                }
                
                GameObject go = GameObject.Find(groupNames[i]);

                if (go != null)
                {

                    if (go.scene != this.gameObject.scene)
                    {

                        tempObjects.Add(go);
                        tempObjects[i].name += i;
                        if (tempObjects[i].GetComponent<BakeController>() != null)
                        {
                            tempObjects[i].GetComponent<BakeController>().enabled = false;
                        }
                        EditorSceneManager.MoveGameObjectToScene(tempObjects[i], this.gameObject.scene);
                        print("added group: " + groupNames[i] + "from: " + scenesToBake[i] + "  to scene: " + this.gameObject.scene.name + " for baking");
                    }
                }
                else
                {
                    print("no game object named: " + groupNames[i]);
                }

                EditorSceneManager.CloseScene(EditorSceneManager.GetSceneByName(scenesToBake[i].name), true);
            }
        }
    }

    [ContextMenu("StealObjects")]
    public void StealObjects()
    {
        if (scenesToBake.Length > 0)
        {
            for (int i = 0; i < scenesToBake.Length; i++)
            {
                

            }
        }
    }

    // Update is called once per frame
    void Update () {


		



		//if (Lightmapping.isRunning == true && b < 1) 
		//{
		//	originalName = this.gameObject.name;
		//	this.gameObject.name = newName;

		//	myDelegate += LoadMoveBakeScenes;
		//	myDelegate ();
		//}






    }

    void OnRenderObject () {
		
		//if (Lightmapping.isRunning != true ) 
		//{
			


		//	if (b == 1 && Lightmapping.lightingDataAsset != null) 
		//	{
		//		this.gameObject.name = originalName;
		//		myDelegate += CleanUpBakeScenes;
		//		myDelegate ();

				
		//	}

		//}
			

	}


	void LoadMoveBakeScenes()
	{
		
		b++;
		myDelegate -= LoadMoveBakeScenes;
	}


    [ContextMenu("CleanUpScenes")]
	void CleanUpBakeScenes()
	{

		if (saveLightHelperScene == true) {
			SaveSceneUpdateBuildScenes ();
		}


		if (tempObjects.Count >= 1) 
		{
			
			for (int i = 0; i < tempObjects.Count; i++) 
			{
				
				if (tempObjects [i] != null && tempObjects [i] != this.gameObject) 
				{
					
					if (debug == true) 
					{
						
						tempObjects [i].SetActive (false);
						print ("DEBUG ON, hid temp bake object: " + tempObjects [i].name);
						tempObjects.RemoveAt(i);

					} else 
						
					{

						print ("removed temp bake object: " + tempObjects [i].name);
						DestroyImmediate (tempObjects [i]);
					}

				}
			}
		}

		//b = 0;

		if (!debug && tempObjects.Count >= 1) {
			tempObjects.Clear ();
		}
			


			//myDelegate -= CleanUpBakeScenes;

	}
    


	public void SaveSceneUpdateBuildScenes()
	{
		
			
			if (Equals (Lightmapping.lightingDataAsset.name, this.gameObject.scene.name) == false) {
				AssetDatabase.RenameAsset (AssetDatabase.GetAssetPath (Lightmapping.lightingDataAsset), this.gameObject.scene.name);
			}

            string savepath = SAVEFOLDERPATH + this.gameObject.scene.name + "_lightLoader.unity";

            LightingDataAsset origData = Lightmapping.lightingDataAsset;
			bool check = false;

			var emptyScene = EditorSceneManager.NewScene (NewSceneSetup.EmptyScene, NewSceneMode.Additive);
			Lightmapping.lightingDataAsset = origData;
			EditorSceneManager.SaveScene (emptyScene, savepath, true);




			List<EditorBuildSettingsScene> editorBuildSettingsScenes = new List<EditorBuildSettingsScene> ();



			foreach (var scene in EditorBuildSettings.scenes) {

				if (scene.path.Equals (savepath) == true) {
					check = true;

				}
				
				editorBuildSettingsScenes.Add (scene);

			}


			if (emptyScene.buildIndex == -1 && check == false)
				editorBuildSettingsScenes.Add (new EditorBuildSettingsScene (savepath, true));

//		if(!this.gameObject.GetComponent<LoadLightingHelper>())
//			this.gameObject.AddComponent<LoadLightingHelper>();


			//this.gameObject.GetComponent<LoadLightingHelper> ().name = this.gameObject.scene.name + "_lightLoader";
			LoadLightingManager.GetComponent<LoadLightingHelper> ().name = this.gameObject.scene.name + "_lightLoader";
            LoadLightingManager.GetComponent<LoadLightingHelper>().lightLoaderScene = new SceneReference();
            LoadLightingManager.GetComponent<LoadLightingHelper>().lightLoaderScene.ScenePath = savepath;


            EditorBuildSettings.scenes = editorBuildSettingsScenes.ToArray ();		
			EditorSceneManager.CloseScene (emptyScene, true);

			check = false;

		}


#endif
}
