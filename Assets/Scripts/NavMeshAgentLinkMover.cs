using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

 


[RequireComponent(typeof(NavMeshAgent))]
public class NavMeshAgentLinkMover : MonoBehaviour
{
    enum OffMeshLinkMoveMethod
    {
        JumpOverObstacle,
        JumpUpDownOrHorizontal,
        Linear
    }
    OffMeshLinkMoveMethod currentOffMeshLinkMoveMethod;

    public bool isTraversingLink;

    // Infos about Current Link
    NavMeshLinkProperties currentNavMeshLinkProperties;
    Vector3 currentLinkEndPosition;
    Vector3 currentLinkStartPosition;
    float currentDistanceToTraverse;
    float currentLinkTraverseDuration;
    float currentTraversalNormalizedTime; //Normalized between 0 and 1 of the currentTraversalDuration
    

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

    //those are already there in movmeent
    NavMeshAgent agent;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    private void Update()
    {
        if (isTraversingLink)
        {
            currentTraversalNormalizedTime += Time.deltaTime / currentLinkTraverseDuration;

            if (currentTraversalNormalizedTime <1)
            {
                

                if (currentOffMeshLinkMoveMethod == OffMeshLinkMoveMethod.JumpOverObstacle)
                {
                    float yOffset = jumpOverObstacleCurve.Evaluate(currentTraversalNormalizedTime) * currentObstacleHeight;
                    agent.transform.position = Vector3.Lerp(currentLinkStartPosition, currentLinkEndPosition, currentTraversalNormalizedTime) + yOffset * Vector3.up;
                }
                else if (currentOffMeshLinkMoveMethod == OffMeshLinkMoveMethod.JumpUpDownOrHorizontal)
                {                    
                    float yOffset = currentCurveForJumpingUpDownOrHorizontal.Evaluate(currentTraversalNormalizedTime) * currentJumpOverHoleHeight;
                    agent.transform.position = Vector3.Lerp(currentLinkStartPosition, currentLinkEndPosition, currentTraversalNormalizedTime) + yOffset * Vector3.up;
                }
                else if (currentOffMeshLinkMoveMethod == OffMeshLinkMoveMethod.Linear)
                {
                    agent.transform.position = Vector3.Lerp(currentLinkStartPosition, currentLinkEndPosition, currentTraversalNormalizedTime);
                }
            }
            else
            {
                isTraversingLink = false;
                agent.CompleteOffMeshLink();
            }
        }
    }

    public void TraverseOffMeshLink()
    {
        isTraversingLink = true;
        OffMeshLinkData data = agent.currentOffMeshLinkData;
        currentNavMeshLinkProperties = ((UnityEngine.AI.NavMeshLink)agent.navMeshOwner).gameObject.GetComponent<NavMeshLinkProperties>();       // use this instead of "data.offMeshLink.gameObject.GetComponent<NavMeshLinkProperties>();" because of a unity bug where the navmeshlink is returned null by the navmeshLinkData?
        currentLinkStartPosition = agent.transform.position;
        currentLinkEndPosition = data.endPos + Vector3.up * agent.baseOffset;
        currentDistanceToTraverse = Vector3.Distance(currentLinkStartPosition, currentLinkEndPosition);
        
       


        currentTraversalNormalizedTime = 0;

        if (currentNavMeshLinkProperties == null)
        {
            currentLinkTraverseDuration = 1;

            StartTraversingLinearly();
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
            else if(currentNavMeshLinkProperties.navMeshLinkType == NavMeshLinkType.DefaultLinearLink)
            {
                StartTraversingLinearly();
            }
        }
        

        
    }

    void StartTraversingLinearly()
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

    void GetPointOnSphericalCurve(float amount)
    {
        Vector3.Slerp(currentLinkStartPosition, currentLinkEndPosition, amount);
    }



}




