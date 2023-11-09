using UnityEngine;

public class OnStateMachineEnterHandler : StateMachineHandler
{
    public override void OnStateMachineEnter(Animator animator, int stateMachinePathHash)
    {
        base.OnStateMachineEnter(animator, stateMachinePathHash);

        foreach(var action in Actions)
        {
            action.Execute(animator);
        }
    }
}
