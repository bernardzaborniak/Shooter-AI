using System.Collections;
using System.Collections.Generic;
using UnityEngine;



namespace BenitosAI
{

    [CreateAssetMenu(menuName = "AI/Consideration Input/Humanoid/Distance from Target TP to Nearest Enemy", fileName = "Distance from Target TP to Nearest Enemy")]

    public class CI_HS_DistanceFromTargetTPToNearestEnemy : ConsiderationInput
    {
        void OnEnable()
        {
            inputParamsType = new ConsiderationInputParams.InputParamsType[]
            {
                ConsiderationInputParams.InputParamsType.Range
            };
        }

        public override float GetConsiderationInput(DecisionContext decisionContext, ConsiderationInputParams considerationInputParams)
        {
            if (((AIController_HumanoidSoldier)decisionContext.aiController).blackboard.enemyInfos.Length == 0) return 0;

            SensedEntityInfo nearestEnemyInfo = ((AIController_HumanoidSoldier)decisionContext.aiController).blackboard.enemyInfos[0];

            return Utility.Remap(Vector3.Distance(nearestEnemyInfo.GetEntityPosition(), decisionContext.targetTacticalPointInfo.tPoint.GetPointPosition()), considerationInputParams.min, considerationInputParams.max, 0, 1, true);
      
        }
    }
}
