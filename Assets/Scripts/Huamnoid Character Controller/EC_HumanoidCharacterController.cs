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
    public enum CharacterStance
    {
        StandingIdle,
        StandingCombatStance,
        Crouching,
    }
    CharacterStance currentStance;

    bool isSprinting; //cached here

    #endregion

    public ActiveCharacterPreventionModifier[] activePrevMods;

    #region Orders

    class Order
    {
        public enum ExecutionStatus
        {
            NoOrder,
            WaitingForExecution, //when paused
            BeingExecuted //when executing
        }

        public ExecutionStatus executionStatus;

        public Order()
        {
            // active = false;
            executionStatus = ExecutionStatus.NoOrder;
        }
    }

    // orders keep informaiton of desired behaviour, even if modifiers prevent this behaviour
    class AimingControllerOrder: Order
    {
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

    }

    class ChangeWeaponOrder: Order
    {
        public int targetItemID;
    }

    class ReloadWeaponOrder: Order
    {

    }

    AimingControllerOrder lookAtOrder = new AimingControllerOrder();
    AimingControllerOrder aimSpineOrder = new AimingControllerOrder();
    AimingControllerOrder aimWeaponOrder = new AimingControllerOrder();

    ChangeWeaponOrder changeWeaponOrder = new ChangeWeaponOrder();

    ReloadWeaponOrder reloadWeaponOrder = new ReloadWeaponOrder();


    #endregion



    public override void SetUpComponent(GameEntity entity)
    {

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
                ChangeCharacterStanceToStandingIdle();
            }
            if (Input.GetKeyDown(KeyCode.X))
            {
                ChangeCharacterStanceToStandingCombatStance();
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
                //StartThrowingGrenade(5f, transform.forward);
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

        // 3. Update Orders
        UpdateOrders();
    }

    void UpdateOrders()
    {
        #region Update Aiming Orders ([ordered termination] - they can only be terminated by a stopping order)

        if (lookAtOrder.executionStatus == Order.ExecutionStatus.WaitingForExecution)
        {
            if (DoModifiersAllowLookAt())
            {
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

                lookAtOrder.executionStatus = Order.ExecutionStatus.BeingExecuted;
            }
        }

        if (aimSpineOrder.executionStatus == Order.ExecutionStatus.WaitingForExecution)
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

                aimSpineOrder.executionStatus = Order.ExecutionStatus.BeingExecuted;

            }
        }

        if (aimWeaponOrder.executionStatus == Order.ExecutionStatus.WaitingForExecution)
        {
            if (DoModifiersAllowLookAt())
            {
                if (interactionController.DoesCurrentItemInteractionStanceAllowAimingWeapon())
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

                    aimWeaponOrder.executionStatus = Order.ExecutionStatus.BeingExecuted;
                }
            }
        }

        #endregion

        #region Update Item Interaction Orders ([automatic termination] - they trminate automatically if requirements have been met)

        // -------------------------- Change Weapon ------------------------
        if(changeWeaponOrder.executionStatus == Order.ExecutionStatus.WaitingForExecution)
        {
            if (DoModifiersAllowItemInteraction())
            {
                //stop aiming weapon if itemChangeInitiated
                if (interactionController.ChangeItemInHand(changeWeaponOrder.targetItemID))
                {
                    if (IsAimingWeapon())
                    {
                        StopAimingWeapon();
                    }

                    changeWeaponOrder.executionStatus = Order.ExecutionStatus.BeingExecuted;
                }
            }
        }
        else if(changeWeaponOrder.executionStatus == Order.ExecutionStatus.BeingExecuted)
        {
            //if change is finished, terminate order
            if(GetCurrentlySelectedItem() == GetItemInInventory(changeWeaponOrder.targetItemID))
            {
                interactionController.AbortChangingItemInHand();
                changeWeaponOrder.executionStatus = Order.ExecutionStatus.NoOrder;
            }
        }

        // -------------------------- Reload Weapon ------------------------
        if(reloadWeaponOrder.executionStatus == Order.ExecutionStatus.WaitingForExecution)
        {
            if (DoModifiersAllowItemInteraction())
            {
                if (IsAimingWeapon())
                {
                    StopAimingWeapon();
                }
                interactionController.StartReloadingWeapon();
                reloadWeaponOrder.executionStatus = Order.ExecutionStatus.BeingExecuted;
            }
        }
        else if(reloadWeaponOrder.executionStatus == Order.ExecutionStatus.BeingExecuted)
        {
            if(GetAmmoRemainingInMagazineRatio() == 1f)
            {
                interactionController.AbortReloadingWeapon();
                reloadWeaponOrder.executionStatus = Order.ExecutionStatus.NoOrder;
            }
        }



        #endregion
    }

    #region Changing Character Stances Orders

    void SetUpCharacter()
    {
        // the same as ChangeCharacterStanceToIdle but isnt blocked if already in idle stance
        currentStance = CharacterStance.StandingIdle;
        movementController.SetDefaultSpeed(idleWalkingSpeed);
        movementController.SetSprintSpeed(idleSprintingSpeed);
        movementController.SetStationaryTurnSpeed(idleStationaryTurnSpeed);
        movementController.SetAcceleration(idleAcceleration);
        animationController.ChangeToIdleStance();
        handsIKController.OnEnterIdleStance();

        StopAimingSpine();
        StopAimingWeapon();
    }

    public void ChangeCharacterStanceToStandingIdle()
    {
        if (currentStance != CharacterStance.StandingIdle)
        {
            currentStance = CharacterStance.StandingIdle;
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

    public void ChangeCharacterStanceToStandingCombatStance()
    {

        if (currentStance != CharacterStance.StandingCombatStance)
        {
            if (interactionController.DoesCurrentItemInHandAllowCombatStance())
            {
                currentStance = CharacterStance.StandingCombatStance;
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
        if (currentStance == CharacterStance.StandingIdle)
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
            //else
            //{
                //StopAimingSpine();
                //StopAimingWeapon();
            //}
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
        lookAtOrder.executionStatus = Order.ExecutionStatus.WaitingForExecution;
        lookAtOrder.orderTargetingMethod = AimingControllerOrder.AimAtTargetingMethod.Transform;
        lookAtOrder.desiredAimTransform = target;
    }

    public void LookAtPosition(Vector3 position)
    {
        // Set order
        lookAtOrder.executionStatus = Order.ExecutionStatus.WaitingForExecution;
        lookAtOrder.orderTargetingMethod = AimingControllerOrder.AimAtTargetingMethod.Position;
        lookAtOrder.desiredAimPosition = position;
    }

    public void LookInDirection(Vector3 direction)
    {
        // Set order
        lookAtOrder.executionStatus = Order.ExecutionStatus.WaitingForExecution;
        lookAtOrder.orderTargetingMethod = AimingControllerOrder.AimAtTargetingMethod.Direction;
        lookAtOrder.desiredAimDirection = direction;
    }

    public void StopLookAt()
    {
        aimingController.StopLookAt();
        lookAtOrder.executionStatus = Order.ExecutionStatus.NoOrder;
    }

    void BlockLookAtByModifier()
    {
        aimingController.StopLookAt();

        if (lookAtOrder.executionStatus == Order.ExecutionStatus.BeingExecuted)
        {
            lookAtOrder.executionStatus = Order.ExecutionStatus.WaitingForExecution;
        }
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
            aimSpineOrder.executionStatus = Order.ExecutionStatus.WaitingForExecution;
            aimSpineOrder.orderTargetingMethod = AimingControllerOrder.AimAtTargetingMethod.Transform;
            aimSpineOrder.desiredAimTransform = target;
        }
    }

    public void AimSpineAtPosition(Vector3 position)
    {
        if (DoesCurrentStanceAllowAiming())
        {
            aimSpineOrder.executionStatus = Order.ExecutionStatus.WaitingForExecution;
            aimSpineOrder.orderTargetingMethod = AimingControllerOrder.AimAtTargetingMethod.Position;
            aimSpineOrder.desiredAimPosition = position;
        }

    }

    public void AimSpineInDirection(Vector3 direction)
    {
        if (DoesCurrentStanceAllowAiming())
        {
            aimSpineOrder.executionStatus = Order.ExecutionStatus.WaitingForExecution;
            aimSpineOrder.orderTargetingMethod = AimingControllerOrder.AimAtTargetingMethod.Direction;
            aimSpineOrder.desiredAimDirection = direction;
        }

    }

    public void StopAimingSpine()
    {
        aimingController.StopAimSpineAtTarget();
        aimSpineOrder.executionStatus = Order.ExecutionStatus.NoOrder;
    }

    void BlockAimingSpineByModifier()
    {
        aimingController.StopAimSpineAtTarget();

        if (aimSpineOrder.executionStatus == Order.ExecutionStatus.BeingExecuted)
        {
            aimSpineOrder.executionStatus = Order.ExecutionStatus.WaitingForExecution;
        }
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
            aimWeaponOrder.executionStatus = Order.ExecutionStatus.WaitingForExecution;
            aimWeaponOrder.orderTargetingMethod = AimingControllerOrder.AimAtTargetingMethod.Transform;
            aimWeaponOrder.desiredAimTransform = target;
        }
    }

    public void AimWeaponAtPosition(Vector3 position)
    {
        if (DoesCurrentStanceAllowAiming())
        {
            aimWeaponOrder.executionStatus = Order.ExecutionStatus.WaitingForExecution;
            aimWeaponOrder.orderTargetingMethod = AimingControllerOrder.AimAtTargetingMethod.Position;
            aimWeaponOrder.desiredAimPosition = position;
        }
    }

    public void AimWeaponInDirection(Vector3 direction)
    {
        if (DoesCurrentStanceAllowAiming())
        {
            aimWeaponOrder.executionStatus = Order.ExecutionStatus.WaitingForExecution;
            aimWeaponOrder.orderTargetingMethod = AimingControllerOrder.AimAtTargetingMethod.Direction;
            aimWeaponOrder.desiredAimDirection = direction;
        }
    }

    public void StopAimingWeapon()
    {
        aimingController.StopAimingWeaponAtTarget();
        aimWeaponOrder.executionStatus = Order.ExecutionStatus.NoOrder;
    }

    void BlockAimingWeaponByModifier()
    {
        aimingController.StopAimingWeaponAtTarget();

        if (aimWeaponOrder.executionStatus == Order.ExecutionStatus.BeingExecuted)
        {
            aimWeaponOrder.executionStatus = Order.ExecutionStatus.WaitingForExecution;
        }
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
        if(changeWeaponOrder.targetItemID != inventoryID)
        {
            changeWeaponOrder.executionStatus = Order.ExecutionStatus.WaitingForExecution;
            changeWeaponOrder.targetItemID = inventoryID;
        }            
    }

    public void AbortChangingSelectedItem()
    {
        interactionController.AbortChangingItemInHand();
        changeWeaponOrder.executionStatus = Order.ExecutionStatus.NoOrder;
    }

    void BlockChangingSelectedItemByModifier()
    {
        interactionController.AbortChangingItemInHand();

        if(changeWeaponOrder.executionStatus == Order.ExecutionStatus.BeingExecuted)
        {
            changeWeaponOrder.executionStatus = Order.ExecutionStatus.WaitingForExecution;
        }
    }

    #endregion

    #region Item Interaction Info

    public Item GetCurrentlySelectedItem()
    {
        return interactionController.GetCurrentSelectedItem();
    }

    public int GetCurrentSelectedItemID()
    {
        return interactionController.currentSelectedItemID;
    }

    public Item GetItemInInventory(int inventoryPosition)
    {
        return interactionController.GetItemInInventory(inventoryPosition);
    }

    public bool DoesCurrentItemInteractionStanceAllowAimingWeapon()
    {
        return interactionController.DoesCurrentItemInteractionStanceAllowAimingWeapon();
    }

    public bool DoesCurrentItemInteractionStanceAllowAimingSpine()
    {
        return interactionController.DoesCurrentItemInteractionStanceAllowAimingSpine();
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
        reloadWeaponOrder.executionStatus = Order.ExecutionStatus.WaitingForExecution;   
    }

    public void AbortReloadingWeapon()
    {
        interactionController.AbortReloadingWeapon();
        reloadWeaponOrder.executionStatus = Order.ExecutionStatus.NoOrder;
    }

    void BlockReloadingWeaponByModifier()
    {
        interactionController.AbortReloadingWeapon();

        if(reloadWeaponOrder.executionStatus == Order.ExecutionStatus.BeingExecuted)
        {
            reloadWeaponOrder.executionStatus = Order.ExecutionStatus.WaitingForExecution;
        }
    }

    public bool IsReloadingWeapon()
    {
        return interactionController.IsReloadingWeapon();
    }


    public void StartThrowingGrenade()//float throwVelocity, Vector3 throwDirection)
    {
        // if (!AreModifiersPreventing())
        if (DoModifiersAllowItemInteraction())
        {
            interactionController.StartThrowingGrenade();//throwVelocity, throwDirection);
        }
    }

    public void UpdateVelocityWhileThrowingGrenade(float currentGrenadeThrowVelocity, Vector3 currentGrenadeThrowDirection)
    {
        interactionController.UpdateVelocityWhileThrowingGrenade(currentGrenadeThrowVelocity, currentGrenadeThrowDirection);
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

    public float GetAmmoRemainingInMagazineRatio()
    {
        //return ration between 0 and 1
        return interactionController.GetAmmoRemainingInMagazineRatio();
    }

    public float GetAmmoRemainingInMagazineRatio(int weaponID)
    {
        //return ration between 0 and 1
        return interactionController.GetAmmoRemainingInMagazineRatio(weaponID);
    }

    public bool IsThrowingGrenade()
    {
        return interactionController.IsThrowingGrenade();
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

            if (preventionMod.characterPreventionType == ActiveCharacterPreventionModifier.CharacterPreventionType.Sprinting)
            {
                OnAddSprintingModifier();
            }
            else if (preventionMod.characterPreventionType == ActiveCharacterPreventionModifier.CharacterPreventionType.Stunned)
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
                if (preventionMod.characterPreventionType == ActiveCharacterPreventionModifier.CharacterPreventionType.Sprinting)
                {
                    OnRemoveSprintingModifier();
                }
                else if (preventionMod.characterPreventionType == ActiveCharacterPreventionModifier.CharacterPreventionType.Stunned)
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
        if (activeCharacterPreventionModifiers.Count == 0)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public bool DoModifiersAllowAimingWeapon()
    {
        if (activeCharacterPreventionModifiers.Count == 0)
        {
            return true;

        }
        else
        {
            return false;
        }
    }

    void OnAddSprintingModifier()
    {
        BlockAimingSpineByModifier();
        BlockAimingWeaponByModifier();
    }

    void OnRemoveSprintingModifier()
    {

    }

    void OnAddStunModifier()
    {
        //AbortReloadingWeapon();
        BlockReloadingWeaponByModifier();
        // StopAimAt();
        //AbortChangingSelectedItem();
        BlockChangingSelectedItemByModifier();
        AbortThrowingGrenade();
        BlockAimingSpineByModifier();
        BlockAimingWeaponByModifier();

        Debug.Log("On Add Stun");
    }

    void OnRemoveStunModifier()
    {
        Debug.Log("On Remove Stun");
        RemoveModifier(staggerMovementSpeedModifier.CreateAndActivateNewModifier());
    }


    void OnAddTraversingOffMeshLinkPreventionModifier()
    {
        //AbortReloadingWeapon();
        BlockReloadingWeaponByModifier();
        //StopAimAt();
        //AbortChangingSelectedItem();
        BlockChangingSelectedItemByModifier();
        //AbortThrowingGrenade();

        BlockAimingSpineByModifier();
        BlockAimingWeaponByModifier();

        //handsIKController.DisableIKs();
        handsIKController.OnStartTraversingOffMeshLink();

    }

    void OnRemoveTraversingOffMeshLinkPreventionModifier()
    {
        handsIKController.OnStopTraversingOffMeshLink();
    }

    #endregion


}
