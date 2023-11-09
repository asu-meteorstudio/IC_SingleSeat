using UnityEngine;
using FluffyUnderware.Curvy;
using GameFlow;
using SubjectNerd.Utilities;
using System.Collections.Generic;

public class GameFlowSplineEvent : CurvyMetadataBase, ICurvyMetadata
{
    [Reorderable]
    public List<Program> Programs = new List<Program>();

    public void Execute()
    {
        foreach(var program in Programs)
        {
            if (program != null)
            { program.Execute(); }
        }
    }
}
