using DitzelGames.FastIK;
using FIMSpace.FLook;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Makes sure that the constraints are applied in the correct order (updates them) and (optimizes them - not yet implemented)
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
    public FLookAnimator lookAtAnimator;

    [Header("3. Weapon Aiming")]
    public Transform weaponAimTarget;
    public Transform weaponAimTransform;
    public float weaponAimWeight;
    Quaternion weaponAimLocalStartRotation;

    [Header("4. Hand IK's")]
    public WhireWhizTwoBoneIK leftHandIK;
    public WhireWhizTwoBoneIK rightHandIK;

    //the ik targets are set here in late update, cause sme ik targets are targeting the animated hand position - for recoil to work correctly
    public Transform leftHandTransform;
    public Transform leftHandIKTarget;
    public Transform rightHandTransform;
    public Transform rightHandIKTarget;

    public enum IKTargetingMode
    {
        AnimatedHandPosition,
        CustomPosition
    }
    IKTargetingMode leftHandIKTargetingMode;
    IKTargetingMode rightHandIKTargetingMode;

    Vector3 leftHandIKTargetPosition;
    Vector3 rightHandIKTargetPosition;

    Quaternion leftHandIKTargetRotation;
    Quaternion rightHandIKTargetRotation;

    public Transform transformToCopyRecoilFrom;


    void Start()
    {
        weaponAimLocalStartRotation = weaponAimTransform.localRotation;

        lookAtAnimator.updateAutomaticlyInLateUpdate = false;

        leftHandIK.automaticlyUpdateInLateUpdate = false;
        rightHandIK.automaticlyUpdateInLateUpdate = false;
    }

    void LateUpdate()
    {
        #region 1. Orient/Update the spine Constraints first

        // The recoil position is used to determine the roation of the spin, as it is the back movement of the gun which rotates the spine
        Quaternion recoilRotationAdder = Quaternion.Slerp(Quaternion.identity, Quaternion.Euler(transformToCopyRecoilFrom.localPosition.z*200,0,0), 0.5f); //used to add some recoil to spine

        Vector3 spineTargetPosition = spineTarget.position; ;

        // Spine 1
        Vector3 directionToTargetS1 = spineTargetPosition - spineBone1.position;
        Quaternion targetRotationS1 = Quaternion.LookRotation(directionToTargetS1);

        Quaternion rotationDifferenceS1 = targetRotationS1 * Quaternion.Inverse(spineBone1.rotation);
        spineBone1.rotation = Quaternion.Slerp(Quaternion.identity, rotationDifferenceS1, spine1Weight) * spineBone1.rotation * recoilRotationAdder;

        // Spine 2
        Vector3 directionToTargetS2 = spineTargetPosition - spineBone2.position;
        Quaternion targetRotationS2 = Quaternion.LookRotation(directionToTargetS2);

        Quaternion rotationDifferenceS2 = targetRotationS2 * Quaternion.Inverse(spineBone2.rotation);
        spineBone2.rotation = Quaternion.Slerp(Quaternion.identity, rotationDifferenceS2, spine2Weight) * spineBone2.rotation * recoilRotationAdder;

        // Spine 3
        Vector3 directionToTargetS3 = spineTargetPosition - spineBone3.position;
        Quaternion targetRotationS3 = Quaternion.LookRotation(directionToTargetS3);

        Quaternion rotationDifferenceS3 = targetRotationS3 * Quaternion.Inverse(spineBone3.rotation);
        spineBone3.rotation = Quaternion.Slerp(Quaternion.identity, rotationDifferenceS3, spine3Weight) * spineBone3.rotation * Quaternion.Inverse(recoilRotationAdder);

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

        // Set The IK Positions

        if(leftHandIKTargetingMode == IKTargetingMode.CustomPosition)
        {
            leftHandIKTarget.position = leftHandIKTargetPosition;
            leftHandIKTarget.rotation = leftHandIKTargetRotation;
        }
        else
        {
            leftHandIKTarget.position = leftHandTransform.position;
            leftHandIKTarget.rotation = leftHandTransform.rotation;
        }

        //apply recoil only to right hand IK
        if (rightHandIKTargetingMode == IKTargetingMode.CustomPosition)
        {
            rightHandIKTarget.position = rightHandIKTargetPosition + transformToCopyRecoilFrom.parent.TransformVector(transformToCopyRecoilFrom.localPosition);
            rightHandIKTarget.rotation = rightHandIKTargetRotation * Quaternion.Inverse(transformToCopyRecoilFrom.localRotation);
        }
        else
        {
            rightHandIKTarget.position = rightHandTransform.position + transformToCopyRecoilFrom.parent.TransformVector(transformToCopyRecoilFrom.localPosition);
            rightHandIKTarget.rotation = rightHandTransform.rotation * Quaternion.Inverse(transformToCopyRecoilFrom.localRotation);
        }



        // Resolve IK's

        // I dont know why, but we need to resolve them twice to have a nice result
        leftHandIK.ResolveIK();
        leftHandIK.ResolveIK();
        rightHandIK.ResolveIK();
        rightHandIK.ResolveIK();

        #endregion
    }


    public void SetDesiredLeftIKTarget(IKTargetingMode targetingMode, Vector3 targetPosition, Quaternion targetRotation)
    {
        leftHandIKTargetingMode = targetingMode;

        if (leftHandIKTargetingMode == IKTargetingMode.CustomPosition)
        {
            leftHandIKTargetPosition = targetPosition;
            leftHandIKTargetRotation = targetRotation;
        }
        /*else if(leftHandIKTargetingMode == IKTargetingMode.AnimatedHandPosition)
        {
            
        }*/
    }

    public void SetDesiredRightIKTarget(IKTargetingMode targetingMode, Vector3 targetPosition, Quaternion targetRotation)
    {
        rightHandIKTargetingMode = targetingMode;

        if (rightHandIKTargetingMode == IKTargetingMode.CustomPosition)
        {
            rightHandIKTargetPosition = targetPosition;
            rightHandIKTargetRotation = targetRotation;
        }
        /*else if (rightHandIKTargetingMode == IKTargetingMode.AnimatedHandPosition)
        {

        }*/
    }



}
