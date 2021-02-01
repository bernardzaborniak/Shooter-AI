using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BenitosAI
{
    // SensingInfo Component saves information about tactical points it has seen in this container.
    [System.Serializable]//For debug purposes
    public class SensedTacticalPointInfo
    {
        int hashCode; // used by sensig info to store in dictionary
        public TacticalPoint tacticalPoint;
        public TacticalPointSensingInterface visInfo;

        public float lastDistanceMeasured;

        public float timeWhenLastSeen;
        public int frameCountWhenLastSeen;

        public SensedTacticalPointInfo()
        {
           
        }

        public void SetUpInfo(TacticalPointSensingInterface visInfo, float distance)
        {
            this.visInfo = visInfo;
            tacticalPoint = visInfo.tacticalPointAssignedTo;
            hashCode = tacticalPoint.GetHashCode();
            lastDistanceMeasured = distance;

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
