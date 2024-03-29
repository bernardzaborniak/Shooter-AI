using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace BenitosAI
{

    [CreateAssetMenu(menuName = "AI/Consideration Input/Humanoid/Weapon Not Null", fileName = "Weapon Not Null")]
    public class CI_HS_WeaponNotNull : ConsiderationInput
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
            if (((AIController_HumanoidSoldier)decisionContext.aiController).characterController.GetItemInInventory(considerationInputParams.weaponID) != null)
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
