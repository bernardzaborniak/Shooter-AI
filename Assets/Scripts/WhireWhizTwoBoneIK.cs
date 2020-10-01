using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//https://wirewhiz.com/how-to-code-two-bone-ik-in-unity/

public class WhireWhizTwoBoneIK : MonoBehaviour
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

    //For optimising performance we cache some values
    Quaternion upperRotation;
    Vector3 upperPosition;
    Quaternion lowerRotation;
    Vector3 lowerPosition;
    Vector3 targetPosition;

    void LateUpdate()
    {
        upperPosition = Upper.position;
        lowerPosition = Lower.position;
        targetPosition = Target.position;

        a = Lower.localPosition.magnitude;
        b = End.localPosition.magnitude;
        c = Vector3.Distance(upperPosition, targetPosition);
        en = Vector3.Cross(targetPosition - upperPosition, Pole.position - upperPosition);

        //Set the rotation of the upper arm
        upperRotation = Quaternion.LookRotation(targetPosition - upperPosition, Quaternion.AngleAxis(UpperElbowRotation, lowerPosition - upperPosition) * (en));
        upperRotation *= Quaternion.Inverse(Quaternion.FromToRotation(Vector3.forward, Lower.localPosition));
        Upper.rotation = Quaternion.AngleAxis(-CosAngle(a, c, b), -en) * upperRotation;

        //set the rotation of the lower arm
        lowerRotation = Quaternion.LookRotation(targetPosition - lowerPosition, Quaternion.AngleAxis(LowerElbowRotation, End.position - lowerPosition) * (en));
        lowerRotation *= Quaternion.Inverse(Quaternion.FromToRotation(Vector3.forward, End.localPosition));
        Lower.rotation = lowerRotation;

        End.rotation = Target.rotation;
        //Lower.LookAt(Lower, Pole.position - Upper.position);
        //Lower.rotation = Quaternion.AngleAxis(CosAngle(a, b, c), en);
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
