using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace BenitosAI
{

    [CreateAssetMenu(menuName = "AI/Consideration Input/Humanoid/Distance to TP", fileName = "Distance to TP")]

    public class CI_HS_DistanceToTP : ConsiderationInput
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
            float input = Utility.Remap(decisionContext.targetTacticalPointInfo.distance, considerationInputParams.min, considerationInputParams.max, 0, 1);
            return Mathf.Clamp(input, 0, 1);
        }
    }
}

