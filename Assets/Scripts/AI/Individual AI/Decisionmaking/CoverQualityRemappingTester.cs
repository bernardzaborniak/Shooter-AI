using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoverQualityRemappingTester : MonoBehaviour
{
    public Transform[] threatTransforms;
    public Transform[] friendlyTransforms;
    public bool crouchTest;
    public QualityOfCoverEvaluationType type;

    public TacticalPoint tPointToTest;

    


    void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            //for (int j = 0; j < 100; j++)
            //{
            /*  (Vector3 threatPosition, float distanceToThreat)[] threatsInfo = new (Vector3 threatPosition, float distanceToThreat)[threatTransforms.Length];
              (Vector3 friendlyPosition, float distanceToThreat)[] friendliesInfo = new (Vector3 threatPosition, float distanceToThreat)[friendlyTransforms.Length];

              for (int i = 0; i < threatsInfo.Length; i++)
              {
                  threatsInfo[i] = (threatTransforms[i].position, Vector3.Distance(threatTransforms[i].position, tPointToTest.GetPointPosition()));
              }

              for (int i = 0; i < friendliesInfo.Length; i++)
              {
                  friendliesInfo[i] = (friendlyTransforms[i].position, Vector3.Distance(friendlyTransforms[i].position, tPointToTest.GetPointPosition()));
              }

          Debug.Log(tPointToTest.DetermineQualityOfCover(type, threatsInfo, friendliesInfo));*/

            //}

            //calculate mean threat direction & closest enemy---------------------

            //TODO, look it up in blackboard
            Vector3 closestEnemyPosition = Vector3.zero;
            float closestDistance = Mathf.Infinity;
            Vector3 mainThreatDirection = Vector3.zero;

            for (int i = 0; i < threatTransforms.Length; i++)
            {
                Vector3 directionToEnemy = (threatTransforms[i].position - tPointToTest.transform.position);
                float distance = directionToEnemy.magnitude;

                mainThreatDirection += directionToEnemy.normalized * (1000 - distance);

                if(distance< closestDistance)
                {
                    closestDistance = distance;
                    closestEnemyPosition = threatTransforms[i].position;
                }

            }

            //Debug.Log(tPointToTest.DetermineQualityOfCoverSimple( mainThreatDirection, closestEnemyPosition));
            Debug.Log(DetermineQualityOfCoverSimple(tPointToTest, mainThreatDirection, closestEnemyPosition));

            
        }


    }

    float DetermineQualityOfCoverSimple(TacticalPoint tPToRate, Vector3 meanThreatDirection, Vector3 closestEnemyPosition)
    {
        float rating = 0;

        if (meanThreatDirection == Vector3.zero) return 0;


        if (tPToRate.tacticalPointType == TacticalPointType.CoverPoint)
        {
            // Rate the actual Cover Point first
            (float distance, float quality) ratingForDirection = tPToRate.GetRatingForDirection(meanThreatDirection);

            if (ratingForDirection.distance < 2)
            {
                if (ratingForDirection.quality > 0.5f) rating = 1;
                else rating = ratingForDirection.quality;    
            }
            else
            {
                return 0;
            }
            rating *= 0.7f;

            //Go through the cover points peek points, take the best rated from them
            float bestPeekPointRating = 0;
            float distanceToEnemyFromPoint = Vector3.Distance(closestEnemyPosition, tPToRate.transform.position); //only calculate the distance to the cover point, this distance will be used in the peek point rating
            for (int i = 0; i < tPToRate.correspondingCoverPeekPoints.Length; i++)
            {
                (float distance, float quality) ratingForDirectionInCorrespondingPeekPoint = tPToRate.correspondingCoverPeekPoints[i].GetRatingForDirection(meanThreatDirection);
                if (distanceToEnemyFromPoint < ratingForDirectionInCorrespondingPeekPoint.distance)
                {
                    bestPeekPointRating = 1;
                }

            }

            rating += bestPeekPointRating * 0.3f;

        }
        else if (tPToRate.tacticalPointType == TacticalPointType.CoverPeekPoint)
        {
            if (closestEnemyPosition == Vector3.zero) return 0;

            //get rating for the actual peek point we are rating + the corresponding cover point
            (float distance, float quality) ratingForDirectionInCorrespondingCoverPoint = tPToRate.correspondingCoverPoint.GetRatingForDirection(meanThreatDirection);
            (float distance, float quality) ratingForDirection = tPToRate.GetRatingForDirection(meanThreatDirection);

            //corresponding cover point is weighted 0.3
            if (ratingForDirectionInCorrespondingCoverPoint.distance < 2)
            {
                if (ratingForDirectionInCorrespondingCoverPoint.quality > 0.5f) rating = 1;
                else rating = ratingForDirectionInCorrespondingCoverPoint.quality;

                rating *= 0.3f ;
            }


            //the actual peek point - weighted 0.7
            float distanceToEnemyFromPoint = Vector3.Distance(closestEnemyPosition, tPToRate.transform.position);
            //Optimisation Idea: instead of calculating this distance - use distance to point + measured distance to enemy combined?
            Debug.Log("distanceToEnemyFromPoint: " + distanceToEnemyFromPoint);
            Debug.Log("ratingForDirection.distance: " + ratingForDirection.distance);
            if (distanceToEnemyFromPoint < ratingForDirection.distance)
            {
                rating += 0.7f;
            }
        }


        return rating;

        //cover and peek point rating methoden zusammenfassen?

        // um zu optimieren - bereits geratete points im blackboard speichern?
    }





}

