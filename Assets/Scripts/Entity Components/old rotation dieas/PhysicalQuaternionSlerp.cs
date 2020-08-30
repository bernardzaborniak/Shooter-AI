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
    Vector3 currentTargetForward;
    Quaternion currentStartRotatingRotation;

    float initialVelocity;
    float maxVelocity;
    float angleDistance;
    float acceleration;

    float startedRotationTime;
    float defaultTime;   // time it would take to perform the rotation if initial velocity would be 0 degrees/s
    float rotationTime;  //time it takes to perform the rotation
    float switchDirectionTime;  //time when we start applying acceleration in the different direction
    bool isBraking;

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
            Rotate(target);
        }

        //Debug.Log("---------------------------------------------");
        //Debug.Log("difference: " + Quaternion.Angle(currentTargetRot, transformToRotate.rotation));
        //if(Quaternion.Angle(currentTargetRot, transformToRotate.rotation.normalized) > 0f)
        if(Vector3.Angle(currentTargetForward, transformToRotate.forward) > 0f)
        {
            //Debug.Log("--------------rot update------------");
            float deltaTime = Time.time - startedRotationTime;


            float distanceTraveled;

            if (!isBraking)
            {
                if(deltaTime < switchDirectionTime) 
                { 
                    distanceTraveled = initialVelocity * deltaTime + 0.5f * acceleration * deltaTime * deltaTime;
                }
                else
                {
                    isBraking = true;

                    //calculate distance traveled before the swithc, set the distanceTraveledAtSwitchValue
                    distanceTraveledAtSwitch = initialVelocity * switchDirectionTime + 0.5f * acceleration * switchDirectionTime * switchDirectionTime;
                    Debug.Log("switch-------------------------------- ");

                    //calculate distance draveled after the switch
                    float deltaTimeBraking = (deltaTime - switchDirectionTime); 
                    distanceTraveled = distanceTraveledAtSwitch + (maxVelocity * deltaTimeBraking + 0.5f * -acceleration * deltaTimeBraking * deltaTimeBraking);
                }
            }
            else
            {
                float deltaTimeBraking = (deltaTime - switchDirectionTime);
                distanceTraveled = distanceTraveledAtSwitch + (maxVelocity * deltaTimeBraking + 0.5f * -acceleration * deltaTimeBraking * deltaTimeBraking);
            }


            //Debug.Log("---------------------------------------");
            //Debug.Log("deltaTime: " + deltaTime);
            //Debug.Log("switchDirectionTime: " + switchDirectionTime);
            // Debug.Log("distanceTraveled: " + distanceTraveled);
            //Debug.Log("distanceTraveledLastFrame: " + distanceTraveledLastFrame);
           // Debug.Log("Time.deltaTime: " + Time.deltaTime);
            angularVelocity = (distanceTraveled - distanceTraveledLastFrame) / Time.deltaTime;
            distanceTraveledLastFrame = distanceTraveled;
            Debug.Log("angular Vel: " + angularVelocity);

            float percentageOfRotationTraveled = distanceTraveled/ angleDistance;  // 0 to 1;
            //Debug.Log("percentageOfRotationTraveled: " + percentageOfRotationTraveled);

            transformToRotate.rotation = Quaternion.Slerp(currentStartRotatingRotation, currentTargetRot, percentageOfRotationTraveled);
            //transformToRotate.rotation = Quaternion.RotateTowards(currentStartRotatingRotation, currentTargetRot, distanceTraveled);
            //Debug.Log("angle after applying rot: " + Quaternion.Angle(currentTargetRot, transformToRotate.rotation.normalized));
            //Debug.Log("vector angle after applying rot: " + Vector3.Angle(currentTargetForward, transformToRotate.forward));

        }
    }

    public void Rotate(Transform targeRotation)
    {
        isBraking = false;

        Debug.Log("rotate");
        currentTargetRot = targeRotation.rotation.normalized;
        currentTargetForward = targeRotation.forward;  // we needthe forward for angle difference, it is more accurate than Quaternion.Angle()
        currentStartRotatingRotation = transformToRotate.rotation.normalized;
        startedRotationTime = Time.time;

        angleDistance = Quaternion.Angle(currentTargetRot, currentStartRotatingRotation);
        Debug.Log("angleDIstance: " + angleDistance);
        initialVelocity = angularVelocity;
        maxVelocity = velocity * 2;

        defaultTime = angleDistance / velocity;   
        rotationTime = defaultTime / 2 + (defaultTime / 2) * ((maxVelocity - initialVelocity) / maxVelocity);
        switchDirectionTime = rotationTime - defaultTime / 2;

        acceleration = maxVelocity / (defaultTime / 2);
        Debug.Log("acceleration: " + acceleration);
    }
}
