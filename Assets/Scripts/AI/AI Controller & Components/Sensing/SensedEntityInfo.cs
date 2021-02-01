using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BenitosAI
{
    [System.Serializable]//For debug purposes
    // SensingInfo Component saves information about other entities it has seen in this container.
    public class SensedEntityInfo
    {
        #region Fields

        int hashCode; // used by sensig info to store in dictionary

        public GameEntity entity;
        public EntityTags entityTags;
        public EntitySensingInterface visInfo;
        public int entityTeamID;

        Vector3 lastSeenEntityPosition;
        public float lastDistanceMeasured;

        //Movement
        public bool hasMovement;
        Vector3 lastSeenVelocity;
        Vector3 lastSeenAngularVelocity;

        //Aim Positions
        Vector3 lastSeenAimPosition;
        Vector3 lastSeenCriticalAimPosition;

        //Information Freshness
        public float timeWhenLastSeen;
        public int frameCountWhenLastSeen;

        float timeDelayAfterWhichPositionIsntUpdated = 1.5f; //if we seen this entity more than x seconds ago, we wont have acess to the current position of the entity, just the last posiiton

        #endregion


        //Later on we need a reference to the skills here if we want to call get CriticalAim pos and similar from the visibility Info

        public SensedEntityInfo()
        {
            
        }

        public SensedEntityInfo(SensedEntityInfo infoToCopyFrom)
        {
            if(infoToCopyFrom != null)
            {
                CopyInfo(infoToCopyFrom);
            }
        }


        public void SetUpInfo(EntitySensingInterface visInfo, float distance)
        {
            this.visInfo = visInfo;
            timeWhenLastSeen = Time.time;
            frameCountWhenLastSeen = Time.frameCount;

            entity = visInfo.entityAssignedTo;
            entityTags = entity.entityTags;
            hashCode = entity.GetHashCode();
            lastSeenEntityPosition = visInfo.GetEntityPosition();
            entityTeamID = entity.teamID;

            lastDistanceMeasured = distance;

            //Set Movement Speeds
            if (visInfo.HasMovement())
            {
                hasMovement = true;
                lastSeenVelocity = visInfo.GetCurrentVelocity();
                lastSeenAngularVelocity = visInfo.GetCurrentAngularVelocity();
            }
            else
            {
                hasMovement = false;
            }

            //Set Aim Positions
            lastSeenAimPosition = visInfo.GetAimPosition();
            lastSeenCriticalAimPosition = visInfo.GetCriticalAimPosition();
        }

        public void CopyInfo(SensedEntityInfo infoToCopy)
        {
            this.visInfo = infoToCopy.visInfo;
            timeWhenLastSeen = infoToCopy.timeWhenLastSeen;
            frameCountWhenLastSeen = infoToCopy.frameCountWhenLastSeen;

            entity = infoToCopy.entity;
            entityTags = infoToCopy.entityTags;
            hashCode = infoToCopy.hashCode;
            lastSeenEntityPosition = infoToCopy.lastSeenEntityPosition;
            entityTeamID = infoToCopy.entityTeamID;

            lastDistanceMeasured = infoToCopy.lastDistanceMeasured;

            hasMovement = infoToCopy.hasMovement;
            lastSeenVelocity = infoToCopy.lastSeenVelocity;
            lastSeenAngularVelocity = infoToCopy.lastSeenAngularVelocity;

            //Set Aim Positions
            lastSeenAimPosition = infoToCopy.lastSeenAimPosition;
            lastSeenCriticalAimPosition = infoToCopy.lastSeenCriticalAimPosition;
        }

        public bool IsAlive()
        {
            return entity;
        }

        public Vector3 GetAimPosition()
        {
            if (entity != null) //If Alive
            {
                if (Time.time - timeWhenLastSeen < timeDelayAfterWhichPositionIsntUpdated)
                {
                    return visInfo.GetAimPosition();
                }
            }

            return lastSeenAimPosition;

        }

        public Vector3 GetCriticalAimPosition()
        {
            if (entity != null)
            {
                if (Time.time - timeWhenLastSeen < timeDelayAfterWhichPositionIsntUpdated)
                {
                    return visInfo.GetCriticalAimPosition();
                }
            }

            return lastSeenCriticalAimPosition;
        }

        public Vector3 GetCurrentVelocity()
        {
            if (entity != null)
            {
                if (Time.time - timeWhenLastSeen < timeDelayAfterWhichPositionIsntUpdated)
                {
                    return visInfo.GetCurrentVelocity();
                }
            }

            return lastSeenVelocity;

        }

        public Vector3 GetCurrentAngularVelocity()
        {
            if (entity != null)
            {
                if (Time.time - timeWhenLastSeen < timeDelayAfterWhichPositionIsntUpdated)
                {
                    return visInfo.GetCurrentAngularVelocity();
                }
            }

            return lastSeenAngularVelocity;

        }

        public Vector3 GetEntityPosition()
        {
            if (entity != null)
            {
                if (Time.time - timeWhenLastSeen < timeDelayAfterWhichPositionIsntUpdated)
                {
                    return visInfo.transform.position;
                }
            }

            return lastSeenEntityPosition;
        }

        /*static int SortByTime(SensedEntityInfo p1, SensedEntityInfo p2)
        {
            return p1.timeWhenLastSeen.CompareTo(p2.timeWhenLastSeen);
        }*/

        public override int GetHashCode()
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

            SensedEntityInfo entity = (SensedEntityInfo)obj;

            if (GetHashCode() != entity.GetHashCode())
                return false;

            return true;
        }
    }

}
