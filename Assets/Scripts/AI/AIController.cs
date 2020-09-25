﻿using System.Collections;
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

        characterController.ChangeSelectedItem(1);
        characterController.ChangeCharacterStanceToCombatStance();


        GameEntity nearestEnemy = sensing.nearestEnemy;

        if (nearestEnemy)
        {
            characterController.AimSpineAtPosition(nearestEnemy.GetAimPosition());
            characterController.AimWeaponInDirection(aimingController.GetDirectionToAimAtTarget(nearestEnemy, characterController.GetCurrentlySelectedItem()));

            if(characterController.GetAmmoRemainingInMagazine() > 0)
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
        }
    }
}
