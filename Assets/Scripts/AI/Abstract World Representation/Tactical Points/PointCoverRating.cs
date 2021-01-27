using FMOD;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PointCoverRating
{
    //represents the rating system for a position

    // Those values need to be either colored or Serialized - otherwise they loose their values on reload or enter/exit playmode

    //one polar array is used to determine the line of sight distance, important for searching a position to shoot & to walk close to cover
    public float[] standingDistanceRating;
    public float[] standingQualityRating;

    //the other polar array tells us the rough cover quality? The cover is determined by the number of raycast stopped/ how many get through?
    public float[] crouchedDistanceRating;
    public float[] crouchedQualityRating;

    public PointCoverRating()
    {
        standingDistanceRating = new float[8];
        standingQualityRating = new float[8];
        crouchedDistanceRating = new float[8];
        crouchedQualityRating = new float[8];
    }

    

    public float DetermineQualityOfDefensiveCover(Vector3 directionTowardsThreat, float distanceToThreat, bool crouching)
    {
        //if(crouching)

        //first determin which index of ratin to use, global transform.forward points towards index 0 -forward points wowards index 4 - indexes go around like a clock .

        //cut the y direction
        directionTowardsThreat.y = 0;



        return 1;
    }

    public float DetermineQualityOfAggressiveCover(Vector3 threatPosition, float distanceToThreat, bool crouching)
    {
        return 1;
    }

    public float DetermineQualityOfLineOfSight(Vector3 directionFromPositionToTarget)
    {
        return 1;
    }
}

