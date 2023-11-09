using UnityEngine;

public class AnimationEventData
{
    public Animator Animator;
    public AnimationEvent AnimationEvent;

    public AnimationEventData(Animator animator, AnimationEvent animationEvent)
    {
        this.Animator = animator;
        this.AnimationEvent = animationEvent;
    }
}