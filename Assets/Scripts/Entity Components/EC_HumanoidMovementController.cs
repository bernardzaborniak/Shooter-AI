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

    //protected float maxAngularSpeed;   //for rotation independent of navmeshAgent;

    public enum MovementType
    {
        Walk,
        Run,
        Crouch,
        Crawl
    }

    public enum Stance
    {
        Standing,
        Crouching,
        Crawling
    }

    //represents an information container about the current order, does not do any logic by itself
    class MovementOrder
    {
        public Vector3 destination;
        public MovementType movementType;
       // NavMeshAgent agent;

        public enum OrderExecutionStatus
        {
            Ordered,
            BeingExecuted,
            Paused,
            NoOrder
        }

         OrderExecutionStatus orderExecutionStatus = OrderExecutionStatus.NoOrder;

        public MovementOrder()
        {

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
            this.movementType = movementType;
        }

        public void OnExecute()
        {
            orderExecutionStatus = OrderExecutionStatus.BeingExecuted;
        }

        public void OnAbort()
        {
            orderExecutionStatus = OrderExecutionStatus.NoOrder;
        }

        public void OnFinishedExecuting()
        {
            orderExecutionStatus = OrderExecutionStatus.NoOrder;
        }

        public void OnPause()
        {
            orderExecutionStatus = OrderExecutionStatus.Paused;
        }

        public void OnResume()
        { 
            orderExecutionStatus = OrderExecutionStatus.BeingExecuted;
        }

        #region Status Checks

        public bool IsBeingExecuted()
        {
            return orderExecutionStatus == OrderExecutionStatus.BeingExecuted;
        }

        public bool IsPaused()
        {
            return orderExecutionStatus == OrderExecutionStatus.Paused;
        }

        public bool IsWaitingForExecution()
        {
            //return orderExecutionStatus == OrderExecutionStatus.Paused || orderExecutionStatus == OrderExecutionStatus.Ordered;
            return  orderExecutionStatus == OrderExecutionStatus.Ordered;
        }

        #endregion
    }

    MovementOrder currentMovementOrder;



    [Header("Debug")]
    [SerializeField]
    bool showGizmo;

    [Header("Rotation")]
    float averageAngularVelocity;
    public float angularAccelerationDistance;
    Vector3 angularVelocity;
    float angularSmoothTime;

    Quaternion targetRotation;
    Quaternion currentRotation;
    Quaternion derivQuaternion;

    public float stationaryTurnSpeed;
    public float runningTurnSpeed;

    Vector3 desiredForward;
    [Tooltip("If false, the agent will rotate towards his movement direction")]
    public bool manualRotation;

    [Header("Movement")]
    public float runningSpeed;
    public float walkingSpeed;
    public float crouchingSpeed;
    public float crawlingSpeed;


    #endregion

    public override void SetUpComponent(GameEntity entity)
    {
        base.SetUpComponent(entity);

        currentMovementOrder = new MovementOrder();

        agent.updateRotation = false;
        desiredForward = transform.forward;
        
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
        //currentMovementOrder.Update();

        // 1. Update movement according to movement Order 
        if (currentMovementOrder.IsWaitingForExecution())
        {
            agent.isStopped = false;

            agent.SetDestination(currentMovementOrder.destination);
            SetMovementType(currentMovementOrder.movementType);
            currentMovementOrder.OnExecute();
        }
        else if (currentMovementOrder.IsBeingExecuted())
        {
            float dist = agent.remainingDistance;
            if ((dist != Mathf.Infinity && agent.pathStatus == NavMeshPathStatus.PathComplete && agent.remainingDistance == 0))
            {
                currentMovementOrder.OnFinishedExecuting();
            }
        }

        // 2. Update rotation according to movement direction or externally set direction if manualRotation=true
        if (!manualRotation)
        {
            if (agent.desiredVelocity != Vector3.zero)
            {
                desiredForward = agent.desiredVelocity;
            }
        }

        RotateTowards(desiredForward);

        //3. Update animation
        if (humanoidAnimationController)humanoidAnimationController.UpdateAnimation(agent.velocity.magnitude, angularVelocity.y);

    }


    #region Movement Orders

    public void MoveTo(Vector3 destination)
    {
        Debug.Log("-------------------------------------------------------Movement order issued---------------------------------------------");
        currentMovementOrder.SetCurrentOrder(destination, MovementType.Run);
    }

    public void MoveTo(Vector3 destination, MovementType movementType)
    {
        Debug.Log("-------------------------------------------------------Movement order issued---------------------------------------------");
        currentMovementOrder.SetCurrentOrder(destination, movementType);
    }

    public void AbortMoving()
    {
        agent.ResetPath();
        currentMovementOrder.OnAbort();
    }

    public void PauseMoving()
    {
        if (currentMovementOrder.IsBeingExecuted())
        {
            agent.isStopped = true;
            currentMovementOrder.OnPause();
        }
    }

    public void ResumeMoving()
    {
        if (currentMovementOrder.IsPaused())
        {
            agent.isStopped = false;
            currentMovementOrder.OnResume();
        }
    
    }

    public void SetMovementType(MovementType movementType)
    {
        currentMovementOrder.movementType = movementType;

        switch (movementType)
        {
            case MovementType.Run:
                agent.speed = runningSpeed;
                break;

            case MovementType.Walk:
                agent.speed = walkingSpeed;
                break;

            case MovementType.Crouch:
                agent.speed = crouchingSpeed;
                break;

            case MovementType.Crawl:
                agent.speed = crawlingSpeed;
                break;
        }
    }

    #endregion




    #region Rotation Orders

    void RotateTowards(Vector3 direction)
    {
        //only rotate on y axis
        currentRotation = transform.rotation;

        averageAngularVelocity = Mathf.Lerp(stationaryTurnSpeed, runningTurnSpeed, (agent.velocity.magnitude / runningSpeed));  //lerp the turn speed - so turning is faster on lower movement velocities

        if (targetRotation != Quaternion.LookRotation(direction))
        {
            targetRotation = Quaternion.LookRotation(direction);

            float distance = Quaternion.Angle(currentRotation, targetRotation);
            //adjust the smooth time to ensure constant speeds at big and at small angles
            angularSmoothTime = Utility.CalculateSmoothTime(distance, averageAngularVelocity, angularAccelerationDistance);
        }

        transform.rotation = Utility.SmoothDamp(currentRotation, targetRotation, ref derivQuaternion, angularSmoothTime);
        angularVelocity = Utility.DerivToAngVelCorrected(currentRotation, derivQuaternion);
    }

    public void SetDesiredForward(Vector3 direction)
    {
        direction.y = 0;
        if (manualRotation) desiredForward = direction;
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
        if (showGizmo && agent != null)
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

