public class SceneStartedEvent
{
    public SceneReference SceneReference { get; private set; }

    public SceneStartedEvent(SceneReference sceneReference)
    {
        SceneReference = sceneReference;
    }
}
