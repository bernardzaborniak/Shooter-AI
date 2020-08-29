using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaxRotCode : MonoBehaviour
{
    public Transform targetRotationTransform;
    public Transform transformToRotate;

    public float maxVelocity;
    public float accelerationDistance;

    Quaternion targetRotation;
    Quaternion derivQuaternion;
    public float smoothTime;


    float biggestVel;
    void Start()
    {
        
    }

    void Update()
    {

        if(targetRotation != targetRotationTransform.rotation)
        {
            targetRotation = targetRotationTransform.rotation;
            float distance = Quaternion.Angle(transformToRotate.rotation, targetRotation);
            smoothTime = Utility.CalculateSmoothTime(distance, maxVelocity, accelerationDistance);
            biggestVel = 0;
        }

        transformToRotate.rotation = Utility.SmoothDamp(transformToRotate.rotation, targetRotationTransform.rotation, ref derivQuaternion, smoothTime);

        float currentVel = Utility.DerivToAngVel(transform.rotation, derivQuaternion).y;
        if (currentVel> biggestVel)
        {
            biggestVel = currentVel;
            Debug.Log("angle vel: " + biggestVel);
        }
        
    }
}
