using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace BenitosAI
{
    [CreateAssetMenu(menuName = "AI/Consideration/Input/Humanoid/Ammo In Weapon", fileName = "AmmoInWeapon")]
    public class CI_HS_AmmoInWeapon : ConsiderationInput
    {
        public override float GetConsiderationInput(DecisionContext decisionContext, Consideration consideration)
        {
            return ((AIController_HumanoidSoldier)decisionContext.aiController).characterController.GetAmmoRemainingInMagazineRatio();          
        }
    }
}
