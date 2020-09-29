using JetBrains.Annotations;
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

    public bool controllByPlayer;


    public override void SetUpComponent(GameEntity entity)
    {
        base.SetUpComponent(entity);

        SetUpCharacter();
    }
    public override void UpdateComponent()
    {
        #region Keyboard Input for Development
        if (controllByPlayer)
        {


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
                //AimAt(aimAtTarget);
                AimSpineAtTransform(aimAtTarget);
                //AimWeaponAtTransform(aimAtTarget);
            }
            if (Input.GetKeyDown(KeyCode.H))
            {
                //StopAimAt();
                StopAimingSpine();
               // StopAimingWeapon();
            }

            // -------Shooting & Reloading -----
            if (Input.GetKeyDown(KeyCode.Space))
            {
                ShootWeapon();

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
                //ThrowGrenade();
            }






            if (Input.GetMouseButtonDown(0))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

                RaycastHit hit;
                if (Physics.Raycast(ray, out hit))
                {
                    Debug.Log("g: " + gameObject.name + " move to ");
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

        #endregion

        // Disable Stun after Time
        if (characterPreventionType == CharacterPreventionType.Stunned)
        {
            if (Time.time > endStunTime)
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


    void SetUpCharacter()
    {
        // the same as ChangeCharacterStanceToIdle but isnt blocked if already in idle stance
        currentStance = CharacterStance.Idle;
        movementController.SetDefaultSpeed(idleWalkingSpeed);
        movementController.SetSprintSpeed(idleSprintingSpeed);
        movementController.SetStationaryTurnSpeed(idleStationaryTurnSpeed);
        movementController.SetAcceleration(idleAcceleration);
        animationController.ChangeToIdleStance();
        handsIKController.OnEnterIdleStance();

        //StopAimAt();
        StopAimingSpine();
        StopAimingWeapon();
    }

    public void ChangeCharacterStanceToIdle()
    {
        if (currentStance != CharacterStance.Idle)
        {
            currentStance = CharacterStance.Idle;
            movementController.SetDefaultSpeed(idleWalkingSpeed);
            movementController.SetSprintSpeed(idleSprintingSpeed);
            movementController.SetStationaryTurnSpeed(idleStationaryTurnSpeed);
            movementController.SetAcceleration(idleAcceleration);
            animationController.ChangeToIdleStance();
            handsIKController.OnEnterIdleStance();

            //StopAimAt();
            StopAimingSpine();
            StopAimingWeapon();
        }
    }

    public void ChangeCharacterStanceToCombatStance()
    {

        if (currentStance != CharacterStance.CombatStance)
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
    }

    public void ChangeCharacterStanceToCrouchingStance()
    {
        if (currentStance != CharacterStance.Crouching)
        {
            currentStance = CharacterStance.Crouching;
            movementController.SetDefaultSpeed(crouchSpeed);
            movementController.SetStationaryTurnSpeed(idleStationaryTurnSpeed);
            movementController.SetAcceleration(crouchAcceleration);
            animationController.ChangeToCrouchedStance();
            handsIKController.OnEnterCombatStance();
        }
    }



    public void MoveTo(Vector3 destination)
    {
        if (characterPreventionType == CharacterPreventionType.NoPrevention)
        {
            movementController.MoveTo(destination, false);
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
                    //StopAimAt();
                    StopAimingSpine();
                    StopAimingWeapon();
                }
            }

            movementController.MoveTo(destination, sprint);
        }
    }

    public void StopMoving()
    {
        movementController.AbortMoving();
    }

    #region Aiming Weapon & Spine And LookAt

    /*public void AimAt(Transform traget)
    {
        if (characterPreventionType == CharacterPreventionType.NoPrevention)
        {
            if (DoesCurrentStanceAllowAiming())
            {
                //LookAt(traget);
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
        //StopLookAt();
        StopAimingSpine();
        StopAimingWeapon();
    }*/


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


    public void AimSpineAtTransform(Transform target)
    {
       // if (characterPreventionType == CharacterPreventionType.NoPrevention)
        //{
            //if (DoesCurrentStanceAllowAiming())
           // {
                aimingController.AimSpineAtTransform(target);
            //}
        //}

    }

    public void AimSpineAtPosition(Vector3 position)
    {
        if (characterPreventionType == CharacterPreventionType.NoPrevention)
        {
            if (DoesCurrentStanceAllowAiming())
            {
                aimingController.AimSpineAtPosition(position);
            }
        }

    }

    public void AimSpineInDirection(Vector3 direction)
    {
        if (characterPreventionType == CharacterPreventionType.NoPrevention)
        {
            if (DoesCurrentStanceAllowAiming())
            {
                aimingController.AimSpineInDirection(direction);
            }
        }

    }

    public void StopAimingSpine()
    {
        aimingController.StopAimSpineAtTarget();
    }

    public float GetCurrentSpineAimingErrorAngle()
    {
        return aimingController.GetCurrentSpineAimingErrorAngle();
    }

    //automaticly takes the spine target
    public void AimWeaponAtTransform(Transform target)
    {
        if (characterPreventionType == CharacterPreventionType.NoPrevention)
        {
            if (DoesCurrentStanceAllowAiming())
            {
                if (interactionController.DoesCurrentItemInteractionStanceAllowAimingWeapon())
                {
                    aimingController.AimWeaponAtTransform(target);
                }
            }
        }
    }

    public void AimWeaponAtPosition(Vector3 position)
    {
        if (characterPreventionType == CharacterPreventionType.NoPrevention)
        {
            if (DoesCurrentStanceAllowAiming())
            {
                if (DoesCurrentItemInteractionStanceAllowAimingWeapon())
                {
                    aimingController.AimWeaponAtPosition(position);
                }
            }
        }
    }

    public void AimWeaponInDirection(Vector3 direction)
    {
        if (characterPreventionType == CharacterPreventionType.NoPrevention)
        {
            if (DoesCurrentStanceAllowAiming())
            {
                if (DoesCurrentItemInteractionStanceAllowAimingWeapon())
                {
                    aimingController.AimWeaponInDirection(direction);
                }
            }
        }
    }

    public float GetCurrentWeaponAimingErrorAngle()
    {
        return aimingController.GetCurrentWeaponAimingErrorAngle();
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

    public Item GetCurrentlySelectedItem()
    {
        return interactionController.GetCurrentSelectedItem();
    }

    public Item GetItemInInventory(int inventoryPosition)
    {
        return interactionController.GetItemInInventory(inventoryPosition);
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

    public int GetAmmoRemainingInMagazine()
    {
        return interactionController.GetAmmoRemainingInMagazine();
    }

    public void ThrowGrenade(float throwVelocity, Vector3 throwDirection)
    {
        if (characterPreventionType == CharacterPreventionType.NoPrevention)
        {
            interactionController.ThrowGrenade(throwVelocity, throwDirection);
        }
    }

    public void AbortThrowingGrenade()
    {
        interactionController.AbortThrowingGrenade();
    }

    bool DoesCurrentStanceAllowSprinting()
    {
        if (currentStance == CharacterStance.Crouching)
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

    public bool DoesCurrentItemInteractionStanceAllowAimingWeapon()
    {
        return interactionController.DoesCurrentItemInteractionStanceAllowAimingWeapon();
    }

    #region Damage And Death Reactions

    void Stagger(float staggerDuration)
    {
        animationController.Stagger(staggerDuration);
        characterPreventionType = CharacterPreventionType.Stunned;
        endStunTime = Time.time + staggerDuration;

        AbortReloadingWeapon();
        // StopAimAt();
        AbortChangingSelectedItem();
        AbortThrowingGrenade();
        StopAimingSpine();
        StopAimingWeapon();


        if (currentStance == CharacterStance.Crouching)
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
        else if (damageInfo.damage > damageThresholdForFlinch)
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

    #endregion

    #region Traversing Offmesh Links

    public void OnStartTraversingOffMeshLink()
    {
        characterPreventionType = CharacterPreventionType.JumpingToTraverseOffMeshLink;

        AbortReloadingWeapon();
        //StopAimAt();
        AbortChangingSelectedItem();
        //AbortThrowingGrenade();
        handsIKController.DisableIKs();
        StopAimingSpine();
        StopAimingWeapon();


    }

    public void OnStopTraversingOffMeshLink()
    {
        if (characterPreventionType == CharacterPreventionType.JumpingToTraverseOffMeshLink)
        {
            characterPreventionType = CharacterPreventionType.NoPrevention;
            handsIKController.ReenableIKs();
        }
    }

    #endregion






}
