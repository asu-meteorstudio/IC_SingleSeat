using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
// Replaces Unity terrain trees with prefab GameObject.
// http://answers.unity3d.com/questions/723266/converting-all-terrain-trees-to-gameobjects.html
[ExecuteInEditMode]
public class TreeReplacerS : EditorWindow
{
    [Header("References")]
    public Terrain _terrain;
    //============================================
    [MenuItem("Window/My/TreeReplacer")]
    static void Init()
    {
        TreeReplacerS window = (TreeReplacerS)GetWindow(typeof(TreeReplacerS));
    }
    void OnGUI()
    {
        _terrain = (Terrain)EditorGUILayout.ObjectField(_terrain, typeof(Terrain), true);
        if (GUILayout.Button("Convert to objects"))
        {
            Convert();
        }
        if (GUILayout.Button("Clear generated trees"))
        {
            Clear();
        }
    }
    //============================================
    public void Convert()
    {
        TerrainData data = _terrain.terrainData;
        float width = data.size.x;
        float height = data.size.z;
        float y = data.size.y;
        // Create parent
        GameObject parent = GameObject.Find("TREES_GENERATED");
        if (parent == null)
        {
            parent = new GameObject("TREES_GENERATED");
        }

        List<GameObject> prototypeRoot = new List<GameObject>();
        for (int i = 0; i<data.treePrototypes.Length; i++)
        {
            GameObject go = new GameObject(data.treePrototypes[i].prefab.name);
            go.transform.parent = parent.transform;
            prototypeRoot.Add(go);
        }

        // Create trees
        foreach (TreeInstance tree in data.treeInstances)
        {
            if (tree.prototypeIndex >= data.treePrototypes.Length)
                continue;
            var _tree = data.treePrototypes[tree.prototypeIndex].prefab;
            Vector3 position = new Vector3(
                tree.position.x * width,
                tree.position.y * y,
                tree.position.z * height) + _terrain.transform.position;
            Vector3 scale = new Vector3(tree.widthScale, tree.heightScale, tree.widthScale);
            //GameObject go = Instantiate(_tree, position, Quaternion.Euler(0f, Mathf.Rad2Deg * tree.rotation, 0f), parent.transform) as GameObject;
            GameObject go = PrefabUtility.InstantiatePrefab(_tree, prototypeRoot[tree.prototypeIndex].transform) as GameObject;
            go.transform.position = position;
            go.transform.rotation = Quaternion.Euler(0f, Mathf.Rad2Deg * tree.rotation, 0f);
            go.transform.localScale = new Vector3(go.transform.localScale.x * scale.x, go.transform.localScale.y * scale.y, go.transform.localScale.z * scale.z);
            
            //EvaluateLod(go);
        }
    }
    public void Clear()
    {
        DestroyImmediate(GameObject.Find("TREES_GENERATED"));
    }

    private void EvaluateLod(GameObject tree)
    {
        LODGroup treeLOD = tree.GetComponent<LODGroup>();
        List<GameObject> toDestroy = new List<GameObject>();
        if (treeLOD)
        {
            int currentLod = GetVisibleLOD(treeLOD, new Vector3(0f, 0f, 0f));
            LOD[] lods = treeLOD.GetLODs();
            for (int i = 0; i < treeLOD.lodCount; i++)
            {
                if (i != currentLod)
                {
                    foreach(Renderer renderer in lods[i].renderers)
                    {
                        //DestroyImmediate(renderer.gameObject);
                        renderer.gameObject.SetActive(false);
                        //renderer.enabled = false;
                        //toDestroy.Add(renderer.gameObject);
                    }
                }
            }
            treeLOD.ForceLOD(currentLod);
            treeLOD.enabled = false;
        }

        /*DestroyImmediate(treeLOD);

        foreach (var go in toDestroy)
            DestroyImmediate(go);*/
    }

