using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EasyButtons;
using UnityEditor;

public class DeleteBehavior : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    [Button]
    public void Delete()
    {
        /*foreach (Behaviour b in GetComponents<Behaviour>())
        {
            if (b is DeleteBehavior)
                Debug.Log("Don't delete this script");
            else
            {
                Debug.Log(b);
                DestroyImmediate(b);
            }               
        }*/
#if UNINY_EDITOR
        GameObjectUtility.RemoveMonoBehavioursWithMissingScript(gameObject);
#endif
    }

    [Button]
    public void DeleteAll()
    {

    }
}
