using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BenitosAI
{
    [System.Serializable]//For debug purposes
    // Saves the information an ai has about other entities inside this container - the values are constantly updated, if you reference this object, its values can change, but the assigned entity never changes
    public class SensedEntityInfo
    {
        #region Fields

        //int hashCode; // used by sensig info to store in dictionary

        public readonly GameEntity entity;
        public readonly EntityTags entityTags;
        public readonly EntitySensingInterface sensInterface;
        public readonly int entityTeamID;

        Vector3 lastSeenEntityPosition;
        public float lastDistanceMeasured;

        //Movement
        //public readonly bool hasMovement;
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

        public SensedEntityInfo(EntitySensingInterface sensInterface, float distance)
        {
            this.sensInterface = sensInterface;
            entity = sensInterface.entityAssignedTo;
            entityTags = entity.entityTags;
            entityTeamID = entity.teamID;

            UpdateInfo(distance);
        }

        /*public SensedEntityInfo(SensedEntityInfo infoToCopyFrom)
        {
            if(infoToCopyFrom != null)
            {
                CopyInfo(infoToCopyFrom);
            }
        }*/


        public void UpdateInfo( float distance)
        {
            timeWhenLastSeen = Time.time;
            frameCountWhenLastSeen = Time.frameCount;

            //hashCode = entity.GetHashCode();
            lastSeenEntityPosition = sensInterface.GetEntityPosition();
            lastDistanceMeasured = distance;

            //Set Movement Speeds
            if (HasMovement())
            {
                lastSeenVelocity = sensInterface.GetCurrentVelocity();
                lastSeenAngularVelocity = sensInterface.GetCurrentAngularVelocity();
            }
            else
            {
            }

            //Set Aim Positions
            lastSeenAimPosition = sensInterface.GetAimPosition();
            lastSeenCriticalAimPosition = sensInterface.GetCriticalAimPosition();
        }


        public bool HasMovement()
        {
            return sensInterface.HasMovement();
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
                    return sensInterface.GetAimPosition();
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
                    return sensInterface.GetCriticalAimPosition();
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
                    return sensInterface.GetCurrentVelocity();
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
                    return sensInterface.GetCurrentAngularVelocity();
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
                    return sensInterface.transform.position;
                }
            }

            return lastSeenEntityPosition;
        }

        /*static int SortByTime(SensedEntityInfo p1, SensedEntityInfo p2)
        {
            return p1.timeWhenLastSeen.CompareTo(p2.timeWhenLastSeen);
        }*/

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

            SensedEntityInfo entity = (SensedEntityInfo)obj;

            if (GetHashCode() != entity.GetHashCode())
                return false;

            return true;
        }*/
    }

}
