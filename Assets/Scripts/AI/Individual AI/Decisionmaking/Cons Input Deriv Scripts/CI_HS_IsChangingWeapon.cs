using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BenitosAI
{

    [CreateAssetMenu(menuName = "AI/Consideration Input/Humanoid/Is Changing Weapon", fileName = "Is Changing Weapon")]
    public class CI_HS_IsChangingWeapon : ConsiderationInput
    {
        public override float GetConsiderationInput(DecisionContext decisionContext, ConsiderationInputParams considerationInputParams)
        {
            if (((AIController_HumanoidSoldier)decisionContext.aiController).characterController.IsChangingWeapon())
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