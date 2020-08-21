using System.Collections;
using System.Collections.Generic;
using System.Security.Policy;
using UnityEngine;
using UnityEngine.AI;

public class EC_HumanoidMovementController : EntityComponent
{
    #region Fields

    [Header("References")]
    [SerializeField]
    protected NavMeshAgent agent;
    [SerializeField]
    protected Rigidbody rb;
    [SerializeField]
    protected EC_HumanoidAnimationController humanoidAnimationController;


    protected enum MovementState
    {
        Default,
        BeingPushed,
    }
    protected MovementState movementState;

    protected float maxAngularSpeed;   //for rotation independent of navmeshAgent;

    enum MovementType
    {
        Walk,
        Run,
        Crouch,
        Crawl
    }

    enum Stance
    {
        Standing,
        Crouching,
        Crawling
    }

    class MovementOrder
    {
        public Vector3 destination;
        public MovementType movementType;
        NavMeshAgent agent;
        
        public enum OrderExecutionStatus
        {
            Ordered,
            BeingExecuted,
            Paused,
            FinishedExecuting,
            NoOrder
        }

        OrderExecutionStatus orderExecutionStatus = OrderExecutionStatus.NoOrder;

        public MovementOrder(NavMeshAgent agent)
        {
            this.agent = agent;
        }

        public void Update()
        {
            if (orderExecutionStatus == OrderExecutionStatus.Ordered)
            {
                Execute();
            }
            else if (IsBeingExecuted())
            {
                float dist = agent.remainingDistance;
                if ((dist != Mathf.Infinity && agent.pathStatus == NavMeshPathStatus.PathComplete && agent.remainingDistance == 0))
                {
                    orderExecutionStatus = OrderExecutionStatus.FinishedExecuting;
                }
            }

            //Debug.Log("current state: " + orderExecutionStatus);
        }


        public void SetCurrentOrder(Vector3 destination)
        {
            orderExecutionStatus = OrderExecutionStatus.Ordered;
            this.destination = destination;
        }

        public void SetCurrentOrder(Vector3 destination, MovementType movementType)
        {
            orderExecutionStatus = OrderExecutionStatus.Ordered;
            this.destination = destination;
        }

        public void Execute()
        {
            agent.isStopped = false;
            agent.SetDestination(destination);
            orderExecutionStatus = OrderExecutionStatus.BeingExecuted;
        }

        public void Abort()
        {
            agent.ResetPath();
            orderExecutionStatus = OrderExecutionStatus.NoOrder;
        }

        public void Pause()
        {
            if(orderExecutionStatus == OrderExecutionStatus.BeingExecuted)
            {
                agent.isStopped = true;
                orderExecutionStatus = OrderExecutionStatus.Paused;
            }    
        }

        public void Resume()
        {
            agent.isStopped = false;
            orderExecutionStatus = OrderExecutionStatus.BeingExecuted;
        }
    
        #region Status Checks

        public bool IsBeingExecuted() 
        { 
            return orderExecutionStatus == OrderExecutionStatus.BeingExecuted;
        }

        public bool IsWaitingForExecution()
        {
            return orderExecutionStatus == OrderExecutionStatus.Paused || orderExecutionStatus == OrderExecutionStatus.Ordered;
        }

        #endregion
    }

    MovementOrder currentMovementOrder;


    [Header("Look At")]
    bool lookAt = false;
    public Transform spine;


    [Header("Debug")]
    [SerializeField]
    bool showGizmo;

    // Measuring angularVelocity
    float angularVelocity;
    Quaternion rotationLastFrame;

    public bool manualRotation;
    Quaternion desiredRotation;

    public float rotSpeed; //? needed?
    float stationaryTurnSpeed;
    float runningTurnSpeed;
    float runningSpeed;
    float walkingSpeed;
    float rouchingSpeed;
    //Vector3 angularVelocity;
    Vector3 desiredForward;


    #endregion

    public override void SetUpComponent(GameEntity entity)
    {
        maxAngularSpeed = agent.angularSpeed;   //almost the same speed as original navmeshAgent?
        currentMovementOrder = new MovementOrder(agent);
        rotationLastFrame = transform.rotation;

        if (manualRotation)
        {
            agent.updateRotation = false;
            desiredForward = transform.forward;
        }
    }

