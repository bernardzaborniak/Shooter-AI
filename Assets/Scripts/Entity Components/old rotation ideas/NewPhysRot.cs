using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewPhysRot : MonoBehaviour
{

    float currentRotationTime;
    Quaternion targetRotation;

    public Transform transformToRotate;
    public Transform targetTransform;
    public float maxAcceleration;
    public float maxVelocity;
    float currentTargetVelocity;
    Vector3 currentVelocity;

    float currentBreakDistance;
    float currentAngleDifference;

    public bool rotating;

    void Start()
    {

    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            StartRotatingTowards(targetTransform.rotation);
            //try out with vector as direion and target too, aiming controller uses Vector3.smoothDamp
        }

        if (rotating)
        {
            Quaternion currentRotation = transformToRotate.rotation;
            currentRotationTime += Time.deltaTime;



            currentAngleDifference = Quaternion.Angle(currentRotation, targetRotation);

            if (currentAngleDifference < 0.1)
            {
                rotating = false;
                transformToRotate.rotation = targetRotation;
            }
            else
            {
                float currentVelocityMagnitude = currentVelocity.magnitude;
                currentBreakDistance = currentVelocityMagnitude * currentVelocityMagnitude / (2 * maxAcceleration);
                if(currentAngleDifference < currentBreakDistance)
                {

                }


                //currentVelocity += Mathf.Clamp(currentTargetVelocity - currentVelocity, -maxAcceleration * Time.deltaTime, maxAcceleration * Time.deltaTime);
                //currentVelocity = Mathf.Clamp(currentVelocity, -maxVelocity, maxVelocity);
                Debug.Log("currentVelocity: " + currentVelocity);

               // Quaternion newRotation = Quaternion.RotateTowards(currentRotation, targetRotation, currentVelocity * Time.deltaTime);




                Debug.Log("DeltaAngles: " + currentAngleDifference);


               // transformToRotate.rotation = newRotation;
            }









            
        }
    }

    void StartRotatingTowards(Quaternion target)
    {
        targetRotation = target;
        currentRotationTime = 0;
        rotating = true;
        //currentVelocity = 0;
        currentTargetVelocity = maxVelocity;
    }

}
