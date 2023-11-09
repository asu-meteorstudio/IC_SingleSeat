using UnityEngine;

public class AnimationEventPublisher : MonoBehaviour
{
    public Animator Animator;

    void Reset()
    {
        if (Animator == null)
        { Animator = GetComponent<Animator>(); }
    }

    public void PublishEvent(AnimationEvent animationEvent)
    {
        EventManager.Publish(new AnimationEventData(Animator, animationEvent));
    }
}
