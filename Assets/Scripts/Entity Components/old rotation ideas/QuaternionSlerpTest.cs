using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuaternionSlerpTest : MonoBehaviour
{
    //public Transform from;
    public enum RotationMode
    {
        PrimitiveSlerp,
        ProperSlerp,
        AdvancedCalculation
    }

    public RotationMode rotationMode;

    public Transform toRotate;
    public Transform target;

    public float rotSpeed;

    Quaternion lastStartRotation = Quaternion.identity;
    Quaternion currentTargetRotation;
    //float lastDistance;
    float interpolationSpeed;
    float interpolationProgress;

    bool rotate;
    float startetRotatingTime;
    float angleDifferenceAtStart;

    //for advanced calculation
    float currentAngularSpeed;
    float angularSpeedAtStart;
    public float acceleration;
    bool reachedMiddle;
    float switchedToDecceleratingTime;

    // Start is called before the first frame update
    void Start()
    {
        lastStartRotation = toRotate.rotation;
        currentTargetRotation = toRotate.rotation;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            Test();
            Debug.Log("test");
        }

        if (rotate)
        {
            if (Quaternion.Angle(toRotate.rotation, target.rotation) < 0.1f)
            {
                rotate = false;
                float rotTime = (Time.time - startetRotatingTime);
                Debug.Log("finished rotation after : " + rotTime + " seconds");
                Debug.Log("avr speed: " + angleDifferenceAtStart / rotTime);

            }
            else
            {
                if (rotationMode == RotationMode.ProperSlerp)
                {
                   
                    interpolationProgress += interpolationSpeed * Time.deltaTime;
                    float smoothInterpolationProgress = Mathf.SmoothStep(0, 1, interpolationProgress); //3rd value 0 to 1 persentage
                    toRotate.rotation = Quaternion.Slerp(lastStartRotation, currentTargetRotation, smoothInterpolationProgress);
                    Debug.Log("interpolation Progress: " + interpolationProgress);
                    Debug.Log("smoothInterpolationProgress: " + smoothInterpolationProgress);
                }
                else if(rotationMode == RotationMode.PrimitiveSlerp)
                {
                    toRotate.rotation = Quaternion.Slerp(toRotate.rotation, target.rotation, rotSpeed * Time.deltaTime);
                }
                else if(rotationMode == RotationMode.AdvancedCalculation)
                {
                    float timePassed = Time.time- startetRotatingTime;
                    //degreesDelta = angularSpeedAtStart* timePassed

                    //toRotate.rotation = Quaternion.Slerp(lastStartRotation, currentTargetRotation, degreesDelta);
                    //toRotate.rotation = Quaternion.RotateTowards(toRotate.rotation, target.rotation, degreesDelta);

                    // angleDifferenceAtStart/
                }
                  
            }
        }




    }

    void Test()
    {
        rotate = true;
        startetRotatingTime = Time.time;
       


        lastStartRotation = toRotate.rotation;
        currentTargetRotation = target.rotation;
        angleDifferenceAtStart = Quaternion.Angle(lastStartRotation, currentTargetRotation);
        Debug.Log("started rotating: angles: " + angleDifferenceAtStart);
        interpolationSpeed = rotSpeed / angleDifferenceAtStart;
        interpolationProgress = 0;

        //advanced calculation 
        //angularSpeedAtStart = currentAngularSpeed;
        angularSpeedAtStart = 0;
    }
}
