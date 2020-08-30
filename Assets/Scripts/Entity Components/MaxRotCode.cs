using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaxRotCode : MonoBehaviour
{
    public Transform targetRotationTransform;
    public Transform transformToRotate;

    public float averageVelocity; //TODO first
    public float accelerationDistance;

    Quaternion targetRotation;
    Quaternion derivQuaternion;
    public float smoothTime;

    //For Development only
    public float roatationStartTime;
    public float rotationStartDistance;
    bool rot;
    Vector3 velocitysTHisRotations;
    int veCounter;
    //------



    void Start()
    {
        
    }

    void Update()
    {

        if(targetRotation != targetRotationTransform.rotation)
        {
            targetRotation = targetRotationTransform.rotation;
            float distance = Quaternion.Angle(transformToRotate.rotation, targetRotation);
            smoothTime = Utility.CalculateSmoothTime(distance, averageVelocity, accelerationDistance);

            //For Development only
            Debug.Log("rotation started-----------------------");
            roatationStartTime = Time.time;
            rotationStartDistance = distance;
            Debug.Log("rotation dist: " + rotationStartDistance);
            rot = true;
            velocitysTHisRotations = Vector3.zero;
            veCounter = 0;
            //------
        }

        Quaternion newRoation = Utility.SmoothDamp(transformToRotate.rotation, targetRotationTransform.rotation, ref derivQuaternion, smoothTime);
        velocitysTHisRotations += Utility.DerivToAngVelCorrected(transformToRotate.rotation, derivQuaternion);
        //Debug.Log("vel: " + Utility.DerivToAngVel(transformToRotate.rotation, derivQuaternion) / Time.deltaTime);
        transformToRotate.rotation = newRoation;

        //current quaternion is the one after or before? //TODO TRY
        veCounter++;

        //For Development only
        if (rot)
        {
            float distan = Quaternion.Angle(transformToRotate.rotation, targetRotation);
            if (distan < 0.01f)
            {
                Debug.Log("rotation finished-------------------------------");
                float rotTime = (Time.time - roatationStartTime);
                Debug.Log("rot time: " + rotTime);
                Debug.Log("rot avg vel: " + (rotationStartDistance / rotTime));

                Debug.Log("mean velocity x: " + (velocitysTHisRotations / veCounter).x);
                Debug.Log("mean velocity y: " + (velocitysTHisRotations / veCounter).y);
                Debug.Log("mean velocity z: " + (velocitysTHisRotations / veCounter).z);

                rot = false;
            }
        }
        //------

        float currentVel = Utility.DerivToAngVel(transform.rotation, derivQuaternion).y;
        
    }
}
