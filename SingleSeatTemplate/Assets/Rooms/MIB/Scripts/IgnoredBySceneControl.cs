using UnityEngine;

public class IgnoredBySceneControl : MonoBehaviour
{
    [EnumMask]
    public SceneControlTag ComponentsToIgnore = ~(SceneControlTag)(0);
}
