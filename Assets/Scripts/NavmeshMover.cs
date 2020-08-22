using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NavmeshMover : MonoBehaviour
{
    //public NavMeshAgent agent;
    public Camera playerCamera;

    public EC_HumanoidMovementController humanoidMovementController;
    //public Animator animator;

    public Vector3 lastForward;

    private void Start()
    {
        lastForward = transform.forward;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = playerCamera.ScreenPointToRay(Input.mousePosition);

            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                // agent.SetDestination(hit.point);
                humanoidMovementController.MoveTo(hit.point);
            }
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            humanoidMovementController.PauseMoving();
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            humanoidMovementController.ResumeMoving();
        }

        /* float agentSpeedNormalized = Utility.Remap(agent.velocity.magnitude, 0.2f, agent.speed, 0, 1);
         if (agentSpeedNormalized < 0) agentSpeedNormalized = 0;


         animator.SetFloat("Forward", agentSpeedNormalized);


         Vector3 currentForward = transform.forward;
         float currentAngularVelocity = Vector3.Angle(currentForward, lastForward) / Time.deltaTime; //degrees per second
         lastForward = currentForward;
         //Debug.Log("currentAngularVelocity: " + currentAngularVelocity);

         float agentAngularSpeedNormalized = 0;
         if (currentAngularVelocity > 0)
         {
             agentAngularSpeedNormalized = Utility.Remap(currentAngularVelocity, 0.2f, agent.angularSpeed/2, 0, 1);
             if (agentAngularSpeedNormalized < 0)
             {
                 agentAngularSpeedNormalized = 0;
             }
         }
         else if (currentAngularVelocity < 0)
         {
             agentAngularSpeedNormalized = Utility.Remap(currentAngularVelocity, -0.2f, -agent.angularSpeed/2, 0, -1);
             if (agentAngularSpeedNormalized > 0)
             {
                 agentAngularSpeedNormalized = 0;
             }
         }

         animator.SetFloat("Turn", agentAngularSpeedNormalized);*/



    }
}
