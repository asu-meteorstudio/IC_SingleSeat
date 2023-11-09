using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace DreamscapeUtil {

    /// <summary>
    /// Utility for iterating through scene hierarchies while skipping over game objects that should be
    /// ignored by scene checker
    /// </summary>
    public class SceneCheckerIterator : IEnumerator<GameObject>
    {
        private Transform root;
        private Transform current;
        private Scene scene;
        private int rootIdx = -1; //current scene object
        private bool skipChildren = false;
        private bool skipAll = false;

        private SceneCheckerIterator()
        {
        }

        private SceneCheckerIterator(Scene s)
        {
            current = null;
            root = null;
            scene = s;
            rootIdx = 0;
        }

        private SceneCheckerIterator(GameObject go)
        {
            root = go.transform;
            scene = go.scene;
            SceneCheckerIgnore ignore = root.GetComponentInParent<SceneCheckerIgnore>();
            if(ignore && ignore.ignoreChildren)
            {
                skipAll = true;
            }
        }

        /// <summary>
        /// Returns an iterator that iterates through all game objects in the scene, skipping
        /// any game objects as specified by scene checker settings
        /// </summary>
        /// <param name="s">Scene to iterator through</param>
        /// <returns></returns>
        public static SceneCheckerIterator IterScene(Scene s)
        {
            return new SceneCheckerIterator(s);
        }

        /// <summary>
        /// Returns an iterator that iterates through an object and all of its descendants, skipping
        /// any gameobjects as specified by scene checker setttings
        /// </summary>
        /// <param name="go">game object to start at</param>
        /// <returns></returns>
        public static SceneCheckerIterator IterHierarchy(GameObject go)
        {
            return new SceneCheckerIterator(go);
        }

        /// <summary>
        /// Returns an iterator for the given context(either a scene or a prefab). Skips any gameobjects as specific by
        /// scene checker settings
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public static SceneCheckerIterator IterContext(SceneCheckerContext context)
        {
            if(context.PrefabRoot != null)
            {
                return new SceneCheckerIterator(context.PrefabRoot);
            }
            else
            {
                return new SceneCheckerIterator(context.Scene);
            }
        }

        /// <summary>
        /// Returns the current gameobject being iterated over
        /// </summary>
        public GameObject Current
        {
            get
            {
                if (current)
                {
                    return current.gameObject;
                }
                else
                {
                    throw new InvalidOperationException();
                }
            }
        }

        object IEnumerator.Current {
            get
            {
                return this.Current;
            }
        }


        public void Dispose()
        {
            current = null;
            root = null;
        }

        /// <summary>
        /// Advances the iterator to the next object. Returns false when the end is reached
        /// </summary>
        /// <returns></returns>
        public bool MoveNext()
        {

            if (skipAll)
            {
                return false;
            }
            bool ret;
            bool shouldIgnoreChildren = false;
            do
            {
                ret = MoveNext_internal();
                shouldIgnoreChildren = false;
                if (current)
                {
                    shouldIgnoreChildren = ShouldIgnoreChildren(current.gameObject);
                    if (shouldIgnoreChildren)
                    {
                        skipChildren = true;
                    }
                }
            } while (ret && current && (shouldIgnoreChildren || ShouldIgnore(current.gameObject)));
            return ret;
        }

        /// <summary>
        /// Tells the iterator to skip over all descendants of the current object
        /// </summary>
        public void SkipChildren()
        {
            skipChildren = true;
        }

        bool ShouldIgnore(GameObject go)
        {
            if (go.TryGetComponent<SceneCheckerIgnore>(out _))
            {
                return true;
            }
            if(go.hideFlags != HideFlags.None)
            {
                return true;
            }

            return false;
        }

        bool ShouldIgnoreChildren(GameObject go)
        {
            if (go.CompareTag("EditorOnly") && !SceneCheckerSettings.Instance.checkEditorOnlyObjects)
            {
                return true;
            }

            if(!go.activeSelf && !SceneCheckerSettings.Instance.checkInactiveObjects)
            {
                return true;
            }

            if(go.TryGetComponent<DestroyInSession>(out _) && SceneCheckerSettings.Instance.ignoreAllObjectsWithDestroyInSession)
            {
                return true;
            }
            if(go.TryGetComponent<SceneCheckerIgnore>(out SceneCheckerIgnore ignore) && ignore.ignoreChildren)
            {
                return true;
            }

            return false;
        }

        private bool MoveNext_internal()
        {
            if (!current)
            {
                if (root)
                {
                    current = root;
                    return true;
                }
                else
                {
                    current = scene.GetRootGameObjects()[0].transform;
                    return true;
                }
            }
            else
            {
                if(current.childCount > 0 && !skipChildren)
                {
                    current = current.GetChild(0);
                    return true;
                }
                else
                {
                    skipChildren = false;
                    Transform t = current;

                    while(t != root && t.parent && t.GetSiblingIndex() >= t.parent.childCount - 1)
                    {
                        t = t.parent;
                    }

                    if(t == root)
                    {
                        current = null;
                        return false;
                    }

                    if (t.parent && t.GetSiblingIndex() < t.parent.childCount - 1)
                    {
                        current = t.parent.GetChild(t.GetSiblingIndex() + 1);
                        return true;
                    }
                    else
                    {
                        if (root)
                        {
                            current = null;
                            return false;
                        }
                        else
                        {
                            rootIdx++;
                            if(rootIdx < scene.GetRootGameObjects().Length)
                            {
                                current = scene.GetRootGameObjects()[rootIdx].transform;
                                return true;
                            }
                            else
                            {
                                current = null;
                                return false;
                            }
                        }
                    }
                }
            }
        }

        public void Reset()
        {
            current = root;
        }
    }
}
