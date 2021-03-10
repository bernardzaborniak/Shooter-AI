using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BenitosAI
{

    [CreateAssetMenu(menuName = "AI/Consideration Input/Humanoid/Deviation from Desired Distance to Nearest Enemy", fileName = "Deviation from Desired Distance to Nearest Enemy")]
    public class CI_HS_DeviationFromDesiredDistanceToNearestEnemy : ConsiderationInput
    {
        void OnEnable()
        {
            inputParamsType = new ConsiderationInputParams.InputParamsType[]
            {
                ConsiderationInputParams.InputParamsType.Range,
                ConsiderationInputParams.InputParamsType.DesiredFloatValue
            };
        }

        public override float GetConsiderationInput(DecisionContext decisionContext, ConsiderationInputParams considerationInputParams)
        {
            SensedEntityInfo[] infos = ((AIController_HumanoidSoldier)decisionContext.aiController).blackboard.enemyInfos;

            if (infos.Length == 0) return 0;

            float input = Utility.Remap(Mathf.Abs(considerationInputParams.desiredFloatValue - infos[0].lastDistanceMeasured), considerationInputParams.min, considerationInputParams.max, 0, 1);
            return Mathf.Clamp(input, 0, 1);
        }
    }

}

