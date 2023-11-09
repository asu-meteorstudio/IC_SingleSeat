using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Dreamscape;
using Artanim.Location.SharedData;
using Artanim.Location.Data;
using Artanim;

public class SceneFanManager : ServerSideBehaviour
{
    [Range(0f, 1f)]
    public float fanConstantValue = 0f;
    // Start is called before the first frame update
    void Start()
    {
        GameController.Instance.OnSceneLoadedInSession += Instance_OnSceneLoadedInSession;        
    }

    private void Instance_OnSceneLoadedInSession(string[] sceneNames, bool sceneLoadTimedOut)
    {        
        HapticManager.Instance.UpdateAllFans(fanConstantValue);
    }

    public void Update()
    {
        if(HapticManager.Instance.m_fanspeed != fanConstantValue)
            HapticManager.Instance.UpdateAllFans(fanConstantValue);
    }

    private void OnDestroy()
    {
        GameController.Instance.OnSceneLoadedInSession -= Instance_OnSceneLoadedInSession;
    }
}