    // Update is only for looks- the rotation is important for logic but it can be a bit jaggy if far away or not on screen - lod this script, only call it every x seconds?
    public override void UpdateComponent()
    {
        /*if (!lookAt)
        {
            //reset the spine to normal position if look at is diabled
            if (spine.localRotation.eulerAngles != Vector3.zero)
            {
                Quaternion desiredSpineRotation = Quaternion.Euler(0, 0, 0);

                spine.localRotation = Quaternion.RotateTowards(spine.localRotation, desiredSpineRotation, angularSpeed * Time.deltaTime);

            }
        }*/
        currentMovementOrder.Update();


        //float rotationDelta = Quaternion.Euler(transform.rotation, Quaternion.LookRotation(agent.velocity));
        if(agent.desiredVelocity!= Vector3.zero)
        {
            desiredForward = agent.desiredVelocity;
        }
        Quaternion rotationDelta = transform.rotation * Quaternion.Inverse(Quaternion.LookRotation(desiredForward));

        // rot code copied from: https://gamedev.stackexchange.com/questions/147409/rotate-from-current-rotation-to-another-using-angular-velocity
        float angleDifference;
        float signedAngleDifference = 0;
        /*Vector3 axis;
        rotationDelta.ToAngleAxis(out angleDifference, out axis);

        //Debug.Log("current rotationDelta: " + rotationDelta.eulerAngles);
        
        Debug.Log("axis: " + axis);



        // We get an infinite axis in the event that our rotation is already aligned.
        if (float.IsInfinity(axis.x))
            return;*/

        angleDifference = rotationDelta.eulerAngles.y;

        signedAngleDifference = angleDifference;

        if (signedAngleDifference > 180f)
        {
            signedAngleDifference = signedAngleDifference - 360f;
            
        }
           
        Debug.Log("angleDifference: " + angleDifference);


        /*if (axis.y > 0) //instead of checking if its pointing down, we could do vector3 angle with the world up -> if >90 degrees - > its pointing down
        {
            signedAngleDifference = -angleDifference;
        }
        else
        {
            signedAngleDifference = angleDifference;
        }*/


        Debug.Log("signedAngleDifference: " + signedAngleDifference);

        // Here I drop down to 0.9f times the desired movement,
        // since we'd rather undershoot and ease into the correct angle
        // than overshoot and oscillate around it in the event of errors.

       // Vector3 angular = (0.9f * Mathf.Deg2Rad * angleDifference / Time.fixedDeltaTime) * axis.normalized;


        // angularVelocity = angular;

        // Clamp the angle difference and save it as current angular speed, also apply it

        if(angleDifference < 0.01f)
        {
            angularVelocity = 0;
        }
        else if(signedAngleDifference < 0.01f)
        {
            angularVelocity = rotSpeed;
        }
        else
        {
            angularVelocity = -rotSpeed;
        }
        
        
        Debug.Log("current angular vel: " + angularVelocity);

        //apply the velocity
        

        if (manualRotation)
        {
            if(agent.desiredVelocity != Vector3.zero)
            {
                //desiredRotation = Quaternion.LookRotation(agent.velocity);
                //desiredRotation = Quaternion.LookRotation(agent.desiredVelocity);
            }
            transform.rotation *= Quaternion.AngleAxis(angularVelocity * Time.deltaTime, transform.up);

            //transform.rotation = Quaternion.RotateTowards(transform.rotation, desiredRotation, rotSpeed * Time.deltaTime);
        }

        //angularVelocity = Vector3.Angle(transform.forward, forwardLastFrame) / Time.deltaTime; //degrees per second
        //Debug.Log("rot this frame: " + transform.rotation.eulerAngles);
        //Debug.Log("rot previous frame: " + rotationLastFrame.eulerAngles);
        //Debug.Log("change in degress: " + Quaternion.Angle(transform.rotation, rotationLastFrame));

        //angularVelocity = Quaternion.Angle(transform.rotation,rotationLastFrame) / Time.deltaTime; //degrees per second
        //rotationLastFrame = transform.rotation;

        humanoidAnimationController.UpdateAnimation(agent.velocity.magnitude, angularVelocity);
        //humanoidAnimationController.UpdateAnimation(agent.velocity.magnitude, 0);

       

    }


    #region Movement Orders

    public void RotateTo(Vector3 direction)
    {
        //only rotate on local x direction
        //first delete side movements- only leave y and z
        Vector3 directionForSpine = transform.InverseTransformDirection(direction);

        Quaternion desiredSpineRotation = Quaternion.LookRotation(directionForSpine);
        desiredSpineRotation = Quaternion.Euler(desiredSpineRotation.eulerAngles.x, 0, 0);
        spine.localRotation = Quaternion.RotateTowards(spine.localRotation, desiredSpineRotation, maxAngularSpeed * Time.deltaTime);



        direction.y = 0;
        Quaternion desiredLookRotation = Quaternion.LookRotation(direction);
        //because we want the same speed as the agent, which has its angular speed saved as degrees per second we use the rotaate towards function
        transform.rotation = Quaternion.RotateTowards(transform.rotation, desiredLookRotation, maxAngularSpeed * Time.deltaTime );
    }

    public void MoveTo(Vector3 destination)
    {
        Debug.Log("-------------------------------------------------------Movement order issued---------------------------------------------");
        currentMovementOrder.SetCurrentOrder(destination, MovementType.Run);
    }

    public void AbortMoving()
    {
        currentMovementOrder.Abort();
    }

    public void PauseMoving()
    {
        currentMovementOrder.Pause();
    }

    public void ResumeMoving()
    {
        currentMovementOrder.Resume();
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

            if (agent.desiredVelocity != null)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawLine(transform.position + Vector3.up, transform.position + agent.desiredVelocity + Vector3.up);

            }
        }
     }

    #endregion
}

