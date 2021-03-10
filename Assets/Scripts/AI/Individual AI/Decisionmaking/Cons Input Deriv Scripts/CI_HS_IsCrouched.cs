using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace BenitosAI
{

    [CreateAssetMenu(menuName = "AI/Consideration Input/Humanoid/Is Crouched", fileName = "Is Crouched")]
    public class CI_HS_IsCrouched : ConsiderationInput
    {
        public override float GetConsiderationInput(DecisionContext decisionContext, ConsiderationInputParams considerationInputParams)
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
