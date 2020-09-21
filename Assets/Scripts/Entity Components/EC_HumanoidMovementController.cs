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
    protected EC_HumanoidAnimationController humanoidAnimationController;
    [SerializeField]
    protected NavMeshAgentLinkMover navMeshAgentLinkMover;


    protected enum PushingState
    {
        Default,
        BeingPushed,
    }
    protected PushingState movementState;

    // Represents an information container about the current order, does not do any logic by itself
    class MovementOrder
    {
        public Vector3 destination;
        public bool sprint;

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

        public void SetCurrentOrder(Vector3 destination, bool sprint)
        {
            orderExecutionStatus = OrderExecutionStatus.Ordered;
            this.destination = destination;
            this.sprint = sprint;
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
    public float sprintingSpeed;
    public float defaultSpeed;


    #endregion

    public override void SetUpComponent(GameEntity entity)
    {
        base.SetUpComponent(entity);

        currentMovementOrder = new MovementOrder();

        agent.updateRotation = false;
        desiredForward = transform.forward;

    }

    public override void UpdateComponent()
    {
        // 1. Update movement according to movement Order 
        if (currentMovementOrder.IsWaitingForExecution())
        {
            agent.isStopped = false;

            agent.SetDestination(currentMovementOrder.destination);
            UpdateAgentSpeed(currentMovementOrder.sprint);
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

        //calculate forward and sideways velocity;
        Vector3 velocityInLocalSpace = transform.InverseTransformVector(agent.velocity);

        if (humanoidAnimationController) humanoidAnimationController.UpdateLocomotionAnimation(agent.velocity.magnitude, velocityInLocalSpace.z, velocityInLocalSpace.x, angularVelocity.y);

        //Navmesh Link Check
        if (!navMeshAgentLinkMover.isTraversingLink)
        {
            if (agent.isOnOffMeshLink)
            {
                Debug.Log("agent is on offmesh link");
                navMeshAgentLinkMover.TraverseOffMeshLink();
            }
        }

    }

    #region Movement Orders

    public void MoveTo(Vector3 destination)
    {
        currentMovementOrder.SetCurrentOrder(destination, false);
    }

    public void MoveTo(Vector3 destination, bool sprint)
    {
        currentMovementOrder.SetCurrentOrder(destination, sprint);
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

    #endregion

    #region Rotation Orders
    void RotateTowards(Vector3 direction)
    {
        //only rotate on y axis
        currentRotation = transform.rotation;

        averageAngularVelocity = Mathf.Lerp(stationaryTurnSpeed, runningTurnSpeed, (agent.velocity.magnitude / sprintingSpeed));  //lerp the turn speed - so turning is faster on lower movement velocities

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

    #region Modify Values

    public void UpdateAgentSpeed(bool sprint)
    {
        if (currentMovementOrder.sprint)
        {
            agent.speed = sprintingSpeed;
        }
        else
        {
            agent.speed = defaultSpeed;
        }
    }

    public void SetDefaultSpeed(float newDefaultSpeed)
    {
        defaultSpeed = newDefaultSpeed;

        if (!currentMovementOrder.sprint)
        {
            agent.speed = defaultSpeed;
        }
    }

    public void SetSprintSpeed(float newSprintSpeed)
    {
        sprintingSpeed = newSprintSpeed;

        if (currentMovementOrder.sprint)
        {
            agent.speed = sprintingSpeed;
        }
    }

    public void SetAcceleration(float newAcceleration)
    {
        agent.acceleration = newAcceleration;
    }

    public void SetStationaryTurnSpeed(float newStationaryTurnSpeed)
    {
        stationaryTurnSpeed = newStationaryTurnSpeed;
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

    public Vector3 GetCurrentAngularVelocity()
    {
        return angularVelocity;
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

