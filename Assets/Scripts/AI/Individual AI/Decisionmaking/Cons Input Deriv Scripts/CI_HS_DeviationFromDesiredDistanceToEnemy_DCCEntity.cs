using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BenitosAI
{

    [CreateAssetMenu(menuName = "AI/Consideration Input/Humanoid/Deviation from Desired Distance to Enemy [DCC_Entity]", fileName = "Deviation from Desired Distance to Enemy [DCC_Entity]")]
    public class CI_HS_DeviationFromDesiredDistanceToEnemy_DCCEntity : ConsiderationInput
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
            float input = Utility.Remap(Mathf.Abs(considerationInputParams.desiredFloatValue - decisionContext.targetEntityInfo.lastDistanceMeasured), considerationInputParams.min, considerationInputParams.max, 0, 1);
            return Mathf.Clamp(input, 0, 1);
        }
    }

}
