using UnityEngine;

[CreateAssetMenu(fileName = "SetBoolParameter", menuName = "State Machine Actions/SetBoolParameter")]
public class SetBoolParameter : StateMachineAction
{
    public string ParameterName;
    public bool TargetValue;
    int parameterHash;

    private void OnEnable()
    {
        parameterHash = Animator.StringToHash(ParameterName);
    }

    public override void Execute(Animator animator)
    {
        animator.SetBool(parameterHash, TargetValue);
    }
}
