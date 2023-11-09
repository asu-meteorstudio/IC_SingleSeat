using UnityEngine;

public abstract class StateMachineAction : ScriptableObject
{
    public abstract void Execute(Animator animator);
}
