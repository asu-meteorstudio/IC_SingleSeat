using UnityEngine;

[CreateAssetMenu(fileName = "PlayAnimation", menuName = "State Machine Actions/PlayAnimation")]
public class PlayAnimation : StateMachineAction
{
    public string StateName;
    public float TransitionTime;

    int stateHash;

    void OnEnable()
    {
        stateHash = Animator.StringToHash(StateName);
    }

    public override void Execute(Animator animator)
    {
        animator.Play(stateHash);
    }
}
