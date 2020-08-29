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


    [Header("Look At")]
    bool lookAt = false;
    public Transform spine;


    [Header("Debug")]
    [SerializeField]
    bool showGizmo;

    // Rotation
    public float currentMaxAngularVelocity;
    public float accelerationDistance;
    float angularVelocity;
    float smoothTime;

    Quaternion targetRotation;
    Quaternion currentRotation;
    Quaternion newRotation;
    Vector4 rotationV;

    float stationaryTurnSpeed;
    float runningTurnSpeed;

    Vector3 desiredForward;

    float runningSpeed;
    float walkingSpeed;
    float crouchingSpeed;


    #endregion

    public override void SetUpComponent(GameEntity entity)
    {
        //maxAngularSpeed = agent.angularSpeed;   //almost the same speed as original navmeshAgent?
        //currentMovementOrder = new MovementOrder(agent);
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




        if (agent.desiredVelocity != Vector3.zero)
        {
            desiredForward = agent.desiredVelocity;
        }

        currentRotation = transform.rotation;

        if (targetRotation != Quaternion.LookRotation(desiredForward))
        {
            targetRotation = Quaternion.LookRotation(desiredForward);

            float distance = Quaternion.Angle(currentRotation, targetRotation);
            //adjust the smooth time to ensure constant speeds at big and at small angles
            smoothTime = Utility.CalculateSmoothTime(distance, currentMaxAngularVelocity, accelerationDistance);
        }

        newRotation = Utility.SmoothDamp(currentRotation, targetRotation, ref rotationV, smoothTime);

        angularVelocity = Utility.CalculateSignedAngularSpeedAroundGlobalY(ref currentRotation, ref newRotation);
        //Debug.Log("relative v: " + angularVelocity);

        transform.rotation = newRotation;

        humanoidAnimationController.UpdateAnimation(agent.velocity.magnitude, angularVelocity);

    }


    #region Movement Orders

    public void RotateTo(Vector3 direction)
    {
        //only rotate on local x direction
        //first delete side movements- only leave y and z
        Vector3 directionForSpine = transform.InverseTransformDirection(direction);

        Quaternion desiredSpineRotation = Quaternion.LookRotation(directionForSpine);
        desiredSpineRotation = Quaternion.Euler(desiredSpineRotation.eulerAngles.x, 0, 0);
        //spine.localRotation = Quaternion.RotateTowards(spine.localRotation, desiredSpineRotation, maxAngularSpeed * Time.deltaTime);



        direction.y = 0;
        Quaternion desiredLookRotation = Quaternion.LookRotation(direction);
        //because we want the same speed as the agent, which has its angular speed saved as degrees per second we use the rotaate towards function
        //transform.rotation = Quaternion.RotateTowards(transform.rotation, desiredLookRotation, maxAngularSpeed * Time.deltaTime );
    }

    public void MoveTo(Vector3 destination)
    {
        Debug.Log("-------------------------------------------------------Movement order issued---------------------------------------------");
        currentMovementOrder.SetCurrentOrder(destination, MovementType.Run);
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

