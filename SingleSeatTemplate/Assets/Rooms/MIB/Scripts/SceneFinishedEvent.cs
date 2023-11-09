public class SceneFinishedEvent
{
    public SceneReference SceneReference { get; private set; }

    public SceneFinishedEvent(SceneReference sceneReference)
    {
        SceneReference = sceneReference;
    }
}