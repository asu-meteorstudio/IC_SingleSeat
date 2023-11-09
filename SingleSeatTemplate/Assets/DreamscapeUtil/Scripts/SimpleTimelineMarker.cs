using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Timeline;

/// <summary>
/// Just used to mark a point in time(e.g. a point to skip ahead to on the timeline)
/// Doesn't do anything in and of itself, but can be read by other code
/// </summary>
public class SimpleTimelineMarker : Marker
{
    public string ID = "";
}
