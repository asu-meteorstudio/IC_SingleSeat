using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationStateHack : MonoBehaviour
{
    private Animator anim;
    public int state;
    public string param = "State";

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        anim.SetInteger(param, state);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
