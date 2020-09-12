using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EC_HumanoidCharacterController : EntityComponent
{
    // Is the Interface between the Ai Controller and all the other Controllers like aiming movement etc...

    public EC_HumanoidMovementController movementController;
    public EC_HumanoidAnimationController animationController;

    public float idleWalkingSpeed;
    public float idleStationaryTurnSpeed;
    public float combatStanceWalkingSpeed;
    public float combatStationaryTurnSpeed;

    enum CharacterStance
    {
        Idle,
        CombatStance
    }
    CharacterStance currentStance;


    public override void SetUpComponent(GameEntity entity)
    {
        base.SetUpComponent(entity);
        ChangeCharacterStanceToIdle();
    }
    public override void UpdateComponent()
    {
        if (Input.GetKeyDown(KeyCode.Y))
        {
            ChangeCharacterStanceToIdle();
        }
        if (Input.GetKeyDown(KeyCode.X))
        {
            ChangeCharacterStanceToCombatStance();
        }
    }



    public void ChangeCharacterStanceToIdle()
    {
        currentStance = CharacterStance.Idle;
        movementController.SetWalkingSpeed(idleWalkingSpeed);
        movementController.SetStationaryTurnSpeed(idleStationaryTurnSpeed);
        animationController.ChangeToIdleStance();

    }

    public void ChangeCharacterStanceToCombatStance()
    {
        currentStance = CharacterStance.CombatStance;
        movementController.SetWalkingSpeed(combatStanceWalkingSpeed);
        movementController.SetStationaryTurnSpeed(combatStationaryTurnSpeed);
        animationController.ChangeToCombatStance();
    }

    public void ChangeMovementStance(EC_HumanoidMovementController.MovementStance stance)
    {

    }

    public void MoveTo(Vector3 destination)
    {

    }

    public void MoveTo(Vector3 destination, EC_HumanoidMovementController.MovementType movementType)
    {

    }

    public void LookAt()
    {

    }

    public void StopLookAt()
    {

    }

    public void AimAt()
    {

    }

    public void StopAimAt()
    {

    }

    public void EquipItem(int inventoryID)
    {

    }

    public void Shoot()
    {

    }

    public void StartReloadingCurrentWeapon()
    {

    }

    public void AbortReloadingCurrentWeapon()
    {

    }




}
