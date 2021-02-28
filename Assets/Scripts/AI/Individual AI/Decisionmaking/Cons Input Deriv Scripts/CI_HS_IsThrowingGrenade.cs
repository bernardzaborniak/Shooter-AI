using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace BenitosAI
{

    [CreateAssetMenu(menuName = "AI/Consideration Input/Humanoid/Is Throwing Grenade", fileName = "Is Throwing Grenade")]
    public class CI_HS_IsThrowingGrenade : ConsiderationInput
    {
        public override float GetConsiderationInput(DecisionContext decisionContext, Consideration consideration)
        {
            if (((AIController_HumanoidSoldier)decisionContext.aiController).characterController.IsThrowingGrenade())
            {
                return 1;
            }
            else
            {
                return 0;
            }
        }
    }
}
