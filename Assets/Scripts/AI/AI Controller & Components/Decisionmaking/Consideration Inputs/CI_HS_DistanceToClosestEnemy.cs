using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BenitosAI
{

    [CreateAssetMenu(menuName = "AI/Consideration/Input/Humanoid/Distance To Closest Enemy", fileName = "DistanceToClosestEnemy")]
    public class CI_HS_DistanceToClosestEnemy : ConsiderationInput
    {
        AIController_HumanoidSoldier aiControllerHuman;

        public override float GetConsiderationInput(DecisionContext decisionContext, Consideration consideration)
        {
            aiControllerHuman = (AIController_HumanoidSoldier)decisionContext.aiController;

            if (aiControllerHuman.humanSensing.currentSensingInfo.nearestEnemyInfo != null)
            {
                //normalize
                float input = Utility.Remap(aiControllerHuman.humanSensing.currentSensingInfo.nearestEnemyInfo.lastSquaredDistanceMeasured, consideration.minSquared, consideration.maxSquared, 0, 1);
                //remap Squared Distance
                //float input = 0

                return Mathf.Clamp(input, 0, 1);
            }
            else
            {
                return Mathf.Infinity;
            }

        }
    }

}
