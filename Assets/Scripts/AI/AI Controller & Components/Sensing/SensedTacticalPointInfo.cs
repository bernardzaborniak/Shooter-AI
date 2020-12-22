using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BenitosAI
{
    // SensingInfo Component saves information about tactical points it has seen in this container.
    public class SensedTacticalPointInfo
    {
        public TacticalPoint tacticalPoint;
        public TacticalPointVisibilityInfo visInfo;

        public float lastSquaredDistanceMeasured;

        public float timeWhenLastSeen;
        public int frameCountWhenLastSeen;

        public SensedTacticalPointInfo()
        {
           
        }

        public void SetUpInfo(TacticalPointVisibilityInfo visInfo, float squaredDistance)
        {
            this.visInfo = visInfo;
            tacticalPoint = visInfo.tacticalPointAssignedTo;

            lastSquaredDistanceMeasured = squaredDistance;

            timeWhenLastSeen = Time.time;
            frameCountWhenLastSeen = Time.frameCount;
        }
    }
}
