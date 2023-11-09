using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace DreamscapeUtil
{

    public class Description : Attribute
    {
        public string description;

        public Description(string description)
        {
            this.description = description;
        }
    }


    /// <summary>
    /// Global check that will automatically be run for each scene that is checked
    /// </summary>
    public interface ISceneCheckerGlobalSceneCheck
    {
        /// <summary>
        /// Returns the number of errors found in the scene
        /// and logs a descriptive message to the console for each one
        /// </summary>
        /// <returns></returns>
        int CheckForErrors(SceneCheckerContext context);
    }
}
