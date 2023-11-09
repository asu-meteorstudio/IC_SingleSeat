using UnityEngine;

public class OnStateExitHandler : StateMachineHandler
{
    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateExit(animator, stateInfo, layerIndex);

        foreach(var action in Actions)
        {
            action.Execute(animator);
        }
    }
}
