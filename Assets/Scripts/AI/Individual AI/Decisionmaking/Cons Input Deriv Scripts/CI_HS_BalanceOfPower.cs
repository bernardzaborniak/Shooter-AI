using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace BenitosAI
{

    [CreateAssetMenu(menuName = "AI/Consideration Input/Humanoid/Balance of Power", fileName = "Balance of Power")]
    public class CI_HS_BalanceOfPower : ConsiderationInput
    {
        public override float GetConsiderationInput(DecisionContext decisionContext, Consideration consideration)
        {
            float input = Utility.Remap(((AIController_HumanoidSoldier)decisionContext.aiController).blackboard.currentBalanceOfPower, consideration.min, consideration.max, 0, 1);
            return Mathf.Clamp(input, 0, 1);
        }
    }
}
