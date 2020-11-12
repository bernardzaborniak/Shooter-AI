using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using UnityEngine.Profiling;

public class CustomAimConstraint : MonoBehaviour
{
    public Transform target;
    [Range(0,1)]
    public float weight;


    public enum ConstraintType
    {
        TransformIsModifiedByAnimatorInPreUpdate,
        RelativeToParentsRotation,
        Absolute
    }
    public ConstraintType constraintType;
    //public bool transformIsModifiedByAnimatorInPreUpdate;
    Quaternion startRotation;



    private void Start()
    {

        if (constraintType == ConstraintType.RelativeToParentsRotation)
        {
            startRotation = transform.localRotation;
        }
        else if(constraintType == ConstraintType.Absolute)
        {
            startRotation = transform.rotation;
        }
    }

    private void LateUpdate()
    {
       // Profiler.BeginSample("Aim At Constraint");
       if(weight > 0)
       {
            Vector3 directionToTarget = target.position - transform.position;
            Quaternion targetRotation = Quaternion.LookRotation(directionToTarget);

            if(constraintType == ConstraintType.TransformIsModifiedByAnimatorInPreUpdate)
            {
                Quaternion rotationDifference = targetRotation * Quaternion.Inverse(transform.rotation);
                transform.rotation = Quaternion.Slerp(Quaternion.identity, rotationDifference, weight) * transform.rotation;
            }
            else if(constraintType == ConstraintType.RelativeToParentsRotation)
            {
                Quaternion rotationDifference = targetRotation * Quaternion.Inverse(transform.parent.rotation * startRotation);
                transform.rotation = Quaternion.Slerp(Quaternion.identity, rotationDifference, weight) * (transform.parent.rotation * startRotation);
            }
            else if(constraintType == ConstraintType.Absolute)
            {
                Quaternion rotationDifference = targetRotation * Quaternion.Inverse(startRotation);
                transform.rotation = Quaternion.Slerp(Quaternion.identity, rotationDifference, weight) * startRotation;
            }

            

            
       } 
       // Profiler.EndSample();
    }

}
