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
    protected EC_HumanoidAnimationController animationController;
    [Tooltip("Only used to inform the character controller if its currently traversing an offmeshLink")]
    [SerializeField]
    protected EC_HumanoidCharacterController characterController;


    protected enum MovementState
    {
        Default,
        TraversingOffMeshLink,
        BeingPushed
    }
    protected MovementState movementState;

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

    [Header("Traversing Off Mesh Links")]
    // For calculating the jump over hole or up and down curve
    public AnimationCurve horizontalJumpCurve = new AnimationCurve();
    public AnimationCurve jumpingDownCuve = new AnimationCurve();
    public AnimationCurve jumpingUpCurve = new AnimationCurve();
    AnimationCurve currentCurveForJumpingUpDownOrHorizontal;
    float distanceHeightRatio = 0.2f; //how much the length is the height of the jump going to be?
    float currentJumpOverHoleHeight;

    // For jumping over obstacle
    float currentObstacleHeight;
    public AnimationCurve jumpOverObstacleCurve = new AnimationCurve();

    // Infos about Current Link
    NavMeshLinkProperties currentNavMeshLinkProperties;
    Vector3 currentLinkEndPosition;
    Vector3 currentLinkStartPosition;
    float currentDistanceToTraverse;
    float currentLinkTraverseDuration;
    float currentTraversalNormalizedTime; //Normalized between 0 and 1 of the currentTraversalDuration

    enum OffMeshLinkMoveMethod
    {
        JumpOverObstacle,
        JumpUpDownOrHorizontal,
        Linear
    }
    OffMeshLinkMoveMethod currentOffMeshLinkMoveMethod;

    // Saved For Ragdolls or others
    Vector3 offMeshLinkTraversalVelocity;



    #endregion

    public override void SetUpComponent(GameEntity entity)
    {
        base.SetUpComponent(entity);

        currentMovementOrder = new MovementOrder();

        agent.updateRotation = false;
        desiredForward = transform.forward;
        movementState = MovementState.Default;

    }

    public override void UpdateComponent()
    {
        if(movementState == MovementState.Default)
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

            if (animationController) animationController.UpdateLocomotionAnimation(agent.velocity.magnitude, velocityInLocalSpace.z, velocityInLocalSpace.x, angularVelocity.y);

            // 4. Navmesh Link Check

            if (agent.isOnOffMeshLink)
            {
                StartTraversingOffMeshLink();
            }
        }
        else if(movementState == MovementState.TraversingOffMeshLink)
        {
            currentTraversalNormalizedTime += Time.deltaTime / currentLinkTraverseDuration;

            if (currentTraversalNormalizedTime < 1)
            {
                Vector3 newPosition = Vector3.zero;

                if (currentOffMeshLinkMoveMethod == OffMeshLinkMoveMethod.JumpOverObstacle)
                {
                    float yOffset = jumpOverObstacleCurve.Evaluate(currentTraversalNormalizedTime) * currentObstacleHeight;
                    newPosition = Vector3.Lerp(currentLinkStartPosition, currentLinkEndPosition, currentTraversalNormalizedTime) + yOffset * Vector3.up;
                }
                else if (currentOffMeshLinkMoveMethod == OffMeshLinkMoveMethod.JumpUpDownOrHorizontal)
                {
                    float yOffset = currentCurveForJumpingUpDownOrHorizontal.Evaluate(currentTraversalNormalizedTime) * currentJumpOverHoleHeight;
                    newPosition = Vector3.Lerp(currentLinkStartPosition, currentLinkEndPosition, currentTraversalNormalizedTime) + yOffset * Vector3.up;
                }
                else if (currentOffMeshLinkMoveMethod == OffMeshLinkMoveMethod.Linear)
                {
                    newPosition = Vector3.Lerp(currentLinkStartPosition, currentLinkEndPosition, currentTraversalNormalizedTime);
                }

                offMeshLinkTraversalVelocity = (newPosition - agent.transform.position)/Time.deltaTime;
                RotateTowards(new Vector3(offMeshLinkTraversalVelocity.x,0,offMeshLinkTraversalVelocity.z));
                agent.transform.position = newPosition;
            }
            else
            {
                FinishTraversingOffMeshLink();
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

    #region Traversing Off Mesh Link

    void StartTraversingOffMeshLink()
    {
        movementState = MovementState.TraversingOffMeshLink;

        OffMeshLinkData data = agent.currentOffMeshLinkData;
        currentNavMeshLinkProperties = ((UnityEngine.AI.NavMeshLink)agent.navMeshOwner).gameObject.GetComponent<NavMeshLinkProperties>();       // use this instead of "data.offMeshLink.gameObject.GetComponent<NavMeshLinkProperties>();" because of a unity bug where the navmeshlink is returned null by the navmeshLinkData?
        currentLinkStartPosition = agent.transform.position;
        currentLinkEndPosition = data.endPos + Vector3.up * agent.baseOffset;
        currentDistanceToTraverse = Vector3.Distance(currentLinkStartPosition, currentLinkEndPosition);




        currentTraversalNormalizedTime = 0;

        if (currentNavMeshLinkProperties == null)
        {
            currentLinkTraverseDuration = 1;

            StartTraversingLinkLinearly();
        }
        else
        {
            currentLinkTraverseDuration = currentNavMeshLinkProperties.traverseDuration;


            if (currentNavMeshLinkProperties.navMeshLinkType == NavMeshLinkType.JumpOverObstacle)
            {
                StartTraversingLinkJumpOverObstacle();
            }
            else if (currentNavMeshLinkProperties.navMeshLinkType == NavMeshLinkType.JumpDownUpOrHorizontal)
            {
                StartTraversingLinkJumpOverHole();
            }
            else if (currentNavMeshLinkProperties.navMeshLinkType == NavMeshLinkType.DefaultLinearLink)
            {
                StartTraversingLinkLinearly();
            }
        }

        // Inform Character Controler
        characterController.OnStartTraversingOffMeshLink();

        // Inform Animation Controller
        animationController.StartJumpingOverObstacle();

    }

    void FinishTraversingOffMeshLink()
    {
        agent.CompleteOffMeshLink();
        movementState = MovementState.Default;

        offMeshLinkTraversalVelocity = Vector3.zero;

        // Inform Character Controler
        characterController.OnStopTraversingOffMeshLink();

        // Inform Animation Controller
    }

    void StartTraversingLinkLinearly()
    {
        currentOffMeshLinkMoveMethod = OffMeshLinkMoveMethod.Linear;
    }

    void StartTraversingLinkJumpOverObstacle()
    {
        currentOffMeshLinkMoveMethod = OffMeshLinkMoveMethod.JumpOverObstacle;

        currentObstacleHeight = currentNavMeshLinkProperties.obstacleHeight;
    }

    void StartTraversingLinkJumpOverHole()
    {
        currentOffMeshLinkMoveMethod = OffMeshLinkMoveMethod.JumpUpDownOrHorizontal;

        currentJumpOverHoleHeight = currentDistanceToTraverse * distanceHeightRatio;

        float heightDifference = currentLinkEndPosition.y - currentLinkStartPosition.y;

        //if the distance between the point in y is big, we adjust the jumpOverHoleHeight based on the highest:
        if (heightDifference > 0.5f)
        {
            //if going up
            currentCurveForJumpingUpDownOrHorizontal = jumpingUpCurve;

        }
        else if (heightDifference < -0.5f)
        {
            heightDifference = -heightDifference;
            currentCurveForJumpingUpDownOrHorizontal = jumpingDownCuve;
        }
        else
        {
            currentCurveForJumpingUpDownOrHorizontal = horizontalJumpCurve;
        }
    }


    #endregion

    #region Status Checks
    public virtual bool IsMoving()
    {
        return GetCurrentVelocityMagnitude() > agent.speed / 2;
    }

    public float GetCurrentVelocityMagnitude()
    {
        return GetCurrentVelocity().magnitude;
    }

    public virtual Vector3 GetCurrentVelocity()
    {
        if(movementState == MovementState.Default)
        {
            return agent.velocity;
            

        }
        else if(movementState == MovementState.TraversingOffMeshLink)
        {
            return offMeshLinkTraversalVelocity;
        }

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

