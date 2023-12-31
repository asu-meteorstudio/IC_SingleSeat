﻿using System.Collections.Generic;
using UnityEngine;

public static partial class AnimatorExtensions
{
    public static AnimatorStateInfo GetCurrentAnimatorStateInfo(this Animator animator, string layerName)
    {
        return animator.GetCurrentAnimatorStateInfo(animator.GetLayerIndex(layerName));
    }

    public static AnimatorStateInfo GetNextAnimatorStateInfo(this Animator animator, string layerName)
    {
        return animator.GetNextAnimatorStateInfo(animator.GetLayerIndex(layerName));
    }

    public static bool IsInTransition(this Animator animator, string layerName)
    {
        return animator.IsInTransition(animator.GetLayerIndex(layerName));
    }

    public static float GetCurrentClipLength(this Animator animator, string layerName)
    {
        return animator.GetCurrentClipLength(animator.GetLayerIndex(layerName));
    }

    public static float GetCurrentClipLength(this Animator animator, int layerIndex = 0)
    {
        return animator.GetCurrentAnimatorStateInfo(layerIndex).length;
    }

    public static float GetNextClipLength(this Animator animator, string layerName)
    {
        return animator.GetNextClipLength(animator.GetLayerIndex(layerName));
    }

    public static float GetNextClipLength(this Animator animator, int layerIndex = 0)
    {
        return animator.GetNextAnimatorStateInfo(layerIndex).length;
    }

    public static int State(this Animator animator, string layerName)
    {
        return animator.GetCurrentAnimatorStateInfo(animator.GetLayerIndex(layerName)).fullPathHash;
    }

    public static int State(this Animator animator, int layerIndex)
    {
        if (animator != null && animator.gameObject.activeInHierarchy && animator.runtimeAnimatorController != null)
        {
            return animator.GetCurrentAnimatorStateInfo(layerIndex).fullPathHash;
        }
        else
        {
            return -1;
        }
    }

    public static int NextState(this Animator animator, string layerName)
    {
        return animator.GetNextAnimatorStateInfo(animator.GetLayerIndex(layerName)).fullPathHash;
    }

    public static int NextState(this Animator animator, int layerIndex)
    {
        return animator.GetNextAnimatorStateInfo(layerIndex).fullPathHash;
    }

    public static bool HasParameter(this Animator animator, string paramName)
    {
        var hash = Animator.StringToHash(paramName);
        return animator.HasParameter(hash);
    }

    public static bool HasParameter(this Animator animator, int paramHash)
    {
        if (animator.runtimeAnimatorController != null && animator.parameterCount > 0)
        {
            foreach (AnimatorControllerParameter param in animator.parameters)
            {
                if (param.nameHash == paramHash) return true;
            }
        }
        return false;
    }

    public static Animator MapParameters(this Animator current, Animator target)
    {
        foreach (var parameter in current.parameters)
        {
            switch (parameter.type)
            {
                case AnimatorControllerParameterType.Bool:
                    target.SetBool(parameter.nameHash, current.GetBool(parameter.nameHash));
                    break;
                case AnimatorControllerParameterType.Int:
                    target.SetInteger(parameter.nameHash, current.GetInteger(parameter.nameHash));
                    break;
                case AnimatorControllerParameterType.Float:
                    target.SetFloat(parameter.nameHash, current.GetFloat(parameter.nameHash));
                    break;
            }
        }
        return target;
    }

    public static Animator SetBoolParameters(this Animator current, bool value)
    {
        foreach (var parameter in current.parameters)
        {
            if (parameter.type == AnimatorControllerParameterType.Bool)
                current.SetBool(parameter.nameHash, value);
        }
        return current;
    }

    public static Animator SetIntParameters(this Animator current, int value)
    {
        foreach (var parameter in current.parameters)
        {
            if(parameter.type == AnimatorControllerParameterType.Int)
                current.SetInteger(parameter.nameHash, value);
        }
        return current;
    }

    public static Animator SetFloatParameters(this Animator current, float value)
    {
        foreach (var parameter in current.parameters)
        {
            if (parameter.type == AnimatorControllerParameterType.Float)
                current.SetFloat(parameter.nameHash, value);
        }
        return current;
    }

    public static Animator ResetParameters(this Animator current)
    {
        foreach (var parameter in current.parameters)
        {
            switch (parameter.type)
            {
                case AnimatorControllerParameterType.Bool:
                    current.SetBool(parameter.nameHash, false);
                    break;
                case AnimatorControllerParameterType.Int:
                    current.SetInteger(parameter.nameHash, 0);
                    break;
                case AnimatorControllerParameterType.Float:
                    current.SetFloat(parameter.nameHash, 0f);
                    break;
            }
        }
        return current;
    }

    public static Animator MapParametersToAnimator(this Animator animator, IEnumerable<AnimatorParameter> parameters)
    {
        foreach (AnimatorParameter p in parameters)
        {
            switch (p.type)
            {
                case AnimatorControllerParameterType.Int:
                    animator.SetInteger(p.parameterHash, (int)p.data);
                    break;
                case AnimatorControllerParameterType.Float:
                    animator.SetFloat(p.parameterHash, (float)p.data);
                    break;
                case AnimatorControllerParameterType.Bool:
                    animator.SetBool(p.parameterHash, (bool)p.data);
                    break;
            }
        }
        return animator;
    }

    public static List<AnimatorParameter> MapAnimatorToParameters(this Animator animator, List<AnimatorParameter> parameters)
    {
        for (int i = 0; i < animator.parameters.Length; i++)
        {
            var p = animator.parameters[i];
            var ap = new AnimatorParameter(animator, p.name, p.type);
            parameters.Add(ap);
        }
        return parameters;
    }
}