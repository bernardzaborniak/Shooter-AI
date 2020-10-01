using DitzelGames.FastIK;
using FIMSpace.FLook;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HumanoidConstraintController : MonoBehaviour
{
    [Header("1. Spine")]
    public Transform spineTarget;

    public Transform spineBone1;
    public Transform spineBone2;
    public Transform spineBone3;

    public float spine1Weight;
    public float spine2Weight;
    public float spine3Weight;

    [Header("2- Look At Animator")]
    public Transform lookAtTarget; //here?
    public FLookAnimator lookAtAnimator;

    [Header("3. Weapon Aiming")]
    public Transform weaponAimTarget;
    public Transform weaponAimTransform;
    public float weaponAimWeight;
    Quaternion weaponAimLocalStartRotation;

    [Header("4. Hand IK's")]
    public FastIKFabricOtherRotation leftHandIK;
    public FastIKFabricOtherRotation rightHandIK;
    [Space(10)]
    public Transform leftHandIKWeightedTarget;
    public Transform leftHandIKDesiredTarget;
    public Transform leftHandTransform;
    [Space(10)]
    public Transform rightHandIKWeightedTarget;  // lerp between current and desired by weight
    public Transform rightHandIKDesiredTarget;
    public Transform rightHandTransform;
    [Space(10)]
    public float leftHandIKWeight;
    public float rightHandIKWeight;


    //one object which takes care of all constraints on a character

    void Start()
    {
        weaponAimLocalStartRotation = weaponAimTransform.localRotation;

        lookAtAnimator.updateAutomaticlyInLateUpdate = false;

        leftHandIK.automaticlyUpdateInLateUpdate = false;
        rightHandIK.automaticlyUpdateInLateUpdate = false;
    }

    void LateUpdate()
    {
        #region 1. Orient the spine Constraints first

        Vector3 spineTargetPosition = spineTarget.position; ;


        // Spine 1
        Vector3 directionToTargetS1 = spineTargetPosition - spineBone1.position;
        Quaternion targetRotationS1 = Quaternion.LookRotation(directionToTargetS1);

        Quaternion rotationDifferenceS1 = targetRotationS1 * Quaternion.Inverse(spineBone1.rotation);
        spineBone1.rotation = Quaternion.Slerp(Quaternion.identity, rotationDifferenceS1, spine1Weight) * spineBone1.rotation;

        // Spine 2
        Vector3 directionToTargetS2 = spineTargetPosition - spineBone2.position;
        Quaternion targetRotationS2 = Quaternion.LookRotation(directionToTargetS2);

        Quaternion rotationDifferenceS2 = targetRotationS2 * Quaternion.Inverse(spineBone2.rotation);
        spineBone2.rotation = Quaternion.Slerp(Quaternion.identity, rotationDifferenceS2, spine2Weight) * spineBone2.rotation;

        // Spine 3
        Vector3 directionToTargetS3 = spineTargetPosition - spineBone3.position;
        Quaternion targetRotationS3 = Quaternion.LookRotation(directionToTargetS3);

        Quaternion rotationDifferenceS3 = targetRotationS3 * Quaternion.Inverse(spineBone3.rotation);
        spineBone3.rotation = Quaternion.Slerp(Quaternion.identity, rotationDifferenceS3, spine3Weight) * spineBone3.rotation;

        #endregion

        #region 2. Update LookAtAnimator

        if (lookAtAnimator.enabled)
        {
            lookAtAnimator.UpdateLookAnimator();
        }
        

        #endregion

        #region 3. Orient the Weapon Aim Constraint

        Vector3 directionToTargetW = weaponAimTarget.position - weaponAimTransform.position;
        Quaternion targetRotationW = Quaternion.LookRotation(directionToTargetW);


        Quaternion rotationDifferenceW = targetRotationW * Quaternion.Inverse(weaponAimTransform.parent.rotation * weaponAimLocalStartRotation);
        weaponAimTransform.rotation = Quaternion.Slerp(Quaternion.identity, rotationDifferenceW, weaponAimWeight) * (weaponAimTransform.parent.rotation * weaponAimLocalStartRotation);


        #endregion

        #region 4. Update The IK's

        //set the positions according to weight
        leftHandIKWeightedTarget.position = Vector3.Lerp(leftHandTransform.position, leftHandIKDesiredTarget.position, leftHandIKWeight);
        leftHandIKWeightedTarget.rotation = Quaternion.Slerp(leftHandTransform.rotation, leftHandIKDesiredTarget.rotation, leftHandIKWeight);

        rightHandIKWeightedTarget.position = Vector3.Lerp(rightHandTransform.position, rightHandIKDesiredTarget.position, rightHandIKWeight);
        rightHandIKWeightedTarget.rotation = Quaternion.Slerp(rightHandTransform.rotation, rightHandIKDesiredTarget.rotation, rightHandIKWeight);

        leftHandIK.ResolveIK();
        rightHandIK.ResolveIK();

        #endregion
    }



}
