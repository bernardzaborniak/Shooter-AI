using FIMSpace.FLook;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

// Positions the spineAimTarget, weaponAimTarget and the lookAtTarget
// Is responsible, together with the constraintController for the correct aiming of the spine bones together with hands and parenting of weapons
public class EC_HumanoidAimingController : EntityComponent
{
    [Header("References")]
    public HumanoidConstraintController constraintController;
    public EC_HumanoidHandsIKController handsIKController;

    #region For Calculating the desired Direction in Different ways

    [Header("Aim at Position Calculation")]
    // Used to determine desired aiming direction - is this really needed?
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
    AimAtTargetingMethod currentLookAtTargetingMethod; //TODO ADD Look At Differeent Targeting methods

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

    #endregion

    #region For Calculating Spine Target Position & Weight

    [Header("Spine Constraint for Aiming up/down")]

    [Tooltip("Target for the animation rigging multi aim constraing (only forward und up - no sideways rotation)")]
    public Transform spineConstraintLocalTarget;
    [Tooltip("The movement Controller is used to rotate to the sides")]
    public EC_HumanoidMovementController movementController;
    [Tooltip("Reference point on human body for aiming direction, can change later to be the gun? OR spine3 is good")]
    public Transform aimingReferencePointOnBody; //could be spine 3
    Vector3 desiredSpineDirection;
    Vector3 currentSpineDirection;
    //Vector3 currentSpineDirectionChangeVelocity;

    float spineConstraint1TargetWeight;
    float spineConstraint2TargetWeight;
    float spineConstraint3TargetWeight;
    float spineConstraint1CurrentWeight;
    float spineConstraint2CurrentWeight;
    float spineConstraint3CurrentWeight;
   // [Tooltip("Basically The Speed at which the spine Rotates Up and Down, the smaller the smooth time, the faster the rotation")]
    //public float spineConstraintDirectionChangeSmoothTime = 0.1f;
    [Tooltip("To Start Or Stop aimingwith the spine, the weight of the multi aim constraints is set by this speed -> provides smooth enabling and disabling of the spine aiming")]
    public float spineConstraintWeightChangeSpeed = 0.5f;

    #endregion

    #region Calculating Look At Target And Enabling & Disabling of It

    [Header("Look At")]

    [Tooltip("Target in This Objects Local Space")]
    public Transform lookAtLocalTarget;
    public FLookAnimator fLookAnimator;

    Vector3 lookAtDirectionToTarget;
    Vector3 lookAtPositionOfTarget;
    Transform lookAtTransformToTarget;

    enum LookAtState
    {
        Enabled,
        Disabled,
        SheduledForDisabling
    }
    LookAtState lookAtState;

    //to ensure that the look at has a smooth transition to the default position before disabling the component
    float nextLookAtDisableTime;
    [Tooltip("The FLookAtScript is disabled after this delay after looka at was disabled -> ensures a smooth transition of the head to its initial position.")]
    public float lookAtDisableDelay;

    #endregion

    #region Caulculating Aiming Weapon Target Weight etc...
    [Header("Aiming the Weapon")]

    public Transform weaponAimLocalTarget;
    public Gun weapon;

    float desiredWeaponAimingConstraintWeight;
    [Tooltip("Ensures a smooth transition between holding weapon idle and aiming")]
    public float aimingWeightChangeSpeed;
    [Tooltip("Weapons get parented to this object when aiming")]
    public Transform weaponAimParentLocalAdjuster;

    [Header("Weapon Aim Target Smoothing & Constraint")]
    [Tooltip("Max Rotation difference between current character forward and the target he aims at, to prrevent aiming too far to the side")]
    public float maxWeaponRotDifference;
    [Tooltip("speed used for Quaternion.RotateTowards() - not used anymore")]
    public float weaponAimRotationSpeed;
    [Tooltip("The smaller this value, the faster the weapon aims, it uses vector3.smoothDamp to smooth the movement a little bit")]
    public float aimingWeaponSmoothTime = 0.05f;
    Vector3 currentWeaponDirection;
    Vector3 desiredWeaponDirection;
    Vector3 weaponAimSpeedRef;

    #endregion

    #region for rotation

    [Header("Rotation")]
    public float spineAverageAngularVelocity;
    public float spineAngularAccelerationDistance;
    Vector3 angularVelocity;
    float rot_spineAgularSmoothTime;

