using UnityEngine;
using FluffyUnderware.Curvy;

public class SplineFollowerOffsetEvent : CurvyMetadataBase, ICurvyMetadata
{
    public float SmoothTime;
    public float Offset;
    public float Duration;
    public bool IsEnabled = true;
}
