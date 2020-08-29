using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnotherFrameBased : MonoBehaviour
{
    public Transform targetRotationT;
    public Transform transformToRotate;

    private Quaternion currentRotation; // The current rotation
    private Quaternion targetRotation; // The rotation it is going towards
    private Vector4 rotationV; // Current rotation velocity
    public float smoothTime = 0.5f; // Smooth value between 0 and 1 //can go hihter too

    void Start()
    {
        // Set currentRotation and targetRotation
      
    }

    void Update()
    {
        currentRotation = transformToRotate.rotation;
        targetRotation = targetRotationT.rotation;


        currentRotation = Utility.SmoothDamp(currentRotation, targetRotation, ref rotationV, smoothTime); // Smoothly change the currentRotation towards the value of targetRotation
        Debug.Log("---------velocity-----------");
        Debug.Log("x: " + rotationV.x);
        Debug.Log("y: " + rotationV.y);
        Debug.Log("z: " + rotationV.z);
        Debug.Log("w: " + rotationV.w);

        transformToRotate.rotation = currentRotation; // Or whatever it is you are trying to rotate
    }

   
}
