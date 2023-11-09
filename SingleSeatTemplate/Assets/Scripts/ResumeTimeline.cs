using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class ResumeTimeline : MonoBehaviour
{
    // Start is called before the first frame update
    public PlayableDirector timeline;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnTriggerEnter(Collider other)
    {
        timeline.Play();
    }
}
