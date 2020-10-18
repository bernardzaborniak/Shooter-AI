using FMOD;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class PositionRating
{
    //represents the rating system for a position

    //one polar array is used to determine the line of sight distance, important for searching a position to shoot & to walk close to cover
    public float[] polarDistanceToCoverStanding = new float[8];
    public float[] polarDistanceToCoverCrouching = new float[8];
    public float[] polarDistanceToCoverUp = new float[8];

    //the other polar array tells us the rough cover quality? The cover is determined by the number of raycast stopped/ how many get through?
    public float[] polarQualityOfCoverStanding = new float[8];
    public float[] polarQualityOfCoverCrouching = new float[8];
    public float[] polarQualityOfCoverUp = new float[8];





    void Start()
    {
        
    }

    void Update()
    {
        
    }

    public float DetermineQualityOfCover(Vector3 directionFromPositionToThreat, bool crouching)
    {
        return 1;
    }

    public float DetermineQualityOfLineOfSight(Vector3 directionFromPositionToTarget)
    {
        return 1;
    }
}
