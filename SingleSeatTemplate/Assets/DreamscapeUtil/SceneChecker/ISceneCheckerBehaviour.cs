using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DreamscapeUtil
{
    /// <summary>
    /// Behaviour to check for errors during scene check.
    /// CheckForErrors will be called on all instances in a scene when
    /// that scene is being checked.
    /// </summary>
    public interface ISceneCheckerBehaviour
    {

        /// <summary>
        /// Returns the number of errors found in the scene
        /// and logs a descriptive warning message to the console for each one
        /// </summary>
        /// <returns></returns>
        int CheckForErrors();

    }
}
