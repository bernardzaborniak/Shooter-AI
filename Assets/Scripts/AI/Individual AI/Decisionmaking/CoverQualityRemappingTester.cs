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

            Debug.Log(tPointToTest.DetermineQualityOfCover(type, mainThreatDirection, closestEnemyPosition));

            
        }
    }

   


}

