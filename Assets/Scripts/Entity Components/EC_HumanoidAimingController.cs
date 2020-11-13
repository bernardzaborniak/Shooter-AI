using FIMSpace.FLook;
using UnityEngine;

// Positions the spineAimTarget, weaponAimTarget and the lookAtTarget
// Is responsible, together with the constraintController for the correct aiming of the spine bones together with hands and parenting of weapons
public class EC_HumanoidAimingController : EntityComponent
{

    #region Fields

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
    AimAtTargetingMethod currentLookAtTargetingMethod; 


    #endregion

    #region For Calculating Spine Target Position & Weight

    [Header("Spine Constraint for Aiming up/down")]

    [Tooltip("Target for the animation rigging multi aim constraing (only forward und up - no sideways rotation)")]
    public Transform spineConstraintLocalTarget;
    [Tooltip("The movement Controller is used to rotate to the sides")]
    public EC_HumanoidMovementController movementController;
    [Tooltip("Reference point on human body for aiming direction, can change later to be the gun? OR spine3 is good")]
    public Transform aimingReferencePointOnBody; //could be spine 3

    //For Calculating the desired Direction in Different ways
    bool aimingSpine;
    Vector3 directionFromAimingReferencePointToSpineTarget;
    Vector3 spinePositionOfTarget;
    Transform spineTransformOfTarget;

    //directions local to the aimingReferencePoint
    Vector3 desiredLocalSpineDirection; 
    Vector3 currentLocalSpineDirection;

    //the non local directions are not used by the spine calculation
    Vector3 desiredSpineDirection;
    Vector3 currentSpineDirection;

    //Used For Vector3 Slerp
    [Space(10)]
    [Tooltip("average velocity of spine rotation up and down")]
    public float spineAverageAngularVelocity;
    [Tooltip("the distance in degrees the spine needs to gain maximum velocity on its rotation up and down")]
    public float spineAngularAccelerationDistance;

    float rot_spineAngularSmoothTime;
    Vector3 rot_spineLastDesiredDirection;
    Vector3 rot_spineCurrentVelocity;

    // Used for Setting Constraints Weights
    float spineConstraint1TargetWeight;
    float spineConstraint2TargetWeight;
    float spineConstraint3TargetWeight;



    [Tooltip("To Start Or Stop aiming with the spine, the weight of the multi aim constraints is set by this speed -> provides smooth enabling and disabling of the spine aiming")]
    public float spineConstraintWeightChangeSpeed = 0.5f;

    float headAimConstraintTargetWeight;

    [Tooltip("Roughly 3x the spineConstraintWeightChangeSpeed prooved a good value")]
    public float spineHeadConstraintWeightChangeSpeed = 1.5f;


    #endregion

    #region Calculating Look At Target And Enabling & Disabling of It

    [Header("Look At")]

    [Tooltip("Target in This Objects Local Space")]
    public Transform lookAtLocalTarget;
    public FLookAnimator fLookAnimator;

    //For Calculating the desired Direction in Different ways
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
    [Tooltip("Max Rotation difference between current character forward / spine direction and the target he aims at, to prevent aiming too far to the side")]
    public float maxWeaponRotDifference;

    //For Calculating the desired Direction in Different ways
    public bool aimingWeapon; //only public to fix this left hand bug
    Vector3 weaponDirectionToTarget;
    Vector3 weaponPositionOfTarget;
    Transform weaponTransformOfTarget;

    Vector3 currentWeaponDirection;
    Vector3 desiredWeaponDirection;

    //Used For Vector3 Slerp
    [Space(10)]
    [Tooltip("average velocity of weapon rotation ")]
    public float weaponAverageAngularVelocity;
    [Tooltip("the distance in degrees the weapon needs to gain maximum velocity")]
    public float weaponAngularAccelerationDistance;

    float rot_weaponAngularSmoothTime;
    Vector3 rot_weaponLastDesiredDirection;
    Vector3 rot_weaponCurrentVelocity;
    #endregion

    [Header("Debug")]
    public bool showGizmos;

    #endregion

    public override void SetUpComponent(GameEntity entity)
    {
        base.SetUpComponent(entity);

        //For Rotating Weapon & Spine Directions using Vector3.Slerp
        rot_spineLastDesiredDirection = Vector3.forward;
        rot_weaponLastDesiredDirection = Vector3.forward;

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

        #region  Calculate local desired & current direction - Only If aimingSpine is true 

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
            desiredSpineDirection = transform.TransformDirection(desiredLocalSpineDirection);
            currentLocalSpineDirection = new Vector3(desiredLocalSpineDirection.x, currentLocalSpineDirection.y, desiredLocalSpineDirection.z); //clap the currentDirection so it only goes up and down, not to the sides, also //adjust the length of current Direction so it has the same length as desired, only then vector slerp in RotateTowards() wil give correct results
        }
        #endregion

