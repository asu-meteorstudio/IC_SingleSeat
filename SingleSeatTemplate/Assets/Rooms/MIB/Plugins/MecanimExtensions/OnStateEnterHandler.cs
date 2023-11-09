using UnityEngine;

public class OnStateEnterHandler : StateMachineHandler
{
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);

        foreach(var action in Actions)
        {
            action.Execute(animator);
        }
    }
}
