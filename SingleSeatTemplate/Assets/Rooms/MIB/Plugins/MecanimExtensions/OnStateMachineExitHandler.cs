using UnityEngine;

public class OnStateMachineExitHandler : StateMachineHandler
{
    public override void OnStateMachineExit(Animator animator, int stateMachinePathHash)
    {
        base.OnStateMachineExit(animator, stateMachinePathHash);

        foreach(var action in Actions)
        {
            action.Execute(animator);
        }
    }
}
