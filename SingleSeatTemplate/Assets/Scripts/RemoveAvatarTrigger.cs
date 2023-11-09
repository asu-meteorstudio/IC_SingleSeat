using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EasyButtons;
using Artanim;

public class RemoveAvatarTrigger : MonoBehaviour
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
    public void Remove()
    {
        Debug.Log("Removing AvatarTriggers");
        foreach(AvatarTrigger trigger in GetComponentsInChildren<AvatarTrigger>())
        {
            Debug.Log("Destroying " + trigger.gameObject.name);
            DestroyImmediate(trigger);
        }
    }
}
