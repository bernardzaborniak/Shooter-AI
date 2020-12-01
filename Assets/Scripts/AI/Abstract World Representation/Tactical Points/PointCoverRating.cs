using FMOD;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]   //TODO do the non unity serialization as shown here:  https://docs.unity3d.com/Manual/script-Serialization-Custom.html
//[Serializable]
public class PointCoverRating//: ISerializationCallbackReceiver
{
    //represents the rating system for a position

    //one polar array is used to determine the line of sight distance, important for searching a position to shoot & to walk close to cover
    public float[] standingDistanceRating;
    public float[] standingQualityRating;

    //the other polar array tells us the rough cover quality? The cover is determined by the number of raycast stopped/ how many get through?
    public float[] crouchedDistanceRating;
    public float[] crouchedQualityRating;

    public void OnBeforeSerialize()
    {

    }

    public void OnAfterDeserialize()
    {

    }

    public void SetUp()
    {
        standingDistanceRating = new float[8];
        standingQualityRating = new float[8];
        crouchedDistanceRating = new float[8];
        crouchedQualityRating = new float[8];
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

[Serializable]
public struct SerializablePointCoverRating
{

}
