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

    //public float minChangeErrorInterval;
    //public float maxChangeErrorInterval;
    //public lfoat nextChangeErrorTime


    public override void SetUpComponent(GameEntity entity)
    {
        base.SetUpComponent(entity);
    }

    public override void UpdateComponent()
    {

    }

    public Vector3 GetDirectionToAimAtTarget(GameEntity target)
    {
        Vector3 aimingVector = target.GetAimPosition() - aimingReference.position;

        Quaternion errorRotater = Quaternion.Euler(Random.Range(-maxAimError, maxAimError), Random.Range(-maxAimError, maxAimError), Random.Range(-maxAimError, maxAimError));

        aimingVector =  errorRotater * aimingVector;

        return aimingVector;
    }
}
