using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EasyButtons;

public class ChangeChildMaterial : MonoBehaviour
{
    public Material mat;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    [Button]
    public void ChangeMaterial()
    {
        foreach(MeshRenderer rend in GetComponentsInChildren<MeshRenderer>())
        {
            rend.material = mat;
        }
    }
}
