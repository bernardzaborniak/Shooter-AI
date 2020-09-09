using FIMSpace.FLook;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class EC_HumanoidAimingController : EntityComponent
{
    //is responsible for the correct aiming of the spine bones together with the animation? 
    //rotates differently depending on the current weapon?

   // [Header("References")]
   

    [Header("Spine Constraint for Aiming up/down")]
    public EC_HumanoidMovementController movementController;
    public Transform aimAtTarget;
    [Tooltip("Target for the animation rigging multi aim constraing (only forward und up - no sideways rotation)")]
    public Transform spineConstraintTarget;
    [Tooltip("Reference point on human body for aiming direction, ca nchange later to be the gun?")]
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
    public float spineConstraintDirectionChangeSmoothTime = 0.3f;
    public float spineConstraintWeightChangeSpeed;

    [Header("Aim at Params")]
    [Tooltip("If we are aiming at a direction, we are aiming at a point which is the direction * this float value")]
    public float defaultDirectionAimDistance;
    public float defaultDirectionAimHeightOffset;

    [Header("Look At")]
    public Transform lookAtTarget;
    public FLookAnimator fLookAnimator;

    [Header("Aiming the Weapon")]
    public Transform weaponAimTarget;
    public Rig weaponAimingRig;


    //public TwoBoneIKConstraint
    public Weapon weapon;
    public Transform rightHandIKTarget;
    public Transform leftHandIKTarget;
    public Vector3 handIKRotationOffset;
   
   // public Transform aimingHand;
   // public float movementSpeed;
   // public float maxAngleDifference; //angle difference between desired rotation and current
    bool aimWeapon;

    [Header(" Weapon aim target movement")]
    /* [Tooltip("compared to aimingDistanceReferencePoint - whoch is spine")]
     public float maxXRotDifference;
     [Tooltip("compared to aimingDistanceReferencePoint - whoch is spine, should be smaller than XRotDifference")]
     public float maxYRotDifference;*/
    public float maxWeaponRotDifference;
    [Tooltip("speed used for Quaternion.RotateTowards()")]
    public float weaponAimRotationSpeed;
    public float currentAimingWeaponSmoothSpeed;
    Vector3 currentWeaponAimDirection;
    Vector3 desiredWeaponAimDirection;
    Vector3 weaponAimSpeed;

    float lookAtCompensationWeightBeforeAimingWeapon;
    float lookAtCompensatiePositionsBeforeAimingWeapon;


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

    public override void SetUpComponent(GameEntity entity)
    {
        base.SetUpComponent(entity);

        //spineConstraintLocalTargetParent = spineConstraintLocalTarget.parent;
        currentSpineDirection = transform.forward;
    }

    public override void UpdateComponent()
    {
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


        if (aimAtActive)
        {
            // 1. Calculate Point to aim at
            // Basically all aim At Modes work the same that we have a point we are looking at in space, just the calculation of this point differs
            pointToAimAt = Vector3.zero;

            if (currentAimAtTargetingMethod == AimAtTargetingMethod.Direction)
            {
                pointToAimAt = transform.position + aimDirection * defaultDirectionAimDistance + Vector3.up * defaultDirectionAimHeightOffset;
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


            // 2. Rotate Horizontally
            movementController.SetDesiredForward(directionToAim.normalized); //do we need to normalize?

            // 3. Rotate Vertically

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

        // Move Aiming Direction with damping
        currentSpineDirection = Vector3.SmoothDamp(currentSpineDirection, desiredSpineDirection, ref currentSpineDirectionChangeVelocity, spineConstraintDirectionChangeSmoothTime); //Damping is static for now, no real velocity value
        spineConstraintTarget.position = aimingDistanceReferencePoint.position + currentSpineDirection;

        // Smooth out weight change
        float maxSpeed = spineConstraintWeightChangeSpeed * Time.deltaTime;
        spineConstraint1CurrentWeight += Mathf.Clamp((spineConstraint1TargetWeight - spineConstraint1.weight), -maxSpeed, maxSpeed);
        spineConstraint1.weight = spineConstraint1CurrentWeight;
        spineConstraint2CurrentWeight += Mathf.Clamp((spineConstraint2TargetWeight - spineConstraint2.weight), -maxSpeed, maxSpeed);
        spineConstraint2.weight = spineConstraint2CurrentWeight;
        spineConstraint3CurrentWeight += Mathf.Clamp((spineConstraint3TargetWeight - spineConstraint3.weight), -maxSpeed, maxSpeed);
        spineConstraint3.weight = spineConstraint3CurrentWeight;

        if (aimWeapon)
        {

            desiredWeaponAimDirection = Vector3.RotateTowards(currentSpineDirection, directionToAim, maxWeaponRotDifference * Mathf.Deg2Rad, 100);

            /*if(newDesiredWeaponAimDirection!= desiredWeaponAimDirection)
            {
                desiredWeaponAimDirection = newDesiredWeaponAimDirection;
                currentAimingWeaponSmoothSpeed =  0.25f * Vector3.Angle(currentWeaponAimDirection, desiredWeaponAimDirection) / weaponAimRotationSpeed;
            }*/

            //currentWeaponAimDirection = Vector3.RotateTowards(currentWeaponAimDirection, desiredWeaponAimDirection, weaponAimRotationSpeed * Mathf.Deg2Rad * Time.deltaTime, 100);
            //currentWeaponAimDirection = Vector3.SmoothDamp(currentWeaponAimDirection, desiredWeaponAimDirection, ref weaponAimSpeed, (0.25f * Vector3.Angle(currentWeaponAimDirection, desiredWeaponAimDirection)) / weaponAimRotationSpeed);

            //calculate currentAimingWeaponSmoothSpeed everytime the target changes ? 

            currentWeaponAimDirection = Vector3.SmoothDamp(currentWeaponAimDirection, desiredWeaponAimDirection, ref weaponAimSpeed, currentAimingWeaponSmoothSpeed);
            weaponAimTarget.position = aimingDistanceReferencePoint.position + currentWeaponAimDirection;

            /*Vector3 desiredWeaponAimDirection;
            //if the current target is at an angle smaller than the constraints - the desired direction is also the spine target, if not, we aim towards the real target
            if (Vector3.Angle(directionToAim, currentSpineDirection)> maxWeaponRotDifference) //directionToAim is the absolute direction, spine direction is the one the spine takes
            {
                desiredWeaponAimDirection = currentSpineDirection;
            }
            else
            {
                desiredWeaponAimDirection = directionToAim;
            }

            //move vector towards deired direction:
            currentWeaponAimDirection = Vector3.RotateTowards(currentWeaponAimDirection, desiredWeaponAimDirection, weaponAimRotationSpeed * Mathf.Deg2Rad * Time.deltaTime,100);
            weaponAimTarget.position = aimingDistanceReferencePoint.position + currentWeaponAimDirection;
                 //in degress pro second
            */





            //weaponAimTarget.position = aimAtTarget.position;


            // rightHandIKTarget = weapon.rightHandIKPosition;
            // leftHandIKTarget = weapon.leftHandIKPosition;

            rightHandIKTarget.position = weapon.rightHandIKPosition.position;
            rightHandIKTarget.rotation = weapon.rightHandIKPosition.rotation * Quaternion.Euler(handIKRotationOffset);

            leftHandIKTarget.position = weapon.leftHandIKPosition.position;
            leftHandIKTarget.rotation = weapon.leftHandIKPosition.rotation * Quaternion.Euler(handIKRotationOffset); ;
        }
    }

    public override void LateUpdateComponent()
    {
        if (aimWeapon)
        {
            //Vector3 previousArmUp = aimingHand.forward;
            //aimingHand.forward = Vector3.up;
            //aimingHand.up = (aimAtTarget.position - aimingHand.position);
            //aimingHand.forward = previousArmUp;
            ////aimingHand.rotation = Quaternion.LookRotation((aimAtTarget.position - aimingHand.position), -aimingHand.up); //multiply with euler, to make lookRotation Aim At Y and not z axies
            //aimingHand.localRotation *= Quaternion.Euler(-90f, 180f, 0f);
            // aimingHand.LookAt(aimAtTarget, aimingHand.forward); 
        }
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

       
    }

    public void StopAimAt()
    {
        aimAtActive = false;
        movementController.manualRotation = false;
    }

    public void AimWeaponAtTarget()//Transform target)
    {
        //weaponAimingConstraint.weight = 1;
        weaponAimingRig.weight = 1;
        aimWeapon = true;

        currentWeaponAimDirection = currentSpineDirection;

        // Modify the compensation weight to achieve correct weapon aim and correct hand ik's
        // cause LookAt modifies the hand and arm positions in Lateupdate
        lookAtCompensationWeightBeforeAimingWeapon = fLookAnimator.CompensationWeight;
        lookAtCompensatiePositionsBeforeAimingWeapon = fLookAnimator.CompensatePositions;

        fLookAnimator.CompensationWeight = 1;
        fLookAnimator.CompensatePositions = 1;
    }

    public void StopAimingWeaponAtTarget()
    {
        //weaponAimingConstraint.weight = 0;
        weaponAimingRig.weight = 0;
        aimWeapon = false;

        // Modify fLookATWeights to their values before aiming the weapon.
        fLookAnimator.CompensationWeight = lookAtCompensationWeightBeforeAimingWeapon;
        fLookAnimator.CompensatePositions = lookAtCompensatiePositionsBeforeAimingWeapon;
    }

    /*private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;

        Gizmos.DrawLine(aimingDistanceReferencePoint.position, aimingDistanceReferencePoint.position + desiredSpineDirection);
    }*/
}