    Quaternion rot_targetSpineRotation;
    Quaternion rot_currentSpineRotation;
    Quaternion rot_spineDerivQuaternion;


    #endregion


    [Header("Debug")]
    public bool showGizmos;

    public override void SetUpComponent(GameEntity entity)
    {
        base.SetUpComponent(entity);

        currentSpineDirection = transform.forward;

        weaponAimParentLocalAdjuster.localPosition = weapon.weaponAimParentLocalAdjusterOffset;
    }

    public override void UpdateComponent()
    {
        #region Look At 

        #region Update current lookAtTargetPosition

        if (lookAtState == LookAtState.Enabled)
        {
            if (currentLookAtTargetingMethod == AimAtTargetingMethod.Direction)
            {
                lookAtLocalTarget.position = aimingReferencePointOnBody.position + lookAtDirectionToTarget * defaultDirectionAimDistance;
            }
            else if (currentLookAtTargetingMethod == AimAtTargetingMethod.Position)
            {
                lookAtLocalTarget.position = lookAtPositionOfTarget;
            }
            else if (currentLookAtTargetingMethod == AimAtTargetingMethod.Transform)
            {
                lookAtLocalTarget.position = lookAtTransformToTarget.position;
            }
        }

        #endregion

        #region Disable LookAt Component after Delay if sheduled for disabling

        else if (lookAtState == LookAtState.SheduledForDisabling)
        {
            if (Time.time > nextLookAtDisableTime)
            {
                fLookAnimator.enabled = false;
                lookAtState = LookAtState.Disabled;
            }
        }

        #endregion

        #endregion

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
            //spineDirectionToTarget.Normalize();
            desiredSpineDirection = new Vector3(0, spineDirectionToTarget.y, new Vector2(spineDirectionToTarget.x, spineDirectionToTarget.z).magnitude);
            desiredSpineDirection = transform.TransformDirection(desiredSpineDirection);
            //TODO think of a different way to calculate this direction
            //desiredSpineDirection = transform.InverseTransformDirection(spineDirectionToTarget);
            //desiredSpineDirection.x = 0;
            //desiredSpineDirection = transform.TransformDirection(desiredSpineDirection);

            // 3.2 Set the target weight of spine constraints depending on if the target is above or below the shoulders
            if (spineConstraintLocalTarget.position.y > aimingReferencePointOnBody.position.y)
            {
                // aiming Up 
                spineConstraint1TargetWeight = 0.05f;//0.05f;
                spineConstraint2TargetWeight = 0.2f;//0.2f;
                spineConstraint3TargetWeight = -0.1f;//0.4f;
            }
            else
            {
                // aiming Down
                spineConstraint1TargetWeight = 0.2f;//0.2f;
                spineConstraint2TargetWeight = 0.5f;//0.7f;
                spineConstraint3TargetWeight = -0.3f;//1f;
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
        currentSpineDirection = new Vector3(desiredSpineDirection.x, currentSpineDirection.y, desiredSpineDirection.z); //currentSpineDirection should be smoothed on the local y, but not on x and z
                                                                                                                        //currentSpineDirection = Vector3.SmoothDamp(currentSpineDirection, desiredSpineDirection, ref currentSpineDirectionChangeVelocity, spineConstraintDirectionChangeSmoothTime); //Damping is static for now, no real velocity value

        currentSpineDirection = RotateSpineTowards(desiredSpineDirection);

        spineConstraintLocalTarget.position = aimingReferencePointOnBody.position + currentSpineDirection;

        // -----------   5 Smooth out Spine Constraints weight change ----------
        float maxSpeed = spineConstraintWeightChangeSpeed * Time.deltaTime;
        /* spineConstraint1CurrentWeight += Mathf.Clamp((spineConstraint1TargetWeight - spineConstraint1.weight), -maxSpeed, maxSpeed);
         spineConstraint1.weight = spineConstraint1CurrentWeight;
         spineConstraint2CurrentWeight += Mathf.Clamp((spineConstraint2TargetWeight - spineConstraint2.weight), -maxSpeed, maxSpeed);
         spineConstraint2.weight = spineConstraint2CurrentWeight;
         spineConstraint3CurrentWeight += Mathf.Clamp((spineConstraint3TargetWeight - spineConstraint3.weight), -maxSpeed, maxSpeed);
         spineConstraint3.weight = spineConstraint3CurrentWeight;*/

        spineConstraint1CurrentWeight += Mathf.Clamp((spineConstraint1TargetWeight - constraintController.spine1Weight), -maxSpeed, maxSpeed);
        constraintController.spine1Weight = spineConstraint1CurrentWeight;
        spineConstraint2CurrentWeight += Mathf.Clamp((spineConstraint2TargetWeight - constraintController.spine2Weight), -maxSpeed, maxSpeed);
        constraintController.spine2Weight = spineConstraint2CurrentWeight;
        spineConstraint3CurrentWeight += Mathf.Clamp((spineConstraint3TargetWeight - constraintController.spine3Weight), -maxSpeed, maxSpeed);
        constraintController.spine3Weight = spineConstraint3CurrentWeight;

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
            //desiredWeaponDirection = Vector3.RotateTowards(currentSpineDirection, weaponDirectionToTarget, maxWeaponRotDifference * Mathf.Deg2Rad, 100);
            desiredWeaponDirection = Vector3.RotateTowards(currentSpineDirection, weaponDirectionToTarget, maxWeaponRotDifference * Mathf.Deg2Rad, 100);


        }
        else
        {
            desiredWeaponDirection = transform.forward;
        }

        #endregion

        #region Smoothly rotate the weapon towards desiredWeaponDirection & smooth out change in weaponAimingRig.weight

        currentWeaponDirection = Vector3.SmoothDamp(currentWeaponDirection, desiredWeaponDirection, ref weaponAimSpeedRef, aimingWeaponSmoothTime);
        weaponAimLocalTarget.position = aimingReferencePointOnBody.position + currentWeaponDirection;

        // Smooth out the change between aiming weapon and holding it idle
        float changeSpeed = aimingWeightChangeSpeed * Time.deltaTime;
        constraintController.weaponAimWeight += Mathf.Clamp((desiredWeaponAimingConstraintWeight - constraintController.weaponAimWeight), -changeSpeed, changeSpeed);

        #endregion

        #endregion
    }

