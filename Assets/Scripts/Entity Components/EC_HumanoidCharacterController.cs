using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EC_HumanoidCharacterController : EntityComponent
{
    // Is the Interface between the Ai Controller and all the other Controllers like aiming movement etc...

    enum CharacterStance
    {
        Idle,
        CombatStance
    }
    CharacterStance currentStance;


    public override void SetUpComponent(GameEntity entity)
    {
        base.SetUpComponent(entity);
    }
    public override void UpdateComponent()
    {

    }



    public void ChangeCharacterStanceToIdle()
    {
        currentStance = CharacterStance.Idle;
    }

    public void ChangeCharacterStanceToCombatStance()
    {
        currentStance = CharacterStance.CombatStance;

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
