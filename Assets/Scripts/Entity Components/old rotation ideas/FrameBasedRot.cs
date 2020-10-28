using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrameBasedRot : MonoBehaviour
{
    public Transform targetRotation;
    public Transform transformToRotate;

    public float maxVelocity;
    public float acceleration;

    float currentVelocity;

    bool rotate;

    float timeRotationStarted;
    float deltaTimeCounter;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.V))
        {
            rotate = true;
            Debug.Log("---------------------------rotate------------------------");
            timeRotationStarted = Time.time;
            deltaTimeCounter = 0;
        }
        if (Input.GetKeyDown(KeyCode.C))
        {
            rotate = false;
        }


        if (rotate)
        {
            // 1. calculate distance left
            float distanceLeft = Vector3.Angle(targetRotation.forward, transformToRotate.forward);
            float initialVelocity = currentVelocity;

            Debug.Log(".------------------distance left: " + distanceLeft + " -----------");
            Debug.Log("time passed: " + (Time.time - timeRotationStarted));
           
            Debug.Log("time passed deltaTimeCounter: " + deltaTimeCounter);
            // 2. calculate distance & time of acceleration & decceleration
            float deccelTime = Mathf.Abs(0 - initialVelocity) / acceleration;
            float deccelDistance = initialVelocity * deccelTime + 0.5f * -acceleration * deccelTime * deccelTime;
            //float distanceAtWhichDeccelerationStarts = distanceLeft - deccelDistance;
            Debug.Log("decel time: " + deccelTime);
            Debug.Log("decel distance: " + deccelDistance);
            float anotherStopDistance = (initialVelocity * initialVelocity) / (2 * acceleration);
            Debug.Log("decel distance 2: " + anotherStopDistance);

            float distanceToTraverseThisFrame;
            //check if current distance is smaller or the same as deccel distane -> deccelerate
            if(distanceLeft <= deccelDistance)
            {
                // deccelerate
                Debug.Log("deccel");
                distanceToTraverseThisFrame = initialVelocity * Time.deltaTime + 0.5f * -acceleration * Time.deltaTime * Time.deltaTime;

                currentVelocity += -acceleration * Time.deltaTime;

            }
            else
            {
                //accelerate
                distanceToTraverseThisFrame = initialVelocity * Time.deltaTime + 0.5f * acceleration * Time.deltaTime * Time.deltaTime;

                //check against time;
                float otherDeltaTime = (Time.time - timeRotationStarted);
                float distanceTraveledOther = 0.5f * 10 * otherDeltaTime;
                Debug.Log("distance traveled should be : " + distanceTraveledOther);
                Debug.Log("distance leftshould be : " + (20- distanceTraveledOther));

                //check if the acceleration would move us past the decceleration distance
                if ((distanceLeft - distanceToTraverseThisFrame)<= deccelDistance)
                {
                    Debug.Log("-------------------------------------starting to break, here lies a problem - but which?------------------------------------");

                   
                    float distanceToTraverseBeforeTresholdIsReached = deccelDistance - (distanceLeft - distanceToTraverseThisFrame);
                    //Debug.Log("distanceToTraverseBeforeTresholdIsReached: " + distanceToTraverseBeforeTresholdIsReached);
                    float timeItTakesToTraverseThisDistance = Mathf.Sqrt((2 * acceleration * distanceToTraverseBeforeTresholdIsReached + initialVelocity * initialVelocity) / (acceleration * acceleration)) + (-initialVelocity / acceleration);
                   // Debug.Log("timeItTakesToTraverseThisDistance: " + timeItTakesToTraverseThisDistance);
                    //Debug.Log("Time.deltaTime : " + Time.deltaTime);

                    float timeRemainingForDecceleration = Time.deltaTime - timeItTakesToTraverseThisDistance;
                    //Debug.Log("timeRemainingForDecceleration: " + timeRemainingForDecceleration);

                    //now the initialVelocityChanges
                    currentVelocity += acceleration * timeItTakesToTraverseThisDistance;
                    initialVelocity = currentVelocity;
                    currentVelocity += -acceleration * timeRemainingForDecceleration;

                    float remainingDeccelerationDistance = (initialVelocity * timeRemainingForDecceleration + 0.5f * -acceleration * timeRemainingForDecceleration * timeRemainingForDecceleration);
                    //Debug.Log("remainingDeccelerationDistance: " + remainingDeccelerationDistance);
                    distanceToTraverseThisFrame = distanceToTraverseBeforeTresholdIsReached + remainingDeccelerationDistance;
                    //Debug.Log("distanceToTraverseThisFrame: " + distanceToTraverseThisFrame);
                }
                else
                {
                    currentVelocity += acceleration * Time.deltaTime;
                }
            }

            //cap the velocity to not accelerate down to negative speed when already at target speed
            if (distanceToTraverseThisFrame > distanceLeft)
            {
                Debug.Log("-----------------------------clamp dist----------------------");
                //distanceToTraverseThisFrame = distanceLeft;
            }
            Debug.Log("distanceToTraverseThisFrame: " + distanceToTraverseThisFrame);

            //apply the distance to the current rotation and calculate the velocity 


            //currentVelocity = distanceToTraverseThisFrame / Time.deltaTime;
            
            Debug.Log("velocity: " + currentVelocity);
            transformToRotate.rotation = Quaternion.RotateTowards(transformToRotate.rotation, targetRotation.rotation, distanceToTraverseThisFrame);

            deltaTimeCounter += Time.deltaTime;

            //else accelerate - check if the acceleration would cross the distance, if yes split distance into acceleration diatnce and decceleration

            // TODO Chekc theese values with debugs and correcting on paper

            //add max velocity later



        }


        /*// 2. calculate distance & time of acceleration & decceleration
          float deccelTime = Mathf.Abs(0 - maxVelocity) / acceleration;
          float deccelDistance = 0 * deccelTime + 0.5f * acceleration * deccelTime * deccelTime;
          float distanceAtWhichDeccelerationStarts = distanceLeft - deccelDistance;

          float accelTime = Mathf.Abs(maxVelocity - initialVelocity) / acceleration;
          float accelDistance = initialVelocity * accelTime + 0.5f * acceleration * accelTime * accelTime;

          // 3. check if acceleration and decceleration are crossing before reaching maxVelocity
          if((accelDistance+ deccelDistance)> distanceLeft)
          {
              float distanceAtWhichBothCross = distanceAtWhichDeccelerationStarts + (accelDistance - distanceAtWhichDeccelerationStarts) / 2f;
          }
          */
    }
}
