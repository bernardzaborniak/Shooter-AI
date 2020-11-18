using DitzelGames.FastIK;
using FIMSpace.FLook;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Makes sure that the constraints are applied in the correct order (updates them) and (optimizes them - not yet implemented)
public class HumanoidConstraintController : MonoBehaviour
{
    [Header("Animator")]
    public bool optimiseAnimator;
    public Animator animator;
    public float animatorUpdateInterval;
    float nextAnimatorUpdateTime;
    float timeOfLastUpdate;
    
    
    [Header("1. Spine")]
    public Transform spineTarget;

    public Transform spineBone1;
    public Transform spineBone2;
    public Transform spineBone3;

    public float spine1Weight;
    public float spine2Weight;
    public float spine3Weight;

    [Space(5)]
    [Tooltip("0-1 value of how much of the animated y Rotation should be overriden")]
    [Range(0,1)]
    public float spineOverrideAnimatedYRotationWeight;

    [Header("1b. Head")]

    public bool headConstraint;
    public Transform headBone;
    public float headConstraintWeight;
    public float headConstraintXOffset;
    //public Vector3 headBoneRotOffset;


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
    public Transform transformToCopyRecoilFrom;

    public Transform leftHandTransform;
    public Transform leftHandIKTarget;
    public Transform rightHandTransform;
    public Transform rightHandIKTarget;

    [Tooltip("Depending on the specific model skeleton hand orientation the ik target is rotated by this offset")]
    public Vector3 leftHandIKRotationOffset;
    public Vector3 rightHandIKRotationOffset;
    //right [-90,180,0]
    //left [90,0,0]
    //[-90,0,180]

    public enum IKTargetingMode
    {
        AnimatedHandPosition,
        CustomPosition,//used for secondary hand weapon ik for example
    }
    IKTargetingMode leftHandIKTargetingMode;
    IKTargetingMode rightHandIKTargetingMode;

    Vector3 leftHandIKTargetPosition;
    Vector3 rightHandIKTargetPosition;

    Quaternion leftHandIKTargetRotation;
    Quaternion rightHandIKTargetRotation;

   
    void Start()
    {

        weaponAimLocalStartRotation = weaponAimTransform.localRotation;

        lookAtAnimator.updateAutomaticlyInLateUpdate = false;

        leftHandIK.automaticlyUpdateInLateUpdate = false;
        rightHandIK.automaticlyUpdateInLateUpdate = false;

        //animator.enabled = false;
        timeOfLastUpdate = Time.time;
        nextAnimatorUpdateTime = Time.unscaledTime + Random.Range(0, animatorUpdateInterval);
        
    }


