using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BenitosAI
{

    [CreateAssetMenu(menuName = "AI/Consideration Input/Humanoid/TP Quality of Cover for Current Situation", fileName = "TP Quality of Cover for Current Situation")]
    public class CI_HS_TPQualityOfCoverForCurrentSituation : ConsiderationInput
    {

        public override float GetConsiderationInput(DecisionContext decisionContext, Consideration consideration)
        {
            //UnityEngine.Profiling.Profiler.BeginSample("RateCoverPointBeginning");
            //this takes way too much performance?

            AIController_Blackboard blackboard = ((AIController_HumanoidSoldier)decisionContext.aiController).blackboard;

            if (blackboard.enemyInfos.Length == 0) return 0;

            SensedEntityInfo closestEntity = blackboard.enemyInfos[0];

            float rating = decisionContext.targetTacticalPointInfo.tPoint.DetermineQualityOfCoverSimple(blackboard.meanThreatDirection, closestEntity.GetEntityPosition());
            //UnityEngine.Profiling.Profiler.EndSample();

            return rating;
        }

        /*public float GetConsiderationInputOld(DecisionContext decisionContext, Consideration consideration)
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
            if (friendlyInfos.Length < friendliesInfoLength)
            {
                friendliesInfoLength = friendlyInfos.Length;
            }


            //get  nearest enemies - check if crocuhed cover direction towards him has quality >0.7 - if yes -> cover quality = 1, else cover qulity = 0
            (Vector3 threatPosition, float distanceToThreat)[] threatsInfo = new (Vector3 threatPosition, float distanceToThreat)[threatsInfoLength];
            (Vector3 friendlyPosition, float distanceToThreat)[] friendliesInfo = new (Vector3 threatPosition, float distanceToThreat)[friendliesInfoLength];

            for (int i = 0; i < threatsInfo.Length; i++)
            {
                threatsInfo[i] = (enemyInfos[i].GetEntityPosition(), Vector3.Distance(enemyInfos[i].GetEntityPosition(), decisionContext.targetTacticalPointInfo.tPoint.GetPointPosition()));
            }

            for (int i = 0; i < friendliesInfo.Length; i++)
            {
                friendliesInfo[i] = (friendlyInfos[i].GetEntityPosition(), Vector3.Distance(friendlyInfos[i].GetEntityPosition(), decisionContext.targetTacticalPointInfo.tPoint.GetPointPosition()));
            }

            float rating = decisionContext.targetTacticalPointInfo.tPoint.DetermineQualityOfCover(consideration.tPointEvaluationType, threatsInfo, friendliesInfo);
            UnityEngine.Profiling.Profiler.EndSample();

            return rating;
        }*/
    }

}

