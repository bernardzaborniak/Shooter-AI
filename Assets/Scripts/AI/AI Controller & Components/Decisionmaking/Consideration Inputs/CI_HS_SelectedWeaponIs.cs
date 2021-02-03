using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BenitosAI
{

    [CreateAssetMenu(menuName = "AI/ConsiderationInput/Humanoid/SelectedWeaponIs", fileName = "SelectedWeaponIs")]
    public class CI_HS_SelectedWeaponIs : ConsiderationInput
    {
        public override float GetConsiderationInput(DecisionContext decisionContext, Consideration consideration)
        {
            if(((AIController_HumanoidSoldier)decisionContext.aiController).characterController.GetCurrentSelectedItemID() == consideration.weaponID)
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
