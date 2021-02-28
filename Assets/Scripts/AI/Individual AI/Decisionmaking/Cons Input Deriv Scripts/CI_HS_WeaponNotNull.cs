using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace BenitosAI
{

    [CreateAssetMenu(menuName = "AI/Consideration Input/Humanoid/WeaponNotNull", fileName = "Weapon Not Null")]
    public class CI_HS_WeaponNotNull : ConsiderationInput
    {
        public override float GetConsiderationInput(DecisionContext decisionContext, Consideration consideration)
        {
            if (((AIController_HumanoidSoldier)decisionContext.aiController).characterController.GetItemInInventory(consideration.weaponID) != null)
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