    #region Aim Spine Orders

    public void AimSpineInDirection(Vector3 direction)
    {
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

    #endregion

    #region Look At Orders

    public void LookAtTransform(Transform target)
    {
        currentLookAtTargetingMethod = AimAtTargetingMethod.Transform;
        lookAtTransformToTarget = target;
        lookAtLocalTarget.position = lookAtTransformToTarget.position;
        fLookAnimator.ObjectToFollow = lookAtLocalTarget;

        fLookAnimator.enabled = true;
        lookAtState = LookAtState.Enabled;
    }

    public void LookAtPosition(Vector3 position)
    {
        currentLookAtTargetingMethod = AimAtTargetingMethod.Position;
        lookAtPositionOfTarget = position;
        lookAtLocalTarget.position = position;
        fLookAnimator.ObjectToFollow = lookAtLocalTarget;

        fLookAnimator.enabled = true;
        lookAtState = LookAtState.Enabled;
    }

    public void LookInDirection(Vector3 direction)
    {
        currentLookAtTargetingMethod = AimAtTargetingMethod.Direction;
        lookAtDirectionToTarget = direction;
        lookAtLocalTarget.position = aimingReferencePointOnBody.position + direction * defaultDirectionAimDistance;
        fLookAnimator.ObjectToFollow = lookAtLocalTarget;

        fLookAnimator.enabled = true;
        lookAtState = LookAtState.Enabled;
    }

    public void StopLookAt()
    {
        fLookAnimator.ObjectToFollow = null;

        lookAtState = LookAtState.SheduledForDisabling;
        nextLookAtDisableTime = Time.time + lookAtDisableDelay;
    }

    #endregion

    #region Aim Weapon Orders

    public void AimWeaponInDirection(Vector3 direction)
    {
        aimingWeapon = true;

        handsIKController.OnStartAimingWeapon();

        currentWeaponTargetingMethod = AimAtTargetingMethod.Direction;
        weaponDirectionToTarget = direction;
        desiredWeaponAimingConstraintWeight = 1;
    }

    public void AimWeaponAtPosition(Vector3 position)
    {
        aimingWeapon = true;

        handsIKController.OnStartAimingWeapon();

        currentWeaponTargetingMethod = AimAtTargetingMethod.Position;
        weaponPositionOfTarget = position;
        desiredWeaponAimingConstraintWeight = 1;
    }

    public void AimWeaponAtTransform(Transform transform)
    {
        aimingWeapon = true;

        handsIKController.OnStartAimingWeapon();

        currentWeaponTargetingMethod = AimAtTargetingMethod.Transform;
        weaponTransformOfTarget = transform;
        desiredWeaponAimingConstraintWeight = 1;
    }

    public void StopAimingWeaponAtTarget()
    {
        handsIKController.OnStopAimingWeapon();

        aimingWeapon = false;
        desiredWeaponAimingConstraintWeight = 0;
    }

    #endregion

    #region Calclate New Rotation

    Vector3 RotateSpineTowards(Vector3 direction)
    {
        //TODO do an vector3 slerp instead, also with adjustable smooth time

        //only rotate on y axis
        rot_currentSpineRotation = Quaternion.LookRotation(currentSpineDirection);


        if (rot_targetSpineRotation != Quaternion.LookRotation(direction))
        {
            rot_targetSpineRotation = Quaternion.LookRotation(direction);

            float distance = Quaternion.Angle(rot_currentSpineRotation, rot_targetSpineRotation);
            //adjust the smooth time to ensure constant speeds at big and at small angles
            rot_spineAgularSmoothTime = Utility.CalculateSmoothTime(distance, spineAverageAngularVelocity, spineAngularAccelerationDistance);
        }

        Debug.Log("rot_currentSpineRotation: " + rot_currentSpineRotation.eulerAngles);
        Debug.Log("rot_targetSpineRotation: " + rot_targetSpineRotation.eulerAngles);

        //return  Utility.SmoothDamp(rot_currentSpineRotation, rot_targetSpineRotation, ref rot_spineDerivQuaternion, rot_spineAgularSmoothTime) * Vector3.forward;
        //return  rot_currentSpineRotation * Quaternion.Inverse(Utility.SmoothDamp(rot_currentSpineRotation, rot_targetSpineRotation, ref rot_spineDerivQuaternion, rot_spineAgularSmoothTime)) * currentSpineDirection;
        return Utility.SmoothDamp(rot_currentSpineRotation, rot_targetSpineRotation, ref rot_spineDerivQuaternion, rot_spineAgularSmoothTime) * Quaternion.Inverse(rot_currentSpineRotation) * currentSpineDirection;



        //angularVelocity = Utility.DerivToAngVelCorrected(rot_currentSpineRotation, rot_spineDerivQuaternion);

    }

    #endregion

    public void OnChangeWeapon(Gun newWeapon)
    {
        if (newWeapon == null)
        {
            weapon = null;

            /*fLookAnimator.CompensationWeight = lookAtCompensationWeightWithoutWeapon;
            fLookAnimator.CompensatePositions = lookAtCompensatiePositionsWithoutWeapon;*/
        }
        else
        {
            weapon = newWeapon;
            weaponAimParentLocalAdjuster.localPosition = weapon.weaponAimParentLocalAdjusterOffset;

            /*fLookAnimator.CompensationWeight = lookAtCompensationWeightWithWeapon;
            fLookAnimator.CompensatePositions = lookAtCompensatiePositionsWithWeapon;*/
        }
    }

    #region Status Checks & Getters

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
        return Vector3.Angle(desiredSpineDirection, currentSpineDirection);
    }

