using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TacticalPointType
{
    OpenFieldPoint,
    CoverPoint,
    CoverShootPoint
    
}

public class TacticalPoint : MonoBehaviour
{
    public TacticalPointType tacticalPointType;
    public GameEntity usingEntity;
    [Space(5)]
    public PointCoverRating coverRating;
   
    [Tooltip("only used if  type is CoverShootPoint")]
    [ShowWhen("tacticalPointType", TacticalPointType.CoverPoint)]
    public TacticalPoint[] PeekPositions; //or ShotPositions


    [Space(5)]
    //for now we only use this to test the character controller
    public int stanceType; //0 is standing, 1 is crouching




    public Vector3 GetPostPosition()
    {
        return transform.position;
    }


    //only used if type is CoverShootPoint
    public Vector3 GetPeekPosition()
    {
        return transform.position;
    }

    // is there room on this point for another soldier?
    public bool IsPointFull()
    {
        if (usingEntity)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public void OnEntityEntersPoint(GameEntity entity)
    {
        usingEntity = entity;
    }

    public void OnEntityExitsPoint(GameEntity entity)
    {
        if(usingEntity == entity)
        {
            usingEntity = null;
        }
    }
}
