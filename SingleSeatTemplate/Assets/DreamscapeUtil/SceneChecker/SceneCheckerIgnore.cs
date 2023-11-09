using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DreamscapeUtil
{
    /// <summary>
    /// Tells the scene checker to not report any errors for this object
    /// </summary>
    public class SceneCheckerIgnore : MonoBehaviour
    {
        /// <summary>
        /// If true, also ignore entire hierarchy under this object
        /// </summary>
        public bool ignoreChildren = true;
    }
}
