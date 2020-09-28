﻿using System.Collections;
using System.Collections.Generic;
using UnityEditorInternal;
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

    //For MY bad ai so far
    bool changedToPistol;
    bool grenadeThrown;
    float nextCheckGrenadeTime;
    float grenadeThrowInterval = 1f;

    public Transform targetPosition;
    public float targetMaxOffset;
    Vector3 currentTargetOffset;


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

        aIState = AIState.FiringSMG;

        currentTargetOffset = new Vector3(Random.Range(-targetMaxOffset, targetMaxOffset), 0, Random.Range(-targetMaxOffset, targetMaxOffset));
    }

    void Update()
    {
        for (int i = 0; i < aIComponents.Length; i++)
        {
            aIComponents[i].UpdateComponent();
        }




        #region AI State update

        #region Set needed Variables

        GameEntity nearestEnemy = sensing.nearestEnemy;

        float distanceToNearestEnemy = 0;
        if (nearestEnemy)
        {
            distanceToNearestEnemy = (nearestEnemy.transform.position - transform.position).magnitude;
        }

        //----------Set selectedGun & selectedGrenade-----------------
        Gun selectedGun = null;
        Grenade selectedGrenade = null;
        if ((characterController.GetCurrentlySelectedItem() is Gun))
        {
            selectedGun = (characterController.GetCurrentlySelectedItem() as Gun);
        }
        else if ((characterController.GetCurrentlySelectedItem() is Grenade))
        {
            selectedGrenade = (characterController.GetCurrentlySelectedItem() as Grenade);
        }

        //-----------Set enemyMovementSpeed----------------
        Vector3 enemyMovementSpeed = Vector3.zero;
        if (nearestEnemy)
        {
            IMoveable movement = nearestEnemy.GetComponent<IMoveable>();
            if (movement != null)
            {
                enemyMovementSpeed = movement.GetCurrentVelocity();
            }
        }


        #endregion

        if (aIState == AIState.FiringSMG)
        {
            characterController.ChangeSelectedItem(1);
            characterController.ChangeCharacterStanceToCombatStance();

            if (nearestEnemy)
            {
                characterController.StopMoving();
                if (characterController.GetCurrentlySelectedItem() == characterController.GetItemInInventory(1) && characterController.DoesCurrentItemInteractionStanceAllowAimingWeapon())
                {

                    characterController.AimSpineAtPosition(nearestEnemy.GetAimPosition());
                    characterController.AimWeaponInDirection(aimingController.GetDirectionToAimAtTarget(nearestEnemy.GetAimPosition(), enemyMovementSpeed, selectedGun.aimWithAngledShotCalculation, selectedGun.projectileLaunchVelocity, true));

                    if (float.IsNaN(aimingController.GetDirectionToAimAtTarget(nearestEnemy.GetAimPosition(), enemyMovementSpeed, selectedGun.aimWithAngledShotCalculation, selectedGun.projectileLaunchVelocity, true).y))
                    {
                        Debug.Log("NAN: " + transform.parent.gameObject.name + " SMG");
                    }

                    if (characterController.GetAmmoRemainingInMagazine() > 0)
                    {
                        characterController.ShootWeapon();
                    }


                    if(Time.time> nextCheckGrenadeTime)
                    {
                        nextCheckGrenadeTime = Time.time + grenadeThrowInterval;
                        if (Random.Range(0f, 1f) < 0.05f)
                        {
                            if (!grenadeThrown)
                            {
                                aIState = AIState.ThrowingGrenade;
                            }
                        } 
                    }



                    if (!(characterController.GetAmmoRemainingInMagazine() > 0))
                    {
                        Debug.Log("ammo in magazine: " + characterController.GetAmmoRemainingInMagazine());
                        Debug.Log("current gun  " + characterController.GetCurrentlySelectedItem());

                        if (!changedToPistol)
                        {
                            if (Random.Range(0f, 1f) < 0.5f)
                            {
                                characterController.StartReloadingWeapon();
                            }
                            else
                            {
                                changedToPistol = true;
                                aIState = AIState.FiringPistol;
                                Debug.Log("changing to pistol");
                            }
                        }
                        else
                        {
                            characterController.StartReloadingWeapon();
                        }


                    }
                }
            }
            else
            {
                //sprint to point + random position offset - which is set at start
                characterController.StopAimingSpine();
                characterController.StopAimingWeapon();
                characterController.MoveTo(targetPosition.position + currentTargetOffset, true);
            }

        }
        else if (aIState == AIState.FiringPistol)
        {

            characterController.ChangeSelectedItem(2);
            characterController.ChangeCharacterStanceToCombatStance();

            if (nearestEnemy)
            {
                characterController.StopMoving();
                if (characterController.GetCurrentlySelectedItem() == characterController.GetItemInInventory(2) && characterController.DoesCurrentItemInteractionStanceAllowAimingWeapon())
                {

                    characterController.AimSpineAtPosition(nearestEnemy.GetAimPosition());
                    characterController.AimWeaponInDirection(aimingController.GetDirectionToAimAtTarget(nearestEnemy.GetAimPosition(), enemyMovementSpeed, selectedGun.aimWithAngledShotCalculation, selectedGun.projectileLaunchVelocity, true));

                    if (float.IsNaN(aimingController.GetDirectionToAimAtTarget(nearestEnemy.GetAimPosition(), enemyMovementSpeed, selectedGun.aimWithAngledShotCalculation, selectedGun.projectileLaunchVelocity, true).y))
                    {
                        Debug.Log("NAN: " + transform.parent.gameObject.name + " Psitol");
                    }

                    if (characterController.GetAmmoRemainingInMagazine() > 0)
                    {
                        characterController.ShootWeapon();
                    }



                    if (!(characterController.GetAmmoRemainingInMagazine() > 0))
                    {
                        aIState = AIState.FiringSMG;
                    }
                }
            }
            else
            {
                //sprint to point + random position offset - which is set at start
                characterController.StopAimingSpine();
                characterController.StopAimingWeapon();
                characterController.MoveTo(targetPosition.position + currentTargetOffset, true);
            }
        }
        else if(aIState == AIState.ThrowingGrenade)
        {
            characterController.ChangeSelectedItem(3);
            characterController.ChangeCharacterStanceToCombatStance();

            if (characterController.GetItemInInventory(3) == null)
            {
                grenadeThrown = false;
                aIState = AIState.FiringSMG;
                return;
            }

            if (nearestEnemy)
            {
                characterController.StopMoving();

                if (characterController.GetCurrentlySelectedItem() == characterController.GetItemInInventory(3) && characterController.DoesCurrentItemInteractionStanceAllowAimingWeapon())
                {

                    Grenade equippedGrenade = characterController.GetCurrentlySelectedItem() as Grenade;

                   // enemyMovementSpeed = Vector3.zero;
                    IMoveable movement = nearestEnemy.GetComponent<IMoveable>();

                    if (movement != null)
                    {
                        enemyMovementSpeed = movement.GetCurrentVelocity();
                    }

                    float grenadeThrowingVelocity = aimingController.DetermineThrowingObjectVelocity(equippedGrenade, distanceToNearestEnemy);//we add 2just to fix some errors - needs refactoring later


                    Vector3 aimSpineDirection = aimingController.GetDirectionToAimAtTarget(nearestEnemy.transform.position, Vector3.zero, true, grenadeThrowingVelocity, false); //dont use enemyMovementVelocityWithgrenade as it will lead to errors and suicidal AI :)

                    if (float.IsNaN(aimingController.GetDirectionToAimAtTarget(nearestEnemy.transform.position, Vector3.zero, true, grenadeThrowingVelocity, false).y))
                    {
                        Debug.Log("NAN: " + transform.parent.gameObject.name + " Throwing Grenade");
                        Debug.Log("-> velöpcoty: " + grenadeThrowingVelocity);
                    }
                    //Vector3 grenadeThrowingDirection = aimingController.AddAimErrorAndHandShakeToAimDirection(aimSpineDirection);
                    Vector3 grenadeThrowingDirection = aimSpineDirection;

                    characterController.AimSpineInDirection(aimSpineDirection);

                    if (characterController.GetCurrentSpineAimingErrorAngle() < 5)
                    {
                        characterController.ThrowGrenade(grenadeThrowingVelocity, grenadeThrowingDirection);
                    }
                }
            }
            else
            {
                //sprint to point + random position offset - which is set at start
                characterController.StopAimingSpine();
                characterController.StopAimingWeapon();
                characterController.MoveTo(targetPosition.position + currentTargetOffset, true);
            }
        }

        //usually shoot smg, if there is no ammo left in smg, theres a 50 % chance that we change to pistol instead of reloading, the same in pistol to smg
        //shot weapon

        #endregion


        /*GameEntity nearestEnemy = sensing.nearestEnemy;
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
            
        }*/




    }
}
