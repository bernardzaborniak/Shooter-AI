using System.Collections;
using System.Collections.Generic;
using UnityEngine;



// Interface between Sensing and the visibility probe system and the attached entity visibility values - results in the info about seeing another entity by the sensing component
public class EntitySensingInterface : MonoBehaviour
{
    public GameEntity entityAssignedTo;

    [Tooltip("If the entity moves, it has a script with an IMoveable interface - we cant assign an interface via inspector, so assigne the gameobject with the interface here")]
    public GameObject objectWithIMoveableScriptAttached;
    IMoveable moveable;
    protected bool hasMovement;

    [Header("For Aiming of Enemies")]
    [Tooltip("collection of positions to aim at")]
    public Transform aimPosition;
    [Tooltip("collection of critical positions to aim at - like the head or some weakpoints")]
    public Transform criticalAimPosition;
    public float width;

    protected void Start()
    {
        SetUp();
    }

    protected virtual void SetUp()
    {
        if (objectWithIMoveableScriptAttached)
        {
            moveable = objectWithIMoveableScriptAttached.GetComponent<IMoveable>();
            hasMovement = true;
        }
        else
        {
            hasMovement = false;
        }
    }

    public void IsVisible()
    {
        //here some calculations must occur if this object will be visible
    }

    //Gets the posiiton based on visibility skill - Expand later
    public Vector3 GetEntityPosition()
    {
        return entityAssignedTo.transform.position;
    }

    public Vector3 GetAimPosition()
    {
        return aimPosition.position;
    }

    public Vector3 GetCriticalAimPosition()
    {
        return criticalAimPosition.position;
    }


    public bool HasMovement()
    {
        return hasMovement;
    }

    public Vector3 GetCurrentVelocity()
    {
        return moveable.GetCurrentVelocity();
    }

    public Vector3 GetCurrentAngularVelocity()
    {
        return moveable.GetCurrentAngularVelocity();
    }

    public int GetTeamID()
    {
        return entityAssignedTo.teamID;
    }

   /* public virtual EntityActionBeingExecuted[] GetActionsBeingExecuted()
    {
        return null;
    }*/


}
