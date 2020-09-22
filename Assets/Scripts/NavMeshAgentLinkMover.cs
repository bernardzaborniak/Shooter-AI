using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

 
public enum OffMeshLinkMoveMethod
{
    //Teleport,
    
    //Parabola,
    //Curve
    JumpOverObstacle,
    JumpUpDownOrHorizontal,
    Linear
}

//From Jacob from https://forum.unity.com/threads/how-to-trigger-a-jump-on-an-offmesh-link.313628/
[RequireComponent(typeof(NavMeshAgent))]
public class NavMeshAgentLinkMover : MonoBehaviour
{
    /*public OffMeshLinkMoveMethod method = OffMeshLinkMoveMethod.Parabola;
    [Tooltip("Only used if method is set to curve")]
    public AnimationCurve curve = new AnimationCurve();
    IEnumerator Start()
    {
        NavMeshAgent agent = GetComponent<NavMeshAgent>();
        agent.autoTraverseOffMeshLink = false;
        while (true)
        {
            if (agent.isOnOffMeshLink)
            {
                if (method == OffMeshLinkMoveMethod.NormalSpeed)
                    yield return StartCoroutine(NormalSpeed(agent));
                else if (method == OffMeshLinkMoveMethod.Parabola)
                    yield return StartCoroutine(Parabola(agent, 2.0f, 0.5f));
                else if (method == OffMeshLinkMoveMethod.Curve)
                    yield return StartCoroutine(Curve(agent, 0.5f));
                agent.CompleteOffMeshLink();
            }
            yield return null;
        }
    }
    IEnumerator NormalSpeed(NavMeshAgent agent)
    {
        OffMeshLinkData data = agent.currentOffMeshLinkData;
        Vector3 endPos = data.endPos + Vector3.up * agent.baseOffset;
        while (agent.transform.position != endPos)
        {
            agent.transform.position = Vector3.MoveTowards(agent.transform.position, endPos, agent.speed * Time.deltaTime);
            yield return null;
        }
    }
    IEnumerator Parabola(NavMeshAgent agent, float height, float duration)
    {
        OffMeshLinkData data = agent.currentOffMeshLinkData;
        Vector3 startPos = agent.transform.position;
        Vector3 endPos = data.endPos + Vector3.up * agent.baseOffset;
        float normalizedTime = 0.0f;
        while (normalizedTime < 1.0f)
        {
            float yOffset = height * 4.0f * (normalizedTime - normalizedTime * normalizedTime);
            agent.transform.position = Vector3.Lerp(startPos, endPos, normalizedTime) + yOffset * Vector3.up;
            normalizedTime += Time.deltaTime / duration;
            yield return null;
        }
    }
    IEnumerator Curve(NavMeshAgent agent, float duration)
    {
        OffMeshLinkData data = agent.currentOffMeshLinkData;
        Vector3 startPos = agent.transform.position;
        Vector3 endPos = data.endPos + Vector3.up * agent.baseOffset;
        float normalizedTime = 0.0f;
        while (normalizedTime < 1.0f)
        {
            float yOffset = curve.Evaluate(normalizedTime);
            agent.transform.position = Vector3.Lerp(startPos, endPos, normalizedTime) + yOffset * Vector3.up;
            normalizedTime += Time.deltaTime / duration;
            yield return null;
        }
    }*/

    public bool isTraversingLink;


    public OffMeshLinkMoveMethod currentOffMeshLinkMoveMethod;
    //OffMeshLinkData currentLinkData;
    NavMeshLinkProperties currentNavMeshLinkProperties;
    Vector3 currentLinkEndPosition;
    Vector3 currentLinkStartPosition;
    float currentDistanceToTraverse;
    float currentLinkTraverseDuration;
    //float currentTraversingStartTime;
    float currentTraversalNormalizedTime; //Normalized between 0 and 1 of the currentTraversalDuration
    NavMeshAgent agent;

    //For calculating the jump over hole or up and down curve
    public AnimationCurve horizontalJumpCurve = new AnimationCurve();
    public AnimationCurve jumpingDownCuve = new AnimationCurve();
    public AnimationCurve jumpingUpCurve = new AnimationCurve();
    AnimationCurve currentCurveForJumpingUpDownOrHorizontal;
    float distanceHeightRatio = 0.2f; //how much the length is the height of the jump going to be?
    float currentJumpOverHoleHeight;

    //FOr jumping over obstacle
    float currentObstacleHeight;
    public AnimationCurve jumpOverObstacleCurve = new AnimationCurve();

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
 

