using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScrollMainTextureUV : MonoBehaviour
{
    private const string mainTexture = "_MainTex";
    public float scrollSpeedU = 1f, scrollSpeedV = 1f;
    private MeshRenderer rend;
    private float initializationTime;
    private Vector2 uvOffset = Vector2.zero;

    private void Start()
    {
        rend = GetComponent<MeshRenderer>();
        initializationTime = 0f;
    }

    void Update()
    {
        initializationTime += Time.deltaTime;
        uvOffset.x = initializationTime * scrollSpeedU;
        uvOffset.y = initializationTime * scrollSpeedV;
            rend.material.SetTextureOffset(mainTexture,uvOffset);
    }
}