using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BenitosAI
{

    [CreateAssetMenu(menuName = "AI/Consideration/Input/Humanoid/TPointCoverQualityForCurrentSituation", fileName = "TPointCoverQualityForCurrentSituation")]
    public class CI_HS_TPointCoverQualityForCurrentSituation : ConsiderationInput
    {

        public override float GetConsiderationInput(DecisionContext decisionContext, Consideration consideration)
        {
            AIC_HumanSensing sensing = ((AIController_HumanoidSoldier)decisionContext.aiController).humanSensing;
            SensedEntityInfo[] enemyInfos = sensing.sensingInfo.enemyInfos;
            SensedEntityInfo[] friendlyInfos = sensing.sensingInfo.friendlyInfos;

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

            return decisionContext.targetTacticalPoint.tacticalPoint.DetermineQualityOfCover(consideration.tPointEvaluationType, threatsInfo, friendliesInfo, consideration.tPointEvaluationCrouching);

        }
    }

}

