using System.Collections;
using System.Collections.Generic;
using UnityEngine;



namespace BenitosAI
{

    [CreateAssetMenu(menuName = "AI/Consideration Input/Humanoid/Is TP In Desired Range To Enemy ", fileName = "Is TP In Desired Range To Enemy ")]

    public class CI_HS_IsTPInDesiredRangeToEnemy : ConsiderationInput
    {
        // uses consideration min & max - needs range type
        // uses targetTPoint

        public override float GetConsiderationInput(DecisionContext decisionContext, Consideration consideration)
        {
            if (((AIController_HumanoidSoldier)decisionContext.aiController).blackboard.enemyInfos.Length == 0) return 0;

            SensedEntityInfo nearestEnemyInfo = ((AIController_HumanoidSoldier)decisionContext.aiController).blackboard.enemyInfos[0];

            return Utility.Remap(Vector3.Distance(nearestEnemyInfo.GetEntityPosition(), decisionContext.targetTacticalPointInfo.tPoint.GetPointPosition()), consideration.min, consideration.max, 0, 1, true);
      
        }
    }
}
