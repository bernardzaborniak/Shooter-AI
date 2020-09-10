using FIMSpace.FLook;
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

    AimAtTargetingMethod currentAimAtTargetingMethod;

    Vector3 aimDirection;
    Vector3 aimAtPosition;
    Transform aimAtTransform;

    bool aimAtActive;

    Vector3 pointToAimAt;
    Vector3 directionToAim;

    #endregion

    #region Other Fields

    [Header("Spine Constraint for Aiming up/down")]

    [Tooltip("The movement Controller is used to rotate to the sides")]
    public EC_HumanoidMovementController movementController;
    [Tooltip("Target at which the spine/body aims")]
    public Transform aimAtTarget;
    [Tooltip("Target for the animation rigging multi aim constraing (only forward und up - no sideways rotation)")]
    public Transform spineConstraintTarget;
    [Tooltip("Reference point on human body for aiming direction, can change later to be the gun? OR spine3 is good")]
    public Transform aimingDistanceReferencePoint; //could be spine 3
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
    public Transform lookAtTarget;

    // Look At distorts the weapon aiming when its 2 compensiation Weights are set on something different than 1, so we set the mto 1 while aiming ,and reset them afterwards.
    float lookAtCompensationWeightBeforeAimingWeapon;
    float lookAtCompensatiePositionsBeforeAimingWeapon;

    [Header("Idle Weapon Holding")]
    public Rig idleWeaponHoldingRig;
    //float desiredIdleWeaponHoldingRigWeight = 1;  //maybe used later when converting from holding weapon to some other animation?
    //public TwoBoneIKConstraint idleLeftHandIK;
   
   

    [Header("Aiming the Weapon")]
    public Transform weaponAimTarget;
    public Weapon weapon;
    public Rig weaponAimingRig;

    float desiredWeaponAimingRigWeight;
    [Tooltip("Ensures a smooth transition between holding weapon idle and aiming")]
    public float changeFromAimingToIdleRigSpeed;
    public float changeFromIdleToAimingRigSpeed;
    [Tooltip("Weapons get parented to this object when aiming")]
    public Transform weaponAimParentLocalAdjuster;

    //public Transform rightHandIKTarget;
    //public Transform leftHandIKTarget;
   

    bool aimWeapon;

    [Header("Weapon aim target movement")]
    [Tooltip("Max Rotation difference between current character forward and the target he aims at, to prrevent aiming too far to the side")]
    public float maxWeaponRotDifference;
    [Tooltip("speed used for Quaternion.RotateTowards()")]
    public float weaponAimRotationSpeed;
    [Tooltip("The smaller this value, the faster the weapon aims, it uses vector3.smoothDamp to smooth the movement a little bit")]
    public float aimingWeaponSmoothTime = 0.05f;
    Vector3 currentWeaponAimDirection;
    Vector3 desiredWeaponAimDirection;
    Vector3 weaponAimSpeedRef;

    [Header("Hand IK")]
    [Tooltip("depending on the specific model skeleton hand orientation?")]
    public Vector3 handIKRotationOffset;
    [Tooltip("Should the Left hand be adjusted when holding a weapon idle ? - for pistols mostly fasle , for rifles mostly true")]
    public bool performLeftHandIKOnIdleWeaponHold;

    public TwoBoneIKConstraint leftHandIKConstraint;
    float desiredLeftHandIKRigWeight;
    public Transform leftHandIKTarget;

    public TwoBoneIKConstraint rightHandIKConstraint;
    float desiredRightHandIKRigWeight;
    public Transform rightHandIKTarget;

    #endregion

    [Header("Fields to be moved to animation controller corresponding to character controller state machine")]
    public Animator animator;

    public override void SetUpComponent(GameEntity entity)
    {
        base.SetUpComponent(entity);

        //spineConstraintLocalTargetParent = spineConstraintLocalTarget.parent;
        currentSpineDirection = transform.forward;

        weaponAimParentLocalAdjuster.localPosition = weapon.weaponAimParentLocalAdjusterOffset;

        if (performLeftHandIKOnIdleWeaponHold)
        {
            desiredLeftHandIKRigWeight = 1;
        }
        else
        {
            desiredLeftHandIKRigWeight = 0;
        }
        desiredRightHandIKRigWeight = 0;
    }

    public override void UpdateComponent()
    {
        #region Key Inputs For Testing

        if (Input.GetKeyDown(KeyCode.L))
        {
            // AimInDirection(Vector3.right);
            LookAtTransform(lookAtTarget);
        }
        if (Input.GetKeyDown(KeyCode.M))
        {
            StopLookAt();
        }
        if (Input.GetKeyDown(KeyCode.K))
        {
            AimAtTransform(aimAtTarget);
        }
        if (Input.GetKeyDown(KeyCode.N))
        {
            StopAimAt();
        }
        if (Input.GetKeyDown(KeyCode.J))
        {
            AimWeaponAtTarget();
        }
        if (Input.GetKeyDown(KeyCode.B))
        {
            StopAimingWeaponAtTarget();
        }


        if (Input.GetKeyDown(KeyCode.U))
        {
            //activate all at once:
            LookAtTransform(lookAtTarget);
            AimAtTransform(aimAtTarget);
            AimWeaponAtTarget();

            animator.SetBool("Aiming", true);

        }

        if (Input.GetKeyDown(KeyCode.Z))
        {
            //deactivate all at once:
            StopLookAt();
            StopAimAt();
            StopAimingWeaponAtTarget();

            animator.SetBool("Aiming", false);

        }
        if (Input.GetKeyDown(KeyCode.I))
        {
            animator.SetBool("Aiming", false);
        }
        if (Input.GetKeyDown(KeyCode.O))
        {
            animator.SetBool("Aiming", true);
        }

        #endregion


        #region Aiming Spine

        if (aimAtActive)
        {
            Debug.Log("2");
            // ---------------  1. Calculate Point to aim at -----------
            // Basically all aim At Modes work the same that we have a point we are looking at in space, just the calculation of this point differs
            pointToAimAt = Vector3.zero;

            if (currentAimAtTargetingMethod == AimAtTargetingMethod.Direction)
            {
                pointToAimAt = aimingDistanceReferencePoint.position + aimDirection * defaultDirectionAimDistance;
            }
            else if (currentAimAtTargetingMethod == AimAtTargetingMethod.Position)
            {
                pointToAimAt = aimAtPosition;
            }
            else if (currentAimAtTargetingMethod == AimAtTargetingMethod.Transform)
            {
                pointToAimAt = aimAtTransform.position;
            }

            directionToAim = pointToAimAt - aimingDistanceReferencePoint.position;


            // -------------   2. Rotate Horizontally  -----------
            movementController.SetDesiredForward(directionToAim.normalized); //do we need to normalize?

            // -------------   3. Calculate vertical desiredSpineDirection & set Weights ----------------

            // 3.1 Limit to rotation on the x axis only
            //We are kind of rotating the vector to point in the Forward Direction
            desiredSpineDirection = new Vector3(0, directionToAim.y, new Vector2(directionToAim.x, directionToAim.z).magnitude);
            desiredSpineDirection = transform.TransformDirection(desiredSpineDirection);

            // 3.2 Set the target weight of spine constraints depending on if the target is above or below the shoulders
            if (spineConstraintTarget.position.y > aimingDistanceReferencePoint.position.y)
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

        //  ----------   4.Rotate towards Aiming Direction with damping  ----------
        currentSpineDirection = Vector3.SmoothDamp(currentSpineDirection, desiredSpineDirection, ref currentSpineDirectionChangeVelocity, spineConstraintDirectionChangeSmoothTime); //Damping is static for now, no real velocity value
        spineConstraintTarget.position = aimingDistanceReferencePoint.position + currentSpineDirection;

        Debug.Log("spineConstraint1TargetWeight: " + spineConstraint1TargetWeight);
        // -----------   5 Smooth out Spine Constraints weight change ----------
        float maxSpeed = spineConstraintWeightChangeSpeed * Time.deltaTime;
        spineConstraint1CurrentWeight += Mathf.Clamp((spineConstraint1TargetWeight - spineConstraint1.weight), -maxSpeed, maxSpeed);
        spineConstraint1.weight = spineConstraint1CurrentWeight;
        spineConstraint2CurrentWeight += Mathf.Clamp((spineConstraint2TargetWeight - spineConstraint2.weight), -maxSpeed, maxSpeed);
        spineConstraint2.weight = spineConstraint2CurrentWeight;
        spineConstraint3CurrentWeight += Mathf.Clamp((spineConstraint3TargetWeight - spineConstraint3.weight), -maxSpeed, maxSpeed);
        spineConstraint3.weight = spineConstraint3CurrentWeight;

        #endregion

        #region Aiming Weapon

        if (aimWeapon)
        {
            // Calculate & Rotate Weapon Aim Direction
            desiredWeaponAimDirection = Vector3.RotateTowards(currentSpineDirection, directionToAim, maxWeaponRotDifference * Mathf.Deg2Rad, 100);
            currentWeaponAimDirection = Vector3.SmoothDamp(currentWeaponAimDirection, desiredWeaponAimDirection, ref weaponAimSpeedRef, aimingWeaponSmoothTime);
            weaponAimTarget.position = aimingDistanceReferencePoint.position + currentWeaponAimDirection;

            // Position The hand's IK's
            rightHandIKTarget.position = weapon.rightHandIKPosition.position;
            rightHandIKTarget.rotation = weapon.rightHandIKPosition.rotation * Quaternion.Euler(handIKRotationOffset);

           // leftHandIKTarget.position = weapon.leftHandIKPosition.position;
           // leftHandIKTarget.rotation = weapon.leftHandIKPosition.rotation * Quaternion.Euler(handIKRotationOffset);
        }

        // Smooth out the change between aiming weapon and holding it idle
        //Two different speeds to prevent gun from floating around between hands
        if(desiredWeaponAimingRigWeight == 1)
        {
            float changeSpeed = changeFromIdleToAimingRigSpeed * Time.deltaTime;
            weaponAimingRig.weight += Mathf.Clamp((desiredWeaponAimingRigWeight - weaponAimingRig.weight), -changeSpeed, changeSpeed);
        }
        else if(desiredWeaponAimingRigWeight == 0)
        {
            float changeSpeed = changeFromAimingToIdleRigSpeed * Time.deltaTime;
            weaponAimingRig.weight += Mathf.Clamp((desiredWeaponAimingRigWeight - weaponAimingRig.weight), -changeSpeed, changeSpeed);
        }



        #endregion

        #region Handle  Hand IK'S
        //maybe expand it later to use both arms as iks here - for a bow for example?


        if (desiredLeftHandIKRigWeight == 1)
        {
            float changeSpeed = changeFromIdleToAimingRigSpeed * Time.deltaTime;
            leftHandIKConstraint.weight += Mathf.Clamp((desiredLeftHandIKRigWeight - leftHandIKConstraint.weight), -changeSpeed, changeSpeed);
        }
        else if (desiredLeftHandIKRigWeight == 0)
        {
            float changeSpeed = changeFromAimingToIdleRigSpeed * Time.deltaTime;
            leftHandIKConstraint.weight += Mathf.Clamp((desiredLeftHandIKRigWeight - leftHandIKConstraint.weight), -changeSpeed, changeSpeed);
        }

        leftHandIKTarget.position = weapon.leftHandIKPosition.position;
        leftHandIKTarget.rotation = weapon.leftHandIKPosition.rotation * Quaternion.Euler(handIKRotationOffset);


        if (desiredRightHandIKRigWeight == 1)
        {
            float changeSpeed = changeFromIdleToAimingRigSpeed * Time.deltaTime;
            rightHandIKConstraint.weight += Mathf.Clamp((desiredRightHandIKRigWeight - rightHandIKConstraint.weight), -changeSpeed, changeSpeed);
        }
        else if (desiredRightHandIKRigWeight == 0)
        {
            float changeSpeed = changeFromAimingToIdleRigSpeed * Time.deltaTime;
            rightHandIKConstraint.weight += Mathf.Clamp((desiredRightHandIKRigWeight - rightHandIKConstraint.weight), -changeSpeed, changeSpeed);
        }

        rightHandIKTarget.position = weapon.rightHandIKPosition.position;
        rightHandIKTarget.rotation = weapon.rightHandIKPosition.rotation * Quaternion.Euler(handIKRotationOffset);







        #endregion
    }



    /*public void AimInDirection(Vector3 direction)
    {
        aimAtActive = true;
        currentAimAtTargetingMethod = AimAtTargetingMethod.Direction;
        movementController.manualRotation = true;
        aimDirection = direction;
    }

    public void AimAtPosition(Vector3 position)
    {
        aimAtActive = true;
        currentAimAtTargetingMethod = AimAtTargetingMethod.Position;
        movementController.manualRotation = true;
        aimAtPosition = position;

    }*/

    public void LookAtTransform(Transform target)
    {
        fLookAnimator.ObjectToFollow = target;
    }

    public void StopLookAt()
    {
        fLookAnimator.ObjectToFollow = null;
    }



    public void AimAtTransform(Transform transform)
    {
        aimAtActive = true;
        currentAimAtTargetingMethod = AimAtTargetingMethod.Transform;
        movementController.manualRotation = true;
        aimAtTransform = transform;
        Debug.Log("1");
    }

    public void StopAimAt()
    {
        aimAtActive = false;
        movementController.manualRotation = false;
    }

    public void AimWeaponAtTarget()
    {
        aimWeapon = true;

        currentWeaponAimDirection = currentSpineDirection;

        // Modify the compensation weight to achieve correct weapon aim and correct hand ik's
        // cause LookAt modifies the hand and arm positions in LateUpdate
        lookAtCompensationWeightBeforeAimingWeapon = fLookAnimator.CompensationWeight;
        lookAtCompensatiePositionsBeforeAimingWeapon = fLookAnimator.CompensatePositions;

        fLookAnimator.CompensationWeight = 1;
        fLookAnimator.CompensatePositions = 1;

        desiredWeaponAimingRigWeight = 1;

        desiredLeftHandIKRigWeight = 1;
        desiredRightHandIKRigWeight = 1;
    }

    public void StopAimingWeaponAtTarget()
    {
        aimWeapon = false;

        // Modify fLookATWeights to their values before aiming the weapon.
        fLookAnimator.CompensationWeight = lookAtCompensationWeightBeforeAimingWeapon;
        fLookAnimator.CompensatePositions = lookAtCompensatiePositionsBeforeAimingWeapon;

        desiredWeaponAimingRigWeight = 0;

        if (performLeftHandIKOnIdleWeaponHold)
        {
            desiredLeftHandIKRigWeight = 1;
        }
        else
        {
            desiredLeftHandIKRigWeight = 0;
        }

        desiredRightHandIKRigWeight = 0;
    }

    /*private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;

        Gizmos.DrawLine(aimingDistanceReferencePoint.position, aimingDistanceReferencePoint.position + desiredSpineDirection);
    }*/
}
