using FIMSpace.FLook;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class EC_HumanoidAimingController : EntityComponent
{
    // Is responsible for the correct aiming of the spine bones together with hands and parenting of weapons?  

    #region Calculate Aim At Position Fields

    [Header("Aim at Posiiton Calculation")]
    // Used to determin desired aiming direction - is this really needed?
    [Tooltip("If we are aiming at a direction, we are aiming at a point which is the direction * this float value")]
    public float defaultDirectionAimDistance;

    enum AimAtTargetingMethod
    {
        Direction,
        Position,
        Transform
    }

    AimAtTargetingMethod currentSpineTargetingMethod;
    AimAtTargetingMethod currentWeaponTargetingMethod;

    //For Spine Aiming
    bool aimingSpine;
    //Vector3 spineAimDirection;
    Vector3 spineDirectionToTarget;
    Vector3 spinePositionOfTarget;
    Transform spineTransformOfTarget;

    //for Weapon Aiming
    bool aimingWeapon; 
    Vector3 weaponDirectionToTarget;
    Vector3 weaponPositionOfTarget;
    Transform weaponTransformOfTarget;


    
    //Vector3 pointToAimAt;
    //Vector3 directionToAim;

    #endregion

    #region Other Fields

    [Header("Spine Constraint for Aiming up/down")]

    [Tooltip("The movement Controller is used to rotate to the sides")]
    public EC_HumanoidMovementController movementController;

    [Tooltip("Target for the animation rigging multi aim constraing (only forward und up - no sideways rotation)")]
    public Transform spineConstraintTarget;
    [Tooltip("Reference point on human body for aiming direction, can change later to be the gun? OR spine3 is good")]
    public Transform aimingReferencePointOnBody; //could be spine 3
    Vector3 desiredSpineDirection;
    Vector3 currentSpineDirection;
    Vector3 currentSpineDirectionChangeVelocity;

    public MultiAimConstraint spineConstraint1;
    public MultiAimConstraint spineConstraint2;
    public MultiAimConstraint spineConstraint3;

    float spineConstraint1TargetWeight;
    float spineConstraint2TargetWeight;
    float spineConstraint3TargetWeight;
    float spineConstraint1CurrentWeight;
    float spineConstraint2CurrentWeight;
    float spineConstraint3CurrentWeight;
    [Tooltip("Basically The Speed at which the spine Rotates Up and Down, the smaller the smooth time, the faster the rotation")]
    public float spineConstraintDirectionChangeSmoothTime = 0.1f;
    [Tooltip("To Start Or Stop aimingwith the spine, the weight of the multi aim constraints is set by this speed -> provides smooth enabling and disabling of the spine aiming")]
    public float spineConstraintWeightChangeSpeed = 0.5f;



    [Header("Look At")]

    public FLookAnimator fLookAnimator;

    // Look At distorts the weapon aiming when its 2 compensiation Weights are set on something different than 1, so we set the mto 1 while aiming ,and reset them afterwards.
    public float lookAtCompensationWeightWithoutWeapon;
    public float lookAtCompensatiePositionsWithoutWeapon;

    public float lookAtCompensationWeightWithWeapon;
    public float lookAtCompensatiePositionsWithWeapon;



    [Header("Idle Weapon Holding")]

    public Rig idleWeaponHoldingRig;



    [Header("Aiming the Weapon")]

    public Transform weaponAimTarget;
    public Gun weapon; //TODO needs to be set up while changing weapons, together with the iks for idle and aiming
    public Rig weaponAimingRig;

    float desiredWeaponAimingRigWeight;
    [Tooltip("Ensures a smooth transition between holding weapon idle and aiming")]
    public float changeFromAimingToIdleRigSpeed;
    public float changeFromIdleToAimingRigSpeed;
    [Tooltip("Weapons get parented to this object when aiming")]
    public Transform weaponAimParentLocalAdjuster;

    [Header("Weapon aim target movement")]
    [Tooltip("Max Rotation difference between current character forward and the target he aims at, to prrevent aiming too far to the side")]
    public float maxWeaponRotDifference;
    [Tooltip("speed used for Quaternion.RotateTowards()")]
    public float weaponAimRotationSpeed;
    [Tooltip("The smaller this value, the faster the weapon aims, it uses vector3.smoothDamp to smooth the movement a little bit")]
    public float aimingWeaponSmoothTime = 0.05f;
    Vector3 currentWeaponDirection;
    Vector3 desiredWeaponDirection;
    Vector3 weaponAimSpeedRef;

    #endregion

    public override void SetUpComponent(GameEntity entity)
    {
        base.SetUpComponent(entity);

        currentSpineDirection = transform.forward;

        weaponAimParentLocalAdjuster.localPosition = weapon.weaponAimParentLocalAdjusterOffset;
    }

    public override void UpdateComponent()
    {
        #region Aiming Spine

        #region Calculate desiredSpineDirection and spineConstraintTargetWeights

        if (aimingSpine)
        {
            // ---------------  1. Calculate Direction to aim at -----------
            // Basically all aim At Modes work the same that we have a point we are looking at in space, just the calculation of this point differs

            if (currentSpineTargetingMethod == AimAtTargetingMethod.Direction)
            {
                //pointToAimSpineAt = aimingReferencePointOnBody.position + spineAimDirection * defaultDirectionAimDistance;
                spineDirectionToTarget = spineDirectionToTarget;
            }
            else if (currentSpineTargetingMethod == AimAtTargetingMethod.Position)
            {
                //pointToAimSpineAt = spinePositionToAimAt;
                spineDirectionToTarget = spinePositionOfTarget - aimingReferencePointOnBody.position;
            }
            else if (currentSpineTargetingMethod == AimAtTargetingMethod.Transform)
            {
                //pointToAimSpineAt = spineTransformToAimAt.position;
                spineDirectionToTarget = spineTransformOfTarget.position - aimingReferencePointOnBody.position;
            }


            // -------------   2. Rotate Horizontally  -----------
            movementController.SetDesiredForward(spineDirectionToTarget.normalized); //do we need to normalize?


            // -------------   3. Calculate vertical desiredSpineDirection & set Weights ----------------

            // 3.1 Limit to rotation on the x axis only
            //We are kind of rotating the vector to point in the Forward Direction
            desiredSpineDirection = new Vector3(0, spineDirectionToTarget.y, new Vector2(spineDirectionToTarget.x, spineDirectionToTarget.z).magnitude);
            desiredSpineDirection = transform.TransformDirection(desiredSpineDirection);

            // 3.2 Set the target weight of spine constraints depending on if the target is above or below the shoulders
            if (spineConstraintTarget.position.y > aimingReferencePointOnBody.position.y)
            {
                // aiming Up 
                spineConstraint1TargetWeight = 0.05f;
                spineConstraint2TargetWeight = 0.2f;
                spineConstraint3TargetWeight = 0.4f;
            }
            else
            {
                // aiming Down
                spineConstraint1TargetWeight = 0.2f;
                spineConstraint2TargetWeight = 0.7f;
                spineConstraint3TargetWeight = 1f;
            }
        }
        else
        {
            // Reset aim Direction & Weight
            desiredSpineDirection = transform.forward;
            spineConstraint1TargetWeight = 0f;
            spineConstraint2TargetWeight = 0f;
            spineConstraint3TargetWeight = 0f;
        }

        #endregion

        #region Smoothly rotate spine towards desiredSpineDirection and smooth out the spineConstraint weights

        //  ----------   4.Rotate towards Aiming Direction with damping  ----------
        currentSpineDirection = Vector3.SmoothDamp(currentSpineDirection, desiredSpineDirection, ref currentSpineDirectionChangeVelocity, spineConstraintDirectionChangeSmoothTime); //Damping is static for now, no real velocity value
        spineConstraintTarget.position = aimingReferencePointOnBody.position + currentSpineDirection;

        // -----------   5 Smooth out Spine Constraints weight change ----------
        float maxSpeed = spineConstraintWeightChangeSpeed * Time.deltaTime;
        spineConstraint1CurrentWeight += Mathf.Clamp((spineConstraint1TargetWeight - spineConstraint1.weight), -maxSpeed, maxSpeed);
        spineConstraint1.weight = spineConstraint1CurrentWeight;
        spineConstraint2CurrentWeight += Mathf.Clamp((spineConstraint2TargetWeight - spineConstraint2.weight), -maxSpeed, maxSpeed);
        spineConstraint2.weight = spineConstraint2CurrentWeight;
        spineConstraint3CurrentWeight += Mathf.Clamp((spineConstraint3TargetWeight - spineConstraint3.weight), -maxSpeed, maxSpeed);
        spineConstraint3.weight = spineConstraint3CurrentWeight;

        #endregion

        #endregion

        #region Aiming Weapon

        #region Calculate desiredWeaponAimDirection 
        if (aimingWeapon)
        {
            // ---------------  1. Calculate Direction to aim at -----------

            if (currentWeaponTargetingMethod == AimAtTargetingMethod.Direction)
            {
                //pointToAimSpineAt = aimingReferencePointOnBody.position + spineAimDirection * defaultDirectionAimDistance;
                weaponDirectionToTarget = weaponDirectionToTarget;
            }
            else if (currentWeaponTargetingMethod == AimAtTargetingMethod.Position)
            {
                //pointToAimSpineAt = spinePositionToAimAt;
                weaponDirectionToTarget = weaponPositionOfTarget - aimingReferencePointOnBody.position;
            }
            else if (currentWeaponTargetingMethod == AimAtTargetingMethod.Transform)
            {
                //pointToAimSpineAt = spineTransformToAimAt.position;
                weaponDirectionToTarget = weaponTransformOfTarget.position - aimingReferencePointOnBody.position;
            }



            // Calculate & Rotate Weapon Aim Direction
            //desiredWeaponAimDirection = Vector3.RotateTowards(currentSpineDirection, spineDirectionToTarget, maxWeaponRotDifference * Mathf.Deg2Rad, 100);
            desiredWeaponDirection = Vector3.RotateTowards(currentSpineDirection, weaponDirectionToTarget, maxWeaponRotDifference * Mathf.Deg2Rad, 100);
           

        }
        else
        {
            desiredWeaponDirection = transform.forward;
        }

        #endregion

        #region Smoothly rotate the weapon towards desiredWeaponDirection & smooth out change in weaponAimingRig.weight

        currentWeaponDirection = Vector3.SmoothDamp(currentWeaponDirection, desiredWeaponDirection, ref weaponAimSpeedRef, aimingWeaponSmoothTime);
        weaponAimTarget.position = aimingReferencePointOnBody.position + currentWeaponDirection;

        // Smooth out the change between aiming weapon and holding it idle
        //Two different speeds to prevent gun from floating around between hands
        if (desiredWeaponAimingRigWeight == 1)
        {
            float changeSpeed = changeFromIdleToAimingRigSpeed * Time.deltaTime;
            weaponAimingRig.weight += Mathf.Clamp((desiredWeaponAimingRigWeight - weaponAimingRig.weight), -changeSpeed, changeSpeed);
        }
        else if (desiredWeaponAimingRigWeight == 0)
        {
            float changeSpeed = changeFromAimingToIdleRigSpeed * Time.deltaTime;
            weaponAimingRig.weight += Mathf.Clamp((desiredWeaponAimingRigWeight - weaponAimingRig.weight), -changeSpeed, changeSpeed);
        }

        #endregion

        #endregion
    }

    public void AimSpineInDirection(Vector3 direction)
    {
        Debug.Log("aim direction 3: " + direction);
        Debug.Log("aim direction 3: currentDirection" + currentSpineDirection);
        Debug.Log("aim direction 3: currentDirection" + desiredSpineDirection);
        aimingSpine = true;
        currentSpineTargetingMethod = AimAtTargetingMethod.Direction;
        movementController.manualRotation = true;
        spineDirectionToTarget = direction;
    }

    public void AimSpineAtPosition(Vector3 position)
    {
        aimingSpine = true;
        currentSpineTargetingMethod = AimAtTargetingMethod.Position;
        movementController.manualRotation = true;
        spinePositionOfTarget = position;

    }

    public void AimSpineAtTransform(Transform transform)
    {
        aimingSpine = true;
        currentSpineTargetingMethod = AimAtTargetingMethod.Transform;
        movementController.manualRotation = true;
        spineTransformOfTarget = transform;
    }

    public void StopAimSpineAtTarget()
    {
        aimingSpine = false;
        movementController.manualRotation = false;
    }

    public void LookAtTransform(Transform target)
    {
        fLookAnimator.ObjectToFollow = target;
    }

    public void StopLookAt()
    {
        fLookAnimator.ObjectToFollow = null;
    }

    public void AimWeaponInDirection(Vector3 direction)
    {
        aimingWeapon = true;
        currentWeaponTargetingMethod = AimAtTargetingMethod.Direction;
        weaponDirectionToTarget = direction;
        desiredWeaponAimingRigWeight = 1;
    }

    public void AimWeaponAtPosition(Vector3 position)
    {
        aimingWeapon = true;
        currentWeaponTargetingMethod = AimAtTargetingMethod.Position;
        weaponPositionOfTarget = position;
        desiredWeaponAimingRigWeight = 1;
    }

    public void AimWeaponAtTransform(Transform transform)
    {
        aimingWeapon = true;
        currentWeaponTargetingMethod = AimAtTargetingMethod.Transform;
        weaponTransformOfTarget = transform;
        desiredWeaponAimingRigWeight = 1;
    }

    public void StopAimingWeaponAtTarget()
    {
        aimingWeapon = false;
        desiredWeaponAimingRigWeight = 0;
    }

    public void OnChangeWeapon(Gun newWeapon)
    {
        if (newWeapon == null)
        {
            weapon = null;

            fLookAnimator.CompensationWeight = lookAtCompensationWeightWithoutWeapon;
            fLookAnimator.CompensatePositions = lookAtCompensatiePositionsWithoutWeapon;
        }
        else
        {
            weapon = newWeapon;
            weaponAimParentLocalAdjuster.localPosition = weapon.weaponAimParentLocalAdjusterOffset;

            fLookAnimator.CompensationWeight = lookAtCompensationWeightWithWeapon;
            fLookAnimator.CompensatePositions = lookAtCompensatiePositionsWithWeapon;
        }
    }

    public bool IsCharacterAimingWeapon()
    {
        return aimingWeapon;
    }

    public bool IsCharacterAimingSpine()
    {
        return aimingSpine;
    }

    public bool IsCharacterLookingAtTarget()
    {
        return fLookAnimator.ObjectToFollow != null;
    }
    public Vector3 GetCurrentSpineAimDirection()
    {
        return currentSpineDirection;
    }

    // What is the difference between desired and current spine direction
    public float GetCurrentSpineAimingErrorAngle()
    {
        return Vector3.Angle(desiredSpineDirection,currentSpineDirection);
    }

    public Vector3 GetCurrentWeaponAimDirection()
    {
        return currentWeaponDirection;
    }

    public float GetCurrentWeaponAimingErrorAngle()
    {
        return Vector3.Angle(desiredWeaponDirection, currentWeaponDirection);
    }
}
