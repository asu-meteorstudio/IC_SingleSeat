using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EasyButtons;

public class EditorMeshColor : MonoBehaviour
{
    public Material mat;


    [Button]
    public void ColorMesh()
    {
        foreach(MeshRenderer rend in GetComponentsInChildren<MeshRenderer>())
        {
            rend.material = mat;
        }
    }
}