    //Return the LODGroup component with a renderer pointing to a specific GameObject. If the GameObject is not part of a LODGroup, returns null 
    /*static public LODGroup GetParentLODGroupComponent(GameObject GO)
    {
        LODGroup LODGroupParent = GO.GetComponentInParent<LODGroup>();
        if (LODGroupParent == null)
            return null;
        LOD[] LODs = LODGroupParent.GetLODs();

        var FoundLOD = LODs.Where(lod => lod.renderers.Where(renderer => renderer == GO.GetComponent<Renderer>()).ToArray().Count() > 0).ToArray();
        if (FoundLOD != null && FoundLOD.Count() > 0)
            return (LODGroupParent);

        return null;
    }


    //Return the GameObject of the LODGroup component with a renderer pointing to a specific GameObject. If the GameObject is not part of a LODGroup, returns null.
    static public GameObject GetParentLODGroupGameObject(GameObject GO)
    {
        var LODGroup = GetParentLODGroupComponent(GO);

        return LODGroup == null ? null : LODGroup.gameObject;
    }

    //Get the LOD # of a selected GameObject. If the GameObject is not part of any LODGroup returns -1.
    static public int GetLODid(GameObject GO)
    {
        LODGroup LODGroupParent = GO.GetComponentInParent<LODGroup>();
        if (LODGroupParent == null)
            return -1;
        LOD[] LODs = LODGroupParent.GetLODs();

        var index = Array.FindIndex(LODs, lod => lod.renderers.Where(renderer => renderer == GO.GetComponent<Renderer>()).ToArray().Count() > 0);
        return index;
    }*/


    //returns the currently visible LOD level of a specific LODGroup, from a specific camera. If no camera is define, uses the Camera.current.
    public static int GetVisibleLOD(LODGroup lodGroup, Vector3 center)
    {
        var lods = lodGroup.GetLODs();
        var relativeHeight = GetRelativeHeight(lodGroup, center);


        int lodIndex = GetMaxLOD(lodGroup);
        for (var i = 0; i < lods.Length; i++)
        {
            var lod = lods[i];

            if (relativeHeight >= lod.screenRelativeTransitionHeight)
            {
                lodIndex = i;
                break;
            }
        }


        return lodIndex;
    }

    //returns the currently visible LOD level of a specific LODGroup, from a the SceneView Camera.
    /*public static int GetVisibleLODSceneView(LODGroup lodGroup)
    {
        Camera camera = SceneView.lastActiveSceneView.camera;
        return GetVisibleLOD(lodGroup, camera);
    }*/

    static float GetRelativeHeight(LODGroup lodGroup, Vector3 center)
    {
        var distance = (lodGroup.transform.TransformPoint(lodGroup.localReferencePoint) - center).magnitude;
        return DistanceToRelativeHeight(90f, (distance / QualitySettings.lodBias), GetWorldSpaceSize(lodGroup));
    }

    static float DistanceToRelativeHeight(float fieldOfView, float distance, float size)
    {
        var halfAngle = Mathf.Tan(Mathf.Deg2Rad * fieldOfView * 0.5F);
        var relativeHeight = size * 0.5F / (distance * halfAngle);
        return relativeHeight;
    }
    public static int GetMaxLOD(LODGroup lodGroup)
    {
        //return lodGroup.lodCount - 1; // handle the culled state
        return lodGroup.lodCount;
    }
    public static float GetWorldSpaceSize(LODGroup lodGroup)
    {
        return GetWorldSpaceScale(lodGroup.transform) * lodGroup.size;
    }
    static float GetWorldSpaceScale(Transform t)
    {
        var scale = t.lossyScale;
        float largestAxis = Mathf.Abs(scale.x);
        largestAxis = Mathf.Max(largestAxis, Mathf.Abs(scale.y));
        largestAxis = Mathf.Max(largestAxis, Mathf.Abs(scale.z));
        return largestAxis;
    }
}