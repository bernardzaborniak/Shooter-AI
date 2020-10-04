using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class RecoilInfo
{
    // Up Recoil
    public float recoilUpShootForce;
    public float maxRecoilUpSpeed;
    public float maxReduceRecoilUpAcceleration;
    public float maxReduceRecoilUpSpeed;
    public float maxRotationUp;

    // Side Recoil
    public float recoilSideShootForce;
    public float maxRecoilSideSpeed;
    public float maxReduceRecoilSideAcceleration;
    public float maxReduceRecoilSideSpeed;
    public float maxRotationSide;

    // Back Recoil
    public float recoilBackShootForce;
    public float maxRecoilBackSpeed;
    public float maxReduceRecoilBackAcceleration;
    public float maxReduceRecoilBackSpeed;
    public float maxPositionBack;


    public RecoilInfo()
    {

    }

    public void SetRecoilForces(float recoilUpShootForce, float recoilSideShootForce, float recoilBackShootForce)
    {
        this.recoilUpShootForce = recoilUpShootForce;
        this.recoilSideShootForce = recoilSideShootForce;
        this.recoilBackShootForce = recoilBackShootForce;
    }

    public void SetRecoilUpValues(float maxRecoilUpSpeed, float maxReduceRecoilUpAcceleration, float maxReduceRecoilUpSpeed, float maxRotationUp)
    {
        this.maxRecoilUpSpeed = maxRecoilUpSpeed;
        this.maxReduceRecoilUpAcceleration = maxReduceRecoilUpAcceleration;
        this.maxReduceRecoilUpSpeed = maxReduceRecoilUpSpeed;
        this.maxRotationUp = maxRotationUp;
    }

    public void SetRecoilSideValues(float maxRecoilSideSpeed, float maxReduceRecoilSideAcceleration, float maxReduceRecoilSideSpeed, float maxRotationSide)
    {
        this.maxRecoilSideSpeed = maxRecoilSideSpeed;
        this.maxReduceRecoilSideAcceleration = maxReduceRecoilSideAcceleration;
        this.maxReduceRecoilSideSpeed = maxReduceRecoilSideSpeed;
        this.maxRotationSide = maxRotationSide;
    }

    public void SetRecoilBackValues(float maxRecoilBackSpeed, float maxReduceRecoilBackAcceleration, float maxReduceRecoilBackSpeed, float maxRotationBack)
    {
        this.maxRecoilBackSpeed = maxRecoilBackSpeed;
        this.maxReduceRecoilBackAcceleration = maxReduceRecoilBackAcceleration;
        this.maxReduceRecoilBackSpeed = maxReduceRecoilBackSpeed;
        this.maxPositionBack = maxRotationBack;
    }


}

[System.Serializable]
public class RecoilStatsGun
{
    [Header("-----------------Up Recoil---------------------")]
    public float recoilUpShootForce;

    [Header("One Handed")]
    public float maxRecoilUpSpeedOneHanded;
    public float maxReduceRecoilUpAccelerationOneHanded;
    public float maxReduceRecoilUpSpeedOneHanded;
    public float maxRotationUpOneHanded;

    [Header("Two Handed")]
    public float maxRecoilUpSpeedTwoHanded;
    public float maxReduceRecoilUpAccelerationTwoHanded;
    public float maxReduceRecoilUpSpeedTwoHanded;
    public float maxRotationUpTwoHanded;


    [Header("-----------------Side Recoil---------------------")]
    public float recoilSideShootForce;

    [Header("One Handed")]
    public float maxRecoilSideSpeedOneHanded;
    public float maxReduceRecoilSideAccelerationOneHanded;
    public float maxReduceRecoilSideSpeedOneHanded;
    public float maxRotationSideOneHanded;

    [Header("Two Handed")]
    public float maxRecoilSideSpeedTwoHanded;
    public float maxReduceRecoilSideAccelerationTwoHanded;
    public float maxReduceRecoilSideSpeedTwoHanded;
    public float maxRotationSideTwoHanded;


    [Header("-----------------Back Recoil---------------------")]
    public float recoilBackShootForce;

    [Header("One Handed")]
    public float maxRecoilBackSpeedOneHanded;
    public float maxReduceRecoilBackAccelerationOneHanded;
    public float maxReduceRecoilBackSpeedOneHanded;
    public float maxPositionBackOneHanded;

    [Header("Two Handed")]
    public float maxRecoilBackSpeedTwoHanded;
    public float maxReduceRecoilBackAccelerationTwoHanded;
    public float maxReduceRecoilBackSpeedTwoHanded;
    public float maxPositionBackTwoHanded;

}
