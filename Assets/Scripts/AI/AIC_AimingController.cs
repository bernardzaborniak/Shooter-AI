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

    public override void SetUpComponent(GameEntity entity)
    {
        base.SetUpComponent(entity);
    }

    public override void UpdateComponent()
    {

    }

    public Vector3 GetDirectionToAimAtTarget(GameEntity target)
    {
        if (Time.time > nextChangeErrorTime)
        {
            ChangeAimingError();

            nextChangeErrorTime = Time.time + Random.Range(minChangeErrorInterval, maxChangeErrorInterval);
        }

        if (handsShake)
        {
            Quaternion handsShakingRotation = Quaternion.Euler(Random.Range(-handsShakingIntensity, handsShakingIntensity), Random.Range(-handsShakingIntensity, handsShakingIntensity), Random.Range(-handsShakingIntensity, handsShakingIntensity));
            return handsShakingRotation * currentAimingError * (target.GetAimPosition() - aimingReference.position);
        }
        else
        {
            return currentAimingError * (target.GetAimPosition() - aimingReference.position);
        }



        //Vector3 aimingVector = currentAimingError * (target.GetAimPosition() - aimingReference.position);
        

       // Quaternion errorRotater = Quaternion.Euler(Random.Range(-maxAimError, maxAimError), Random.Range(-maxAimError, maxAimError), Random.Range(-maxAimError, maxAimError));

        //aimingVector = currentAimingError * aimingVector;

        //return aimingVector;
    }

    void ChangeAimingError()
    {
        currentAimingError = Quaternion.Euler(Random.Range(-maxAimError, maxAimError), Random.Range(-maxAimError, maxAimError), Random.Range(-maxAimError, maxAimError));
    }
}
