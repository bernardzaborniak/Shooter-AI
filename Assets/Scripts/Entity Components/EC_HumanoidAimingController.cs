using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EC_HumanoidAimingController : EntityComponent
{
    //is responsible for the correct aiming of the spine bones together with the animation? 
    //rotates differently depending on the current weapon?

    [Header("References")]
    public EC_HumanoidMovementController movementController;
    public Transform testingTarget;

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

    public override void SetUpComponent(GameEntity entity)
    {
        base.SetUpComponent(entity);
    }

    public override void UpdateComponent()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            AimInDirection(Vector3.right);
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
            Vector3 pointToAimAt = Vector3.zero;

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


            movementController.SetDesiredForward(pointToAimAt-transform.position);
        }
    }

    public void AimInDirection(Vector3 direction)
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
}
