using System.Collections;
using System.Collections.Generic;
using System.Security.Policy;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Experimental.AI;

public class EC_HumanoidMovementController : EntityComponent, IMoveable
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

    //[Header("State Management")]
    protected enum MovementState
    {
        Default,
        TraversingOffMeshLink,
        BeingPushed
    }
    protected MovementState movementState;

  
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

    public Vector3 desiredForward; //only public for debug
    [Tooltip("If false, the agent will rotate towards his movement direction")]
    bool manualRotation;


    [Header("Movement Speeds")]
    public float sprintingSpeed;
    public float defaultSpeed;
    float currentDesiredSpeed; //set the agent.speed as desiredSpeed * speedModifiers
    float currentSpeed;

    public CharacterModifierCreator jumpDownBigLedgeLandingSpeedModifier;  //Modifying speed after landing, makes sure that the character doesnt sprint while playing hte landing animation
    public CharacterModifierCreator steepSlopeMovementSpeedModifier;


    [Header("Traversing Off Mesh Links")]
    #region Traversing NavmeshLink Fields
    public float maximalAngleToNavMeshLinkDirectionToEnterTraversal;
    public CharacterModifierCreator traversingOffMeshLinkPreventionModifier;

    // For jumping over obstacle
    float currentObstacleHeight;
    float currentRelativeObstacleOffset; //the offset betwen currentObstacleHeight and the animatedObstacleHeight - 0.8m;
    [Space(5)]
    public AnimationCurve jumpOverObstacleYOffsetCurve = new AnimationCurve();
    public AnimationCurve jumpOverObstacleTraversalSpeedCurve = new AnimationCurve();

    // For calculating the jump over hole or up and down curve
    [Space(5)]
    public AnimationCurve horizontalJumpYOffsetCurve = new AnimationCurve();
    public AnimationCurve horizontalJumpTraversalSpeedCurve = new AnimationCurve();
    [Space(5)]
    public AnimationCurve jumpingUpSmallLedgeYOffsetCurve = new AnimationCurve();
    public AnimationCurve jumpingUpSmallLedgeTraversalSpeedCurve = new AnimationCurve();
    [Space(5)]
    public AnimationCurve jumpingDownSmallLedgeYOffsetCurve = new AnimationCurve();
    public AnimationCurve jumpingDownSmallLedgeTraversalSpeedCurve = new AnimationCurve();
    [Space(5)]
    public AnimationCurve jumpingDownBigLedgeYOffsetCurve = new AnimationCurve();
    public AnimationCurve jumpingDownBigLedgeTraversalSpeedCurve = new AnimationCurve();

    AnimationCurve currentYOffsetCurveForJumpingUpDownOrHorizontal;
    AnimationCurve currentTraversalSpeedCurveForJumpingUpDownOrHorizontal;

    float distanceHeightRatio = 0.2f; //how much the length is the height of the jump going to be?
    float currentJumpOverHoleHeight;

 

    // Infos about Current Link
    NavMeshLinkProperties currentNavMeshLinkProperties;
    Vector3 currentLinkEndPosition;
    Vector3 currentLinkStartPosition; 
    float currentDistanceToTraverse;
    float currentLinkTraverseDuration;
    float currentTraversalNormalizedTime; //Normalized between 0 and 1 of the currentTraversalDuration
    Vector3 offMeshLinkTraversalDirection;

    // Saved For Ragdolls or others
    Vector3 offMeshLinkTraversalVelocity;




    enum OffMeshLinkMoveMethod
    {
        JumpOverObstacle,
        JumpUpDownOrHorizontal,
        Linear
    }
    OffMeshLinkMoveMethod currentOffMeshLinkMoveMethod;

    //Subtype
    public enum TraversingLinkJumpUpDownOrHorizontalType
    {
        JumpingHorizontal,
        JumpingUpSmallLedge,
        JumpingDownSmallLedge,
        JumpingDownBigLedge
    }
    TraversingLinkJumpUpDownOrHorizontalType traversingLinkJumpUpDownOrHorizontalType;

    #endregion


    [Header("Traversing Sloped Surfaces")]
    
    NavMeshHit navMeshHit = new NavMeshHit();
    public string steepSlopeNavmeshAreaName;
    int steepSlopeNavmeshAreaID;


    [Header("Debug")]
    [SerializeField]
    bool showGizmo;


    #region Movement Order

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
            return orderExecutionStatus == OrderExecutionStatus.Paused || orderExecutionStatus == OrderExecutionStatus.Ordered;
            //return orderExecutionStatus == OrderExecutionStatus.Ordered;
        }

        #endregion
    }

    MovementOrder currentMovementOrder;

    #endregion


    #endregion


    public override void SetUpComponent(GameEntity entity)
    {
        base.SetUpComponent(entity);

        currentMovementOrder = new MovementOrder();

        agent.updateRotation = false;
        desiredForward = transform.forward;
        movementState = MovementState.Default;

        steepSlopeNavmeshAreaID = 1 << NavMesh.GetAreaFromName(steepSlopeNavmeshAreaName);

    }

    public override void UpdateComponent()
    {
        /*if (Input.GetKeyDown(KeyCode.N))
        {
            agent.CompleteOffMeshLink();
        }*/
        /*if (Input.GetKeyDown(KeyCode.M))
        {
            agent.ResetPath();
            currentMovementOrder.OnAbort();
        }*/

        bool rotatingTowardsOffMeshLinkOverridesOtherRotation = false;

        if (movementState == MovementState.Default)
        {

                //1. Navmesh Link Check
                if (agent.isOnOffMeshLink)
                {
                    OffMeshLinkData data = agent.currentOffMeshLinkData;
                    currentLinkStartPosition = data.startPos + Vector3.up * agent.baseOffset; //we use the link start position, the transform.pisitoon can cause errors
                    currentLinkEndPosition = data.endPos + Vector3.up * agent.baseOffset;
                    offMeshLinkTraversalDirection = currentLinkEndPosition - currentLinkStartPosition;
                    Vector3 offMeshLinkTraversalDirectionNoY = new Vector3(offMeshLinkTraversalDirection.x, 0, offMeshLinkTraversalDirection.z);

                    //the angle check prevents units from using offmeshLinks, although they are not facing them -> unnatural jump
                    if (Vector3.Angle(offMeshLinkTraversalDirectionNoY, transform.forward) < maximalAngleToNavMeshLinkDirectionToEnterTraversal)
                    {
                        //only if we decide to traverse, based on angle, the start posiiton is set to transform.pos to smooth things out
                        currentLinkStartPosition = transform.position;
                        StartTraversingOffMeshLink(data);
                        return;
                    }
                    else
                    {
                        rotatingTowardsOffMeshLinkOverridesOtherRotation = true;
                        RotateTowards(offMeshLinkTraversalDirectionNoY);
                    }

                }
 


            //2. Check if agent is on sloped surface
            agent.SamplePathPosition(NavMesh.AllAreas, 0.0f, out navMeshHit);

            if(navMeshHit.mask == steepSlopeNavmeshAreaID)
            {
                characterController.AddModifier(steepSlopeMovementSpeedModifier.CreateAndActivateNewModifier());
            }
            else
            {
               characterController.RemoveModifier(steepSlopeMovementSpeedModifier.CreateAndActivateNewModifier());
            }
          
            // 3. Update movement according to movement Order 
            if (currentMovementOrder.IsWaitingForExecution())
            {
                agent.isStopped = false;

                agent.SetDestination(currentMovementOrder.destination);
                
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

            if (!rotatingTowardsOffMeshLinkOverridesOtherRotation)
            {
                // 4. Update rotation according to movement direction or externally set direction if manualRotation=true
                if (!manualRotation)
                {
                    if (agent.desiredVelocity != Vector3.zero)
                    {
                        desiredForward = agent.desiredVelocity;
                    }
                }


                RotateTowards(desiredForward);
            }
            

            //5. Update animation
            //calculate forward and sideways velocity;
            Vector3 velocityInLocalSpace = Vector3.zero;
            float speedParamForAnim = 0;

            if (agent.hasPath)
            {
                speedParamForAnim = agent.velocity.magnitude;
                velocityInLocalSpace = transform.InverseTransformVector(agent.velocity);
            }

            if (animationController) animationController.UpdateLocomotionAnimation(speedParamForAnim, velocityInLocalSpace.z, velocityInLocalSpace.x, angularVelocity.y);


            //7. update agent speed
            UpdateAgentSpeed(currentMovementOrder.sprint);
           
            //return; 
            //without the return we could get from this if statement to the next when we set the movement State to ttraversing offmesh link, because Navmesh.CompleteOffmeshLink is asynchronous, 
            //this could teleport the agent back to the offmesh Link start point, bause traverng offmesh link would be started again - do we really need this?
        }
        else if(movementState == MovementState.TraversingOffMeshLink)
        {
            currentTraversalNormalizedTime += Time.deltaTime / currentLinkTraverseDuration;

            if (currentTraversalNormalizedTime < 1)
            {
                Vector3 newPosition = Vector3.zero;


                if (currentOffMeshLinkMoveMethod == OffMeshLinkMoveMethod.JumpOverObstacle)
                {
                    float modifiedCurrentTraversalNormalizedTime = jumpOverObstacleTraversalSpeedCurve.Evaluate(currentTraversalNormalizedTime);

                    float yOffset = jumpOverObstacleYOffsetCurve.Evaluate(modifiedCurrentTraversalNormalizedTime) * currentRelativeObstacleOffset;
                    newPosition = Vector3.Lerp(currentLinkStartPosition, currentLinkEndPosition, modifiedCurrentTraversalNormalizedTime) + yOffset * Vector3.up;
                }
                else if (currentOffMeshLinkMoveMethod == OffMeshLinkMoveMethod.JumpUpDownOrHorizontal)
                {
                    float modifiedCurrentTraversalNormalizedTime = currentTraversalSpeedCurveForJumpingUpDownOrHorizontal.Evaluate(currentTraversalNormalizedTime);

                    float yOffset = currentYOffsetCurveForJumpingUpDownOrHorizontal.Evaluate(modifiedCurrentTraversalNormalizedTime) * currentJumpOverHoleHeight;
                    newPosition = Vector3.Lerp(currentLinkStartPosition, currentLinkEndPosition, modifiedCurrentTraversalNormalizedTime) + yOffset * Vector3.up;
                }
                else if (currentOffMeshLinkMoveMethod == OffMeshLinkMoveMethod.Linear)
                {
                    newPosition = Vector3.Lerp(currentLinkStartPosition, currentLinkEndPosition, currentTraversalNormalizedTime);
                }

                offMeshLinkTraversalVelocity = (newPosition - transform.position)/Time.deltaTime;
                RotateTowards(offMeshLinkTraversalDirection);
                transform.position = newPosition;
            }
            else
            {
                
                FinishTraversingOffMeshLink();
            }

           // return;
        }
    }

    #region Movement Orders

    public void MoveTo(Vector3 destination)
    {
        if(movementState == MovementState.Default)
        {
            Debug.Log("move 3");
            currentMovementOrder.SetCurrentOrder(destination, false);
        }
       
    }

    public void MoveTo(Vector3 destination, bool sprint)
    {
        if (movementState == MovementState.Default)
        {
            currentMovementOrder.SetCurrentOrder(destination, sprint);
        }
    }

    public void SetSprint(bool sprint)
    {
        if (movementState == MovementState.Default)
        {
            currentMovementOrder.sprint = sprint;
        }
    }

    public void AbortMoving()
    {
        if (movementState == MovementState.Default)
        {
            agent.ResetPath();
            currentMovementOrder.OnAbort();
        }
    }

    public void PauseMoving()
    {
        if (movementState == MovementState.Default)
        {
            if (currentMovementOrder.IsBeingExecuted())
            {
                agent.isStopped = true;
                currentMovementOrder.OnPause();
            }
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
        direction.y = 0;
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

    public void SetManualRotation(bool manualRotation)
    {
        this.manualRotation = manualRotation;
    }

    #endregion

    #region Modify Speed Values

    void UpdateAgentSpeed(bool sprint)
    {
        if (currentMovementOrder.sprint)
        {
            currentDesiredSpeed = sprintingSpeed;

            //Update Movement speed with modifiers
            currentSpeed = currentDesiredSpeed;
            foreach (ActiveCharacterMovementSpeedModifier modifier in characterController.GetActiveMovementSpeedModifiers())
            {
                 currentSpeed *= modifier.sprintingSpeedMod; 
            }
        }
        else
        {
            currentDesiredSpeed = defaultSpeed;

            //Update Movement speed with modifiers
            currentSpeed = currentDesiredSpeed;
            foreach (ActiveCharacterMovementSpeedModifier modifier in characterController.GetActiveMovementSpeedModifiers())
            {
                currentSpeed *= modifier.walkingSpeedMod;
            }
        }

        
        agent.speed = currentSpeed;
    }

    //-----------Setting Speed Values according to stances---------------

    public void SetDefaultSpeed(float newDefaultSpeed) //make public later
    {
        defaultSpeed = newDefaultSpeed;

        /*if (!currentMovementOrder.sprint)
        {
            agent.speed = defaultSpeed;
        }*/
        UpdateAgentSpeed(currentMovementOrder.sprint);
    }

    public void SetSprintSpeed(float newSprintSpeed) //make public later
    {
        sprintingSpeed = newSprintSpeed;

        /*if (currentMovementOrder.sprint)
        {
            agent.speed = sprintingSpeed;
        }*/
        UpdateAgentSpeed(currentMovementOrder.sprint);

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

    void StartTraversingOffMeshLink(OffMeshLinkData data)
    {
        movementState = MovementState.TraversingOffMeshLink;
        currentNavMeshLinkProperties = ((UnityEngine.AI.NavMeshLink)agent.navMeshOwner).gameObject.GetComponent<NavMeshLinkProperties>();       // use this instead of "data.offMeshLink.gameObject.GetComponent<NavMeshLinkProperties>();" because of a unity bug where the navmeshlink is returned null by the navmeshLinkData?
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
                StartTraversingLinkJumpDownUpOrHorizontal();
            }
            else if (currentNavMeshLinkProperties.navMeshLinkType == NavMeshLinkType.DefaultLinearLink)
            {
                StartTraversingLinkLinearly();
            }
        }

        // Inform Character Controler
        characterController.AddModifier(traversingOffMeshLinkPreventionModifier.CreateAndActivateNewModifier());

        // Inform Animation Controller
        animationController.StartJumpingOverObstacle(GetAnimationIDCorrespondingToTraversingType(), currentLinkTraverseDuration);

        //agent.CompleteOffMeshLink(); //I thought this should be called on FinishTraversingNavmesh, but if we call it there it can cause issues, where the agent snaps back to ist previous posiiton
        //-> snaps agent to the end position, when jumping beginns -> it seems its better to enable and disable the navmesh agent

        agent.enabled = false;

    }

    void StartTraversingLinkLinearly()
    {
        currentOffMeshLinkMoveMethod = OffMeshLinkMoveMethod.Linear;
    }

    void StartTraversingLinkJumpOverObstacle()
    {
        currentOffMeshLinkMoveMethod = OffMeshLinkMoveMethod.JumpOverObstacle;

        currentObstacleHeight = currentNavMeshLinkProperties.obstacleHeight;
        currentRelativeObstacleOffset = currentObstacleHeight - 0.8f;//0.65f;
    }

    void StartTraversingLinkJumpDownUpOrHorizontal()
    {
        currentOffMeshLinkMoveMethod = OffMeshLinkMoveMethod.JumpUpDownOrHorizontal;

        currentJumpOverHoleHeight = currentDistanceToTraverse * distanceHeightRatio;

        float heightDifference = currentLinkEndPosition.y - currentLinkStartPosition.y;

        //if the distance between the point in y is big, we adjust the jumpOverHoleHeight based on the highest:
        if (heightDifference > 0.5f)
        {
            //if going up
            currentYOffsetCurveForJumpingUpDownOrHorizontal = jumpingUpSmallLedgeYOffsetCurve;
            currentTraversalSpeedCurveForJumpingUpDownOrHorizontal = jumpingUpSmallLedgeTraversalSpeedCurve;

            traversingLinkJumpUpDownOrHorizontalType = TraversingLinkJumpUpDownOrHorizontalType.JumpingUpSmallLedge;

        }
        else if (heightDifference < -0.5f)
        {
            

            if (heightDifference < -1.3f)
            {
                traversingLinkJumpUpDownOrHorizontalType = TraversingLinkJumpUpDownOrHorizontalType.JumpingDownBigLedge;
                currentYOffsetCurveForJumpingUpDownOrHorizontal = jumpingDownBigLedgeYOffsetCurve;
                currentTraversalSpeedCurveForJumpingUpDownOrHorizontal = jumpingDownBigLedgeTraversalSpeedCurve;

            }
            else
            {
                traversingLinkJumpUpDownOrHorizontalType = TraversingLinkJumpUpDownOrHorizontalType.JumpingDownSmallLedge;
                currentYOffsetCurveForJumpingUpDownOrHorizontal = jumpingDownSmallLedgeYOffsetCurve;
                currentTraversalSpeedCurveForJumpingUpDownOrHorizontal = jumpingDownSmallLedgeTraversalSpeedCurve;

            }
        }
        else
        {
            currentYOffsetCurveForJumpingUpDownOrHorizontal = horizontalJumpYOffsetCurve;
            traversingLinkJumpUpDownOrHorizontalType = TraversingLinkJumpUpDownOrHorizontalType.JumpingHorizontal;
            currentTraversalSpeedCurveForJumpingUpDownOrHorizontal = jumpingUpSmallLedgeTraversalSpeedCurve;
        }
    }

    void FinishTraversingOffMeshLink()
    {
        //Debug.Log("finish traversing movement");
        currentTraversalNormalizedTime = 1;
        offMeshLinkTraversalDirection = Vector3.zero;

        //Add the Speed Modifier and set the timer to disable it 
        if (currentNavMeshLinkProperties.navMeshLinkType == NavMeshLinkType.JumpDownUpOrHorizontal)
        {
            if(traversingLinkJumpUpDownOrHorizontalType == TraversingLinkJumpUpDownOrHorizontalType.JumpingDownBigLedge)
            {
                characterController.AddModifier(jumpDownBigLedgeLandingSpeedModifier.CreateAndActivateNewModifier());
                agent.velocity = Vector3.zero;
            }
        }

        movementState = MovementState.Default;

        StartCoroutine(DisableTraversingLinkPreventionModifierAfterDelay(0.2f));

        // Inform Animation Controller
        animationController.StopJumpingOverObstacle();

        agent.enabled = true;

        agent.velocity = offMeshLinkTraversalVelocity;

        //make sure thereenabled agent has a movement order
        agent.SetDestination(currentMovementOrder.destination);

        currentMovementOrder.OnExecute();
    }

    IEnumerator DisableTraversingLinkPreventionModifierAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        characterController.RemoveModifier(traversingOffMeshLinkPreventionModifier.CreateAndActivateNewModifier()); //this could be dalyed by 0.2f seconds using coroutine
    }

    int GetAnimationIDCorrespondingToTraversingType( )
    {
        if(currentOffMeshLinkMoveMethod == OffMeshLinkMoveMethod.JumpOverObstacle)
        {
            return 0;
        }
        else if (traversingLinkJumpUpDownOrHorizontalType == TraversingLinkJumpUpDownOrHorizontalType.JumpingHorizontal)
        {
            return 1;
        }
        else if(traversingLinkJumpUpDownOrHorizontalType == TraversingLinkJumpUpDownOrHorizontalType.JumpingUpSmallLedge)
        {
            return 2;
        }
        else if (traversingLinkJumpUpDownOrHorizontalType == TraversingLinkJumpUpDownOrHorizontalType.JumpingDownSmallLedge)
        {
            return 3;
        }
        else if (traversingLinkJumpUpDownOrHorizontalType == TraversingLinkJumpUpDownOrHorizontalType.JumpingDownBigLedge)
        {
            return 4;
        }

        return 1;
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

    public float GetRemainingDistance()
    {
        if (agent.enabled)
        {
            return agent.remainingDistance;

        }

        return Mathf.Infinity;
    }

    public bool IsSprinting()
    {
        return currentMovementOrder.sprint;
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

        if (offMeshLinkTraversalDirection != Vector3.zero)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(transform.position + Vector3.up, transform.position + offMeshLinkTraversalDirection + Vector3.up);
        }

        if(currentLinkStartPosition != Vector3.zero)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawCube(currentLinkStartPosition, new Vector3(0.2f, 0.2f, 0.2f));
        }

        if (currentLinkEndPosition != Vector3.zero)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawCube(currentLinkEndPosition, new Vector3(0.2f, 0.2f, 0.2f));
        }
    }

    #endregion

}

