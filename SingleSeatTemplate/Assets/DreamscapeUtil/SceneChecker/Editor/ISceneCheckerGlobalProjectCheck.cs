using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DreamscapeUtil
{
    /// <summary>
    /// Global check that will be run once when check all scenes in build is run.
    /// These are intended for checking project settings or other items that do not
    /// exist in any scene
    /// </summary>
    public interface ISceneCheckerGlobalProjectCheck
    {
        int CheckForErrors();
    }
}
