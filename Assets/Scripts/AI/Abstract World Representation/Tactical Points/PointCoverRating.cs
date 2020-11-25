using FMOD;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PointCoverRating
{
    //represents the rating system for a position

    //one polar array is used to determine the line of sight distance, important for searching a position to shoot & to walk close to cover
    public float[] standingDistanceRating;
    public float[] standingQualityRating;

    //the other polar array tells us the rough cover quality? The cover is determined by the number of raycast stopped/ how many get through?
    public float[] crouchedDistanceRating;
    public float[] crouchedQualityRating;

    public float DetermineQualityOfCover(Vector3 directionFromPositionToThreat, bool crouching)
    {
        return 1;
    }

    public float DetermineQualityOfLineOfSight(Vector3 directionFromPositionToTarget)
    {
        return 1;
    }
}
