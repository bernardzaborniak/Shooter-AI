using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.EventSystems; // used for DoesMouseClickOnThisPositionHitUIElement();


// This class ombines some usefull functions
public static class Utility
{
    public static float Remap(this float value, float from1, float to1, float from2, float to2)
    {
        return (value - from1) / (to1 - from1) * (to2 - from2) + from2;
    }

    #region Quaternion Utils

    //the queternion utils are from a guy from github: siehe unten
    /*
     * https://gist.github.com/maxattack/4c7b4de00f5c1b95a33b
    Copyright 2016 Max Kaufmann (max.kaufmann@gmail.com)
    Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
    The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
    */

    public static Quaternion AngVelToDeriv(Quaternion Current, Vector3 AngVel)
    {
        var Spin = new Quaternion(AngVel.x, AngVel.y, AngVel.z, 0f);
        var Result = Spin * Current;
        return new Quaternion(0.5f * Result.x, 0.5f * Result.y, 0.5f * Result.z, 0.5f * Result.w);
        //return new Quaternion(0.05f * Result.x, 0.05f * Result.y, 0.05f * Result.z, 0.05f * Result.w);
    }

    public static Vector3 DerivToAngVel(Quaternion Current, Quaternion Deriv)
    {
        var Result = Deriv * Quaternion.Inverse(Current);
        return new Vector3(2f * Result.x, 2f * Result.y, 2f * Result.z);  // this is some kind of custom velocity, not degrees per second
    }

    public static Vector3 DerivToAngVelCorrected(Quaternion Current, Quaternion Deriv)
    {
        var Result = Deriv * Quaternion.Inverse(Current);
        return new Vector3(120f * Result.x, 120f * Result.y, 120f * Result.z);  // this is some kind of custom velocity, not degrees per second
    }



    public static Quaternion IntegrateRotation(Quaternion Rotation, Vector3 AngularVelocity, float DeltaTime)
    {
        if (DeltaTime < Mathf.Epsilon) return Rotation;
        var Deriv = AngVelToDeriv(Rotation, AngularVelocity);
        var Pred = new Vector4(
                Rotation.x + Deriv.x * DeltaTime,
                Rotation.y + Deriv.y * DeltaTime,
                Rotation.z + Deriv.z * DeltaTime,
                Rotation.w + Deriv.w * DeltaTime
        ).normalized;
        return new Quaternion(Pred.x, Pred.y, Pred.z, Pred.w);
    }

    public static Quaternion SmoothDamp(Quaternion rot, Quaternion target, ref Quaternion deriv, float time)
    {
        if (Time.deltaTime < Mathf.Epsilon) return rot;

        // account for double-cover
        var Dot = Quaternion.Dot(rot, target);
        var Multi = Dot > 0f ? 1f : -1f;
        target.x *= Multi;
        target.y *= Multi;
        target.z *= Multi;
        target.w *= Multi;
        // smooth damp (nlerp approx)
        var Result = new Vector4(
            Mathf.SmoothDamp(rot.x, target.x, ref deriv.x, time),
            Mathf.SmoothDamp(rot.y, target.y, ref deriv.y, time),
            Mathf.SmoothDamp(rot.z, target.z, ref deriv.z, time),
            Mathf.SmoothDamp(rot.w, target.w, ref deriv.w, time)
        ).normalized;

        // ensure deriv is tangent
        var derivError = Vector4.Project(new Vector4(deriv.x, deriv.y, deriv.z, deriv.w), Result);
        deriv.x -= derivError.x;
        deriv.y -= derivError.y;
        deriv.z -= derivError.z;
        deriv.w -= derivError.w;

        return new Quaternion(Result.x, Result.y, Result.z, Result.w);
    }

    public static Quaternion SmoothDamp(Quaternion rot, Quaternion target, ref Quaternion deriv, float time, float maxSpeed, float deltaTime)
    {
        if (deltaTime < Mathf.Epsilon) return rot;

        // account for double-cover
        var Dot = Quaternion.Dot(rot, target);
        var Multi = Dot > 0f ? 1f : -1f;
        target.x *= Multi;
        target.y *= Multi;
        target.z *= Multi;
        target.w *= Multi;
        // smooth damp (nlerp approx)
        var Result = new Vector4(
            Mathf.SmoothDamp(rot.x, target.x, ref deriv.x, time, maxSpeed, deltaTime),
            Mathf.SmoothDamp(rot.y, target.y, ref deriv.y, time, maxSpeed, deltaTime),
            Mathf.SmoothDamp(rot.z, target.z, ref deriv.z, time, maxSpeed, deltaTime),
            Mathf.SmoothDamp(rot.w, target.w, ref deriv.w, time, maxSpeed, deltaTime)
        ).normalized;

        // ensure deriv is tangent
        var derivError = Vector4.Project(new Vector4(deriv.x, deriv.y, deriv.z, deriv.w), Result);
        deriv.x -= derivError.x;
        deriv.y -= derivError.y;
        deriv.z -= derivError.z;
        deriv.w -= derivError.w;

        return new Quaternion(Result.x, Result.y, Result.z, Result.w);
    }


