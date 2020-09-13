using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EC_HumanoidCharacterController : EntityComponent
{
    // Is the Interface between the Ai Controller and all the other Controllers like aiming movement etc...

    public EC_HumanoidMovementController movementController;
    public EC_HumanoidAnimationController animationController;
    public EC_HumanoidInterationController interactionController;

    public float idleWalkingSpeed;
    public float idleStationaryTurnSpeed;
    public float idleAcceleration;
    public float combatStanceWalkingSpeed;
    public float combatStationaryTurnSpeed;
    public float combatStanceAcceleration;


    public float crouchSpeed;
    public float crouchAcceleration;

    enum CharacterStance
    {
        Idle,
        CombatStance,
        Crouching,
    }
    CharacterStance currentStance;


    //add a bool to character stance which tells if stance allows sprinting or not


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
        if (Input.GetKeyDown(KeyCode.C))
        {
            ChangeCharacterStanceToCrouchingStance();
        }

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            interactionController.SelectRifle();
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            interactionController.SelectPistol();
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            interactionController.SelectNothing();
        }




        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                MoveTo(hit.point);

            }
        }

        if (Input.GetMouseButtonDown(2))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                MoveTo(hit.point, true);

            }
        }
    }



    public void ChangeCharacterStanceToIdle()
    {
        currentStance = CharacterStance.Idle;
        movementController.SetDefaultSpeed(idleWalkingSpeed);
        movementController.SetStationaryTurnSpeed(idleStationaryTurnSpeed);
        movementController.SetAcceleration(idleAcceleration);
        animationController.ChangeToIdleStance();

    }

    public void ChangeCharacterStanceToCombatStance()
    {
        currentStance = CharacterStance.CombatStance;
        movementController.SetDefaultSpeed(combatStanceWalkingSpeed);
        movementController.SetStationaryTurnSpeed(combatStationaryTurnSpeed);
        movementController.SetAcceleration(combatStanceAcceleration);
        animationController.ChangeToCombatStance();
    }

    public void ChangeCharacterStanceToCrouchingStance()
    {
        currentStance = CharacterStance.Crouching;
        movementController.SetDefaultSpeed(crouchSpeed);
        movementController.SetStationaryTurnSpeed(idleStationaryTurnSpeed);
        movementController.SetAcceleration(crouchAcceleration);
        animationController.ChangeToCrouchedStance();
    }

   /* public void ChangeMovementStance(EC_HumanoidMovementController.MovementStance stance)
    {

    }*/

    public void MoveTo(Vector3 destination)
    {
        movementController.MoveTo(destination);
    }

    public void MoveTo(Vector3 destination, bool sprint)
    {
        movementController.MoveTo(destination, sprint);
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
