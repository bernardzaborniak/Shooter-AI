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
    public CharacterModifierCreator stunModifier;
    public CharacterModifierCreator staggerMovementSpeedModifier;




    [Header("Death Effect")]
    public HumanoidDeathEffect humanoidDeathEffect;


    [Header("For development only")]
    public Transform aimAtTarget;
    public Transform lookAtTarget;
    public bool controllByPlayer;
    //add a bool to character stance which tells if stance allows sprinting or not

    [Header("Modifiers")]

    HashSet<ActiveCharacterMovementSpeedModifier> activeMovementSpeedModifiers = new HashSet<ActiveCharacterMovementSpeedModifier>();
    HashSet<ActiveCharacterMovementSpeedModifier> movementSpeedModifiersToDeleteThisFrame = new HashSet<ActiveCharacterMovementSpeedModifier>();


    HashSet<ActiveCharacterPreventionModifier> activeCharacterPreventionModifiers = new HashSet<ActiveCharacterPreventionModifier>();
    HashSet<ActiveCharacterPreventionModifier> characterPreventionModifiersToDeleteThisFrame = new HashSet<ActiveCharacterPreventionModifier>();

    

    /*HashSet<CharacterPreventionModifier> activeStunModifers = new HashSet<CharacterPreventionModifier>();
    HashSet<CharacterPreventionModifier> stunModifiersToDeleteThisFrame = new HashSet<CharacterPreventionModifier>();

    HashSet<CharacterPreventionModifier> activeTraversingOffMeshLinkPreventionModifers = new HashSet<CharacterPreventionModifier>();
    HashSet<CharacterPreventionModifier> traversingOffmeshLinkPreventionModifiersToDeleteThisFrame = new HashSet<CharacterPreventionModifier>();*/




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

                handsIKController.OnEnterCombatOrCrouchedStance();
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

            handsIKController.OnEnterCombatOrCrouchedStance();
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
            ActiveCharacterModifier mod = stunModifier.CreateAndActivateNewModifier();
            AddModifier(mod);
            animationController.Stagger(mod.currentModifierDuration);
            AddModifier(staggerMovementSpeedModifier.CreateAndActivateNewModifier());
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

    //public CharacterModifierCreator AddModifier(CharacterModifierCreator modifier)
    public void AddModifier(ActiveCharacterModifier modifier)
    {
        if (modifier is ActiveCharacterMovementSpeedModifier)
        {
            activeMovementSpeedModifiers.Add(modifier as ActiveCharacterMovementSpeedModifier);
        }
        else if (modifier is ActiveCharacterPreventionModifier)
        {
            ActiveCharacterPreventionModifier preventionMod = modifier as ActiveCharacterPreventionModifier;

            activeCharacterPreventionModifiers.Add(preventionMod);

            if (preventionMod.characterPreventionType == ActiveCharacterPreventionModifier.CharacterPreventionType.Stunned)
            {
                OnAddStunModifier(); //only execute this when the stun starts, stun cant stack, mor stun modifiers can only increase the stun duration
            }
            else if (preventionMod.characterPreventionType == ActiveCharacterPreventionModifier.CharacterPreventionType.JumpingToTraverseOffMeshLink)
            {
                OnAddTraversingOffMeshLinkPreventionModifier(); //only execute this when the stun starts, stun cant stack, mor stun modifiers can only increase the stun duration
            }

        }

        /* if (modifier is MovementSpeedModifier)
         {
             activeMovementSpeedModifiers.Add(modifier as MovementSpeedModifier);
         }
         else if (modifier is CharacterPreventionModifier)
         {

             CharacterPreventionModifier preventionMod = modifier as CharacterPreventionModifier;

             if (preventionMod.characterPreventionType == CharacterPreventionModifier.CharacterPreventionType.Stunned)
             {
                 if (activeStunModifers.Count == 0)
                 {
                     activeStunModifers.Add(preventionMod);
                     OnAddStunModifier(); //only execute this when the stun starts, stun cant stack, mor stun modifiers can only increase the stun duration
                 }
                 else
                 {
                     activeStunModifers.Add(preventionMod);
                 }
             }
             else if (preventionMod.characterPreventionType == CharacterPreventionModifier.CharacterPreventionType.JumpingToTraverseOffMeshLink)
             {
                 if (activeTraversingOffMeshLinkPreventionModifers.Count == 0)
                 {
                     activeTraversingOffMeshLinkPreventionModifers.Add(preventionMod);
                     OnAddTraversingOffMeshLinkPreventionModifier(); //only execute this when the stun starts, stun cant stack, mor stun modifiers can only increase the stun duration
                 }
                 else
                 {
                     activeTraversingOffMeshLinkPreventionModifers.Add(preventionMod);
                 }
             }

         }

         modifier.Activate();
         return modifier;*/
    }

    public void RemoveModifier(ActiveCharacterModifier modifier)
    {
        ActiveCharacterModifier ac = new ActiveCharacterModifier(stunModifier, 2);
        //Debug.Log("ac: hash" + ac.GetHashCode());

        if (modifier is ActiveCharacterMovementSpeedModifier)
        {
            activeMovementSpeedModifiers.Remove(modifier as ActiveCharacterMovementSpeedModifier);
        }
        else if (modifier is ActiveCharacterPreventionModifier)
        {
            Debug.Log("C Remove Mod 2 - prev");

            ActiveCharacterPreventionModifier preventionMod = modifier as ActiveCharacterPreventionModifier;
            Debug.Log("hash of the mod to delete: " + preventionMod.GetHashCode());
            Debug.Log("creatoe of the mod to delete: " + preventionMod.creator);
            Debug.Log("hash of creatoe of the mod to delete: " + preventionMod.creator.GetHashCode());
            foreach (ActiveCharacterPreventionModifier mod in activeCharacterPreventionModifiers)
            {
                Debug.Log("hash of the mod in collection: " + mod.GetHashCode());
                Debug.Log("creatoe of the mod in collection: " + mod.creator);
                Debug.Log("hash of creatoe of the mod in collection: " + mod.creator.GetHashCode());

                Debug.Log("EQUALS: " + (preventionMod.Equals( mod)));
            }
            Debug.Log("contains: " + activeCharacterPreventionModifiers.Contains(preventionMod));
            if (activeCharacterPreventionModifiers.Remove(preventionMod))
            {
                if (preventionMod.characterPreventionType == ActiveCharacterPreventionModifier.CharacterPreventionType.Stunned)
                {
                     OnRemoveStunModifier(); //only execute when the last stun is removed
                }
                else if (preventionMod.characterPreventionType == ActiveCharacterPreventionModifier.CharacterPreventionType.JumpingToTraverseOffMeshLink)
                {
                     OnRemoveTraversingOffMeshLinkPreventionModifier(); //only execute this when the stun starts, stun cant stack, mor stun modifiers can only increase the stun duration
                }
            }

           
        }


        /*if (modifier is MovementSpeedModifier)
        {
            activeMovementSpeedModifiers.Remove(modifier as MovementSpeedModifier);
        }
        else if (modifier is CharacterPreventionModifier)
        {
            Debug.Log("C Remove Mod 2 - prev");

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
                Debug.Log("C Remove Mod 3 - link");

                Debug.Log("hash of the mod to remove: " + modifier.GetHashCode());
                foreach (CharacterPreventionModifier mod in activeTraversingOffMeshLinkPreventionModifers)
                {
                    Debug.Log("hash of the mod there: " + mod.GetHashCode());
                }
                Debug.Log("contains: " + activeTraversingOffMeshLinkPreventionModifers.Contains(preventionMod));
                activeTraversingOffMeshLinkPreventionModifers.Remove(preventionMod);

                if (activeTraversingOffMeshLinkPreventionModifers.Count == 0)
                {
                    OnRemoveTraversingOffMeshLinkPreventionModifier(); //only execute this when the stun starts, stun cant stack, mor stun modifiers can only increase the stun duration
                }
            }
        }*/

    }

    void UpdateModifiers()
    {

        //Debug.Log("contains mov: " + activeMovementSpeedModifiers.Contains(staggerMovementSpeedModifier));

        //Debug.Log("Modifiers this frame----------------------------------------------------");
        foreach (ActiveCharacterMovementSpeedModifier modifier in activeMovementSpeedModifiers)
        {
            //Debug.Log("mod: " + modifier + " modifier.name: " + modifier.name);
            if (modifier.HasModifierTimeRunOut())
            {
                movementSpeedModifiersToDeleteThisFrame.Add(modifier);
            }
        }
        foreach (ActiveCharacterMovementSpeedModifier modifier in movementSpeedModifiersToDeleteThisFrame)
        {
            activeMovementSpeedModifiers.Remove(modifier);
        }
        movementSpeedModifiersToDeleteThisFrame.Clear();



        foreach (ActiveCharacterPreventionModifier modifier in activeCharacterPreventionModifiers)
        {
            //Debug.Log("mod: " + modifier + " modifier.name: " + modifier.name);

            if (modifier.HasModifierTimeRunOut())
            {
                characterPreventionModifiersToDeleteThisFrame.Add(modifier);
            }
        }
        foreach (ActiveCharacterPreventionModifier modifier in characterPreventionModifiersToDeleteThisFrame)
        {
            activeCharacterPreventionModifiers.Remove(modifier);
        }
        characterPreventionModifiersToDeleteThisFrame.Clear();



        /*foreach (CharacterPreventionModifier modifier in activeTraversingOffMeshLinkPreventionModifers)
        {
            // Debug.Log("mod: " + modifier + " modifier.name: " + modifier.name);

            if (modifier.HasModifierTimeRunOut())
            {
                traversingOffmeshLinkPreventionModifiersToDeleteThisFrame.Add(modifier);
            }
        }
        foreach (CharacterPreventionModifier modifier in traversingOffmeshLinkPreventionModifiersToDeleteThisFrame)
        {
            activeTraversingOffMeshLinkPreventionModifers.Remove(modifier);
        }
        traversingOffmeshLinkPreventionModifiersToDeleteThisFrame.Clear();*/


    }

    public HashSet<ActiveCharacterMovementSpeedModifier> GetActiveMovementSpeedModifiers()
    {
        return activeMovementSpeedModifiers;
    }



    public bool AreModifiersPreventing()
    {
        /*if (activeStunModifers.Count > 0)
        {
            if (activeTraversingOffMeshLinkPreventionModifers.Count > 0)
            {
                return true;
            }
        }*/
        if (activeCharacterPreventionModifiers.Count > 0)
        {
            return true;
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
        AbortReloadingWeapon();
        //StopAimAt();
        AbortChangingSelectedItem();
        //AbortThrowingGrenade();

        StopAimingSpine();
        StopAimingWeapon();

        //handsIKController.DisableIKs();
        handsIKController.OnStartTraversingOffMeshLink();

        Debug.Log("C AddModifier");

    }

    void OnRemoveTraversingOffMeshLinkPreventionModifier()
    {
        Debug.Log("C RemoveModifier Link");
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
