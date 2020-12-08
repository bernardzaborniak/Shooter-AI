using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Sensing Component saves information about tactical points it has seen in this container

public class AIC_S_TacticalPointVisibilityInfo 
{
    public TacticalPoint point;
    public TacticalPointVisibilityInfo visInfo;

    public float lastSquaredDistanceMeasured;

    public AIC_S_TacticalPointVisibilityInfo(TacticalPointVisibilityInfo visInfo)
    {
        this.visInfo = visInfo;
        point = visInfo.tacticalPointAssignedTo;
    }


}
