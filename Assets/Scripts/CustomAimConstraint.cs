using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using UnityEngine.Profiling;

public class CustomAimConstraint : MonoBehaviour
{
    public Transform target;
    [Range(0,1)]
    public float weight;




    private void Start()
    {

        
    }

    private void LateUpdate()
    {
       // Profiler.BeginSample("Aim At Constraint");
        Vector3 directionToTarget = target.position - transform.position;

        Quaternion targetRotation = Quaternion.LookRotation(directionToTarget);
        Quaternion rotationDifference = targetRotation * Quaternion.Inverse(transform.rotation);
        transform.rotation = Quaternion.Slerp(Quaternion.identity, rotationDifference, weight) * transform.rotation;
       // Profiler.EndSample();
    }

}
