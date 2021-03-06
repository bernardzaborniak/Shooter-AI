using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace BenitosAI
{

    [CreateAssetMenu(menuName = "AI/Consideration Input/Humanoid/Distance to TP", fileName = "Distance to TP")]

    public class CI_HS_DistanceToTP : ConsiderationInput
    {
        public override float GetConsiderationInput(DecisionContext decisionContext, Consideration consideration)
        {
            float input = Utility.Remap(decisionContext.targetTacticalPointInfo.distance, consideration.min, consideration.max, 0, 1);
            return Mathf.Clamp(input, 0, 1);
        }
    }
}

