using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BenitosAI
{

    [CreateAssetMenu(menuName = "AI/Consideration Input/Humanoid/Selected Weapon Is", fileName = "Selected Weapon Is")]
    public class CI_HS_SelectedWeaponIs : ConsiderationInput
    {
        void OnEnable()
        {
            inputParamsType = new ConsiderationInputParams.InputParamsType[]
            {
                ConsiderationInputParams.InputParamsType.WeaponID
            };
        }

        public override float GetConsiderationInput(DecisionContext decisionContext, ConsiderationInputParams considerationInputParams)
        {
            if(((AIController_HumanoidSoldier)decisionContext.aiController).characterController.GetCurrentSelectedItemID() == considerationInputParams.weaponID)
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
