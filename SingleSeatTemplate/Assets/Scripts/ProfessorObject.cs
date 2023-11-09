using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProfessorObject : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        foreach(MeshRenderer mesh in GetComponentsInChildren<MeshRenderer>(true))
        {
            mesh.enabled = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
