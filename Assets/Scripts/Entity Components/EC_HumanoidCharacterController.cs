﻿using System.Collections;
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
    public float idleSprintingSpeed;
    public float idleStationaryTurnSpeed;
    public float idleAcceleration;

    public float combatStanceWalkingSpeed;
    public float combatStanceSprintingSpeed;
    public float combatStationaryTurnSpeed;
    public float combatStanceAcceleration;

    public float crouchSpeed;
    public float crouchAcceleration;

    public float stunnedMovementSpeed;
    public float stunnedSprintingSpeed;
    public float stunnedCrouchedMovementSpeed;

    [Header("Damage Reactions")]
    public float damageThresholdForFlinch;
    public float damageThresholdForStagger;
    public float staggerDuration;

    // ---------- States & Stances ---------
    enum CharacterStance
    {
        Idle,
        CombatStance,
        Crouching,
    }
    CharacterStance currentStance;

    enum CharacterPreventionType
    {
        NoPrevention,
        Stunned,
        JumpingToTraverseOffMeshLink //similar to stunned but allows look at?
    }

    CharacterPreventionType characterPreventionType;
    //bool stunned;
    float endStunTime;



    [Header("For development only")]
    public Transform aimAtTarget;
    public Transform lookAtTarget;
    //add a bool to character stance which tells if stance allows sprinting or not

    [Header("Death Effect")]
    public HumanoidDeathEffect humanoidDeathEffect;


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
            ChangeSelectedItem(1);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            ChangeSelectedItem(2);
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            ChangeSelectedItem(3);
        }
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            ChangeSelectedItem(0);
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
            //ShootWeapon();
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            StartReloadingWeapon();
        }
        if (Input.GetKeyDown(KeyCode.Q))
        {
            AbortReloadingWeapon();
        }
        if (Input.GetKeyDown(KeyCode.G))
        {
            ThrowGrenade();
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

        // Disable Stun after Time
        if (characterPreventionType == CharacterPreventionType.Stunned)
        {
            if(Time.time> endStunTime)
            {
                characterPreventionType = CharacterPreventionType.NoPrevention;

                //reset speds
                if (currentStance == CharacterStance.Idle)
                {
                    movementController.SetDefaultSpeed(idleWalkingSpeed);
                    movementController.SetSprintSpeed(idleSprintingSpeed);
                }
                else if (currentStance == CharacterStance.CombatStance)
                {
                    movementController.SetDefaultSpeed(combatStanceWalkingSpeed);
                    movementController.SetSprintSpeed(combatStanceSprintingSpeed);
                }
                else if (currentStance == CharacterStance.Crouching)
                {
                    movementController.SetDefaultSpeed(crouchSpeed);
                }
            }
        }
    }


    public void ChangeCharacterStanceToIdle()
    {
        currentStance = CharacterStance.Idle;
        movementController.SetDefaultSpeed(idleWalkingSpeed);
        movementController.SetSprintSpeed(idleSprintingSpeed);
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
            movementController.SetSprintSpeed(combatStanceSprintingSpeed);
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
        if (characterPreventionType == CharacterPreventionType.NoPrevention)
        {
            movementController.MoveTo(destination);
        } 
    }

    public void MoveTo(Vector3 destination, bool sprint)
    {
        if (characterPreventionType == CharacterPreventionType.NoPrevention)
        {
            if (sprint)
            {
                if (!DoesCurrentStanceAllowSprinting())
                {
                    sprint = false;
                }
                else 
                {
                    StopAimAt();
                }
            }

            movementController.MoveTo(destination, sprint);
        }
    }

    public void StopMoving()
    {
        movementController.AbortMoving();
    }

    #region Aiming Weapon Spine And Look at

    public void AimAt(Transform traget)
    {
        if (characterPreventionType == CharacterPreventionType.NoPrevention)
        {
            if (DoesCurrentStanceAllowAiming())
            {
                LookAt(traget);
                AimSpineAtTarget(traget);

                if (interactionController.GetCurrentSelecteditem() is Gun)
                {
                    AimWeaponAtTarget();
                }

            }
        }
    }

    public void StopAimAt()
    {
        StopLookAt();
        StopAimingSpine();
        StopAimingWeapon();
    }


    public void LookAt(Transform target)
    {
        if (characterPreventionType == CharacterPreventionType.NoPrevention)
        {
            aimingController.LookAtTransform(target);
        }   
    }

    public void StopLookAt()
    {
        aimingController.StopLookAt();
    }


    public void AimSpineAtTarget(Transform target)
    {
        if (characterPreventionType == CharacterPreventionType.NoPrevention)
        {
            aimingController.AimSpineAtTransform(target);
        }
        
    }

    public void StopAimingSpine()
    {
        aimingController.StopAimSpineAtTarget();
    }

    //automaticly takes the spine target
    public void AimWeaponAtTarget()
    {
        if (characterPreventionType == CharacterPreventionType.NoPrevention)
        {
            aimingController.AimWeaponAtTarget();
        }
    }

    public void StopAimingWeapon()
    {
        aimingController.StopAimingWeaponAtTarget();
    }



    public bool IsAimingWeapon()
    {
        return aimingController.IsCharacterAimingWeapon();
    }

    public bool IsAimingSpine()
    {
        return aimingController.IsCharacterAimingSpine();
    }

    public bool IsLookingAtTarget()
    {
        return aimingController.IsCharacterLookingAtTarget();
    }

    #endregion

    public void ChangeSelectedItem(int inventoryID)
    {
        if (characterPreventionType == CharacterPreventionType.NoPrevention)
        {
            if (IsAimingWeapon())
            {
                StopAimingWeapon();
            }
            //stop aiming weapon if here
            interactionController.ChangeItemInHand(inventoryID);
        }
    }

    public void AbortChangingSelectedItem()
    {
        interactionController.AbortChangingItemInHand();
    }

    public void ShootWeapon()
    {
        if (characterPreventionType == CharacterPreventionType.NoPrevention)
        {
            interactionController.ShootWeapon();
        }
    }

    public void StartReloadingWeapon()
    {
        if (characterPreventionType == CharacterPreventionType.NoPrevention)
        {
            if (IsAimingWeapon())
            {
                StopAimingWeapon();
            }

            interactionController.StartReloadingWeapon();
        }
    }

    public void AbortReloadingWeapon()
    {
        interactionController.AbortReloadingWeapon();
    }

    public void ThrowGrenade()
    {
        if (characterPreventionType == CharacterPreventionType.NoPrevention)
        {
            interactionController.ThrowGrenade();
        }  
    }

    public void AbortThrowingGrenade()
    {
        interactionController.AbortThrowingGrenade();
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

    void Stagger(float staggerDuration)
    {
        animationController.Stagger(staggerDuration);
        characterPreventionType = CharacterPreventionType.Stunned;
        endStunTime = Time.time + staggerDuration;

        AbortReloadingWeapon();
        StopAimAt();
        AbortChangingSelectedItem();
        AbortThrowingGrenade();


        if(currentStance == CharacterStance.Crouching)
        {
            movementController.SetDefaultSpeed(stunnedCrouchedMovementSpeed);
        }
        else
        {
            movementController.SetDefaultSpeed(stunnedMovementSpeed);
            movementController.SetSprintSpeed(stunnedSprintingSpeed);
        }
      


    }

    public override void OnTakeDamage(ref DamageInfo damageInfo)
    {

        if (damageInfo.damage > damageThresholdForStagger)
        {
            //Stagger
            Stagger(staggerDuration);
        }
        else if(damageInfo.damage > damageThresholdForFlinch)
        {
            animationController.Flinch();
        }
        else
        {
           
            //nothing ?  maybe tell the AI Controller that he was hit 
        }
    }

    public override void OnDie(ref DamageInfo damageInfo)
    {
        humanoidDeathEffect.EnableDeathEffect(movementController.GetCurrentVelocity(), movementController.GetCurrentAngularVelocity(), ref damageInfo);
    }

    public void OnStartTraversingOffMeshLink()
    {
        characterPreventionType = CharacterPreventionType.JumpingToTraverseOffMeshLink;

        AbortReloadingWeapon();
        StopAimAt();
        AbortChangingSelectedItem();
        //AbortThrowingGrenade();
    }

    public void OnStopTraversingOffMeshLink()
    {
        if(characterPreventionType == CharacterPreventionType.JumpingToTraverseOffMeshLink)
        {
            characterPreventionType = CharacterPreventionType.NoPrevention;
        }
    }






}
