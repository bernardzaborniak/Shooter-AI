using FIMSpace.FLook;
using UnityEngine;

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
    Vector3 directionFromAimingReferencePointToSpineTarget;
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
  
    
    Vector3 desiredLocalSpineDirection; //direction localToThe aimingReferencePoint
    
    Vector3 currentLocalSpineDirection;
    //Vector3 currentSpineDirectionChangeVelocity;
    //float desiredSpineXRotation;

    //the non local directions are not used by the spine calculation
    Vector3 desiredSpineDirection;
    Vector3 currentSpineDirection;

    float spineConstraint1TargetWeight;
    float spineConstraint2TargetWeight;
    float spineConstraint3TargetWeight;
   //float spineConstraint1CurrentWeight;
    //float spineConstraint2CurrentWeight;
   // float spineConstraint3CurrentWeight;
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
    //[Tooltip("The smaller this value, the faster the weapon aims, it uses vector3.smoothDamp to smooth the movement a little bit")]
    //public float aimingWeaponSmoothTime = 0.05f;
    Vector3 currentWeaponDirection;
    Vector3 desiredWeaponDirection;
   // Vector3 weaponAimSpeedRef;

    #endregion

    #region for rotation

    [Header("Rotation")]
    public float spineAverageAngularVelocity;
    public float spineAngularAccelerationDistance;
    //Vector3 spineAngularVelocity;
    float rot_spineAngularSmoothTime;

    Quaternion rot_spineTargetRotation;
    Vector3 rot_spineLastDesiredDirection;
    Quaternion rot_spineCurrentRotation;
    Quaternion rot_spineDerivQuaternion;
    Vector3 rot_spineCurrentVelocity;

    [Space(10)]
    public float weaponAverageAngularVelocity;
    public float weaponAngularAccelerationDistance;
    //Vector3 weaponAngularVelocity;
    float rot_weaponAngularSmoothTime;

    Quaternion rot_weaponTargetRotation;
    Quaternion rot_weaponCurrentRotation;
    Quaternion rot_weaponDerivQuaternion;



    #endregion


    [Header("Debug")]
    public bool showGizmos;


    public override void SetUpComponent(GameEntity entity)
    {
        base.SetUpComponent(entity);

        //currentSpineDirection = transform.forward;
       // desiredSpineDirection = currentSpineDirection;

        rot_spineLastDesiredDirection = Vector3.forward;

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

        #region  Calculate Desired Direction - Only If aimingSpine == true -

        if (aimingSpine)
        {
            // ---------------  Calculate Direction to aim at -----------
            // Basically all aim At Modes work the same that we have a direction in whihc to aim - from the aimingReferencePoint

            if (currentSpineTargetingMethod == AimAtTargetingMethod.Direction)
            {
                directionFromAimingReferencePointToSpineTarget = directionFromAimingReferencePointToSpineTarget;
            }
            else if (currentSpineTargetingMethod == AimAtTargetingMethod.Position)
            {
                directionFromAimingReferencePointToSpineTarget = spinePositionOfTarget - aimingReferencePointOnBody.position;
            }
            else if (currentSpineTargetingMethod == AimAtTargetingMethod.Transform)
            {
                directionFromAimingReferencePointToSpineTarget = spineTransformOfTarget.position - aimingReferencePointOnBody.position;
            }

            // -------------   Rotate Horizontally ----------------
            movementController.SetDesiredForward(directionFromAimingReferencePointToSpineTarget);

            // -------------   Calculate V3 desiredLocalSpineDirection & current -----------------
            desiredLocalSpineDirection = new Vector3(0, directionFromAimingReferencePointToSpineTarget.y, new Vector2(directionFromAimingReferencePointToSpineTarget.x, directionFromAimingReferencePointToSpineTarget.z).magnitude);
            currentLocalSpineDirection = new Vector3(desiredLocalSpineDirection.x, currentLocalSpineDirection.y, desiredLocalSpineDirection.z);
        }
        #endregion

        #region Move Current Direction towards desired, update spineConstraintLocalTarget position - Only If Hasnt Reached Desired Direction, 

        bool reachedDesiredDirection = false;
        if (Vector3.Angle(desiredLocalSpineDirection, currentLocalSpineDirection) < 0.1f)
        {
            reachedDesiredDirection = true;
        }

        if (!reachedDesiredDirection)
        {
            //  ----------    Update current direction -> Rotate towards Aiming Direction with damping  ----------
            currentLocalSpineDirection = RotateSpineTowards(currentLocalSpineDirection,desiredLocalSpineDirection);

            // ----------    Set the world directions for further use: ----------  
            desiredSpineDirection = transform.TransformDirection(desiredLocalSpineDirection);
            currentSpineDirection = transform.TransformDirection(currentLocalSpineDirection);

            //----------     Set the target Transform position for the constraint Controller ----------  
            spineConstraintLocalTarget.position = aimingReferencePointOnBody.position + currentSpineDirection;
        }

        #endregion

        #region Set the target weight of spine constraints depending on if the target is above or below the shoulders 

        if (aimingSpine)
        {
            //Todo solve this weight problem


            /*float lerpAmount = Mathf.Clamp(currentLocalSpineDirection.normalized.y - (0 - 0.5f), 0, 1); // normalize between 0.5 and +0.5 y Difference
                                                     //down //up value
            spineConstraint1TargetWeight = Mathf.Lerp(0.2f, 0.05f, lerpAmount);
            spineConstraint2TargetWeight = Mathf.Lerp(0.5f, 0.2f, lerpAmount);
            spineConstraint3TargetWeight = Mathf.Lerp(-0.3f, -0.1f, lerpAmount);*/
           
            
            
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
            spineConstraint1TargetWeight = 0f;
            spineConstraint2TargetWeight = 0f;
            spineConstraint3TargetWeight = 0f;
        }

        #endregion

        #region Smooth Out Spine Constraints weight change, set the weights inside constraintController

        // -----------   8 Smooth out Spine Constraints weight change ----------
        float maxSpeed = spineConstraintWeightChangeSpeed * Time.deltaTime;

        constraintController.spine1Weight += Mathf.Clamp((spineConstraint1TargetWeight - constraintController.spine1Weight), -maxSpeed, maxSpeed);
        constraintController.spine2Weight += Mathf.Clamp((spineConstraint2TargetWeight - constraintController.spine2Weight), -maxSpeed, maxSpeed);
        constraintController.spine3Weight += Mathf.Clamp((spineConstraint3TargetWeight - constraintController.spine3Weight), -maxSpeed, maxSpeed);

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
           
            //desiredWeaponDirection = Vector3.RotateTowards(currentSpineDirection, weaponDirectionToTarget, maxWeaponRotDifference * Mathf.Deg2Rad, 100);
            desiredWeaponDirection = Vector3.RotateTowards(currentSpineDirection, weaponDirectionToTarget, maxWeaponRotDifference * Mathf.Deg2Rad, 100);
            currentWeaponDirection = currentWeaponDirection.normalized * desiredWeaponDirection.magnitude;

        }
        else
        {
            //desiredWeaponDirection = transform.forward;
            //currentWeaponDirection = currentSpineDirection;
            //desiredWeaponDirection = currentWeaponDirection;
        }

        #endregion

        #region Smoothly rotate the weapon towards desiredWeaponDirection & smooth out change in weaponAimingRig.weight
        /*
        // currentWeaponDirection = Vector3.SmoothDamp(currentWeaponDirection, desiredWeaponDirection, ref weaponAimSpeedRef, aimingWeaponSmoothTime);
        currentWeaponDirection = RotateWeaponTowards(desiredWeaponDirection);
        weaponAimLocalTarget.position = aimingReferencePointOnBody.position + currentWeaponDirection;

        // Smooth out the change between aiming weapon and holding it idle
        float changeSpeed = aimingWeightChangeSpeed * Time.deltaTime;
        constraintController.weaponAimWeight += Mathf.Clamp((desiredWeaponAimingConstraintWeight - constraintController.weaponAimWeight), -changeSpeed, changeSpeed);
        */
        #endregion

        #endregion
    }

    #region Aim Spine Orders

    public void AimSpineInDirection(Vector3 direction)
    {
        if (!aimingSpine)
        {
            aimingSpine = true;
            currentSpineTargetingMethod = AimAtTargetingMethod.Direction;
            movementController.manualRotation = true;
            directionFromAimingReferencePointToSpineTarget = direction;

            //currentSpineDirection = transform.forward;
            //currentSpineDirection = spineConstraintLocalTarget.position - aimingReferencePointOnBody.position;

            //SetSpineDirectionToTarget();

            movementController.SetDesiredForward(direction);
        }
       
    }

    public void AimSpineAtPosition(Vector3 position)
    {
        if (!aimingSpine)
        {
            aimingSpine = true;
            currentSpineTargetingMethod = AimAtTargetingMethod.Position;
            movementController.manualRotation = true;
            spinePositionOfTarget = position;

            //currentSpineDirection = spineConstraintLocalTarget.position - aimingReferencePointOnBody.position;

        }
    }

    public void AimSpineAtTransform(Transform transform)
    {
        if (!aimingSpine)
        {
            aimingSpine = true;
            currentSpineTargetingMethod = AimAtTargetingMethod.Transform;
            movementController.manualRotation = true;
            spineTransformOfTarget = transform;

            //currentSpineDirection = transform.forward;
            //currentSpineDirection = spineConstraintLocalTarget.position - aimingReferencePointOnBody.position; //we just take the current direction, so

        }
    }



    public void StopAimSpineAtTarget()
    {
        if (aimingSpine)
        {
            aimingSpine = false;
            movementController.manualRotation = false;

            desiredSpineDirection = transform.forward;
           /* spineConstraint1TargetWeight = 0f;
            spineConstraint2TargetWeight = 0f;
            spineConstraint3TargetWeight = 0f;*/
        }
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

        // currentWeaponDirection = transform.forward; //could be something else - lik,e the actual weaponAImTransform?
        //currentWeaponDirection = currentSpineDirection; //could be something else - lik,e the actual weaponAImTransform?
        //currentWeaponDirection.magnitude = direction-
    }

    public void AimWeaponAtPosition(Vector3 position)
    {
        aimingWeapon = true;

        handsIKController.OnStartAimingWeapon();

        currentWeaponTargetingMethod = AimAtTargetingMethod.Position;
        weaponPositionOfTarget = position;
        desiredWeaponAimingConstraintWeight = 1;

        //currentWeaponDirection = transform.forward;
        //currentWeaponDirection = currentSpineDirection;
    }

    public void AimWeaponAtTransform(Transform transform)
    {
        aimingWeapon = true;

        handsIKController.OnStartAimingWeapon();

        currentWeaponTargetingMethod = AimAtTargetingMethod.Transform;
        weaponTransformOfTarget = transform;
        desiredWeaponAimingConstraintWeight = 1;

        //currentWeaponDirection = transform.forward;
        //currentWeaponDirection = currentSpineDirection;
    }

    public void StopAimingWeaponAtTarget()
    {
        handsIKController.OnStopAimingWeapon();

        aimingWeapon = false;
        desiredWeaponAimingConstraintWeight = 0;
    }

    #endregion

    #region Calclate New Rotation

     Vector3 RotateSpineTowards(Vector3 fromRotation, Vector3 desiredDirection)
     {

        //adjust the smooth time , if target changes -> to ensure constant speeds at big and at small angles
        if (Vector3.Angle(rot_spineLastDesiredDirection, desiredDirection)>0.1f)
        {
            rot_spineLastDesiredDirection = desiredDirection;

            float distance = Vector3.Angle(currentSpineDirection, rot_spineLastDesiredDirection);
            rot_spineAngularSmoothTime = Utility.CalculateSmoothTime(distance, spineAverageAngularVelocity, spineAngularAccelerationDistance);
        }
        return Vector3.SmoothDamp(fromRotation, rot_spineLastDesiredDirection, ref rot_spineCurrentVelocity, rot_spineAngularSmoothTime);
     }

    Vector3 RotateWeaponTowards(Vector3 direction)
    {
        //TODO do an vector3 slerp instead, also with adjustable smooth time
        /*if(currentWeaponDirection == Vector3.zero)
        {
            currentWeaponDirection = weaponAimTransform.forward;
        }*/


        //only rotate on y axis
        rot_weaponCurrentRotation = Quaternion.LookRotation(currentWeaponDirection);
        //Debug.Log("currentWeaponDirection: " + currentWeaponDirection);


        if (rot_weaponTargetRotation != Quaternion.LookRotation(direction))
        {
            rot_weaponTargetRotation = Quaternion.LookRotation(direction);

            float distance = Quaternion.Angle(rot_weaponCurrentRotation, rot_weaponTargetRotation);
            //adjust the smooth time to ensure constant speeds at big and at small angles
            rot_weaponAngularSmoothTime = Utility.CalculateSmoothTime(distance, weaponAverageAngularVelocity, weaponAngularAccelerationDistance);
        }

       // Debug.Log("rot_currentSpineRotation: " + rot_weaponCurrentRotation.eulerAngles);
        //Debug.Log("rot_targetSpineRotation: " + rot_weaponTargetRotation.eulerAngles);

        //return  Utility.SmoothDamp(rot_currentSpineRotation, rot_targetSpineRotation, ref rot_spineDerivQuaternion, rot_spineAgularSmoothTime) * Vector3.forward;
        //return  rot_currentSpineRotation * Quaternion.Inverse(Utility.SmoothDamp(rot_currentSpineRotation, rot_targetSpineRotation, ref rot_spineDerivQuaternion, rot_spineAgularSmoothTime)) * currentSpineDirection;
        Vector3 ret = Utility.SmoothDamp(rot_weaponCurrentRotation, rot_weaponTargetRotation, ref rot_weaponDerivQuaternion, rot_weaponAngularSmoothTime) * Quaternion.Inverse(rot_weaponCurrentRotation) * currentWeaponDirection;

        Vector3 vel = Utility.DerivToAngVelCorrected(rot_weaponCurrentRotation, rot_weaponDerivQuaternion);
        //Debug.Log("vel.x: " + vel.x);
        //Debug.Log("vel.y: " + vel.y);
       // Debug.Log("vel.z: " + vel.z);

        return ret;


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
            Gizmos.DrawSphere(spineConstraintLocalTarget.position, 0.05f);

/*
            Gizmos.color = Color.blue;
            Gizmos.DrawSphere(aimingReferencePointOnBody.position + currentSpineDirection, 0.05f);

            Gizmos.color = Color.red;
            Gizmos.DrawSphere(aimingReferencePointOnBody.position + desiredSpineDirection, 0.05f);*/

            // Gizmos.color = Color.green;
            //Gizmos.DrawSphere(aimingReferencePointOnBody.position + desiredWeaponDirection, 0.05f);

            // Gizmos.color = Color.red;
            //Gizmos.DrawSphere(aimingReferencePointOnBody.position + currentWeaponDirection.normalized * desiredWeaponDirection.magnitude, 0.05f);
            // Gizmos.DrawSphere(weaponAimLocalTarget.position, 0.05f);

            //Gizmos.color = Color.cyan;
            //Gizmos.DrawLine(aimingReferencePointOnBody.position, aimingReferencePointOnBody.position + spineDirectionToTarget);
            //Gizmos.DrawSphere(aimingReferencePointOnBody.position + desiredWeaponDirection, 0.05f);

            Gizmos.color = Color.yellow;
            Gizmos.DrawSphere(aimingReferencePointOnBody.position + transform.forward*3, 0.2f);

        }
    }
   
    #endregion
}
