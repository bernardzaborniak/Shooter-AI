using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BenitosAI
{

    [CreateAssetMenu(menuName = "AI/Consideration Input/Humanoid/Deviation from Desired Distance to Enemy", fileName = "Deviation from Desired Distance to Enemy")]
    public class CI_HS_DeviationFromDesiredDistanceToEnemy : ConsiderationInput
    {

        public override float GetConsiderationInput(DecisionContext decisionContext, Consideration consideration)
        {
            float input = Utility.Remap(Mathf.Abs(consideration.desiredFloatValue - decisionContext.targetEntityInfo.lastDistanceMeasured), consideration.min, consideration.max, 0, 1);
            return Mathf.Clamp(input, 0, 1);
        }
    }

}
