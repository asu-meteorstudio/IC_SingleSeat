using UnityEngine;

[CreateAssetMenu(fileName = "SetIntParameter", menuName = "State Machine Actions/SetIntParameter")]
public class SetIntParameter : StateMachineAction
{
    public string ParameterName;

    public IntValueType ValueType;
    public int ConstValue;
    [Tooltip("Selects an integer between min [inclusive] and max [exclusive]")]
    public RangedInt RangedValue;
    public bool IsRandom = false;

    int state = 0;

    int parameterHash;

    void OnEnable()
    {
        parameterHash = Animator.StringToHash(ParameterName);
    }

    public override void Execute(Animator animator)
    {
        if (ValueType == IntValueType.Constant)
        {
            animator.SetInteger(parameterHash, ConstValue);
        }
        else
        {
            if (IsRandom == true)
            {
                state = Random.Range(RangedValue.MinValue, RangedValue.MaxValue);
            }
            else
            {
                state += 1;
                if (state >= RangedValue.MaxValue)
                { state = 0; }
            }

            animator.SetInteger(parameterHash, state);
        }
    }
}

public enum IntValueType
{
    Constant,
    Ranged,
}
