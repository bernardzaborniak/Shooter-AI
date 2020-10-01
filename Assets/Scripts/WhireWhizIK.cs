using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Profiling;

//https://wirewhiz.com/how-to-code-two-bone-ik-in-unity/

//[ExecuteInEditMode]
public class WhireWhizIK : MonoBehaviour
{
    public Transform Upper;//root of upper arm
    public Transform Lower;//root of lower arm
    public Transform End;//root of hand
    public Transform Target;//target position of hand
    public Transform Pole;//direction to bend towards 
    public float UpperElbowRotation;//Rotation offsetts
    public float LowerElbowRotation;

    private float a;//values for use in cos rule
    private float b;
    private float c;
    private Vector3 en;//Normal of plane we want our arm to be on

    void LateUpdate()
    {
        Profiler.BeginSample("IK Update");
        a = Lower.localPosition.magnitude;
        b = End.localPosition.magnitude;
        c = Vector3.Distance(Upper.position, Target.position);
        en = Vector3.Cross(Target.position - Upper.position, Pole.position - Upper.position);
        //Debug.Log("The angle is: " + CosAngle(a, b, c));
        //Debug.DrawLine(Upper.position, Target.position);
        //Debug.DrawLine((Upper.position + Target.position) / 2, Lower.position);

        //Set the rotation of the upper arm
        Quaternion upperRotation = Quaternion.LookRotation(Target.position - Upper.position, Quaternion.AngleAxis(UpperElbowRotation, Lower.position - Upper.position) * (en));
        upperRotation *= Quaternion.Inverse(Quaternion.FromToRotation(Vector3.forward, Lower.localPosition));
        Upper.rotation = Quaternion.AngleAxis(-CosAngle(a, c, b), -en) * upperRotation;

        //set the rotation of the lower arm
        Quaternion lowerRotation = Quaternion.LookRotation(Target.position - Lower.position, Quaternion.AngleAxis(LowerElbowRotation, End.position - Lower.position) * (en));
        lowerRotation *= Quaternion.Inverse(Quaternion.FromToRotation(Vector3.forward, End.localPosition));
        Lower.rotation = lowerRotation;

        End.rotation = Target.rotation;
        //Lower.LookAt(Lower, Pole.position - Upper.position);
        //Lower.rotation = Quaternion.AngleAxis(CosAngle(a, b, c), en);
        Profiler.EndSample();
    }

    //function that finds angles using the cosine rule 
    float CosAngle(float a, float b, float c)
    {
        if (!float.IsNaN(Mathf.Acos((-(c * c) + (a * a) + (b * b)) / (-2 * a * b)) * Mathf.Rad2Deg))
        {
            return Mathf.Acos((-(c * c) + (a * a) + (b * b)) / (2 * a * b)) * Mathf.Rad2Deg;
        }
        else
        {
            return 1;
        }
    }
}
