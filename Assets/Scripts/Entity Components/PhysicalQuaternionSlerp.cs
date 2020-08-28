using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicalQuaternionSlerp : MonoBehaviour
{
    public Transform transformToRotate;
    public Transform target;

    [Tooltip("avg velocity in degrees")]
    public float velocity;
 

    float angularVelocity;

    //for rotation
    Quaternion currentTargetRot;
    Quaternion currentStartRotatingRotation;

    float initialVelocity;
    float maxVelocity;
    float angleDistance;
    float acceleration;

    float startedRotationTime;
    float defaultTime;   // time it would take to perform the rotation if initial velocity would be 0 degrees/s
    float rotationTime;  //time it takes to perform the rotation
    float switchDirectionTime;  //time when we start applying acceleration in the different direction

    float distanceTraveledAtSwitch;

    float distanceTraveledLastFrame;

    void Start()
    {
        angularVelocity = 0;
        distanceTraveledLastFrame = 0;
        currentTargetRot = transformToRotate.rotation;
        currentStartRotatingRotation = transformToRotate.rotation;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.G))
        {
            Rotate(target.rotation);
        }



        if(Quaternion.Angle(currentTargetRot, transformToRotate.rotation) > 0f)
        {
            //Debug.Log("--------------rot update------------");
            float deltaTime = Time.time - startedRotationTime;


            float distanceTraveled;

            if (deltaTime< switchDirectionTime)
            {
                distanceTraveled = initialVelocity * deltaTime + 0.5f * acceleration * deltaTime * deltaTime;
                distanceTraveledAtSwitch = distanceTraveled;
            }
            else
            {
                float deltaTimeSecond = (deltaTime - switchDirectionTime);

                distanceTraveled = distanceTraveledAtSwitch + (maxVelocity * deltaTimeSecond + 0.5f * -acceleration * deltaTimeSecond * deltaTimeSecond);
            }
            //Debug.Log("deltaTime: " + deltaTime);

            Debug.Log("distanceTraveled: " + distanceTraveled);
            
            angularVelocity = (distanceTraveled - distanceTraveledLastFrame) / deltaTime;
            distanceTraveledLastFrame = distanceTraveled;
            //Debug.Log("angular Vel: " + angularVelocity);

            float percentageOfRotationTraveled = distanceTraveled/ angleDistance;  // 0 to 1;
            //Debug.Log("percentageOfRotationTraveled: " + percentageOfRotationTraveled);


            transformToRotate.rotation = Quaternion.Slerp(currentStartRotatingRotation, currentTargetRot, percentageOfRotationTraveled);
        }
    }

    public void Rotate(Quaternion targeRotation)
    {
        Debug.Log("rotate");
        currentTargetRot = targeRotation;
        currentStartRotatingRotation = transformToRotate.rotation;
        startedRotationTime = Time.time;

        angleDistance = Quaternion.Angle(currentTargetRot, currentStartRotatingRotation);
        Debug.Log("angleDIstance: " + angleDistance);
        initialVelocity = angularVelocity;
        maxVelocity = velocity * 2;

        defaultTime = angleDistance / velocity;   
        rotationTime = defaultTime / 2 + (defaultTime / 2) * ((maxVelocity - initialVelocity) / maxVelocity);
        switchDirectionTime = rotationTime - defaultTime / 2;

        acceleration = maxVelocity / (defaultTime / 2);
    }
}
