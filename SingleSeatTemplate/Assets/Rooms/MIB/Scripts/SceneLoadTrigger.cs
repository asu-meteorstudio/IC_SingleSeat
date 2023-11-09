using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Artanim;


public class SceneLoadTrigger : MonoBehaviour {
    public NetworkActivated networkActivated;

    private void OnEnable()
    {
        if (GameController.Instance)
        {
            GameController.Instance.OnSceneLoadedInSession += OnSceneLoad;
        }
        else
        {
            //for local/non-standalone mode
            networkActivated.OnActivated.Invoke();
        }

    }

    private void OnDisable()
    {
        if (GameController.Instance)
        {
            GameController.Instance.OnSceneLoadedInSession -= OnSceneLoad;
        }
    }

    private void OnSceneLoad(string[] sceneNames, bool sceneLoadTimedOut)
    {
        networkActivated.Activate();
        if (GameController.Instance)
        {
            GameController.Instance.OnSceneLoadedInSession -= OnSceneLoad;
        }
    }
}