        #region Move currentLocalDirection towards desired, update spineConstraintLocalTarget position 

        //  ----------    Update current direction -> Rotate towards Aiming Direction with damping  ----------
        currentLocalSpineDirection = RotateSpineTowards(currentLocalSpineDirection, desiredLocalSpineDirection);

        // ----------    Set the world direction for further use: ----------  
        currentSpineDirection = transform.TransformDirection(currentLocalSpineDirection);

        //----------     Set the target Transform position for the constraint Controller ----------  
        spineConstraintLocalTarget.position = aimingReferencePointOnBody.position + currentSpineDirection;

        #endregion

        #region Set the target weight of spine constraints depending on if the target is above or below the shoulders 

        if (aimingSpine)
        {
            //Todo solve this weight problem - Both versions arent perfect

            // --------------------Version 1:------------------
            /*float lerpAmount = Mathf.Clamp(currentLocalSpineDirection.normalized.y - (0 - 0.5f), 0, 1); // normalize between 0.5 and +0.5 y Difference
                                                     //down //up value
            spineConstraint1TargetWeight = Mathf.Lerp(0.2f, 0.05f, lerpAmount);
            spineConstraint2TargetWeight = Mathf.Lerp(0.5f, 0.2f, lerpAmount);
            spineConstraint3TargetWeight = Mathf.Lerp(-0.3f, -0.1f, lerpAmount);*/


            // --------------------Version 2:------------------
            if (spineConstraintLocalTarget.position.y > aimingReferencePointOnBody.position.y)
            {
                // aiming Up 
                spineConstraint1TargetWeight = 0.1f;//0.05f;
                spineConstraint2TargetWeight = 0.3f;//0.2f;
                spineConstraint3TargetWeight = 0.2f;//-0.1f;//0.4f;

                headAimConstraintTargetWeight = 1;
            }
            else
            {
                // aiming Down
                spineConstraint1TargetWeight = 0.2f;//0.2f;
                spineConstraint2TargetWeight = 0.5f;//0.7f;
                spineConstraint3TargetWeight = -0.3f;//1f;

                headAimConstraintTargetWeight = 1;
            }
            
        }
        else
        {
            spineConstraint1TargetWeight = 0f;
            spineConstraint2TargetWeight = 0f;
            spineConstraint3TargetWeight = 0f;

            headAimConstraintTargetWeight = 0;
        }

        #endregion

        #region Smooth Out Spine Constraints weight change, set the weights inside constraintController

        // -----------   8 Smooth out Spine Constraints weight change ----------
        float maxSpeed = spineConstraintWeightChangeSpeed * Time.deltaTime;

        constraintController.spine1Weight += Mathf.Clamp((spineConstraint1TargetWeight - constraintController.spine1Weight), -maxSpeed, maxSpeed);
        constraintController.spine2Weight += Mathf.Clamp((spineConstraint2TargetWeight - constraintController.spine2Weight), -maxSpeed, maxSpeed);
        constraintController.spine3Weight += Mathf.Clamp((spineConstraint3TargetWeight - constraintController.spine3Weight), -maxSpeed, maxSpeed);

        maxSpeed = spineHeadConstraintWeightChangeSpeed * Time.deltaTime;
        //changing the head weight is faster
        constraintController.headConstraintWeight += Mathf.Clamp((headAimConstraintTargetWeight - constraintController.headConstraintWeight), -maxSpeed, maxSpeed);
        

        #endregion

        #endregion

        #region Aiming Weapon

        #region Calculate desired & current Direction - Only If aimingWeapon is true 

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

