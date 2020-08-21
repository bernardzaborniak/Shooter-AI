using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MovementOld : EntityComponent
{
    #region Fields

    [Header("References")]
    [SerializeField]
    protected NavMeshAgent agent;
    [SerializeField]
    protected Rigidbody rb;


    protected enum MovementState
    {
        Default,
        BeingPushed,
    }
    protected MovementState movementState;

    protected float angularSpeed;   //for rotation independent of navmeshAgent;


    [Header("Look At")]
    bool lookAt = false;
    public Transform spine;


    [Header("PushPhysics")]
    public bool canBePushed;
    //bool isBeingPushed = false;
    [Tooltip("under which velocity is the pushed agent not considered pushed anymore")]
    [SerializeField]
    float pushEndTreshold;
    [Tooltip("a force must be larger than this force to initiate a push")]
    [SerializeField]
    float pushBeginnTreshold;
    float velocityLastTime;
    bool movementOrderIssuedWhileBeingPushed = false;
    Vector3 targetMovePositionNotYetOrdered;


    [Header("Debug")]
    [SerializeField]
    bool showGizmo;

    #endregion

    public override void SetUpComponent(GameEntity entity)
    {
        angularSpeed = agent.angularSpeed;   //almost the same speed as original navmeshAgent?

        pushEndTreshold *= pushEndTreshold; //because we cheking against squared values for optimisation
        pushBeginnTreshold *= pushBeginnTreshold;
    }

    // Update is only for looks- the rotation is important for logic but it can be a bit jaggy if far away or not on screen - lod this script, only call it every x seconds?
    public override void UpdateComponent()
    {
        if (!lookAt)
        {
            //reset the spine to normal position if look at is diabled
            if (spine.localRotation.eulerAngles != Vector3.zero)
            {
                Quaternion desiredSpineRotation = Quaternion.Euler(0, 0, 0);

                spine.localRotation = Quaternion.RotateTowards(spine.localRotation, desiredSpineRotation, angularSpeed * Time.deltaTime);

            }
        }

        if (Input.GetKeyDown(KeyCode.P))
        {
            Push(new Vector3(50, 0, 0));
        }
    }

    public override void FixedUpdateComponent()
    {
        if (movementState == MovementState.BeingPushed)
        {

            rb.AddForce(-transform.up * (Physics.gravity.magnitude), ForceMode.Acceleration);

            float velocityThisTime = rb.velocity.sqrMagnitude;

            //because of the start of the push the velocity can also be like 0, so we only check against the treshold, when the velocity is getting smaller
            if (velocityThisTime < velocityLastTime)
            {
                if (velocityThisTime < pushEndTreshold)
                {
                    //the unit can only move back to normal movement - if it is grounded -> raycast against navmesh
                    
                    //check if gorunded
                    RaycastHit hit;
                    // Does the ray intersect any objects excluding the player layer
                    if (Physics.Raycast(transform.position + transform.up*0.1f, -Vector3.up, out hit, 0.2f))
                    {
                        movementState = MovementState.Default;
                        if (agent != null)
                        {
                            agent.enabled = true;
                            rb.isKinematic = true;
                        }

                        if (movementOrderIssuedWhileBeingPushed)
                        {
                            MoveTo(targetMovePositionNotYetOrdered);
                        }
                    }               
                }
            }


            velocityLastTime = rb.velocity.sqrMagnitude;
        }
    }

    #region Movement Orders

    public virtual void Push(Vector3 force)
    {
        if (canBePushed)
        {
            //Debug.Log("push force is: " + force.sqrMagnitude);
            //Debug.Log("must be higher than: " + pushBeginnTreshold);
            if (force.sqrMagnitude > pushBeginnTreshold)
            {
                if (agent.destination != null)
                {
                    movementOrderIssuedWhileBeingPushed = true;
                    targetMovePositionNotYetOrdered = agent.destination;
                }

                movementState = MovementState.BeingPushed;
                Vector3 agentVelocity = agent.velocity;
                agent.enabled = false;
                rb.isKinematic = false;
                rb.velocity = agentVelocity;

                rb.AddForce(force, ForceMode.Impulse);
                velocityLastTime = 0;
            }
        }
    }

    //sets the agent to rotate 
    public void RotateTo(Vector3 direction)
    {
        //only rotate on local x direction
        //first delete side movements- only leave y and z
        Vector3 directionForSpine = transform.InverseTransformDirection(direction);

        Quaternion desiredSpineRotation = Quaternion.LookRotation(directionForSpine);
        desiredSpineRotation = Quaternion.Euler(desiredSpineRotation.eulerAngles.x, 0, 0);
        spine.localRotation = Quaternion.RotateTowards(spine.localRotation, desiredSpineRotation, angularSpeed * Time.deltaTime);



        direction.y = 0;
        Quaternion desiredLookRotation = Quaternion.LookRotation(direction);
        //because we want the same speed as the agent, which has its angular speed saved as degrees per second we use the rotaate towards function
        transform.rotation = Quaternion.RotateTowards(transform.rotation, desiredLookRotation, angularSpeed * Time.deltaTime );
    }

    //for now simple moveTo without surface ship or flying
    public void MoveTo(Vector3 destination)
    {
        if (movementState == MovementState.Default)
        {
            agent.SetDestination(destination);
        }
        else
        {
            movementOrderIssuedWhileBeingPushed = true;
            targetMovePositionNotYetOrdered = destination;
        }
    }

    public void Stop()
    {
        if (agent.isActiveAndEnabled)
        {
            agent.ResetPath();

        }
    }

    #endregion

    #region LookAt

    //this enables the use of the method lookAT
    public void ActivateLookAt()
    {
        lookAt = true;
        agent.updateRotation = false;
    }

    //looks into the desired direction, needs to be called every frame
    public void LookAt(Vector3 direction)
    {
        RotateTo(direction);
    }

    public void StopLookAt()
    {
        agent.updateRotation = true;
        lookAt = false;
    }


    #endregion

    #region Status Checks
    public virtual bool IsMoving()
    {
         return agent.velocity.magnitude > agent.speed / 2;
    }

    public float GetCurrentVelocityMagnitude()
    {
        return agent.velocity.magnitude;
    }

    public virtual Vector3 GetCurrentVelocity()
    {
        return agent.velocity;
    }

    public float GetMaxSpeed()
    {
        return agent.speed;
    }



    #endregion

    #region Debug

    private void OnDrawGizmos()
     {
         if (showGizmo && agent!=null)
         {
             if (agent.destination != null)
             {
                 Gizmos.color = Color.green;
                 Gizmos.DrawCube(agent.destination, new Vector3(0.2f, 2, 0.2f));

             }
         }
     }

    #endregion
}

