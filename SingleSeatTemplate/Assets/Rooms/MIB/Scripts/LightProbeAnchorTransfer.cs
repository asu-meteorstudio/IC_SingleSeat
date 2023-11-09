using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Artanim;

public class LightProbeAnchorTransfer : MonoBehaviour
{
    public Transform startingAnchor;
    public Transform endingAnchor;
    public float transferTime;
    public bool transferToNoAnchor;
    [HideInInspector]
    public List<Renderer> renderers;
    private bool isTransfering;
    private float currentTransferTime;
    private Vector3 startingPos;

    // Use this for initialization
    void Start () {
        renderers = new List<Renderer>();
        startingPos = startingAnchor.position;
    }
	
    [ContextMenu("LockPlayerProbes")]
    public void LockPlayerAnchorPoints()
    {
        if(GameController.Instance)
        {
            foreach (RuntimePlayer rp in GameController.Instance.RuntimePlayers)
            {
                renderers.AddRange(rp.AvatarController.GetComponentsInChildren<Renderer>());
            }
        }
#if UNITY_EDITOR
        List<AvatarController> debugAvatars = new List<AvatarController>(GameObject.FindObjectsOfType<AvatarController>());
        foreach (AvatarController ac in debugAvatars)
            renderers.AddRange(ac.GetComponentsInChildren<Renderer>());
#endif
        foreach(Renderer r in renderers)
        {
            if (startingAnchor)
                r.probeAnchor = startingAnchor;
            else
            {
                //instantiate an empty gameobject to be an anchor point
            }
        }
    }

    [ContextMenu("TransferAnchorPoints")]
    public void TransferAnchorPoints(bool releaseAfter)
    {
        currentTransferTime = 0;
        StartCoroutine(MoveAnchorPoint(releaseAfter));
    }

    private IEnumerator MoveAnchorPoint(bool shouldRelease)
    {
        while(currentTransferTime <= transferTime)
        {
            currentTransferTime += Time.fixedDeltaTime;
            float t = Mathf.Clamp(currentTransferTime / transferTime, 0, 1);
            startingAnchor.position = Vector3.Lerp(startingPos, endingAnchor.position, t);
            yield return new WaitForFixedUpdate();
        }
        if(shouldRelease)
        {
            RemoveAllAnchors();
        }
    }

    [ContextMenu("ReleaseAnchors")]
    public void RemoveAllAnchors()
    {
        foreach (Renderer r in renderers)
        {
            r.probeAnchor = null;
        }
        startingAnchor.position = startingPos;
    }
	
}
