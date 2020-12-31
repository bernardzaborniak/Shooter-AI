using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BenitosAI
{

    [CreateAssetMenu(menuName = "AI/Consideration/Input/Humanoid/Deviation From Desired Distance To Enemy", fileName = "DeviationFromDesiredDistanceToEnemy")]
    public class CI_HS_DeviationFromDesiredDistanceToEnemy : ConsiderationInput
    {
        //AIController_HumanoidSoldier aiControllerHuman;
        //SensingInfo sensingInfo;

        public override float GetConsiderationInput(DecisionContext decisionContext, Consideration consideration)
        {
            float input = Utility.Remap(Mathf.Abs(consideration.desiredFloatValue - decisionContext.targetEntity.lastDistanceMeasured), consideration.min, consideration.max, 0, 1);
            return Mathf.Clamp(input, 0, 1);
        }
    }

}
