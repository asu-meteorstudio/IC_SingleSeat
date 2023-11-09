using System;
using UnityEngine;

[Serializable]
public struct RangedFloat
{
    public RangedFloat(float min, float max)
    {
        MinValue = min;
        MaxValue = max;
    }

    public float MinValue;
    public float MaxValue;
}

public static partial class FloatExtensions
{
    public static float Remap(this float value, RangedFloat inputRange, RangedFloat outputRange)
    {
        value = Mathf.InverseLerp(inputRange.MinValue, inputRange.MaxValue, value);
        value = Mathf.Lerp(outputRange.MinValue, outputRange.MaxValue, value);
        return value;
    }
}