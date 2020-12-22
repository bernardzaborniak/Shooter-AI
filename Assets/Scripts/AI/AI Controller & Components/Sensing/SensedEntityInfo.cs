﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BenitosAI
{
    // SensingInfo Component saves information about other entities it has seen in this container.
    public class SensedEntityInfo
    {
        #region Fields

        public GameEntity entity;
        public EntityVisibilityInfo visInfo;
        public int entityTeamID;

        Vector3 lastSeenEntityPosition;
        public float lastSquaredDistanceMeasured;

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

        public void SetUpInfo(EntityVisibilityInfo visInfo, float squaredDistance)
        {
            this.visInfo = visInfo;
            timeWhenLastSeen = Time.time;
            frameCountWhenLastSeen = Time.frameCount;

            entity = visInfo.entityAssignedTo;
            lastSeenEntityPosition = visInfo.GetEntityPosition();
            entityTeamID = entity.teamID;

            lastSquaredDistanceMeasured = squaredDistance;

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

    }

}
