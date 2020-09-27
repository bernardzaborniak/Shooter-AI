using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIController : MonoBehaviour
{
    public GameEntity entityAttachedTo;
    public AIComponent[] aIComponents;

    public EC_HumanoidCharacterController characterController;
    public AIC_HumanSensing sensing;
    public AIC_AimingController aimingController;

    public float maxRangeToEnemy;
    public float minRangeToEnemy;
    public float desiredRangeToEnemy;


    //public float throwGrenadeVelocity;
    //only basic AI for now
    enum AIState
    {
        FiringSMG,
        FiringPistol,
        ThrowingGrenade
    }
    AIState aIState;

    void Start()
    {
        for (int i = 0; i < aIComponents.Length; i++)
        {
            aIComponents[i].SetUpComponent(entityAttachedTo);
        }

        
    }

    void Update()
    {
        for (int i = 0; i < aIComponents.Length; i++)
        {
            aIComponents[i].UpdateComponent();
        }


        #region AI State update
        /*
        //usually shoot smg, if there is no ammo left in smg, theres a 50 % chance that we change to pistol instead of reloading, the same in pistol to smg
        //shot weapon
        characterController.ChangeSelectedItem(1);
        characterController.ChangeCharacterStanceToCombatStance();


        GameEntity nearestEnemy = sensing.nearestEnemy;

        if (nearestEnemy)
        {
            float launchVelocity = 0;
            bool launchAtAnArc = false;
            if ((characterController.GetCurrentlySelectedItem() is Gun))
            {
                Gun gun = (characterController.GetCurrentlySelectedItem() as Gun);
                launchVelocity = gun.projectileLaunchVelocity;
                launchAtAnArc = gun.aimWithAngledShotCalculation;
            }



            Vector3 enemyMovementSpeed = Vector3.zero;
            IMoveable movement = nearestEnemy.GetComponent<IMoveable>();

            if (movement != null)
            {
                enemyMovementSpeed = movement.GetCurrentVelocity();
            }
            characterController.AimSpineAtPosition(nearestEnemy.GetAimPosition());
            characterController.AimWeaponInDirection(aimingController.GetDirectionToAimAtTarget(nearestEnemy.GetAimPosition(), enemyMovementSpeed, launchAtAnArc, launchVelocity, true));

            if (characterController.GetAmmoRemainingInMagazine() > 0)
            {
                characterController.ShootWeapon();
            }

        }
        else
        {
            characterController.StopAimingSpine();
            characterController.StopAimingWeapon();
        }

        if (!(characterController.GetAmmoRemainingInMagazine() > 0))
        {
            characterController.StartReloadingWeapon();
        }*/
        #endregion

        
        GameEntity nearestEnemy = sensing.nearestEnemy;
        float distanceToNearestEnemy = 0;
        if (nearestEnemy)
        {
            distanceToNearestEnemy = (nearestEnemy.transform.position - transform.position).magnitude;
        }
       
        characterController.ChangeCharacterStanceToCombatStance();

        if (characterController.GetItemInInventory(3) != null)
        {
            characterController.ChangeSelectedItem(3);

            if (nearestEnemy)
            {
                if (characterController.GetCurrentlySelectedItem() == characterController.GetItemInInventory(3))
                {
                    Grenade equippedGrenade = characterController.GetCurrentlySelectedItem() as Grenade;

                    Vector3 enemyMovementSpeed = Vector3.zero;
                    IMoveable movement = nearestEnemy.GetComponent<IMoveable>();

                    if (movement != null)
                    {
                        enemyMovementSpeed = movement.GetCurrentVelocity();
                    }

                    float grenadeThrowingVelocity = aimingController.DetermineThrowingObjectVelocity(equippedGrenade, distanceToNearestEnemy);//we add 2just to fix some errors - needs refactoring later


                    Vector3 aimSpineDirection = aimingController.GetDirectionToAimAtTarget(nearestEnemy.transform.position, Vector3.zero, true, grenadeThrowingVelocity, false); //dont use enemyMovementVelocityWithgrenade as it will lead to errors and suicidal AI :)
                    //Vector3 grenadeThrowingDirection = aimingController.AddAimErrorAndHandShakeToAimDirection(aimSpineDirection);
                    Vector3 grenadeThrowingDirection = aimSpineDirection;

                    characterController.AimSpineInDirection(aimSpineDirection);

                    if (characterController.GetCurrentSpineAimingErrorAngle() < 5)
                    {
                        characterController.ThrowGrenade(grenadeThrowingVelocity, grenadeThrowingDirection);
                    }

                }
            }
            
        }




    }
}
