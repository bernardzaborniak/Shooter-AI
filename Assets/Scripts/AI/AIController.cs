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
                    float grenadeThrowingVelocity = aimingController.DetermineThrowingObjectVelocity(equippedGrenade, distanceToNearestEnemy);
                    Vector3 aimSpineDirection = aimingController.GetDirectionToAimAtTarget(nearestEnemy.transform.position, true, grenadeThrowingVelocity, false);
                    Vector3 grenadeThrowingDirection = aimingController.AddAimErrorAndHandShakeToAimDirection(aimSpineDirection);

                    characterController.AimSpineInDirection(aimSpineDirection);

                    if (characterController.GetCurrentSpineAimingErrorAngle() < 5)
                    {
                        characterController.ThrowGrenade(grenadeThrowingVelocity, grenadeThrowingDirection);
                    }

                }
            }
            
        }

       

        //shot weapon
        /*characterController.ChangeSelectedItem(1);
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
        }*/
    }
}
