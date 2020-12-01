using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PointCastRaysContainer 
{
    //is use array flattening as described here: https://answers.unity.com/questions/1485842/serialize-custom-multidimensional-array-from-inspe.html

    //TacticalPoint.UsedRaycast[][][] raycastsUsedForGeneratingRating;
    //public RayDirectionGroup[] raysForCrouching = new RayDirectionGroup[8];
    //public RayDirectionGroup[] raysForStanding = new RayDirectionGroup[8];

    public TacticalPoint.UsedRaycast[] raycastsUsedForGeneratingRating;

    public int currentNumberOfRaycastsPerDirection;


    public void SetUpRays(int numberOfRaycastsPerDirection)
    {
        currentNumberOfRaycastsPerDirection = numberOfRaycastsPerDirection;
        raycastsUsedForGeneratingRating = new TacticalPoint.UsedRaycast[2 * 8 * numberOfRaycastsPerDirection];

        //raysForCrouching = new RayDirectionGroup[8];
        //raysForStanding = new RayDirectionGroup[8];

        /* for (int i = 0; i < 8; i++)
         {
             Debug.Log("raysForCrouching " + raysForCrouching[i].name);
             //Debug.Log("raysForCrouching[i].raycastsForDirection == null " + raysForCrouching[i].raycastsForDirection == null);
             //raysForCrouching[i].raycastsForDirection = new TacticalPoint.UsedRaycast[numberOfRaycastsPerDirection];
             //raysForStanding[i].raycastsForDirection = new TacticalPoint.UsedRaycast[numberOfRaycastsPerDirection];
             raysForCrouching[i].SetUp(numberOfRaycastsPerDirection);
             raysForStanding[i].SetUp(numberOfRaycastsPerDirection);
         }*/

        /*raycastsUsedForGeneratingRating = new TacticalPoint.UsedRaycast[2][][];
        for (int i = 0; i < 2; i++)
        {
            raycastsUsedForGeneratingRating[i] = new TacticalPoint.UsedRaycast[8][];
            for (int j = 0; j < 8; j++)
            {
                raycastsUsedForGeneratingRating[i][j] = new TacticalPoint.UsedRaycast[numberOfRaycastsPerDirection];
            }
        }*/
    }

    TacticalPoint.UsedRaycast GetRayAtIndex(int stance, int direction, int raycastNumber)
    {
        //Flat[x + WIDTH * (y + DEPTH * z)] = Original[x, y, z]

        return raycastsUsedForGeneratingRating[stance + 2 * (direction + 8 * raycastNumber)];
    }

    void SetRayAtIndex(int stance, int direction, int raycastNumber, TacticalPoint.UsedRaycast ray)
    {
        raycastsUsedForGeneratingRating[stance + 2 * (direction + 8 * raycastNumber)] = ray;
    }

    public TacticalPoint.UsedRaycast GetRay(int stance, int direction, int raycastNumber)
    {
        return GetRayAtIndex(stance,direction,raycastNumber);
        //return raycastsUsedForGeneratingRating[stance][direction][raycastNumber]; 
        /*if(stance == 0)
        {
            return raysForCrouching[direction].raycastsForDirection[raycastNumber];
        }
        else if(stance == 1)
        {
            return raysForStanding[direction].raycastsForDirection[raycastNumber];

        }

        Debug.Log("somethings wrong here!");
        return null;*/
    }

    public TacticalPoint.UsedRaycast[] GetAllRaysOfDirection(int stance, int direction)
    {
        TacticalPoint.UsedRaycast[] rays = new TacticalPoint.UsedRaycast[currentNumberOfRaycastsPerDirection];

        for (int i = 0; i < currentNumberOfRaycastsPerDirection; i++)
        {
            rays[i] = GetRay(stance, direction, i);
        }

        return rays;

        //return raycastsUsedForGeneratingRating[stance][direction];
        /* if (stance == 0)
         {
             return raysForCrouching[direction].raycastsForDirection;
         }
         else if (stance == 1)
         {
             return raysForStanding[direction].raycastsForDirection;

         }

         Debug.Log("somethings wrong here!");
         return null;*/
    }

    public void SetRay(int stance, int direction, int raycastNumber, TacticalPoint.UsedRaycast ray)
    {
        SetRayAtIndex(stance, direction, raycastNumber, ray);
        //raycastsUsedForGeneratingRating[stance][direction][raycastNumber] = ray;
        /*if (stance == 0)
        {
             raysForCrouching[direction].raycastsForDirection[raycastNumber] = ray;
        }
        else if (stance == 1)
        {
             raysForStanding[direction].raycastsForDirection[raycastNumber] = ray;

        }*/
    }
}

/*[System.Serializable]
public class RayDirectionGroup
{
    public string name = "name";
    public TacticalPoint.UsedRaycast[] raycastsForDirection;

    public void SetUp(int numberOfRaycastsPerDirection)
    {
        //raycastsForDirection = new TacticalPoint.UsedRaycast[numberOfRaycastsPerDirection];
    }
}*/
