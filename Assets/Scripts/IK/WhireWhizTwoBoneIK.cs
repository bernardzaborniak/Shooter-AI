using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//got it  from Eli from Wire Whiz, expanded it a bit
//https://wirewhiz.com/how-to-code-two-bone-ik-in-unity/

public class WhireWhizTwoBoneIK : MonoBehaviour
{
    public bool automaticlyUpdateInLateUpdate;
    [Space(10)]
    [Range(0,1)]
    public float weight;
    [Space(10)]
    public Transform Upper;//root of upper arm
    public Transform Lower;//root of lower arm
    public Transform End;//root of hand
    [Space(10)]
    public Transform Target;//target position of hand
    public Transform Pole;//direction to bend towards 
    [Space(10)]
    public float UpperElbowRotation;//Rotation offsetts
    public float LowerElbowRotation;

    private float a;//values for use in cos rule
    private float b;
    private float c;
    private Vector3 en;//Normal of plane we want our arm to be on

    //For optimising performance we cache some values
    Quaternion upperTargetRotation;
    Vector3 upperPosition;
    Quaternion lowerTargetRotation;
    Vector3 lowerPosition;
    Vector3 targetPosition;


    void LateUpdate()
    {
        if (automaticlyUpdateInLateUpdate)
        {
            ResolveIK();
        }
       
    }

    public void ResolveIK()//resolveCount -> how myn times is it resolved? //Vector3 handPositionBeforeIKResolve)  //the position of the end needs to be saved, as it changes if we reolve the iks twice in one frame
    {
        if (weight > 0)
        {
            upperPosition = Upper.position;
            lowerPosition = Lower.position;
            targetPosition = Vector3.Lerp(End.position, Target.position, weight);

            a = Lower.localPosition.magnitude;
            b = End.localPosition.magnitude;
            c = Vector3.Distance(upperPosition, targetPosition);
            en = Vector3.Cross(targetPosition - upperPosition, Pole.position - upperPosition);


            //Set the rotation of the upper arm
            upperTargetRotation = Quaternion.LookRotation(targetPosition - upperPosition, Quaternion.AngleAxis(UpperElbowRotation, lowerPosition - upperPosition) * (en));
            upperTargetRotation *= Quaternion.Inverse(Quaternion.FromToRotation(Vector3.forward, Lower.localPosition));
            upperTargetRotation = Quaternion.AngleAxis(-CosAngle(a, c, b), -en) * upperTargetRotation;
            Upper.rotation = Quaternion.Slerp(Upper.rotation, upperTargetRotation, weight);

            lowerPosition = Lower.position;

            //set the rotation of the lower arm
            lowerTargetRotation = Quaternion.LookRotation(targetPosition - lowerPosition, Quaternion.AngleAxis(LowerElbowRotation, End.position - lowerPosition) * (en));
            lowerTargetRotation *= Quaternion.Inverse(Quaternion.FromToRotation(Vector3.forward, End.localPosition));
            Lower.rotation = Quaternion.Slerp(Lower.rotation, lowerTargetRotation, weight);

            End.rotation = Quaternion.Slerp(End.rotation, Target.rotation, weight);
            //Lower.LookAt(Lower, Pole.position - Upper.position);
            //Lower.rotation = Quaternion.AngleAxis(CosAngle(a, b, c), en);
        }

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