    public static float CalculateSmoothTime(float remainingDistance, float averageSpeed, float accelerationDistance)
    {

        // to move over 1 degree with smooth 1 - the max speed is 735613 - this should be a coefficient (quaternionDampSpeedCoefficient)
        // formula: maxSpeed = (distance/smooth) * (coefficient: 0.735613)
        // smooth = (coefficient * distance)/maxSpeed

        // acceleration distance is basicly a capping ditance- distances below this distance wont reach the max elocity , just distance/acceleration * maxSpeed
        //this formal was changed to average velocity, nothing changed in the calculation, just the names

        float modifiedSpeed = averageSpeed;

        if (remainingDistance < accelerationDistance)
        {
            modifiedSpeed = (remainingDistance / accelerationDistance) * averageSpeed;
        }

        float smoothSpeed = (0.25f * remainingDistance) / modifiedSpeed;

        if (float.IsNaN(smoothSpeed)) //if modifiedMaxSpeed is 0 it returns an NaN - destryong the whole rotation
        {
            smoothSpeed = 0;
        }

        return smoothSpeed; 
    }



    #endregion

    #region Launch Angle Calculations

    //Formel von  https://gamedev.stackexchange.com/questions/53552/how-can-i-find-a-projectiles-launch-angle

    public static float CalculateProjectileLaunchAngle(float launchVelocity, Vector3 startPosition, Vector3 targetPosition, bool directShot = true)
    {
        Vector3 distDelta = targetPosition - startPosition;

        return CalculateProjectileLaunchAngle(launchVelocity, new Vector3(distDelta.x, 0f, distDelta.z).magnitude, distDelta.y, directShot);
    }

    public static float CalculateProjectileLaunchAngle(float speed, float horizontalDistance, float heightDifference, bool directShoot = true)
    {
        //directShoot i true dann nehmen wir die niedrigere Schussbahn, wenn false, dann eine kurvigere die mehr nach oben geht
        float theta = 0f;
        float gravityConstant = Physics.gravity.magnitude;

        if (directShoot)
        {
            theta = Mathf.Atan((Mathf.Pow(speed, 2) - Mathf.Sqrt(Mathf.Pow(speed, 4) - gravityConstant * (gravityConstant * Mathf.Pow(horizontalDistance, 2) + 2 * heightDifference * Mathf.Pow(speed, 2)))) / (gravityConstant * horizontalDistance));
        }
        else
        {
            theta = Mathf.Atan((Mathf.Pow(speed, 2) + Mathf.Sqrt(Mathf.Pow(speed, 4) - gravityConstant * (gravityConstant * Mathf.Pow(horizontalDistance, 2) + 2 * heightDifference * Mathf.Pow(speed, 2)))) / (gravityConstant * horizontalDistance));
        }

        return (theta * (180 / Mathf.PI));  //change into degrees
    }

    public static float CalculateTimeOfFlightOfProjectileLaunchedAtAnAngle(float projectileLaunchVelocity, float launchAngleInDegrees, Vector3 projectileLaunchPosition, Vector3 targetPosition)
    {
        float timeInAir;
        float g = Physics.gravity.magnitude;
        float vY = projectileLaunchVelocity * Mathf.Sin(launchAngleInDegrees * (Mathf.PI / 180));

        float startH = projectileLaunchPosition.y;
        float finalH = targetPosition.y;

        if (finalH < startH)
        {
            timeInAir = (vY + Mathf.Sqrt((float)(Mathf.Pow(vY, 2) - 4 * (0.5 * g) * (-(startH - finalH))))) / g;
        }
        else
        {
            float vX = projectileLaunchVelocity * Mathf.Cos(launchAngleInDegrees * (Mathf.PI / 180));
            float distanceX = Vector3.Distance(projectileLaunchPosition, targetPosition);
            timeInAir = distanceX / vX;
        }

        return timeInAir;
    }

    #endregion

    public static Quaternion CalculateRandomBloomInConeShapeAroundTransformForward(Transform relativeTransform, float bloomAngle) //relative transform would be the shoot point transform
    {
        // Imagine a circle one unit in front of the 0/0/0 point - the radius of the circle is depending on the desired bloom/spread angle. Now in this circle we do the randomInsideCircle.
       
        //tan(alpha) = b/a  -> tan(alpha) * a = b
        //a = 1, b varies

        float unitSphereRadius = Mathf.Tan(bloomAngle * Mathf.Deg2Rad);

        //to make the random more often in the middle of the circle, we add a random scaler, which reduces the radius
        Vector2 insideUnitCircle = Random.insideUnitCircle * unitSphereRadius * Random.Range(0.05f, 1f);

        return Quaternion.LookRotation(relativeTransform.TransformDirection(new Vector3(insideUnitCircle.x, insideUnitCircle.y, 1f)));
    }

    public static bool DoesMouseClickOnThisPositionHitUIElement(Vector3 mousePosition)
    {
        PointerEventData pointerEventData = new PointerEventData(EventSystem.current);
        pointerEventData.position = mousePosition;

        List<RaycastResult> results = new List<RaycastResult>();

        EventSystem.current.RaycastAll(pointerEventData, results);

        if (results.Count > 0) return true;
        else return false;
    }
}