    public Vector3 GetCurrentWeaponAimDirection()
    {
        return currentWeaponDirection;
    }

    public float GetCurrentWeaponAimingErrorAngle(bool ignoreRecoil)
    {
        if (ignoreRecoil)
        {
            return Vector3.Angle(desiredWeaponDirection, currentWeaponDirection);
        }
        else
        {
            return Vector3.Angle(desiredWeaponDirection, weapon.transform.forward);
        }

    }

    #endregion

    #region Debug

    private void OnDrawGizmos()
    {
        if (showGizmos)
        {
         


            Gizmos.color = Color.blue;
            Gizmos.DrawSphere(aimingReferencePointOnBody.position + currentSpineDirection, 0.05f);

            Gizmos.color = Color.green;
            Gizmos.DrawSphere(aimingReferencePointOnBody.position + desiredSpineDirection, 0.05f);

            Gizmos.color = Color.cyan;
            Gizmos.DrawLine(aimingReferencePointOnBody.position, aimingReferencePointOnBody.position + spineDirectionToTarget);
            //Gizmos.DrawSphere(aimingReferencePointOnBody.position + desiredWeaponDirection, 0.05f);

            Gizmos.color = Color.yellow;
            Gizmos.DrawSphere(aimingReferencePointOnBody.position + transform.forward*3, 0.2f);

        }
    }
   
    #endregion
}
