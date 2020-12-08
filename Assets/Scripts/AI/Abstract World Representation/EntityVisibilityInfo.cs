using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//communicates with sensing - results in the info about seeing another entity by the sensing component
public class EntityVisibilityInfo : MonoBehaviour
{
    public GameEntity entityAssignedTo;

    [Tooltip("If the entity moves, it has a script with an IMoveable interface - we cant assign an interface via inspector, so assigne the gameobject with the interface here")]
    public GameObject objectWithIMoveableScriptAttached;
    IMoveable moveable;
    bool hasMovement;

    void Start()
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

    public bool HasMovement()
    {
        return hasMovement;
    }

    public Vector3  GetCurrentVelocity()
    {
        return moveable.GetCurrentVelocity();
    }

    public Vector3 GetCurrentAngularVelocity()
    {
        return moveable.GetCurrentAngularVelocity();
    }

    /*public int GetTeamID()
    {
        return entityAssignedTo.teamID;
    }*/


}
