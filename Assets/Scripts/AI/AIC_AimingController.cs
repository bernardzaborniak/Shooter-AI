using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIC_AimingController : AIComponent
{
    [Header("For Calculating Direction")]
    [Tooltip("Reference for aiming, propably spine 3")]
    public Transform aimingReference;

    [Header("Skillbased Error")]
    public float maxAimError;

    public float minChangeErrorInterval;
    public float maxChangeErrorInterval;
    float nextChangeErrorTime;

    Quaternion currentAimingError;

    [Header("Hands Shaking")]
    public bool handsShake;
    public float handsShakingIntensity;

    enum DirectionToAimCalculationMode
    {
        StraightGun,
        GunWithArc,
        Grenade
    }

    DirectionToAimCalculationMode directionToAimCalculationMode;

    public override void SetUpComponent(GameEntity entity)
    {
        base.SetUpComponent(entity);
    }

    public override void UpdateComponent()
    {
        if (Time.time > nextChangeErrorTime)
        {
            currentAimingError = Quaternion.Euler(Random.Range(-maxAimError, maxAimError), Random.Range(-maxAimError, maxAimError), Random.Range(-maxAimError, maxAimError));

            nextChangeErrorTime = Time.time + Random.Range(minChangeErrorInterval, maxChangeErrorInterval);
        }
    }

    public Vector3 GetDirectionToAimAtTarget(Vector3 target, bool launchProjectileInArc = false, float projectileLaunchVelocity = 0, bool directShot = false, bool addAimErrorAndhandShakeToDirection = false) // , Vector3 targetMovementVelocity
    {
        Vector3 aimDirection = Vector3.zero;

        if (!launchProjectileInArc)
        {
            aimDirection = target - aimingReference.position;
        }
        else
        {
            Vector3 directionToTarget = target - aimingReference.position;
            Vector3 directionToTargetNoY = new Vector3(directionToTarget.x, 0, directionToTarget.z);

            float launchAngle = Utility.CalculateProjectileLaunchAngle(projectileLaunchVelocity, directionToTargetNoY.magnitude, directionToTarget.y, directShot);
            aimDirection = Quaternion.AngleAxis(-launchAngle, transform.right) * directionToTargetNoY;
        }

        if (addAimErrorAndhandShakeToDirection)
        {
            return AddAimErrorAndHandShakeToAimDirection(aimDirection);
        }
        else
        {
            return aimDirection;
        }
        
    }

    public Vector3 AddAimErrorAndHandShakeToAimDirection(Vector3 aimDirection)
    {
        if (handsShake)
        {
            Quaternion handsShakingRotation = Quaternion.Euler(Random.Range(-handsShakingIntensity, handsShakingIntensity), Random.Range(-handsShakingIntensity, handsShakingIntensity), Random.Range(-handsShakingIntensity, handsShakingIntensity));
            return handsShakingRotation * currentAimingError * aimDirection;
        }
        else
        {
            return currentAimingError * aimDirection;
        }
    }


    public float DetermineThrowingObjectVelocity(Item throwingObject, float distanceToTarget) //we can throw it low or high in most cases, both have different velocities?
    {
        if(throwingObject is Grenade)
        {
            float velocityAt10m = (throwingObject as Grenade).throwVelocityAt10mDistance;

            return  ((3f/5f) * velocityAt10m) + (distanceToTarget / 10f) * ((2f/5f)*velocityAt10m);
        }

        return 0;
    }
}
