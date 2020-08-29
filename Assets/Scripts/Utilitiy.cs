using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Utility
{
    public static float Remap(this float value, float from1, float to1, float from2, float to2)
    {
        return (value - from1) / (to1 - from1) * (to2 - from2) + from2;
    }

    //https://answers.unity.com/questions/265325/smooth-rotation.html

    public static Vector4 ToVector4(this Quaternion quaternion)
    {
        return new Vector4(quaternion.x, quaternion.y, quaternion.z, quaternion.w);
    }

    public static Quaternion ToQuaternion(this Vector4 vector)
    {
        return new Quaternion(vector.x, vector.y, vector.z, vector.w);
    }

    public static Vector4 SmoothDamp(Vector4 current, Vector4 target, ref Vector4 currentVelocity, float smoothTime)
    {
        float x = Mathf.SmoothDamp(current.x, target.x, ref currentVelocity.x, smoothTime);
        float y = Mathf.SmoothDamp(current.y, target.y, ref currentVelocity.y, smoothTime);
        float z = Mathf.SmoothDamp(current.z, target.z, ref currentVelocity.z, smoothTime);
        float w = Mathf.SmoothDamp(current.w, target.w, ref currentVelocity.w, smoothTime);

        return new Vector4(x, y, z, w);
    }

    public static Quaternion SmoothDamp(Quaternion current, Quaternion target, ref Vector4 currentVelocity, float smoothTime)
    {
        Vector4 smooth = SmoothDamp(current.ToVector4(), target.ToVector4(), ref currentVelocity, smoothTime);
        return smooth.ToQuaternion();
    }
}
