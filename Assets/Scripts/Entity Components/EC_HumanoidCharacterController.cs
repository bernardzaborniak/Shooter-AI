using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EC_HumanoidCharacterController : EntityComponent
{
    // Is the Interface between the Ai Controller and all the other Controllers like aiming movement etc...

    #region Fields 

    [Header("References")]
    public EC_HumanoidMovementController movementController;
    public EC_HumanoidAnimationController animationController;
    public EC_HumanoidInterationController interactionController;
    public EC_HumanoidAimingController aimingController;
    public EC_HumanoidHandsIKController handsIKController;

    [Header("Movement Speeds")]
    [Space(5)]
    public float idleWalkingSpeed;
    public float idleSprintingSpeed;
    public float idleStationaryTurnSpeed;
    public float idleAcceleration;
    [Space(5)]
    public float combatStanceWalkingSpeed;
    public float combatStanceSprintingSpeed;
    public float combatStationaryTurnSpeed;
    public float combatStanceAcceleration;
    [Space(5)]
    public float crouchSpeed;
    public float crouchAcceleration;

    [Header("Damage Reactions")]
    public float damageThresholdForFlinch;
    public float damageThresholdForStagger;
    public CharacterPreventionModifier stunModifier;
    public MovementSpeedModifier staggerMovementSpeedModifier;


  

    [Header("Death Effect")]
    public HumanoidDeathEffect humanoidDeathEffect;


    [Header("For development only")]
    public Transform aimAtTarget;
    public Transform lookAtTarget;
    public bool controllByPlayer;
    //add a bool to character stance which tells if stance allows sprinting or not

    [Header("Modifiers")]

    HashSet<MovementSpeedModifier> activeMovementSpeedModifiers = new HashSet<MovementSpeedModifier>();
    HashSet<MovementSpeedModifier> movementSpeedModifiersToDeleteThisFrame = new HashSet<MovementSpeedModifier>();

    HashSet<CharacterPreventionModifier> activeStunModifers = new HashSet<CharacterPreventionModifier>();
    HashSet<CharacterPreventionModifier> stunModifiersToDeleteThisFrame = new HashSet<CharacterPreventionModifier>();

    HashSet<CharacterPreventionModifier> activeTraversingOffmeshLinkPreventionModifers = new HashSet<CharacterPreventionModifier>();
    HashSet<CharacterPreventionModifier> traversingOffmeshLinkPreventionModifiersToDeleteThisFrame = new HashSet<CharacterPreventionModifier>();




    #endregion

    #region State Fields

    // ---------- CharacterStance ---------
    enum CharacterStance
    {
        Idle,
        CombatStance,
        Crouching,
    }
    CharacterStance currentStance;

    // ---------- CharacterPreventionType ---------

   
    //float endStunTime;

    #endregion


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
                AimWeaponAtTransform(aimAtTarget);
            }
            if (Input.GetKeyDown(KeyCode.H))
            {
                //StopAimAt();
                StopAimingSpine();
                StopAimingWeapon();
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
                ThrowGrenade(5f, transform.forward);
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

        // 1. Disable Stun after Time
       /* if (characterPreventionType == CharacterPreventionType.Stunned)
        {
            if (Time.time > endStunTime)
            {
                characterPreventionType = CharacterPreventionType.NoPrevention;
                //RemoveModifier(staggerMovementSpeedModifier);
            }
        }else *//*if(characterPreventionType == CharacterPreventionType.JumpingToTraverseOffMeshLink)
        {
            if (onStopTraversingOffMeshLinkIsDelayed)
            {
                if (Time.time > nextOffMeshFinishTime)
                {
                    onStopTraversingOffMeshLinkIsDelayed = false;

                    characterPreventionType = CharacterPreventionType.NoPrevention;
                    handsIKController.OnStopTraversingOffMeshLink();
                }
            } 
        }*/

        // 2. Update Modifiers
        UpdateModifiers();
    }



    #region Changing Character Stances Orders

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

    #endregion

    #region Character Stances Info

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

    public bool IsCrouched()
    {
        if (currentStance == CharacterStance.Crouching)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    #endregion


    #region Movement Orders

    public void MoveTo(Vector3 destination)
    {
        if (!AreModifiersPreventing())
        {
            movementController.MoveTo(destination, false);
        }
    }


    public void MoveTo(Vector3 destination, bool sprint)
    {
        if (!AreModifiersPreventing())
        {
            if (sprint)
            {
                if (!DoesCurrentStanceAllowSprinting())
                {
                    sprint = false;
                }
                else
                {
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

    #endregion

    #region Movement Info

    public bool IsMoving()
    {
        return movementController.IsMoving();
    }

    public bool IsSprinting()
    {
        return movementController.IsSprinting();
    }

    public float GetRemainingDistanceToCurrentMovementTarget()
    {
        return movementController.GetRemainingDistance();
    }

    #endregion




    #region Look At Orders

    public void LookAt(Transform target)
    {
        if (!AreModifiersPreventing())
        {
            aimingController.LookAtTransform(target);
        }
    }

    public void StopLookAt()
    {
        aimingController.StopLookAt();
    }

    #endregion

    #region Look At Info

    public bool IsLookingAtTarget()
    {
        return aimingController.IsCharacterLookingAtTarget();
    }

    #endregion

    #region Aim Spine Orders
    public void AimSpineAtTransform(Transform target)
    {
        if (!AreModifiersPreventing())
        {
            if (DoesCurrentStanceAllowAiming())
            {
                aimingController.AimSpineAtTransform(target);

                if (movementController.IsSprinting())
                {
                    movementController.SetSprint(false);
                }
            }
        }
    }

    public void AimSpineAtPosition(Vector3 position)
    {
        if (!AreModifiersPreventing())
        {
            if (DoesCurrentStanceAllowAiming())
            {
                aimingController.AimSpineAtPosition(position);

                if (movementController.IsSprinting())
                {
                    movementController.SetSprint(false);
                }
            }
        }

    }

    public void AimSpineInDirection(Vector3 direction)
    {
        if (!AreModifiersPreventing())
        {
            if (DoesCurrentStanceAllowAiming())
            {
                aimingController.AimSpineInDirection(direction);

                if (movementController.IsSprinting())
                {
                    movementController.SetSprint(false);
                }
            }
        }

    }

    public void StopAimingSpine()
    {
        aimingController.StopAimSpineAtTarget();
    }

    #endregion

    #region Aim Spine Info

    public float GetCurrentSpineAimingErrorAngle()
    {
        return aimingController.GetCurrentSpineAimingErrorAngle();
    }

    public bool IsAimingSpine()
    {
        return aimingController.IsCharacterAimingSpine();
    }

    #endregion

    #region Aim Weapon Orders Info

    public void AimWeaponAtTransform(Transform target)
    {
        if (!AreModifiersPreventing())
        {
            if (DoesCurrentStanceAllowAiming())
            {
                if (interactionController.DoesCurrentItemInteractionStanceAllowAimingWeapon())
                {
                    aimingController.AimWeaponAtTransform(target);

                    if (movementController.IsSprinting())
                    {
                        movementController.SetSprint(false);
                    }
                }
            }
        }
    }

    public void AimWeaponAtPosition(Vector3 position)
    {
        if (!AreModifiersPreventing())
        {
            if (DoesCurrentStanceAllowAiming())
            {
                if (DoesCurrentItemInteractionStanceAllowAimingWeapon())
                {
                    aimingController.AimWeaponAtPosition(position);

                    if (movementController.IsSprinting())
                    {
                        movementController.SetSprint(false);
                    }
                }
            }
        }
    }

    public void AimWeaponInDirection(Vector3 direction)
    {
        if (!AreModifiersPreventing())
        {
            if (DoesCurrentStanceAllowAiming())
            {
                if (DoesCurrentItemInteractionStanceAllowAimingWeapon())
                {
                    aimingController.AimWeaponInDirection(direction);

                    if (movementController.IsSprinting())
                    {
                        movementController.SetSprint(false);
                    }
                }
            }
        }
    }

    public void StopAimingWeapon()
    {
        aimingController.StopAimingWeaponAtTarget();
    }

    #endregion

    #region Aim Weapon Orders Info

    public float GetCurrentWeaponAimingErrorAngle(bool ignoreRecoil)
    {
        return aimingController.GetCurrentWeaponAimingErrorAngle(ignoreRecoil);
    }


    public bool IsAimingWeapon()
    {
        return aimingController.IsCharacterAimingWeapon();
    }

    #endregion


    #region Item Interation Orders
    public void ChangeSelectedItem(int inventoryID)
    {
        if (!AreModifiersPreventing())
        {
            //stop aiming weapon if itemChangeInitiated
            if (interactionController.ChangeItemInHand(inventoryID))
            {
                if (IsAimingWeapon())
                {
                    StopAimingWeapon();
                }
            }
           
        }
    }

    public void AbortChangingSelectedItem()
    {
        interactionController.AbortChangingItemInHand();
    }

    #endregion

    #region Item Interaction Info

    public Item GetCurrentlySelectedItem()
    {
        return interactionController.GetCurrentSelectedItem();
    }

    public Item GetItemInInventory(int inventoryPosition)
    {
        return interactionController.GetItemInInventory(inventoryPosition);
    }

    public bool DoesCurrentItemInteractionStanceAllowAimingWeapon()
    {
        return interactionController.DoesCurrentItemInteractionStanceAllowAimingWeapon();
    }

    #endregion


    #region Weapon Combat Orders

    public void ShootWeapon()
    {
        if (!AreModifiersPreventing())
        {
            interactionController.ShootWeapon();
        }
    }


    public void StartReloadingWeapon()
    {
        if (!AreModifiersPreventing())
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


    public void ThrowGrenade(float throwVelocity, Vector3 throwDirection)
    {
        if (!AreModifiersPreventing())
        {
            interactionController.ThrowGrenade(throwVelocity, throwDirection);
        }
    }

    public void AbortThrowingGrenade()
    {
        interactionController.AbortThrowingGrenade();
    }

    #endregion

    #region Weapon Combat Info

    public int GetAmmoRemainingInMagazine()
    {
        return interactionController.GetAmmoRemainingInMagazine();
    }

    #endregion


    #region Damage And Death Reactions

  

    public override void OnTakeDamage(ref DamageInfo damageInfo)
    {

        if (damageInfo.damage > damageThresholdForStagger)
        {
            //Stagger
            //Stagger(staggerDuration);
            CharacterModifier addedModifier = AddModifier(stunModifier);
            animationController.Stagger(addedModifier.currentModifierDuration);
            AddModifier(staggerMovementSpeedModifier);
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


    #region Modifiers

    public CharacterModifier AddModifier(CharacterModifier modifier)
    {
        Debug.Log("onADD 0: " + modifier.name);
        if (modifier is MovementSpeedModifier)
        {
            activeMovementSpeedModifiers.Add(modifier as MovementSpeedModifier);
        }
        else if(modifier is CharacterPreventionModifier)
        {
           
            CharacterPreventionModifier preventionMod = modifier as CharacterPreventionModifier;
            Debug.Log("onADD 1.1: prev type" + preventionMod.characterPreventionType);

            if (preventionMod.characterPreventionType == CharacterPreventionModifier.CharacterPreventionType.Stunned)
            {
                if(activeStunModifers.Count == 0)
                {
                    activeStunModifers.Add(preventionMod);
                    OnAddStunModifier(); //only execute this when the stun starts, stun cant stack, mor stun modifiers can only increase the stun duration
                }
                else
                {
                    activeStunModifers.Add(preventionMod);
                }
            }
            else if(preventionMod.characterPreventionType == CharacterPreventionModifier.CharacterPreventionType.JumpingToTraverseOffMeshLink)
            {
                Debug.Log("onADD 1.2");
                if (activeTraversingOffmeshLinkPreventionModifers.Count == 0)
                {
                    activeTraversingOffmeshLinkPreventionModifers.Add(preventionMod);
                    OnAddTraversingOffMeshLinkPreventionModifier(); //only execute this when the stun starts, stun cant stack, mor stun modifiers can only increase the stun duration
                }
                else
                {
                    activeTraversingOffmeshLinkPreventionModifers.Add(preventionMod);
                }
            }

        }

        modifier.Activate();
        return modifier;
    }

    public void RemoveModifier(CharacterModifier modifier)
    {
        bool remove = true;
        //if(modifier.type == CharacterModifier.ModiferType.DeactivateManuallyWithDelay)
        //{
            if(!modifier.HasDeactivationDelayPassed())
            {
                remove = false;
            }
        //}

        if (remove)
        {
            if (modifier is MovementSpeedModifier)
            {
                activeMovementSpeedModifiers.Remove(modifier as MovementSpeedModifier);
            }
            else if (modifier is CharacterPreventionModifier)
            {

                CharacterPreventionModifier preventionMod = modifier as CharacterPreventionModifier;

                if (preventionMod.characterPreventionType == CharacterPreventionModifier.CharacterPreventionType.Stunned)
                {
                    activeStunModifers.Remove(preventionMod);
                    if (activeStunModifers.Count == 0)
                    {
                        OnRemoveStunModifier(); //only execute when the last stun is removed
                    }
                }
                else if (preventionMod.characterPreventionType == CharacterPreventionModifier.CharacterPreventionType.JumpingToTraverseOffMeshLink)
                {
                    activeTraversingOffmeshLinkPreventionModifers.Remove(preventionMod);

                    if (activeTraversingOffmeshLinkPreventionModifers.Count == 0)
                    {
                        OnRemoveTraversingOffMeshLinkPreventionModifier(); //only execute this when the stun starts, stun cant stack, mor stun modifiers can only increase the stun duration
                    }
                }
            }
        }
        
    }

    void UpdateModifiers()
    {
        //Debug.Log("Modifiers this frame----------------------------------------------------");
        foreach (MovementSpeedModifier modifier in activeMovementSpeedModifiers)
        {
            //Debug.Log("mod: " + modifier + " modifier.name: " + modifier.name);
            if (modifier.HasModifierTimeRunOut())
            {
                movementSpeedModifiersToDeleteThisFrame.Add(modifier);             
            }
        }
        foreach (MovementSpeedModifier modifier in movementSpeedModifiersToDeleteThisFrame)
        {
            activeMovementSpeedModifiers.Remove(modifier);
        }
        movementSpeedModifiersToDeleteThisFrame.Clear();



        foreach (CharacterPreventionModifier modifier in activeStunModifers)
        {
            //Debug.Log("mod: " + modifier + " modifier.name: " + modifier.name);

            if (modifier.HasModifierTimeRunOut())
            {
                stunModifiersToDeleteThisFrame.Add(modifier);
            }
        }
        foreach (CharacterPreventionModifier modifier in stunModifiersToDeleteThisFrame)
        {
            activeStunModifers.Remove(modifier);
        }
        stunModifiersToDeleteThisFrame.Clear();



        foreach (CharacterPreventionModifier modifier in activeTraversingOffmeshLinkPreventionModifers)
        {
            //Debug.Log("mod: " + modifier + " modifier.name: " + modifier.name);

            if (modifier.HasModifierTimeRunOut())
            {
                Debug.Log("Remove");
                traversingOffmeshLinkPreventionModifiersToDeleteThisFrame.Add(modifier);
            }
        }
        foreach (CharacterPreventionModifier modifier in traversingOffmeshLinkPreventionModifiersToDeleteThisFrame)
        {
            activeTraversingOffmeshLinkPreventionModifers.Remove(modifier);
        }
        traversingOffmeshLinkPreventionModifiersToDeleteThisFrame.Clear();


    }

    public HashSet<MovementSpeedModifier> GetActiveMovementSpeedModifiers()
    {
        return activeMovementSpeedModifiers;
    }



    public bool AreModifiersPreventing()
    {
        if (activeStunModifers.Count > 0 )
        {
            if(activeTraversingOffmeshLinkPreventionModifers.Count > 0)
            {
                return true;
            } 
        }

        return false;

    }


    void OnAddStunModifier()
    {
        AbortReloadingWeapon();
        // StopAimAt();
        AbortChangingSelectedItem();
        AbortThrowingGrenade();
        StopAimingSpine();
        StopAimingWeapon();
    }

    void OnRemoveStunModifier()
    {

    }

    void OnAddTraversingOffMeshLinkPreventionModifier()
    {
        Debug.Log("onADD 2");

        AbortReloadingWeapon();
        //StopAimAt();
        AbortChangingSelectedItem();
        //AbortThrowingGrenade();

        StopAimingSpine();
        StopAimingWeapon();

        //handsIKController.DisableIKs();
        handsIKController.OnStartTraversingOffMeshLink();


    }

    void OnRemoveTraversingOffMeshLinkPreventionModifier()
    {
        Debug.Log("onRemove 1");

        handsIKController.OnStopTraversingOffMeshLink();
        /*if (characterPreventionType == CharacterPreventionType.JumpingToTraverseOffMeshLink)
        {*/


        //onStopTraversingOffMeshLinkIsDelayed = true;
        //nextOffMeshFinishTime = Time.time + offMeshLinkFinishDelayTime;


        //characterPreventionType = CharacterPreventionType.NoPrevention;
        //handsIKController.OnStopTraversingOffMeshLink();
        //}

    }

    #endregion


}
