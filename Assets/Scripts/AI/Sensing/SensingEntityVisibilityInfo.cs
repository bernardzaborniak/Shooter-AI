using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Sensing Component saves information about other entities it has seen in this container
public class SensingEntityVisibilityInfo 
{
    public GameEntity entity;
    public VisibilityInfo visInfo;
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


    public SensingEntityVisibilityInfo()
    {

    }

    public void SetUpInfo(VisibilityInfo visInfo)//, IMoveable moveable)
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

    public Vector3 GetAimPosition()
    {
        if(Time.time- timeWhenLastSeen< timeDelayAfterWhichPositionIsntUpdated)
        {
            return entity.GetAimPosition();
        }
        else
        {
            return lastSeenAimPosition;
        }
    }

    public Vector3 GetCriticalAimPosition()
    {
        if (Time.time - timeWhenLastSeen < timeDelayAfterWhichPositionIsntUpdated)
        {
            return entity.GetCriticalAimPosition();
        }
        else
        {
            return lastSeenCriticalAimPosition;
        }
    }

    public Vector3 GetCurrentVelocity()
    {
        if (Time.time - timeWhenLastSeen < timeDelayAfterWhichPositionIsntUpdated)
        {
            return visInfo.GetCurrentVelocity();
        }
        else
        {
            return lastSeenVelocity;
        }
    }

    public Vector3 GetCurrentAngularVelocity()
    {
        if (Time.time - timeWhenLastSeen < timeDelayAfterWhichPositionIsntUpdated)
        {
            return visInfo.GetCurrentAngularVelocity();
        }
        else
        {
            return lastSeenAngularVelocity;
        }
    }

    public Vector3 GetEntityPosition()
    {
        if (Time.time - timeWhenLastSeen < timeDelayAfterWhichPositionIsntUpdated)
        {
            return entity.transform.position;
        }
        else
        {
            return lastSeenEntityPosition;
        }
    }

}
