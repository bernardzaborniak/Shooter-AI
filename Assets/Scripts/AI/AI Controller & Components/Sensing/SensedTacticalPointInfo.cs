using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BenitosAI
{
    // SensingInfo Component saves information about tactical points it has seen in this container.
    public class SensedTacticalPointInfo
    {
        int hashCode; // used by sensig info to store in dictionary
        public TacticalPoint tacticalPoint;
        public TacticalPointVisibilityInfo visInfo;

        public float lastDistanceMeasured;

        public float timeWhenLastSeen;
        public int frameCountWhenLastSeen;

        public SensedTacticalPointInfo()
        {
           
        }

        public void SetUpInfo(TacticalPointVisibilityInfo visInfo, float squaredDistance)
        {
            this.visInfo = visInfo;
            tacticalPoint = visInfo.tacticalPointAssignedTo;
            hashCode = tacticalPoint.GetHashCode();
            lastDistanceMeasured = squaredDistance;

            timeWhenLastSeen = Time.time;
            frameCountWhenLastSeen = Time.frameCount;
        }

        public bool IsValid()
        {
            //can be destryoed
            return tacticalPoint;
        }

        public override int GetHashCode()
        {
            return hashCode;
        }
    }
}
