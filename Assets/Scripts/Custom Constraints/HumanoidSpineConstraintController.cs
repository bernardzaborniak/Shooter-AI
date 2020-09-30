using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HumanoidSpineConstraintController : MonoBehaviour
{
    [Header("Spine")]
    public Transform spineTarget;

    public Transform spineBone1;
    public Transform spineBone2;
    public Transform spineBone3;

    public float spine1Weight;
    public float spine2Weight;
    public float spine3Weight;


    //one object which takes care of all constraints on a character



    void LateUpdate()
    {
        #region Orient the spine Constraints first

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

       
    }



}
