using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace BenitosAI
{

    [CreateAssetMenu(menuName = "AI/ConsiderationInput/Humanoid/IsCrouched", fileName = "IsCrouched")]
    public class CI_HS_IsCrouched : ConsiderationInput
    {
        public override float GetConsiderationInput(DecisionContext decisionContext, Consideration consideration)
        {
            if (((AIController_HumanoidSoldier)decisionContext.aiController).characterController.IsCrouched())
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
