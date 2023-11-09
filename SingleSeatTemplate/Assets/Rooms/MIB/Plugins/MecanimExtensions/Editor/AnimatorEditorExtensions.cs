using UnityEngine;
using UnityEditor.Animations;
using System.Collections.Generic;

public static partial class AnimatorEditorExtensions
{
    public static List<UnityEditor.Animations.AnimatorState> GetChildStates(this AnimatorStateMachine asm, List<UnityEditor.Animations.AnimatorState> states = null)
    {
        if (states == null)
        { states = new List<UnityEditor.Animations.AnimatorState>(); }

        foreach (var state in asm.states)
        {
            states.Add(state.state);
        }

        foreach (var ssm in asm.stateMachines)
        {
            GetChildStates(ssm.stateMachine, states);
        }

        return states;
    }

    public static List<string> GetChildStatePaths(this AnimatorStateMachine asm, List<string> paths = null, string currentPath = "")
    {
        if (paths == null)
        { paths = new List<string>(); }

        foreach (var state in asm.states)
        {
            paths.Add(currentPath + state.state.name);
        }

        foreach (var ssm in asm.stateMachines)
        {
            //full path doesn't seem to work for runtime playback...
            //...so we just use the state name prepended with it's parent state machine name
            //GetChildStatePaths(ssm.stateMachine, paths, currentPath + ssm.stateMachine.name + ".");
            GetChildStatePaths(ssm.stateMachine, paths, ssm.stateMachine.name + ".");
        }

        return paths;
    }
}