            desiredWeaponDirection = Vector3.RotateTowards(currentSpineDirection, weaponDirectionToTarget, maxWeaponRotDifference * Mathf.Deg2Rad, 100);
            currentWeaponDirection = currentWeaponDirection.normalized * desiredWeaponDirection.magnitude; //adjust the length of current Direction so it has the same length as desired, only then vector slerp in RotateTowards() wil give correct results
        }

        #endregion

        #region Move Current Direction towards desired, update spineConstraintLocalTarget position 

        //  ----------    Update current direction -> Rotate towards Aiming Direction with damping  ----------
        //clamp the current rotation too
        currentWeaponDirection = Vector3.RotateTowards(currentWeaponDirection, RotateWeaponTowards(currentWeaponDirection, desiredWeaponDirection), maxWeaponRotDifference * Mathf.Deg2Rad, 100); 
        //currentWeaponDirection = RotateWeaponTowards(currentWeaponDirection, desiredWeaponDirection);

        //----------     Set the target Transform position for the constraint Controller ----------  
        weaponAimLocalTarget.position = aimingReferencePointOnBody.position + currentWeaponDirection;

        // -------------- Smooth out the change between aiming weapon and holding it idle
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
        //movementController.manualRotation = true;
        movementController.SetManualRotation(true);
    }

    public void AimSpineAtPosition(Vector3 position)
    {
        aimingSpine = true;
        currentSpineTargetingMethod = AimAtTargetingMethod.Position;
        //movementController.manualRotation = true;
        movementController.SetManualRotation(true);
        spinePositionOfTarget = position;
    }

    public void AimSpineAtTransform(Transform transform)
    {
        aimingSpine = true;
        currentSpineTargetingMethod = AimAtTargetingMethod.Transform;
        //movementController.manualRotation = true;
        movementController.SetManualRotation(true);
        spineTransformOfTarget = transform;
    }

    public void StopAimSpineAtTarget()
    {
        if (aimingSpine)
        {
            aimingSpine = false;
            //movementController.manualRotation = false;
            movementController.SetManualRotation(false);

            desiredSpineDirection = transform.forward;
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
        if (!aimingWeapon)
        {
            currentWeaponDirection = currentSpineDirection;
            handsIKController.OnStartAimingWeapon();

            aimingWeapon = true;
        }

        currentWeaponTargetingMethod = AimAtTargetingMethod.Direction;
        weaponDirectionToTarget = direction;
        desiredWeaponAimingConstraintWeight = 1;
    }

    public void AimWeaponAtPosition(Vector3 position)
    {
        if (!aimingWeapon)
        {
            currentWeaponDirection = currentSpineDirection;

            currentWeaponDirection = currentSpineDirection;
            handsIKController.OnStartAimingWeapon();

            aimingWeapon = true;
        }
    
        currentWeaponTargetingMethod = AimAtTargetingMethod.Position;
        weaponPositionOfTarget = position;
        desiredWeaponAimingConstraintWeight = 1;
    }

    public void AimWeaponAtTransform(Transform transform)
    {
        if (!aimingWeapon)
        {
            currentWeaponDirection = currentSpineDirection;

            currentWeaponDirection = currentSpineDirection;
            handsIKController.OnStartAimingWeapon();

            aimingWeapon = true;
        }

        currentWeaponTargetingMethod = AimAtTargetingMethod.Transform;
        weaponTransformOfTarget = transform;
        desiredWeaponAimingConstraintWeight = 1;
    }

    public void StopAimingWeaponAtTarget()
    {
        if (aimingWeapon)
        {
            handsIKController.OnStopAimingWeapon();

            aimingWeapon = false;
            desiredWeaponAimingConstraintWeight = 0;
        }

        
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

    Vector3 RotateWeaponTowards(Vector3 fromRotation, Vector3 desiredDirection)
    {

        //adjust the smooth time , if target changes -> to ensure constant speeds at big and at small angles
        if (Vector3.Angle(rot_weaponLastDesiredDirection, desiredDirection) > 0.1f)
        {
            rot_weaponLastDesiredDirection = desiredDirection;

            float distance = Vector3.Angle(currentWeaponDirection, rot_weaponLastDesiredDirection);
            rot_weaponAngularSmoothTime = Utility.CalculateSmoothTime(distance, weaponAverageAngularVelocity, weaponAngularAccelerationDistance);
        }
        return Vector3.SmoothDamp(fromRotation, rot_weaponLastDesiredDirection, ref rot_weaponCurrentVelocity, rot_weaponAngularSmoothTime);
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
        Vector3 directionToCurrentAimTarget = Vector3.zero;

        if(currentWeaponTargetingMethod == AimAtTargetingMethod.Direction)
        {
            directionToCurrentAimTarget = weaponDirectionToTarget;
        }
        else if (currentWeaponTargetingMethod == AimAtTargetingMethod.Position)
        {
            directionToCurrentAimTarget = weaponPositionOfTarget - aimingReferencePointOnBody.position;
        }
        else if (currentWeaponTargetingMethod == AimAtTargetingMethod.Direction)
        {
            directionToCurrentAimTarget = weaponTransformOfTarget.position - aimingReferencePointOnBody.position;    
        }

        if (ignoreRecoil)
        {
            return Vector3.Angle(directionToCurrentAimTarget, currentWeaponDirection);
        }
        else
        {
            return Vector3.Angle(directionToCurrentAimTarget, weapon.transform.forward);
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

            Gizmos.color = Color.cyan;
            Gizmos.DrawSphere(aimingReferencePointOnBody.position + desiredSpineDirection, 0.05f);


            Gizmos.color = Color.red;
            Gizmos.DrawSphere(aimingReferencePointOnBody.position + currentWeaponDirection, 0.05f);

            Gizmos.color = Color.green;
            Gizmos.DrawSphere(aimingReferencePointOnBody.position + desiredWeaponDirection, 0.05f);

            Gizmos.color = Color.yellow;
            Gizmos.DrawSphere(aimingReferencePointOnBody.position + transform.forward*3, 0.2f);

        }
    }
   
    #endregion
}
