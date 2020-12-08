using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Sensing Component saves information about other entities it has seen in this container
public class AIC_S_EntityVisibilityInfo 
{
    public GameEntity entity;
    public EntityVisibilityInfo visInfo;
    public int entityTeamID;


    Vector3 lastSeenEntityPosition;

    //Movement
    public bool hasMovement;
    Vector3 lastSeenVelocity;
    Vector3 lastSeenAngularVelocity;

    //Aim Positions
    Vector3 lastSeenAimPosition;
    Vector3 lastSeenCriticalAimPosition;

    public float timeWhenLastSeen;
    float timeDelayAfterWhichPositionIsntUpdated = 1.5f; //if we seen this entity more than x seconds ago, we wont have acess to the current position of the entity, just the last posiiton

    public float lastSquaredDistanceMeasured;



    public AIC_S_EntityVisibilityInfo(EntityVisibilityInfo visInfo)
    {
        this.visInfo = visInfo;
        timeWhenLastSeen = Time.time;

        entity = visInfo.entityAssignedTo;
        lastSeenEntityPosition = entity.transform.position;
        entityTeamID = entity.teamID;

        // Set Movement Speeds.
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

        //Set Aim Positions.
        lastSeenAimPosition = entity.GetAimPosition();
        lastSeenCriticalAimPosition = entity.GetCriticalAimPosition();
    }

    /*public void SetUpInfo(EntityVisibilityInfo visInfo)//, IMoveable moveable)
    {
        this.visInfo = visInfo;
        timeWhenLastSeen = Time.time;

        entity = visInfo.entityAssignedTo;
        lastSeenEntityPosition = entity.transform.position;
        entityTeamID = entity.teamID;

        // Set Movement Speeds.
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

        //Set Aim Positions.
        lastSeenAimPosition = entity.GetAimPosition();
        lastSeenCriticalAimPosition = entity.GetCriticalAimPosition();



    }*/

   /* public bool IsValid()
    {
        return entity;
    }*/

    public Vector3 GetAimPosition()
    {
        if (entity != null)
        {
            if (Time.time - timeWhenLastSeen < timeDelayAfterWhichPositionIsntUpdated)
            {
                return entity.GetAimPosition();
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
                return entity.GetCriticalAimPosition();
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
                return entity.transform.position;
            }
        }
        
        return lastSeenEntityPosition;
    }

}
