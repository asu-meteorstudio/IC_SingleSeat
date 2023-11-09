using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DreamscapeUtil
{
    /// <summary>
    /// Singleton class to allow easy and efficient access to specific GameObjects within a scene(or across scenes/prefabs).
    /// 
    /// The easiest way to use this is to place an instance of the SceneReference script  with a unique id on
    /// any object that you want to reference this way. Then call SceneReferenceManager.Instance.GetReference
    /// to find the gameobject with the specified id.
    /// </summary>
    public class SceneReferenceManager
    {
        private Dictionary<string, GameObject> refs = new Dictionary<string, GameObject>();

        private static SceneReferenceManager _inst = null;

        private SceneReferenceManager()
        {
        }

        /// <summary>
        /// returns the singleton isntance of this class
        /// </summary>
        public static SceneReferenceManager Instance
        {
            get
            {
                if (_inst == null)
                {
                    _inst = new SceneReferenceManager();
                }

                return _inst;
            }
        }

        /// <summary>
        /// registers a new game object reference
        /// </summary>
        /// <param name="id">unique id string for the object</param>
        /// <param name="obj">the game object being referenced</param>
        public void RegisterReference(string id, GameObject obj)
        {
            if (refs.ContainsKey(id))
            {
                Debug.LogErrorFormat("There is already a scene reference with id '{0}'", id);
            }
            else
            {
                refs[id] = obj;
            }
        }

        /// <summary>
        /// unregisters the gameobject with the given id previously registered via RegisterReference
        /// </summary>
        /// <param name="id">id string given when registering the object</param>
        public void RemoveReference(string id)
        {
            refs.Remove(id);
        }

        /// <summary>
        /// returns the game object registered with the given unique id.
        /// 
        /// Will cause an exception if no game object is currently registered with the given id
        /// </summary>
        /// <param name="id">id that object was registered with</param>
        /// <returns>the game object</returns>
        public GameObject GetReference(string id)
        {
            return refs[id];
        }

        /// <summary>
        /// returns specific component attached to the game object registered with the given unique id
        /// 
        /// Will cause an exception if not game object is currently registerd with the given id, or the
        /// game object does not contain a component of type T.
        /// </summary>
        /// <typeparam name="T">type of component to return</typeparam>
        /// <param name="id">id that object was registered with</param>
        /// <returns>the component</returns>
        public T GetReference<T>(string id) where T : Component
        {
            return refs[id].GetComponent<T>();
        }

        /// <summary>
        /// gets the game object registered with the given unique id, if it exists
        /// </summary>
        /// <param name="id">id that object was registered with</param>
        /// <param name="obj">the target game object</param>
        /// <returns>true, if the gameobject was found, false otherwise</returns>
        public bool TryGetReference(string id, out GameObject obj)
        {
            return refs.TryGetValue(id, out obj);
        }

        /// <summary>
        /// gets the specified component on the gameobject with the given unique id, it it exists
        /// </summary>
        /// <typeparam name="T">type of the component to get</typeparam>
        /// <param name="id">id that the object was registered with</param>
        /// <param name="component">the component on the target game object</param>
        /// <returns>true if the component was found, false if there was no gameobject with the given id, or 
        /// if that gameobject does not contain a component of type T</returns>
        public bool TryGetReference<T>(string id, out T component) where T : Component
        {
            if (refs.TryGetValue(id, out GameObject go))
            {
                return go.TryGetComponent<T>(out component);
            }
            else
            {
                component = null;
                return false;
            }
        }
    }
}
