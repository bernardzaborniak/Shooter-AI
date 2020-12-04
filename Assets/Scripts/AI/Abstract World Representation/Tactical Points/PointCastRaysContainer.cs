using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//custom class containing the collection of all rays cast
[System.Serializable]
public class PointCastRaysContainer 
{
    // I use array flattening as described here: https://answers.unity.com/questions/1485842/serialize-custom-multidimensional-array-from-inspe.html
    // 3 Dimensional Array ->         //Flat[x + WIDTH * (y + DEPTH * z)] = Original[x, y, z]


    // Those values need to be either colored or Serialized - otherwise they loose their values on reload or enter/exit playmode
    [SerializeField] RaycastUsedToGenerateCoverRating[] raycastsUsedForGeneratingRating;
    [SerializeField] int currentNumberOfRaycastsPerDirection;
    
    public PointCastRaysContainer()
    {

    }

    public void SetUpRays(int numberOfRaycastsPerDirection)
    {
        currentNumberOfRaycastsPerDirection = numberOfRaycastsPerDirection;
        raycastsUsedForGeneratingRating = new RaycastUsedToGenerateCoverRating[2 * 8 * numberOfRaycastsPerDirection];
    }

    public RaycastUsedToGenerateCoverRating GetRay(int stance, int direction, int raycastNumber)
    {
        //Flat[x + WIDTH * (y + DEPTH * z)] = Original[x, y, z]
        return raycastsUsedForGeneratingRating[stance + 2 * (direction + 8 * raycastNumber)];
    }

    public void SetRay(int stance, int direction, int raycastNumber, RaycastUsedToGenerateCoverRating ray)
    {
        //Flat[x + WIDTH * (y + DEPTH * z)] = Original[x, y, z]
        raycastsUsedForGeneratingRating[stance + 2 * (direction + 8 * raycastNumber)] = ray;
    }


    public RaycastUsedToGenerateCoverRating[] GetAllRaysOfDirection(int stance, int direction)
    {
        RaycastUsedToGenerateCoverRating[] rays = new RaycastUsedToGenerateCoverRating[currentNumberOfRaycastsPerDirection];

        for (int i = 0; i < currentNumberOfRaycastsPerDirection; i++)
        {
            rays[i] = GetRay(stance, direction, i);
        }

        return rays;
    }
}

