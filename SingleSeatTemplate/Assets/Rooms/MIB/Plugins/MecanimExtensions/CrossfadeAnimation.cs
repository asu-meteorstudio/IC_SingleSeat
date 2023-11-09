using UnityEngine;

[CreateAssetMenu(fileName = "CrossfadeAnimation", menuName = "State Machine Actions/CrossfadeAnimation")]
public class CrossfadeAnimation : StateMachineAction
{
    public string StateName;
    public float TransitionTime;

    public bool IsRandomTransition;
    public RangedFloat TransitionRange;

    int stateHash;

    void OnEnable()
    {
        stateHash = Animator.StringToHash(StateName);
    }

    public override void Execute(Animator animator)
    {
        if(IsRandomTransition)
        {
            animator.CrossFade(stateHash, Random.Range(TransitionRange.MinValue, TransitionRange.MaxValue));
        }
        else
        {
            animator.CrossFade(stateHash, TransitionTime);
        }
    }
}
