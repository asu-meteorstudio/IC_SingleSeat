using UnityEngine;

namespace MIB
{
    [ExecuteInEditMode]
    public class ScrollMainTextureUV : MonoBehaviour
    {
        private const string mainTexture = "_MainTex";
        public float scrollSpeedU = 1f, scrollSpeedV = 1f;
        private MeshRenderer rend;
        private float initializationTime;
        private Vector2 uvOffset = Vector2.zero;

#if UNITY_EDITOR
        public bool updateInEditMode = false;
#endif

        void Start()
        {
            rend = GetComponent<MeshRenderer>();
            initializationTime = 0f;
        }

        void Update()
        {
#if UNITY_EDITOR
            if (!updateInEditMode && !Application.isPlaying)
                return;
#endif
            initializationTime += Time.deltaTime;
            uvOffset.x = initializationTime * scrollSpeedU;
            uvOffset.y = initializationTime * scrollSpeedV;
            rend.material.SetTextureOffset(mainTexture, uvOffset);
        }
    }
}