using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Sensing Component saves information about other entities it has seen in this container
public class SensingEntityVisibilityInfo 
{
    public GameEntity entity;
    public Vector3 entityPosition;
    public int entityTeamID;

    //Movement
    public bool hasMovement;
    public Vector3 velocity;
    public Vector3 angularVelocity;

    //Aim Positions
    public Vector3 aimPosition;
    public Vector3 criticalAimPosition;



    public float timeWhenLastSeen;


    public SensingEntityVisibilityInfo()
    {

    }

    public void SetUpInfo(VisibilityInfo visInfo)//, IMoveable moveable)
    {
        timeWhenLastSeen = Time.time;

        entity = visInfo.entityAssignedTo;
        entityTeamID = entity.teamID;

        // Set Movement Speeds.
        if (visInfo.HasMovement())
        {
            hasMovement = true;
            velocity = visInfo.GetCurrentVelocity();
            angularVelocity = visInfo.GetCurrentAngularVelocity();
        }
        else
        {
            hasMovement = false;
        }

        //Set Aim Positions.
        aimPosition = entity.GetAimPosition();
        criticalAimPosition = entity.GetCriticalAimPosition();



    }
  
}
