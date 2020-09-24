using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIC_AimingController : AIComponent
{
    [Tooltip("Reference for aiming, propably spine 3")]
    public Transform aimingReference;

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
        return aimingVector;
    }
}
