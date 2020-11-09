using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[ExecuteInEditMode]

public class InverseKinematics : MonoBehaviour {

	public Transform upperArm;
	public Transform forearm;
	public Transform hand;
	public Transform poleTarget;
	public Transform target;
	[Space(20)]
	public Vector3 uppperArm_OffsetRotation;
	public Vector3 forearm_OffsetRotation;
	public Vector3 hand_OffsetRotation;
	[Space(20)]
	public bool handMatchesTargetRotation = true;
	[Space(20)]
	public bool debug;

	float angle;
	float upperArm_Length;
	float forearm_Length;
	float arm_Length;
	float targetDistance;
	float adyacent;

	[Range(0,1)]
	public float weight;


	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void LateUpdate () 
	{
		//Vector3 newHandPos;
		Quaternion newHandRot = hand.rotation;

		//Vector3 newForearmPos;
		Quaternion newForearmRot;

		//Vector3 newUpperArmPos;
		Quaternion newUpperArmRot;



        newUpperArmRot = Quaternion.LookRotation(target.position - upperArm.position, poleTarget.position - upperArm.position);
        //upperArm.LookAt(target, poleTarget.position - upperArm.position);
        newUpperArmRot = Quaternion.Euler(uppperArm_OffsetRotation) * newUpperArmRot;
        //upperArm.Rotate(uppperArm_OffsetRotation);

        Vector3 cross = Vector3.Cross(poleTarget.position - upperArm.position, forearm.position - upperArm.position);


         upperArm_Length = Vector3.Distance(upperArm.position, forearm.position);
         forearm_Length = Vector3.Distance(forearm.position, hand.position);
         arm_Length = upperArm_Length + forearm_Length;
         targetDistance = Vector3.Distance(upperArm.position, target.position);
         targetDistance = Mathf.Min(targetDistance, arm_Length - arm_Length * 0.001f);

         adyacent = ((upperArm_Length * upperArm_Length) - (forearm_Length * forearm_Length) + (targetDistance * targetDistance)) / (2 * targetDistance);

         angle = Mathf.Acos(adyacent / upperArm_Length) * Mathf.Rad2Deg;

        newUpperArmRot = Quaternion.AngleAxis(-angle, cross) * newUpperArmRot;
        //upperArm.RotateAround(upperArm.position, cross, -angle);

        newForearmRot = Quaternion.LookRotation(target.position - forearm.position, cross);
        //forearm.LookAt(target, cross);
        newForearmRot = Quaternion.Euler(forearm_OffsetRotation) * newForearmRot;
        //forearm.Rotate(forearm_OffsetRotation);

         if (handMatchesTargetRotation)
         {
            newHandRot = Quaternion.Euler(hand_OffsetRotation) * target.rotation;
            //hand.rotation = target.rotation;
            //hand.Rotate(hand_OffsetRotation);
        }

         if (debug)
         {
             if (forearm != null && poleTarget != null)
             {
                 Debug.DrawLine(forearm.position, poleTarget.position, Color.blue);
             }

             if (upperArm != null && target != null)
             {
                 Debug.DrawLine(upperArm.position, target.position, Color.red);
             }
         }


        hand.rotation = Quaternion.Slerp(hand.rotation, newHandRot, weight);

        forearm.rotation = Quaternion.Slerp(forearm.rotation, newForearmRot, weight);

        upperArm.rotation = Quaternion.Slerp(upperArm.rotation, newUpperArmRot, weight);




        /*
     
        upperArm.LookAt(target, poleTarget.position - upperArm.position);
        upperArm.Rotate(uppperArm_OffsetRotation);

        Vector3 cross = Vector3.Cross(poleTarget.position - upperArm.position, forearm.position - upperArm.position);
             
         upperArm_Length = Vector3.Distance(upperArm.position, forearm.position);
        forearm_Length = Vector3.Distance(forearm.position, hand.position);
        arm_Length = upperArm_Length + forearm_Length;
        targetDistance = Vector3.Distance(upperArm.position, target.position);
        targetDistance = Mathf.Min(targetDistance, arm_Length - arm_Length * 0.001f);

        adyacent = ((upperArm_Length * upperArm_Length) - (forearm_Length * forearm_Length) + (targetDistance * targetDistance)) / (2 * targetDistance);

        angle = Mathf.Acos(adyacent / upperArm_Length) * Mathf.Rad2Deg;

        upperArm.RotateAround(upperArm.position, cross, -angle);

        forearm.LookAt(target, cross);
        forearm.Rotate(forearm_OffsetRotation);

        if (handMatchesTargetRotation)
        {
            hand.rotation = target.rotation;
            hand.Rotate(hand_OffsetRotation);
        }

        if (debug)
        {
            if (forearm != null && poleTarget != null)
            {
                Debug.DrawLine(forearm.position, poleTarget.position, Color.blue);
            }

            if (upperArm != null && target != null)
            {
                Debug.DrawLine(upperArm.position, target.position, Color.red);
            }
        }*/


    }

    void OnDrawGizmos(){
		if (debug) {
			if(upperArm != null && poleTarget != null && hand != null && target != null && poleTarget != null){
				Gizmos.color = Color.gray;
				Gizmos.DrawLine (upperArm.position, forearm.position);
				Gizmos.DrawLine (forearm.position, hand.position);
				Gizmos.color = Color.red;
				Gizmos.DrawLine (upperArm.position, target.position);
				Gizmos.color = Color.blue;
				Gizmos.DrawLine (forearm.position, poleTarget.position);
			}
		}
	}

}
