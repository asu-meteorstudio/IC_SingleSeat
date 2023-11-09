using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BirdController : MonoBehaviour
{
    public Animator[] birds;

    // Start is called before the first frame update
    void Start()
    {
        birds[0].SetBool("flap", true);
        birds[1].SetBool("land", true);
        birds[2].SetBool("flap", true);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
