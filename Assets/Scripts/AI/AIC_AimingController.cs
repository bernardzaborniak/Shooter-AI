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

    public Vector3 GetDirectionToAimAtTarget(GameEntity target, Item currentlySelectedItem) // , Vector3 targetMovementVelocity
    {
        Vector3 aimDirection = Vector3.zero;
       
        bool projectileShotAtAnArc = false;
        Gun equippedGun = null;

        if (currentlySelectedItem)
        {
            if(currentlySelectedItem is Gun)
            {
                equippedGun = (currentlySelectedItem as Gun);
                if (equippedGun.aimWithAngledShotCalculation)
                {
                    projectileShotAtAnArc = true;
                }
            }
        }

        if (projectileShotAtAnArc)
        {
            Vector3 aimDirectionNoY = target.GetAimPosition() - aimingReference.position;
            aimDirectionNoY.y = 0;
            float launchAngle = Utility.CalculateProjectileLaunchAngle(equippedGun.projectileLaunchVelocity, aimingReference.position, target.GetAimPosition());
            aimDirection = Quaternion.AngleAxis(-launchAngle, aimingReference.right) * aimDirectionNoY;
        }
        else
        {
            aimDirection = target.GetAimPosition() - aimingReference.position;
        }
        
        
        if (Time.time > nextChangeErrorTime)
        {
            ChangeAimingError();

            nextChangeErrorTime = Time.time + Random.Range(minChangeErrorInterval, maxChangeErrorInterval);
        }

        if (handsShake)
        {
            Quaternion handsShakingRotation = Quaternion.Euler(Random.Range(-handsShakingIntensity, handsShakingIntensity), Random.Range(-handsShakingIntensity, handsShakingIntensity), Random.Range(-handsShakingIntensity, handsShakingIntensity));
            return handsShakingRotation * currentAimingError * aimDirection;
        }
        else
        {
            return currentAimingError * aimDirection;
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

    /*public void UpdateCurrentSelectedItem(Item currentlySelectedItem)
    {
        this.currentlySelectedItem = currentlySelectedItem;
    }*/
}
