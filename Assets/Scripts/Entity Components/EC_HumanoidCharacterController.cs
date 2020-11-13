using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

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

    bool isSprinting; //cached here

    #endregion

    public ActiveCharacterPreventionModifier[] activePrevMods;


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
                LookAtTransform(lookAtTarget);
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

    /*public void MoveTo(Vector3 destination)
    {
        //if (!AreModifiersPreventing())
        if (DoModifersAllowMovement())
        {
            movementController.MoveTo(destination, false);
        }
    }*/


    public void MoveTo(Vector3 destination, bool sprint = false)
    {
        //if (!AreModifiersPreventing())
        if (DoModifersAllowMovement())
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
        return movementController.IsSprintingAccordingToOrder();
    }

    public float GetRemainingDistanceToCurrentMovementTarget()
    {
        return movementController.GetRemainingDistance();
    }

    #endregion


    #region Look At Orders

    public void LookAtTransform(Transform target)
    {
        //if (!AreModifiersPreventing())
        if (DoModifersAllowLookAt())
        {
            aimingController.LookAtTransform(target);
        }
    }

    public void LookAtPosition(Vector3 position)
    {
        //if (!AreModifiersPreventing())
        if (DoModifersAllowLookAt())
        {
            aimingController.LookAtPosition(position);
        }
    }

    public void LookInDirection(Vector3 direction)
    {
        //if (!AreModifiersPreventing())
        if (DoModifersAllowLookAt())
        {
            aimingController.LookInDirection(direction);
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
        //if (!AreModifiersPreventing())
        if (DoModifersAllowAimingSpine())
        {
            if (DoesCurrentStanceAllowAiming())
            {
                aimingController.AimSpineAtTransform(target);

                /*if (movementController.IsSprintingAccordingToOrder())
                {
                    movementController.SetSprint(false);
                }*/
            }
        }
    }

    public void AimSpineAtPosition(Vector3 position)
    {
        //if (!AreModifiersPreventing())
        if (DoModifersAllowAimingSpine())
        {
            if (DoesCurrentStanceAllowAiming())
            {
                aimingController.AimSpineAtPosition(position);

               /* if (movementController.IsSprintingAccordingToOrder())
                {
                    movementController.SetSprint(false);
                }*/
            }
        }

    }

    public void AimSpineInDirection(Vector3 direction)
    {
        //if (!AreModifiersPreventing())
        if (DoModifersAllowAimingSpine())
        {
            if (DoesCurrentStanceAllowAiming())
            {
                aimingController.AimSpineInDirection(direction);

               /* if (movementController.IsSprintingAccordingToOrder())
                {
                    movementController.SetSprint(false);
                }*/
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

    #region Aim Weapon Orders 

    public void AimWeaponAtTransform(Transform target)
    {
        //if (!AreModifiersPreventing())
        if (DoModifiersAllowAimingWeapon())
        {
            if (DoesCurrentStanceAllowAiming())
            {
                if (interactionController.DoesCurrentItemInteractionStanceAllowAimingWeapon())
                {
                    aimingController.AimWeaponAtTransform(target);

                    //if (isSprinting)
                    //{
                        //movementController.SetSprint(false);
                    //}
                }
            }
        }
    }

    public void AimWeaponAtPosition(Vector3 position)
    {
        // if (!AreModifiersPreventing())
        if (DoModifiersAllowAimingWeapon())
        {
            if (DoesCurrentStanceAllowAiming())
            {
                if (DoesCurrentItemInteractionStanceAllowAimingWeapon())
                {
                    aimingController.AimWeaponAtPosition(position);

                    //if (isSprinting)
                    //{
                        //movementController.SetSprint(false);
                    //}
                }
            }
        }
    }

    public void AimWeaponInDirection(Vector3 direction)
    {
        //if (!AreModifiersPreventing())
        if(DoModifiersAllowAimingWeapon())
        {
            if (DoesCurrentStanceAllowAiming())
            {
                if (DoesCurrentItemInteractionStanceAllowAimingWeapon())
                {
                    aimingController.AimWeaponInDirection(direction);

                    /*if (isSprinting)
                    {*/
                        //movementController.SetSprint(false);
                    //}
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
        if (DoModifiersAllowItemInteraction())
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
        //if (!AreModifiersPreventing())
        if (DoModifiersAllowItemInteraction())
        {
            interactionController.ShootWeapon();
        }
    }


    public void StartReloadingWeapon()
    {
        //if (!AreModifiersPreventing())
        if (DoModifiersAllowItemInteraction())
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
        // if (!AreModifiersPreventing())
        if (DoModifiersAllowItemInteraction())
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
            ActiveCharacterModifier stunMod = AddModifier(stunModifier);
            animationController.Stagger(stunMod.currentModifierDuration);
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

    public ActiveCharacterModifier AddModifier(CharacterModifierCreator creator, float delay = 0)
    {
        ActiveCharacterModifier mod = creator.CreateAndActivateNewModifier();

        if (delay > 0)
        {
            StartCoroutine(AddModifierDelayed(mod, delay));
        }
        else
        {
            AddModifier(mod);
        }
        return mod;

    }

    IEnumerator AddModifierDelayed(ActiveCharacterModifier modifier, float delay)
    {
        yield return new WaitForSeconds(delay);

        AddModifier(modifier);
    }

    void AddModifier(ActiveCharacterModifier modifier)
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

        //StopCoroutine(RemoveModifierDelayed(modifier,0));
    }


    public ActiveCharacterModifier RemoveModifier(CharacterModifierCreator creator, float delay = 0)
    {
        ActiveCharacterModifier mod = creator.CreateAndActivateNewModifier();

        if (delay > 0)
        {
            StartCoroutine(RemoveModifierDelayed(mod, delay));
        }
        else
        {
            RemoveModifier(mod);
        }

        return mod;
    }

    IEnumerator RemoveModifierDelayed(ActiveCharacterModifier modifier, float delay)
    {
        yield return new WaitForSeconds(delay);

        RemoveModifier(modifier);
    }

    void RemoveModifier(ActiveCharacterModifier modifier)
    {
        //Debug.Log("remove modifier: " + modifier.name);

        if (modifier is ActiveCharacterMovementSpeedModifier)
        {
            ActiveCharacterMovementSpeedModifier preventionMod = modifier as ActiveCharacterMovementSpeedModifier;

            activeMovementSpeedModifiers.Remove(modifier as ActiveCharacterMovementSpeedModifier);
        }
        else if (modifier is ActiveCharacterPreventionModifier)
        {
            ActiveCharacterPreventionModifier preventionMod = modifier as ActiveCharacterPreventionModifier;

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
    }

  

    void UpdateModifiers()
    {
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
            RemoveModifier(modifier);
        }
        movementSpeedModifiersToDeleteThisFrame.Clear();

        activePrevMods = new ActiveCharacterPreventionModifier[activeCharacterPreventionModifiers.Count];
        int it = 0;

        foreach (ActiveCharacterPreventionModifier modifier in activeCharacterPreventionModifiers)
        {
            activePrevMods[it] = modifier;
            it++;
            //Debug.Log("mod: " + modifier + " modifier.name: " + modifier.name + "remaining duration");
            /*if(modifier.characterPreventionType == ActiveCharacterPreventionModifier.CharacterPreventionType.Sprinting)
            {
                isSprinting = true;
            }
            else
            {
                isSprinting = false;
            }*/

            if (modifier.HasModifierTimeRunOut())
            {
                characterPreventionModifiersToDeleteThisFrame.Add(modifier);
            }
        }
        foreach (ActiveCharacterPreventionModifier modifier in characterPreventionModifiersToDeleteThisFrame)
        {
            RemoveModifier(modifier);
        }
        characterPreventionModifiersToDeleteThisFrame.Clear();
    }

    public HashSet<ActiveCharacterMovementSpeedModifier> GetActiveMovementSpeedModifiers()
    {
        return activeMovementSpeedModifiers;
    }



    public bool DoModifiersAllowItemInteraction()
    {
        if (activeCharacterPreventionModifiers.Count == 0)
        {
            Debug.Log("allow true");
            return true;
        }
        /*else
        {
            Debug.Log("allow false");
            foreach (ActiveCharacterPreventionModifier prevMod in activeCharacterPreventionModifiers)
            {
                Debug.Log("active prev mod: " + prevMod.characterPreventionType);
            }
        }*/

        return false;

    }

    public bool DoModifersAllowMovement()
    {
        bool allows = true;
        foreach (ActiveCharacterPreventionModifier prevMod in activeCharacterPreventionModifiers)
        {
            if (prevMod.characterPreventionType != ActiveCharacterPreventionModifier.CharacterPreventionType.Sprinting)
            {
                allows = false;
                return allows;
            }
        }

        return allows;
    }

    public bool DoModifersAllowLookAt()
    {
        bool allows = true;
        foreach (ActiveCharacterPreventionModifier prevMod in activeCharacterPreventionModifiers)
        {
            if (prevMod.characterPreventionType != ActiveCharacterPreventionModifier.CharacterPreventionType.Sprinting)
            {
                allows = false;
                return allows;
            }
        }

        return allows;
    }

    public bool DoModifersAllowAimingSpine()
    {
        /*bool allows = true;
        foreach (ActiveCharacterPreventionModifier prevMod in activeCharacterPreventionModifiers)
        {
            if (prevMod.characterPreventionType != ActiveCharacterPreventionModifier.CharacterPreventionType.Sprinting)
            {
                allows = false;
                return allows;
            }
        }

        return allows;*/
        if (activeCharacterPreventionModifiers.Count == 0)
        {
            //Debug.Log("modifiers allow aiming");
            return true;

        }
        else
        {
            //Debug.Log("modifiers dont allow aiming");
            return false;
        }
    }

    public bool DoModifiersAllowAimingWeapon()
    {
        //Rework this - will always result in false?
        //bool allows = DoModifersAllowMovement();
        //bool allows = true;

        if (activeCharacterPreventionModifiers.Count == 0)
        {
            //Debug.Log("modifiers allow aiming");
            return true;
            
        }
        else
        {
            //Debug.Log("modifiers dont allow aiming");
            return false;
        }

        /*foreach (ActiveCharacterPreventionModifier prevMod in activeCharacterPreventionModifiers)
        {
            if (prevMod.characterPreventionType != ActiveCharacterPreventionModifier.CharacterPreventionType.Sprinting)
            {
                allows = false;
                return allows;
            }
        }*/


        //return allows;



    }


    void OnAddStunModifier()
    {
        AbortReloadingWeapon();
        // StopAimAt();
        AbortChangingSelectedItem();
        AbortThrowingGrenade();
        StopAimingSpine();
        StopAimingWeapon();

        Debug.Log("On Add Stun");
    }

    void OnRemoveStunModifier()
    {
        Debug.Log("On Remove Stun");
        RemoveModifier(staggerMovementSpeedModifier.CreateAndActivateNewModifier());

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


    }

    void OnRemoveTraversingOffMeshLinkPreventionModifier()
    {
        handsIKController.OnStopTraversingOffMeshLink();

    }

    #endregion


}
