using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace BenitosAI
{

    [CreateAssetMenu(menuName = "AI/ConsiderationInput/Humanoid/Has Weapon in Hand", fileName = "HasWeaponInHand")]
    public class CI_HS_HasWeaponInHand : ConsiderationInput
    {
        public override float GetConsiderationInput(DecisionContext decisionContext, Consideration consideration)
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