    void LateUpdate()
    {
        bool updateConstraints = true;

        if (optimiseAnimator)
        {
            
            if (Time.unscaledTime > nextAnimatorUpdateTime)
            {
                UnityEngine.Profiling.Profiler.BeginSample("Anim Optimised ");
                //Debug.Log("anm update");
                animator.Update((Time.time-timeOfLastUpdate));
                timeOfLastUpdate = Time.time;
                //nextAnimatorUpdateTime = nextAnimatorUpdateTime + animatorUpdateInterval;
                nextAnimatorUpdateTime = Time.unscaledTime + animatorUpdateInterval;

                UnityEngine.Profiling.Profiler.EndSample();

            }
            else
            {
                updateConstraints = false;
            }
            

        }

        if (updateConstraints)
        {
            #region 1. Orient/Update the spine Constraints first

            UnityEngine.Profiling.Profiler.BeginSample("Constraints ifs");
            // The recoil position is used to determine the roattion of the spine, as it is the back movement of the gun which rotates the spine
            Quaternion recoilRotationAdder = Quaternion.Euler(transformToCopyRecoilFrom.localPosition.z * 100, 0, 0); //used to add some recoil to spine

            Vector3 spineTargetPosition = spineTarget.position;

            // Spine 1
            if (spine1Weight > 0)
            {
                Vector3 directionToTargetS1 = spineTargetPosition - spineBone1.position;
                Quaternion targetRotationS1 = Quaternion.LookRotation(directionToTargetS1);
                // The rotation is only aplied at the bones local x axis, not on the z and y to improve the look of the aiming (as when saiming, your spine is slightly rotated to the side
                //targetRotationS1 = Quaternion.Euler(targetRotationS1.eulerAngles.x, spineBone1.rotation.eulerAngles.y, targetRotationS1.eulerAngles.z);
                targetRotationS1 = Quaternion.Euler(targetRotationS1.eulerAngles.x, Quaternion.Slerp(spineBone1.rotation, targetRotationS1, spineOverrideAnimatedYRotationWeight).eulerAngles.y, targetRotationS1.eulerAngles.z);

                Quaternion rotationDifferenceS1 = targetRotationS1 * Quaternion.Inverse(spineBone1.rotation);
                spineBone1.rotation = Quaternion.Slerp(Quaternion.identity, rotationDifferenceS1, spine1Weight) * spineBone1.rotation * recoilRotationAdder;
            }
            // Spine 2
            if (spine2Weight > 0)
            {
                Vector3 directionToTargetS2 = spineTargetPosition - spineBone2.position;
                Quaternion targetRotationS2 = Quaternion.LookRotation(directionToTargetS2);
                //targetRotationS2 = Quaternion.Euler(targetRotationS2.eulerAngles.x, spineBone2.rotation.eulerAngles.y, targetRotationS2.eulerAngles.z);
                targetRotationS2 = Quaternion.Euler(targetRotationS2.eulerAngles.x, Quaternion.Slerp(spineBone2.rotation, targetRotationS2, spineOverrideAnimatedYRotationWeight).eulerAngles.y, targetRotationS2.eulerAngles.z);

                Quaternion rotationDifferenceS2 = targetRotationS2 * Quaternion.Inverse(spineBone2.rotation);
                spineBone2.rotation = Quaternion.Slerp(Quaternion.identity, rotationDifferenceS2, spine2Weight) * spineBone2.rotation * recoilRotationAdder;
            }
            // Spine 3
            if (spine3Weight > 0)
            {
                Vector3 directionToTargetS3 = spineTargetPosition - spineBone3.position;
                Quaternion targetRotationS3 = Quaternion.LookRotation(directionToTargetS3);
                //targetRotationS3 = Quaternion.Euler(targetRotationS3.eulerAngles.x, spineBone3.rotation.eulerAngles.y, targetRotationS3.eulerAngles.z);
                targetRotationS3 = Quaternion.Euler(targetRotationS3.eulerAngles.x, Quaternion.Slerp(spineBone3.rotation, targetRotationS3, spineOverrideAnimatedYRotationWeight).eulerAngles.y, targetRotationS3.eulerAngles.z);

                Quaternion rotationDifferenceS3 = targetRotationS3 * Quaternion.Inverse(spineBone3.rotation);
                spineBone3.rotation = Quaternion.Slerp(Quaternion.identity, rotationDifferenceS3, spine3Weight) * spineBone3.rotation * Quaternion.Slerp(Quaternion.identity, Quaternion.Inverse(recoilRotationAdder), 0.5f);
            }
            #endregion

            #region 1.b Orient The Head - may get overriden by lookAtAnimator

            //uses the same target as spine
            /* if (headConstraintWeight > 0)
             {
                 Vector3 directionToTargetH = spineTargetPosition - headBone.position;
                 Quaternion targetRotationH = Quaternion.LookRotation(directionToTargetH);
                 //targetRotationH = Quaternion.Euler(targetRotationH.eulerAngles.x, spineBone2.rotation.eulerAngles.y, targetRotationH.eulerAngles.z);

                 //Quaternion rotationDifferenceH = targetRotationH * Quaternion.Inverse(headBone.rotation);
                 headBone.rotation = Quaternion.Slerp(Quaternion.identity, targetRotationH, headConstraintWeight);// * headBone.rotation; //* recoilRotationAdder;
             }*/

            if (headConstraint)
            {
                if (headConstraintWeight > 0)
                {
                    Vector3 directionToTargetH = spineTargetPosition - headBone.position;
                    Quaternion targetRotationH = Quaternion.LookRotation(directionToTargetH);
                    // targetRotationH =  Quaternion.Euler(targetRotationH.eulerAngles.x, headBone.rotation.eulerAngles.y, targetRotationH.eulerAngles.z) * Quaternion.Euler(headBoneRotOffset);
                    //targetRotationH =  Quaternion.Euler(targetRotationH.eulerAngles.x, headBone.rotation.eulerAngles.y, targetRotationH.eulerAngles.z) * Quaternion.AngleAxis(50, transform.right);

                    //targetRotationH = Quaternion.Euler(targetRotationH.eulerAngles.x, headBone.rotation.eulerAngles.y, targetRotationH.eulerAngles.z);
                    //targetRotationH = Quaternion.Euler(targetRotationH.eulerAngles.x + headConstraintXOffset, headBone.rotation.eulerAngles.y, targetRotationH.eulerAngles.z);
                    targetRotationH = Quaternion.Euler(targetRotationH.eulerAngles.x + headConstraintXOffset, Quaternion.Slerp(headBone.rotation, targetRotationH, spineOverrideAnimatedYRotationWeight).eulerAngles.y, targetRotationH.eulerAngles.z);
                    //targetRotationH = Quaternion.Euler(targetRotationH.eulerAngles.x, headBone.rotation.eulerAngles.y, targetRotationH.eulerAngles.z) * Quaternion.AngleAxis(headConstraintXOffset, headBone.transform.right);


                    //Quaternion rotationOffsetTransformedFromLocalToWorldSpace = transform.rotation * Quaternion.Euler(headBoneRotOffset);
                    //targetRotationH =  Quaternion.Euler(targetRotationH.eulerAngles.x, headBone.rotation.eulerAngles.y, targetRotationH.eulerAngles.z) * rotationOffsetTransformedFromLocalToWorldSpace;

                    //Quaternion rotationDifferenceH = targetRotationH * Quaternion.Inverse(headBone.rotation);
                    //headBone.rotation = Quaternion.Slerp(Quaternion.identity, targetRotationH, headConstraintWeight);// * headBone.rotation; //* recoilRotationAdder;
                    headBone.rotation = Quaternion.Slerp(headBone.rotation, targetRotationH, headConstraintWeight);// * headBone.rotation; //* recoilRotationAdder;
                }
            }

            #endregion

            #region 2. Update LookAtAnimator

            if (lookAtAnimator.enabled)
            {
                lookAtAnimator.UpdateLookAnimator();
            }

            #endregion

            #region 3. Orient the Weapon Aim Constraint
            if(weaponAimWeight > 0)  //No Slerp with weapon weight needed here, it can be found right Hand IK
            {
                Vector3 directionToTargetW = weaponAimTarget.position - weaponAimTransform.position;
                Quaternion targetRotationW = Quaternion.LookRotation(directionToTargetW);

                Quaternion rotationDifferenceW = targetRotationW * Quaternion.Inverse(weaponAimTransform.parent.rotation * weaponAimLocalStartRotation);
                weaponAimTransform.rotation = rotationDifferenceW * (weaponAimTransform.parent.rotation * weaponAimLocalStartRotation); //instead use the aimingWeight somewhere else
                //weaponAimTransform.rotation = Quaternion.Slerp(Quaternion.identity, rotationDifferenceW, weaponAimWeight) * (weaponAimTransform.parent.rotation * weaponAimLocalStartRotation); //instead use the aimingWeight somewhere else
            }
            UnityEngine.Profiling.Profiler.EndSample();


            #endregion

            UnityEngine.Profiling.Profiler.BeginSample("Constraints IKS");
            #region 4. Update The IK's

            // 1. Set The  left IK Position
            // The IK targets are set here in late update, cause some IK targets are targeting the animated hand position - for recoil to work correctly, so they need to be set after the animator updated the positions


            if (leftHandIKTargetingMode == IKTargetingMode.CustomPosition)
            {
                leftHandIKTarget.position = leftHandIKTargetPosition;
                leftHandIKTarget.rotation = leftHandIKTargetRotation * Quaternion.Euler(leftHandIKRotationOffset);
            }
            else
            {
                leftHandIKTarget.position = leftHandTransform.position;
                leftHandIKTarget.rotation = leftHandTransform.rotation;
            }

            // 2. Set The  right IK Position - Apply recoil only to right hand IK
            //this is specific to the skeleton hand orientation - i dont know how to change it otherwise than hardcode here
            Quaternion recoil = Quaternion.Euler(-transformToCopyRecoilFrom.localRotation.eulerAngles.x, 0, transformToCopyRecoilFrom.localRotation.eulerAngles.y);

            if (weaponAimWeight > 0)
            {
                rightHandIKTarget.position = Vector3.Lerp(rightHandTransform.position + transformToCopyRecoilFrom.parent.TransformVector(transformToCopyRecoilFrom.localPosition), rightHandIKTargetPosition + transformToCopyRecoilFrom.parent.TransformVector(transformToCopyRecoilFrom.localPosition), weaponAimWeight);
                rightHandIKTarget.rotation = Quaternion.Slerp(rightHandTransform.rotation * recoil, rightHandIKTargetRotation * Quaternion.Euler(rightHandIKRotationOffset) * recoil, weaponAimWeight);
            }
            else
            {
                // The same as above, but performance optimisation through shorter calculation when weight = 0.
                rightHandIKTarget.position = rightHandTransform.position + transformToCopyRecoilFrom.parent.TransformVector(transformToCopyRecoilFrom.localPosition);
                rightHandIKTarget.rotation = rightHandTransform.rotation * recoil;
            }


            // 3. Resolve IK's


            leftHandIK.ResolveIK();
            rightHandIK.ResolveIK();

            UnityEngine.Profiling.Profiler.EndSample();


            #endregion
        }
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
    }



}
