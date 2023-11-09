using UnityEngine;
using UnityEditor;

static class RemoveAllComponents
{

    [MenuItem("Dreamscape/Remove Components Recursively")]
    static void RemoveComponents()
    {
        GameObject go = Selection.activeGameObject;
        Transform[] transforms = go.GetComponentsInChildren<Transform>();
        foreach (Transform t in transforms)
        {
            foreach (var comp in t.GetComponents<Component>())
            {
                if (!(comp is Transform))
                {
                    Object.DestroyImmediate(comp as Object, true);
                }
            }
        }
        
    }
    
}
