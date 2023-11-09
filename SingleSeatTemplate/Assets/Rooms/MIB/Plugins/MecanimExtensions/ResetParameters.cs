using UnityEngine;

[CreateAssetMenu(fileName = "ResetParameters", menuName = "State Machine Actions/ResetParameters")]
public class ResetParameters : StateMachineAction
{
    public override void Execute(Animator animator)
    {
        animator.ResetParameters();
    }
}
