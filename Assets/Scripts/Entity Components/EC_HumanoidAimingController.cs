using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class EC_HumanoidAimingController : EntityComponent
{
    //is responsible for the correct aiming of the spine bones together with the animation? 
    //rotates differently depending on the current weapon?

    [Header("References")]
    public EC_HumanoidMovementController movementController;
    public Transform testingTarget;

    [Header("Spine Constraint for aiming up/down")]
    //[Tooltip("Spine Constraint needs a local target which is on the forward axis of the character, to it never rotates to the sides, the character is already rotating towards the sides")]
    public Transform spineConstraintTarget;
    //Transform spineConstraintLocalTargetParent;
    [Tooltip("Needed to detemrine if the current target needs looking up or looking down")]
    //public float aimingReferencePointLocalY;
    bool aimingUpwards;
    //public Transform spine1;
   // public Transform spine2;
    public Transform aimingDistanceReferencePoint; //could be spine 3

    public MultiAimConstraint spineConstraint1;
    public MultiAimConstraint spineConstraint2;
    public MultiAimConstraint spineConstraint3;

    [Header("Aim at Params")]
    [Tooltip("If we are aiming at a direction, we are aiming at a point which is the direction * this float value")]
    public float defaultDirectionAimDistance;
    public float defaultDirectionAimHeightOffset;

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
    }

    public override void UpdateComponent()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
           // AimInDirection(Vector3.right);
        }
        if (Input.GetKeyDown(KeyCode.K))
        {
            AimAtTransform(testingTarget);
        }
        if (Input.GetKeyDown(KeyCode.M))
        {
            StopAimAt();
        }

        if (aimAtActive)
        {
            // Basically all aim At Modes work the same that we have a point we are looking at in space, just the calculation of this point differs
            pointToAimAt = Vector3.zero;
            //directionToAim;

            if(currentAimAtTargetingMethod == AimAtTargetingMethod.Direction)
            {
                pointToAimAt = transform.position + aimDirection * defaultDirectionAimDistance + Vector3.up * defaultDirectionAimHeightOffset;
            }
            else if(currentAimAtTargetingMethod == AimAtTargetingMethod.Position)
            {
                pointToAimAt = aimAtPosition;
            }
            else if (currentAimAtTargetingMethod == AimAtTargetingMethod.Transform)
            {
                pointToAimAt = aimAtTransform.position;
            }

           // directionToAim = (pointToAimAt - (transform.position + transform.up * aimingReferencePointLocalY)).normalized;
            directionToAim = (pointToAimAt - aimingDistanceReferencePoint.position);

            // Rotate Horizontally
            movementController.SetDesiredForward(directionToAim.normalized); //do we need to normalize?

            // Rotate Vertically
            //Limit to rotation on the x axis only
            Vector3 spineDirection = transform.InverseTransformDirection(directionToAim);
            spineDirection.x = 0;
            Debug.Log("spineDirection: " + spineDirection);
            if (spineDirection.z < 0.01f) spineDirection.z = 0.01f; //dont allow negative values to prevent strange rotation
            spineDirection = transform.TransformDirection(spineDirection);

            spineConstraintTarget.position = aimingDistanceReferencePoint.position + spineDirection.normalized * 10; //Todo calculate correct distance here
            //spineConstraintTarget.position = aimingDistanceReferencPoint.position + spineDirection.normalized * directionToAim.sqrMagnitude; //Todo calculate correct distance here


            //1. set the weight depending on if the target is above or below the shoulders
            if (spineConstraintTarget.position.y > aimingDistanceReferencePoint.position.y)
            {
                aimingUpwards = true;
                spineConstraint1.weight = 0.05f;
                spineConstraint2.weight = 0.2f;
                spineConstraint3.weight = 0.4f;
            }
            else
            {
                aimingUpwards = false;
                spineConstraint1.weight = 0.2f;
                spineConstraint2.weight = 0.7f;
                spineConstraint3.weight = 1f;
            }
        }
        else
        {
            spineConstraint1.weight = 0f;
            spineConstraint2.weight = 0f;
            spineConstraint3.weight = 0f;
        }
    }

    public override void LateUpdateComponent()
    {
        /*if (aimAtActive)
        {
            //Limit to rotation on the x axis only
            Vector3 spineDirection = transform.InverseTransformDirection(directionToAim);
            spineDirection.x = 0;
            Debug.Log("spineDirection: " + spineDirection);
            if (spineDirection.z < 0.01f) spineDirection.z = 0.01f; //dont allow negative values to prevent strange rotation
            spineDirection = transform.TransformDirection(spineDirection);

            spineConstraintTarget.position = spine3.position + spineDirection.normalized * 10;
            //spine3.rotation = Quaternion.LookRotation(spineDirection);
            Debug.Log("rotation: " + spine3.rotation.eulerAngles);
        }*/
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
}
