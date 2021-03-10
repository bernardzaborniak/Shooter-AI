using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace BenitosAI
{

    [CreateAssetMenu(menuName = "AI/Consideration Input/Humanoid/Distance to Nearest Enemy", fileName = "Distance to Nearest Enemy")]
    public class CI_HS_DistanceToNearestEnemy : ConsiderationInput
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
            SensedEntityInfo[] infos = ((AIController_HumanoidSoldier)decisionContext.aiController).blackboard.enemyInfos;

            if (infos.Length == 0) return 0;

            float input = Utility.Remap(infos[0].lastDistanceMeasured, considerationInputParams.min, considerationInputParams.max, 0, 1);
            return Mathf.Clamp(input, 0, 1);
        }
    }

}
