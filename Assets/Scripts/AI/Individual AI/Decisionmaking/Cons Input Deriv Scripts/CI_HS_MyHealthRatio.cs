using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BenitosAI
{

    [CreateAssetMenu(menuName = "AI/Consideration Input/Humanoid/My Health Ratio", fileName = "My Health Ratio")]
    public class CI_HS_MyHealthRatio : ConsiderationInput
    {
        public override float GetConsiderationInput(DecisionContext decisionContext, ConsiderationInputParams considerationInputParams)
        {
            return ((AIController_HumanoidSoldier)decisionContext.aiController).blackboard.GetRemainingHealthToMaxHalthRatio();
        }

    }

}
