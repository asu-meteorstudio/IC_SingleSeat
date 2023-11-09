using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace DreamscapeUtil
{
    /// <summary>
    /// A pool of gameobjects. Used instead of instantiating objects at runtime
    /// </summary>
    public class ObjectPool : MonoBehaviour
    {
        private struct ActivePoolObject
        {
            public float spawnTime;
            public GameObject obj;
        }


        [Tooltip("Template object that will be spawned by this pool")]
        [NonNull]
        public GameObject templateObject;
        [Tooltip("Number of objects in the pool")]
        public int size;

        [Tooltip("If greater than 0 - automatically return object to the pool after this amount of time in seconds")]
        public float lifeSpan = -1;

        //Private Variables
        private Queue<GameObject> pool;

        private List<ActivePoolObject> activeInstances;

        // Use this for initialization
        void Start()
        {
            pool = new Queue<GameObject>(size);

            activeInstances = new List<ActivePoolObject>();

            for (int i = 0; i < size; i++)
            {
                GameObject obj = Instantiate<GameObject>(templateObject, this.transform);

                //set pool to be under this object
                obj.transform.parent = transform;
                obj.transform.localPosition = Vector3.zero;

                obj.SetActive(false);
                pool.Enqueue(obj);
            }
        }

        /// <summary>
        /// Gets an object from the pool and sets it to active
        /// </summary>
        /// <returns></returns>
        public GameObject GetPoolObject(bool setActive = true)
        {
            if (pool.Count > 0)
            {
                GameObject go = pool.Dequeue();
                go.SetActive(setActive);

                activeInstances.Add(new ActivePoolObject { spawnTime = Time.time, obj = go });

                return go;
            }
            else
            {
                Debug.LogWarningFormat(this, "Object Pool '{0}' is not large enough. Instantiating new object instead.", gameObject.name);
                GameObject obj = Instantiate<GameObject>(templateObject);

                //set pool to be under this object
                obj.transform.parent = transform;
                obj.transform.localPosition = Vector3.zero;

                if (lifeSpan > 0)
                {
                    activeInstances.Add(new ActivePoolObject
                    {
                        spawnTime = Time.time,
                        obj = obj
                    });
                }

                return obj;
            }
        }

        /// <summary>
        /// Returns an object to the pool and deactivates it
        /// </summary>
        /// <param name="obj"></param>
        public void ReturnToPool(GameObject obj)
        {
            DoReturnToPool(obj);

            if (lifeSpan > 0)
            {
                activeInstances.RemoveAll(x => x.obj == obj);
            }
        }

        private void DoReturnToPool(GameObject obj)
        {
            obj.SetActive(false);
            obj.transform.parent = transform;
            pool.Enqueue(obj);
        }

        private void Update()
        {
            if (lifeSpan > 0)
            {
                while (activeInstances.Count > 0 && activeInstances[0].spawnTime < Time.time - lifeSpan)
                {
                    DoReturnToPool(activeInstances[0].obj);
                    activeInstances.RemoveAt(0);
                }
            }
        }
    }
}
