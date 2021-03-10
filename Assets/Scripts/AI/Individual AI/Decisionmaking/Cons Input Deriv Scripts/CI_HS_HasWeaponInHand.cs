using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace BenitosAI
{

    [CreateAssetMenu(menuName = "AI/Consideration Input/Humanoid/Has Weapon in Hand", fileName = "Has Weapon in Hand")]
    public class CI_HS_HasWeaponInHand : ConsiderationInput
    {


        public override float GetConsiderationInput(DecisionContext decisionContext, ConsiderationInputParams considerationInputParams)
        {
            if (((AIController_HumanoidSoldier)decisionContext.aiController).characterController.GetCurrentlySelectedItem() != null)
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

