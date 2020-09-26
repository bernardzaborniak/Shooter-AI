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

    }

    public Vector3 GetDirectionToAimAtTarget(Vector3 target, Item currentlySelectedItem) // , Vector3 targetMovementVelocity
    {
        Vector3 aimDirection = Vector3.zero;
       
        //bool projectileShotAtAnArc = false;
        Gun equippedGun = null;
        Grenade equippedGrenade = null;

        #region Determin directionToAimCalculationMode 

        directionToAimCalculationMode = DirectionToAimCalculationMode.StraightGun;

        if (currentlySelectedItem)
        {
            if(currentlySelectedItem is Gun)
            {
                equippedGun = (currentlySelectedItem as Gun);
                if (equippedGun.aimWithAngledShotCalculation)
                {
                    directionToAimCalculationMode = DirectionToAimCalculationMode.GunWithArc;
                    //projectileShotAtAnArc = true;
                }
            }
            else if(currentlySelectedItem is Grenade)
            {
                equippedGrenade = currentlySelectedItem as Grenade;
                directionToAimCalculationMode = DirectionToAimCalculationMode.Grenade;
                //projectileShotAtAnArc = true;
            }
        }

        #endregion


        #region Calculate Aim Direction
        if (directionToAimCalculationMode == DirectionToAimCalculationMode.StraightGun)
        {
            aimDirection = target - aimingReference.position;
        }
        else if(directionToAimCalculationMode == DirectionToAimCalculationMode.GunWithArc)
        {
            Vector3 aimDirectionNoY = target - aimingReference.position;
            aimDirectionNoY.y = 0;
            float launchAngle = Utility.CalculateProjectileLaunchAngle(equippedGun.projectileLaunchVelocity, aimingReference.position, target);
            aimDirection = Quaternion.AngleAxis(-launchAngle, aimingReference.right) * aimDirectionNoY;
        }
        else if(directionToAimCalculationMode == DirectionToAimCalculationMode.Grenade)
        {
            //calculate proper launch velocity too here
            Vector3 aimDirectionNoY = target - aimingReference.position;
            aimDirectionNoY.y = 0;
            float launchAngle = Utility.CalculateProjectileLaunchAngle(equippedGrenade.throwVelocityAt10mDistance, aimingReference.position, target);
            aimDirection = Quaternion.AngleAxis(-launchAngle, aimingReference.right) * aimDirectionNoY;
        }

        #endregion

        #region Add Hand Shake and Aiming Error based on Skill

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

        #endregion


    }

    void ChangeAimingError()
    {
        currentAimingError = Quaternion.Euler(Random.Range(-maxAimError, maxAimError), Random.Range(-maxAimError, maxAimError), Random.Range(-maxAimError, maxAimError));
    }

    public float DetermineThrowingObjectVelocity(Item throwingObject, Vector3 target)//float throwVelocityAt10mDistance, Vector3 target)
    {
        float distance = (target - aimingReference.position).magnitude;
        if(throwingObject is Grenade)
        {
            float velocityAt10m = (throwingObject as Grenade).throwVelocityAt10mDistance;
            return distance / 10 * velocityAt10m;
        }
        return 0;
    }

    /*public void UpdateCurrentSelectedItem(Item currentlySelectedItem)
    {
        this.currentlySelectedItem = currentlySelectedItem;
    }*/
}
