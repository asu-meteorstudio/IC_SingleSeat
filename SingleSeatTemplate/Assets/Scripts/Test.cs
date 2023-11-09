using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        string s = "PROFESSOR";
        string[] seperator = { "0", "-" };

        string[] parts = s.Split(seperator, System.StringSplitOptions.RemoveEmptyEntries);

        foreach(string x in parts)
        {
            Debug.Log(x);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
