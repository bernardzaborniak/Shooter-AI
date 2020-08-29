using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Utility
{
    public static float Remap(this float value, float from1, float to1, float from2, float to2)
    {
        return (value - from1) / (to1 - from1) * (to2 - from2) + from2;
    }

    #region Smooth Rotation

    //https://answers.unity.com/questions/265325/smooth-rotation.html

    static Vector4 ToVector4(this Quaternion quaternion)
    {
        return new Vector4(quaternion.x, quaternion.y, quaternion.z, quaternion.w);
    }

    static Quaternion ToQuaternion(this Vector4 vector)
    {
        return new Quaternion(vector.x, vector.y, vector.z, vector.w);
    }

    static Vector4 SmoothDamp(Vector4 current, Vector4 target, ref Vector4 currentVelocity, float smoothTime)
    {
        float x = Mathf.SmoothDamp(current.x, target.x, ref currentVelocity.x, smoothTime);
        float y = Mathf.SmoothDamp(current.y, target.y, ref currentVelocity.y, smoothTime);
        float z = Mathf.SmoothDamp(current.z, target.z, ref currentVelocity.z, smoothTime);
        float w = Mathf.SmoothDamp(current.w, target.w, ref currentVelocity.w, smoothTime);

        return new Vector4(x, y, z, w);
    }

    public static Quaternion SmoothDamp(Quaternion current, Quaternion target, ref Vector4 currentVelocityVec4, float smoothTime)
    {
        Vector4 smooth = SmoothDamp(current.ToVector4(), target.ToVector4(), ref currentVelocityVec4, smoothTime);
        return smooth.ToQuaternion();
    }

    //returns only positive velocity values
    public static float CalculateAbsoluteAngularSpeed(ref Quaternion previousRotation, ref Quaternion newRotation)
    {
        return Quaternion.Angle(previousRotation, newRotation) / Time.deltaTime;
    }

    //returns signed values based on axiesalues //doesnt work yet
    public static float CalculateSignedAngularSpeed(ref Quaternion previousRotation, ref Quaternion newRotation, Vector3 vectorToWhichVelocityIsRelativeTo)
    {
        //
        float angleDifference;
        Vector3 axis;

        Quaternion rotationDelta = previousRotation * Quaternion.Inverse(newRotation);
        Debug.Log("delta angles: " + rotationDelta.eulerAngles);
        rotationDelta.ToAngleAxis(out angleDifference, out axis);
        Debug.Log("axis: " + axis);
        
       



        //if signed angle difference is >0 , it means the target is on the right side




        float signedAngleDifference = angleDifference;

        if (signedAngleDifference > 180f)
        {
            signedAngleDifference -= 360f;
        }

        //check if left or right  - check vector3 angle instead?
        //if (axis.x > vectorToWhichVelocityIsRelativeTo.x || axis.y> vectorToWhichVelocityIsRelativeTo.y || axis.z> vectorToWhichVelocityIsRelativeTo.z)
        if (Vector3.Angle(vectorToWhichVelocityIsRelativeTo,axis)>1f)
        {
            signedAngleDifference = -signedAngleDifference;
        }

        

        return signedAngleDifference / Time.deltaTime;

    }

    public static float CalculateSignedAngularSpeedAroundGlobalY(ref Quaternion previousRotation, ref Quaternion newRotation)
    {
        Quaternion rotationDelta = previousRotation * Quaternion.Inverse(newRotation);

        float angleDifference;
        float signedAngleDifference = 0;


        //if signed angle difference is >0 , it means the target is on the right side
        //if angular velocity is >0 , it means the velocity is going into the right side

        angleDifference = rotationDelta.eulerAngles.y;



        signedAngleDifference = angleDifference;

        if (signedAngleDifference > 180f)
        {
            signedAngleDifference -= 360f;
        }
        signedAngleDifference = -signedAngleDifference;


        return signedAngleDifference / Time.deltaTime;

    }

    // this formula should be called when the current tagret rotation changes, not every frame
    public static float CalculateSmoothTime(float remainingDistance, float maxSpeed, float accelerationDistance)
    {

        // to move over 1 degree with smooth 1 - the max speed is 735613 - this should be a coefficient (quaternionDampSpeedCoefficient)
        // formula: maxSpeed = (distance/smooth) * (coefficient: 0.735613)
        // smooth = (coefficient * distance)/maxSpeed

        // acceleration distance is basicly a capping ditance- distances below this distance wont reach the max elocity , just distance/acceleration * maxSpeed

        float modifiedMaxSpeed = maxSpeed;

        if (remainingDistance < accelerationDistance)
        {
            modifiedMaxSpeed = (remainingDistance / accelerationDistance) * maxSpeed;
        }

        return  (0.735613f * remainingDistance) / modifiedMaxSpeed;
    }




    #endregion
}
