using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnotherFrameBased : MonoBehaviour
{
    public Transform targetRotationT;
    public Transform transformToRotate;

    private Quaternion currentRotation; // The current rotation
    Quaternion newRotation;
    private Quaternion targetRotation; // The rotation it is going towards
    private Vector4 rotationV; // Current rotation velocity
    public float maxSpeed;
    public float accelerationDistance; //basicly capping ditance- distances below this distance wont reach the max elocity , just distance/acceleration * maxSpeed
    public float smoothTime; // Smooth value between 0 and 1 //can go hihter too



    void Start()
    {
        // Set currentRotation and targetRotation
      
    }

    void Update()
    {
        currentRotation = transformToRotate.rotation;
        
        /*if(targetRotation!= targetRotationT.rotation)
        {
            float distance = Quaternion.Angle(currentRotation, targetRotationT.rotation);
            //adjust the smooth time to ensure constant speeds at big and at small angles
            smoothTime = Utility.CalculateSmoothTime(distance, maxSpeed, accelerationDistance);
            targetRotation = targetRotationT.rotation;
        }*/
        
       // newRotation = Utility.SmoothDamp(currentRotation, targetRotation, ref rotationV, smoothTime);

       // Debug.Log("relative v: " + Utility.CalculateSignedAngularSpeedAroundGlobalY(ref currentRotation, ref newRotation));

        transformToRotate.rotation = newRotation; 

    }
}
