using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BenitosAI
{

    [CreateAssetMenu(menuName = "AI/Consideration Input/Humanoid/Distance to Enemy", fileName = "Distance to Enemy")]
    public class CI_HS_DistanceToEnemy : ConsiderationInput
    {

        public override float GetConsiderationInput(DecisionContext decisionContext, Consideration consideration)
        {
            float input = Utility.Remap(decisionContext.targetEntity.lastDistanceMeasured, consideration.min, consideration.max, 0, 1);
            return Mathf.Clamp(input, 0, 1);
        }
    }

}
