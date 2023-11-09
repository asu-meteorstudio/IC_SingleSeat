using Artanim;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class StartTimeline : MonoBehaviour
{
    private PlayableDirector timeline;


    // Start is called before the first frame update
    void Start()
    {
        timeline = GetComponent<PlayableDirector>();
        GameController.Instance.OnSceneLoadedInSession += Instance_OnSceneLoadedInSession;
    }

    private void Instance_OnSceneLoadedInSession(string[] sceneNames, bool sceneLoadTimedOut)
    {
        StartCoroutine(PlayTimeline());
    }

    private IEnumerator PlayTimeline()
    {
        yield return new WaitForSeconds(5.0f);

        timeline.Play();
    }

    private void OnDestroy()
    {
        GameController.Instance.OnSceneLoadedInSession -= Instance_OnSceneLoadedInSession;
    }
}
