using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BenitosAI
{

    [CreateAssetMenu(menuName = "AI/ConsiderationInput/Humanoid/TPointCoverQualityForCurrentSituation", fileName = "TPointCoverQualityForCurrentSituation")]
    public class CI_HS_TPointCoverQualityForCurrentSituation : ConsiderationInput
    {

        public override float GetConsiderationInput(DecisionContext decisionContext, Consideration consideration)
        {
            UnityEngine.Profiling.Profiler.BeginSample("RateCoverPoint");

            AIController_Blackboard blackboard = ((AIController_HumanoidSoldier)decisionContext.aiController).blackboard;
            SensedEntityInfo[] enemyInfos = blackboard.enemyInfos;
            SensedEntityInfo[] friendlyInfos = blackboard.friendlyInfos;

            //cap the lengths if needed
            int threatsInfoLength = consideration.tPointEvaluationMaxEnemiesToAcknowledgeWhileRating;
            int friendliesInfoLength = consideration.tPointEvaluationMaxFriendliesToAcknowledgeWhileRating;
            if (enemyInfos.Length < threatsInfoLength)
            {
                threatsInfoLength = enemyInfos.Length;
            }
            if(friendlyInfos.Length < friendliesInfoLength)
            {
                friendliesInfoLength = friendlyInfos.Length;
            }


            //get  nearest enemies - check if crocuhed cover direction towards him has quality >0.7 - if yes -> cover quality = 1, else cover qulity = 0
            (Vector3 threatPosition, float distanceToThreat)[] threatsInfo = new (Vector3 threatPosition, float distanceToThreat)[threatsInfoLength];
            (Vector3 friendlyPosition, float distanceToThreat)[] friendliesInfo = new (Vector3 threatPosition, float distanceToThreat)[friendliesInfoLength];

            for (int i = 0; i < threatsInfo.Length; i++)
            {
                threatsInfo[i] = (enemyInfos[i].GetEntityPosition(), Vector3.Distance(enemyInfos[i].GetEntityPosition(), decisionContext.targetTacticalPoint.tacticalPoint.GetPointPosition()));
            }

            for (int i = 0; i < friendliesInfo.Length; i++)
            {
                friendliesInfo[i] = (friendlyInfos[i].GetEntityPosition(), Vector3.Distance(friendlyInfos[i].GetEntityPosition(), decisionContext.targetTacticalPoint.tacticalPoint.GetPointPosition()));
            }

            /* Debug.Log("CI_HS_TPointCoverQualityForCurrentSituation returns: " + decisionContext.targetTacticalPoint.tacticalPoint.DetermineQualityOfCover(consideration.tPointEvaluationType, threatsInfo, friendliesInfo, consideration.tPointEvaluationCrouching));
             Debug.Log("decisionContext " + decisionContext);
             Debug.Log("decisionContext.targetTacticalPoint " + decisionContext.targetTacticalPoint);
             Debug.Log("consideration.tPointEvaluationType " + consideration.tPointEvaluationType);
             Debug.Log("threatsInfo " + threatsInfo);
             Debug.Log("friendliesInfo " + friendliesInfo);*/
            //if(float.IsNaN( decisionContext.targetTacticalPoint.tacticalPoint.DetermineQualityOfCover(consideration.tPointEvaluationType, threatsInfo, friendliesInfo, consideration.tPointEvaluationCrouching)))
            // {
            //     Debug.Log("NANANANANA");
            // }
            float rating = decisionContext.targetTacticalPoint.tacticalPoint.DetermineQualityOfCover(consideration.tPointEvaluationType, threatsInfo, friendliesInfo, consideration.tPointEvaluationCrouching);
            UnityEngine.Profiling.Profiler.EndSample();

            return rating;
        }
    }

}

