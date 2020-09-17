using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EC_HumanoidCharacterController : EntityComponent
{
    // Is the Interface between the Ai Controller and all the other Controllers like aiming movement etc...

    [Header("References")]
    public EC_HumanoidMovementController movementController;
    public EC_HumanoidAnimationController animationController;
    public EC_HumanoidInterationController interactionController;
    public EC_HumanoidAimingController aimingController;
    public EC_HumanoidHandsIKController handsIKController;

    [Header("Movement Speeds")]
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

    [Header("For development only")]
    public Transform aimAtTarget;
    public Transform lookAtTarget;
    //add a bool to character stance which tells if stance allows sprinting or not


    public override void SetUpComponent(GameEntity entity)
    {
        base.SetUpComponent(entity);
        ChangeCharacterStanceToIdle();
    }
    public override void UpdateComponent()
    {
        #region Keyboard Input for Development

        // -------- Stances -------
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

        // -------- Weapons -------
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
           // interactionController.SelectRifle();
            interactionController.ChangeWeapon(1);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            interactionController.ChangeWeapon(2);
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            interactionController.ChangeWeapon(0);
        }

        // -------- Aiming & Look at -------
        if (Input.GetKeyDown(KeyCode.L))
        {
            LookAt(lookAtTarget);
        }
        if (Input.GetKeyDown(KeyCode.K))
        {
            StopLookAt();
        }
        if (Input.GetKeyDown(KeyCode.J))
        {
            AimAt(aimAtTarget);
        }
        if (Input.GetKeyDown(KeyCode.H))
        {
            StopAimAt();
        }

        // -------Shooting & Reloading -----
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Shoot();
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

        #endregion

    }


    public void ChangeCharacterStanceToIdle()
    {
        currentStance = CharacterStance.Idle;
        movementController.SetDefaultSpeed(idleWalkingSpeed);
        movementController.SetStationaryTurnSpeed(idleStationaryTurnSpeed);
        movementController.SetAcceleration(idleAcceleration);
        animationController.ChangeToIdleStance();
        handsIKController.OnEnterIdleStance();

        StopAimAt();

    }

    public void ChangeCharacterStanceToCombatStance()
    {
        if (interactionController.DoesCurrentItemInHandAllowCombatStance())
        {
            currentStance = CharacterStance.CombatStance;
            movementController.SetDefaultSpeed(combatStanceWalkingSpeed);
            movementController.SetStationaryTurnSpeed(combatStationaryTurnSpeed);
            movementController.SetAcceleration(combatStanceAcceleration);
            animationController.ChangeToCombatStance();
            handsIKController.OnEnterCombatStance();

        }  
    }

    public void ChangeCharacterStanceToCrouchingStance()
    {
        currentStance = CharacterStance.Crouching;
        movementController.SetDefaultSpeed(crouchSpeed);
        movementController.SetStationaryTurnSpeed(idleStationaryTurnSpeed);
        movementController.SetAcceleration(crouchAcceleration);
        animationController.ChangeToCrouchedStance();
        handsIKController.OnEnterCombatStance();

    }



    public void MoveTo(Vector3 destination)
    {
        movementController.MoveTo(destination);
    }

    public void MoveTo(Vector3 destination, bool sprint)
    {
        if (sprint)
        {
            if (!DoesCurrentStanceAllowSprinting())
            {
                sprint = false;
            }
        }


        movementController.MoveTo(destination, sprint);
    }

    public void LookAt(Transform target)
    {
        aimingController.LookAtTransform(target);
    }

    public void StopLookAt()
    {
        aimingController.StopLookAt();
    }

    public void AimAt(Transform traget)
    {
        if (DoesCurrentStanceAllowAiming())
        {
            aimingController.LookAtTransform(traget);
            aimingController.AimSpineAtTransform(traget);
            aimingController.AimWeaponAtTarget();
        }  
    }

    public void StopAimAt()
    {
        aimingController.StopLookAt();
        aimingController.StopAimSpineAt();
        aimingController.StopAimingWeaponAtTarget();
    }

    public void EquipItem(int inventoryID)
    {

    }

    public void Shoot()
    {
        interactionController.ShootWeapon();
    }

    public void StartReloadingCurrentWeapon()
    {

    }

    public void AbortReloadingCurrentWeapon()
    {

    }

    bool DoesCurrentStanceAllowSprinting()
    {
        if(currentStance == CharacterStance.Crouching)
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    bool DoesCurrentStanceAllowAiming()
    {
        if (currentStance == CharacterStance.Idle)
        {
            return false;
        }
        else
        {
            return true;
        }
    }






}
