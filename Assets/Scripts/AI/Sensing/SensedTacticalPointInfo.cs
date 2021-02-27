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
        public readonly TacticalPoint tacticalPoint;
       // public TacticalPointSensingInterface visInfo;

        public float lastDistanceMeasured;

        public float timeWhenLastSeen;
        public int frameCountWhenLastSeen;

        public SensedTacticalPointInfo(TacticalPoint tacticalPoint, float distance)
        {
            this.tacticalPoint = tacticalPoint;
            //hashCode = tacticalPoint.GetHashCode();
            UpdateInfo(distance);
        }

        /*public SensedTacticalPointInfo(SensedTacticalPointInfo infoToCopyFrom)
        {
            if (infoToCopyFrom != null)
            {
                CopyInfo(infoToCopyFrom);
            }
        }*/

        //public void SetUpInfo(TacticalPointSensingInterface visInfo, float distance)
        /*public void SetUpInfo(TacticalPoint tacticalPoint, float distance)
        {
            //this.visInfo = visInfo;
            //tacticalPoint = visInfo.tacticalPointAssignedTo;
            this.tacticalPoint = tacticalPoint;
            hashCode = tacticalPoint.GetHashCode();
            lastDistanceMeasured = distance;

            timeWhenLastSeen = Time.time;
            frameCountWhenLastSeen = Time.frameCount;
        }*/

        public void UpdateInfo(float distance)
        {
            //this.visInfo = visInfo;
            //tacticalPoint = visInfo.tacticalPointAssignedTo;
            //this.tacticalPoint = tacticalPoint;
            //hashCode = tacticalPoint.GetHashCode();
            lastDistanceMeasured = distance;

            timeWhenLastSeen = Time.time;
            frameCountWhenLastSeen = Time.frameCount;
        }

        /*public void CopyInfo(SensedTacticalPointInfo infoToCopyFrom)
        {
            tacticalPoint = infoToCopyFrom.tacticalPoint;
            hashCode = infoToCopyFrom.hashCode;
            lastDistanceMeasured = infoToCopyFrom.lastDistanceMeasured;

            timeWhenLastSeen = infoToCopyFrom.timeWhenLastSeen;
            frameCountWhenLastSeen = infoToCopyFrom.frameCountWhenLastSeen;
        }*/

        public bool IsValid()
        {
            //can be destryoed
            return tacticalPoint;
        }

        /*public override int GetHashCode()
        {
            return hashCode;
        }

        //when 2 objects have the same hashcode, equals is checked on them before adding them to a hash set?
        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;

            if (GetType() != obj.GetType())
                return false;

            SensedTacticalPointInfo point = (SensedTacticalPointInfo)obj;

            if (GetHashCode() != point.GetHashCode())
                return false;

            return true;
        }*/
    }
}
