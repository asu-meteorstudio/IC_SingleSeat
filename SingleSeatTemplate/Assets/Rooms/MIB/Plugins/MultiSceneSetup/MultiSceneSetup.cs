using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Utilities;
using SubjectNerd.Utilities;

public class MultiSceneSetup : ScriptableObject
{
	[Reorderable]
	public SceneSetup[] Setups;

#if UNITY_EDITOR
    [ContextMenu("Select All")]
    void SelectAll()
    {
        foreach (var setup in Setups)
            setup.isLoaded = true;
    }

    [ContextMenu("Deselect All")]
    void DeselectAll()
    {
        foreach (var setup in Setups)
            setup.isLoaded = false;
    }
#endif
}

namespace Utilities
{
	[System.Serializable]
	public class SceneSetup
	{
        public SceneReference SceneReference;
		public bool isActive;
		public bool isLoaded;
	}
}