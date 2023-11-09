using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EasyButtons;
using Artanim;

public class QuizzNameChanger : MonoBehaviour
{
    public string modifier;

    [Button]
    public void ChangeString()
    {
        Debug.Log("Adding modifier: " + modifier);
        foreach(AvatarTrigger trigger in GetComponentsInChildren<AvatarTrigger>())
        {
            trigger.ObjectId += modifier;
            Debug.Log("New trigger string is " + trigger.ObjectId + modifier);
        }
    }
}
