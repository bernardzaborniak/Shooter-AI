using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// SensingInfo Component saves information about tactical points it has seen in this container.
public class AI_SI_TacticalPointVisibilityInfo 
{
    public TacticalPoint point;
    public TacticalPointVisibilityInfo visInfo;

    public float lastSquaredDistanceMeasured;

    public float timeWhenLastSeen;
    public int frameCountWhenLastSeen;

    public AI_SI_TacticalPointVisibilityInfo(TacticalPointVisibilityInfo visInfo)
    {
        this.visInfo = visInfo;
        point = visInfo.tacticalPointAssignedTo;

        timeWhenLastSeen = Time.time;
        frameCountWhenLastSeen = Time.frameCount;
    }
}
