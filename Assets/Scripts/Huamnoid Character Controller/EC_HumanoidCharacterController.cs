using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class EC_HumanoidCharacterController : EntityComponent
{
    // Is the Interface between the Ai Controller and all the other Controllers like aiming movement etc...

    public HumanoidCharacterComponent[] components;

    #region Fields 

    [Header("References")]
    public HCC_HumanoidMovementController movementController;
    public HCC_HumanoidAnimationController animationController;
    public HCC_HumanoidInterationController interactionController;
    public HCC_HumanoidAimingController aimingController;
    public HCC_HumanoidHandsIKController handsIKController;

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

    #region Orders

    // orders keep informaiton of desired behaviour, even if modifiers prevent this behaviour
    class AimingControllerOrder
    {
        public bool active; // is the order active?

        public enum AimAtTargetingMethod
        {
            Direction,
            Position,
            Transform
        }

        public AimAtTargetingMethod orderTargetingMethod;

        public Vector3 desiredAimDirection;
        public Vector3 desiredAimPosition;
        public Transform desiredAimTransform;

        public AimingControllerOrder()
        {
            active = false;
        }


    }

    AimingControllerOrder lookAtOrder;
    AimingControllerOrder aimSpineOrder;
    AimingControllerOrder aimWeaponOrder;


    #endregion



    public override void SetUpComponent(GameEntity entity)
    {
        //Aim Orders
        lookAtOrder = new AimingControllerOrder();
        aimSpineOrder = new AimingControllerOrder();
        aimWeaponOrder = new AimingControllerOrder();

        base.SetUpComponent(entity);

        //1. Set Up components
        for (int i = 0; i < components.Length; i++)
        {
            components[i].SetUpComponent(myEntity);
        }

        SetUpCharacter();
    }
    public override void UpdateComponent()
    {
        // Debug.Log("look at active: " + lookAtOrder.active);
        // Debug.Log("aim spine active: " + aimSpineOrder.active);
        // Debug.Log("aim weapon active: " + aimWeaponOrder.active);

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
                    Debug.Log("g: " + gameObject.name + " move to ");
                    MoveTo(hit.point, true);

                }
            }
        }

        #endregion

        //1. Update components
        for (int i = 0; i < components.Length; i++)
        {
            components[i].UpdateComponent();
        }

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
        //Debug.Log("move to ----------------");
        //if (!AreModifiersPreventing())

        //if (DoModifersAllowMovement())
        //{
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
        //}
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
        // Set order
        lookAtOrder.active = true;
        lookAtOrder.orderTargetingMethod = AimingControllerOrder.AimAtTargetingMethod.Transform;
        lookAtOrder.desiredAimTransform = target;

        // If modifiers arent preventing, start executing Order
        if (DoModifiersAllowLookAt())
        {
            //aimingController.LookAtTransform(target);
            aimingController.LookAtTransform(lookAtOrder.desiredAimTransform);
        }
    }

    public void LookAtPosition(Vector3 position)
    {
        // Set order
        lookAtOrder.active = true;
        lookAtOrder.orderTargetingMethod = AimingControllerOrder.AimAtTargetingMethod.Position;
        lookAtOrder.desiredAimPosition = position;

        // If modifiers arent preventing, start executing Order
        if (DoModifiersAllowLookAt())
        {
            aimingController.LookAtPosition(lookAtOrder.desiredAimPosition);
        }
    }

    public void LookInDirection(Vector3 direction)
    {
        // Set order
        lookAtOrder.active = true;
        lookAtOrder.orderTargetingMethod = AimingControllerOrder.AimAtTargetingMethod.Direction;
        lookAtOrder.desiredAimDirection = direction;

        // If modifiers arent preventing, start executing Order
        if (DoModifiersAllowLookAt())
        {
            aimingController.LookInDirection(lookAtOrder.desiredAimDirection);
        }
    }

    public void StopLookAt()
    {
        lookAtOrder.active = false;

        aimingController.StopLookAt();
    }

    void StopLookAtByModifier()
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
        if (DoesCurrentStanceAllowAiming())
        {
            // Set order
            aimSpineOrder.active = true;
            aimSpineOrder.orderTargetingMethod = AimingControllerOrder.AimAtTargetingMethod.Transform;
            aimSpineOrder.desiredAimTransform = target;

            // If modifiers arent preventing, start executing Order
            if (DoModifersAllowAimingSpine())
            {
                aimingController.AimSpineAtTransform(aimSpineOrder.desiredAimTransform);
            }
        }
    }

    public void AimSpineAtPosition(Vector3 position)
    {
        if (DoesCurrentStanceAllowAiming())
        {
            // Set order
            aimSpineOrder.active = true;
            aimSpineOrder.orderTargetingMethod = AimingControllerOrder.AimAtTargetingMethod.Position;
            aimSpineOrder.desiredAimPosition = position;

            // If modifiers arent preventing, start executing Order
            if (DoModifersAllowAimingSpine())
            {
                aimingController.AimSpineAtPosition(aimSpineOrder.desiredAimPosition);
            }
        }

    }

    public void AimSpineInDirection(Vector3 direction)
    {
        if (DoesCurrentStanceAllowAiming())
        {
            // Set order
            aimSpineOrder.active = true;
            aimSpineOrder.orderTargetingMethod = AimingControllerOrder.AimAtTargetingMethod.Direction;
            aimSpineOrder.desiredAimDirection = direction;

            // If modifiers arent preventing, start executing Order
            if (DoModifersAllowAimingSpine())
            {
                aimingController.AimSpineInDirection(aimSpineOrder.desiredAimDirection);
            }
        }

    }

    public void StopAimingSpine()
    {
        aimSpineOrder.active = false;

        aimingController.StopAimSpineAtTarget();
    }
    void StopAimingSpineByModifier()
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
        if (DoesCurrentStanceAllowAiming())
        {
            if (interactionController.DoesCurrentItemInteractionStanceAllowAimingWeapon())
            {
                // Set order
                aimWeaponOrder.active = true;
                aimWeaponOrder.orderTargetingMethod = AimingControllerOrder.AimAtTargetingMethod.Transform;
                aimWeaponOrder.desiredAimTransform = target;

                // If modifiers arent preventing, start executing Order
                if (DoModifiersAllowAimingWeapon())
                {
                    aimingController.AimWeaponAtTransform(aimWeaponOrder.desiredAimTransform);
                }
            }
        }
    }

    public void AimWeaponAtPosition(Vector3 position)
    {
        if (DoesCurrentStanceAllowAiming())
        {
            if (DoesCurrentItemInteractionStanceAllowAimingWeapon())
            {
                // Set order
                aimWeaponOrder.active = true;
                aimWeaponOrder.orderTargetingMethod = AimingControllerOrder.AimAtTargetingMethod.Position;
                aimWeaponOrder.desiredAimPosition = position;

                // If modifiers arent preventing, start executing Order
                if (DoModifiersAllowAimingWeapon())
                {
                    aimingController.AimWeaponAtPosition(aimWeaponOrder.desiredAimPosition);
                }
            }
        }
    }

    public void AimWeaponInDirection(Vector3 direction)
    {
        if (DoesCurrentStanceAllowAiming())
        {
            if (DoesCurrentItemInteractionStanceAllowAimingWeapon())
            {
                // Set order
                aimWeaponOrder.active = true;
                aimWeaponOrder.orderTargetingMethod = AimingControllerOrder.AimAtTargetingMethod.Direction;
                aimWeaponOrder.desiredAimPosition = direction;

                // If modifiers arent preventing, start executing Order
                if (DoModifiersAllowAimingWeapon())
                {

                    aimingController.AimWeaponInDirection(aimWeaponOrder.desiredAimPosition);

                }
            }
        }
    }

    public void StopAimingWeapon()
    {
        aimWeaponOrder.active = false;

        aimingController.StopAimingWeaponAtTarget();
    }

    void StopAimingWeaponByModifier()
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
            //Debug.Log("allow true");
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

    public bool DoModifiersAllowMovement()
    {
        foreach (ActiveCharacterPreventionModifier prevMod in activeCharacterPreventionModifiers)
        {
            if (prevMod.characterPreventionType != ActiveCharacterPreventionModifier.CharacterPreventionType.Sprinting)
            {
                return false;
            }
        }

        return true;
    }

    public bool DoModifiersAllowLookAt()
    {
        foreach (ActiveCharacterPreventionModifier prevMod in activeCharacterPreventionModifiers)
        {
            if (prevMod.characterPreventionType != ActiveCharacterPreventionModifier.CharacterPreventionType.Sprinting)
            {
                return false;
            }
        }

        return true;
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
        StopAimingSpineByModifier();
        StopAimingWeaponByModifier();

        Debug.Log("On Add Stun");
    }

    void OnRemoveStunModifier()
    {
        Debug.Log("On Remove Stun");
        RemoveModifier(staggerMovementSpeedModifier.CreateAndActivateNewModifier());

        ReactivateAimingOrdersAfterRemovingModifier();


    }

    void OnAddTraversingOffMeshLinkPreventionModifier()
    {
        AbortReloadingWeapon();
        //StopAimAt();
        AbortChangingSelectedItem();
        //AbortThrowingGrenade();

        StopAimingSpineByModifier();
        StopAimingWeaponByModifier();

        //handsIKController.DisableIKs();
        handsIKController.OnStartTraversingOffMeshLink();

    }

    void OnRemoveTraversingOffMeshLinkPreventionModifier()
    {
        handsIKController.OnStopTraversingOffMeshLink();

        ReactivateAimingOrdersAfterRemovingModifier();
    }

    void ReactivateAimingOrdersAfterRemovingModifier()
    {
        Debug.Log("Reactivate aiming after removing");
        if (lookAtOrder.active)
        {
            Debug.Log("Reactivate aiming 1.1");

            if (DoModifiersAllowLookAt())
            {
                Debug.Log("Reactivate aiming 1.2");

                if (lookAtOrder.orderTargetingMethod == AimingControllerOrder.AimAtTargetingMethod.Transform)
                {
                    aimingController.LookAtTransform(lookAtOrder.desiredAimTransform);
                }
                else if (lookAtOrder.orderTargetingMethod == AimingControllerOrder.AimAtTargetingMethod.Position)
                {
                    aimingController.LookAtPosition(lookAtOrder.desiredAimPosition);
                }
                else if (lookAtOrder.orderTargetingMethod == AimingControllerOrder.AimAtTargetingMethod.Direction)
                {
                    aimingController.LookInDirection(lookAtOrder.desiredAimDirection);
                }
            }
        }

        if (aimSpineOrder.active)
        {
            if (DoModifiersAllowLookAt())
            {
                if (aimSpineOrder.orderTargetingMethod == AimingControllerOrder.AimAtTargetingMethod.Transform)
                {
                    aimingController.AimSpineAtTransform(aimSpineOrder.desiredAimTransform);
                }
                else if (aimSpineOrder.orderTargetingMethod == AimingControllerOrder.AimAtTargetingMethod.Position)
                {
                    aimingController.AimSpineAtPosition(aimSpineOrder.desiredAimPosition);
                }
                else if (aimSpineOrder.orderTargetingMethod == AimingControllerOrder.AimAtTargetingMethod.Direction)
                {
                    aimingController.AimSpineInDirection(aimSpineOrder.desiredAimDirection);
                }
            }
        }

        if (aimWeaponOrder.active)
        {
            if (DoModifiersAllowLookAt())
            {

                if (aimWeaponOrder.orderTargetingMethod == AimingControllerOrder.AimAtTargetingMethod.Transform)
                {
                    aimingController.AimWeaponAtTransform(aimWeaponOrder.desiredAimTransform);
                }
                else if (aimWeaponOrder.orderTargetingMethod == AimingControllerOrder.AimAtTargetingMethod.Position)
                {
                    aimingController.AimWeaponAtPosition(aimWeaponOrder.desiredAimPosition);
                }
                else if (aimWeaponOrder.orderTargetingMethod == AimingControllerOrder.AimAtTargetingMethod.Direction)
                {
                    aimingController.AimWeaponInDirection(aimWeaponOrder.desiredAimDirection);
                }
            }
        }



    }

    #endregion


}
