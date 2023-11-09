using UnityEngine;
using System.Collections.Generic;
using SubjectNerd.Utilities;

public class StateMachineHandler : StateMachineBehaviour
{
    [Reorderable][Expandable]
    public List<StateMachineAction> Actions = new List<StateMachineAction>();
}
