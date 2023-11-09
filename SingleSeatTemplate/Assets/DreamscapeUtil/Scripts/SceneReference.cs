using DreamscapeUtil;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DreamscapeUtil
{
    /// <summary>
    /// Place this script on a gameobject and give it a unique id to make it easy and efficient to find via SceneReferenceManager.
    /// Automatically registers this gameobject with the SceneReferenceManager on Awake and unregisters it on Destroy.
    /// </summary>
    public class SceneReference : MonoBehaviour
    {
        [NonEmpty]
        public string id;


        // Start is called before the first frame update
        void Awake()
        {
            SceneReferenceManager.Instance.RegisterReference(id, this.gameObject);
        }

        // Update is called once per frame
        void OnDestroy()
        {
            SceneReferenceManager.Instance.RemoveReference(id);
        }
    }
}
