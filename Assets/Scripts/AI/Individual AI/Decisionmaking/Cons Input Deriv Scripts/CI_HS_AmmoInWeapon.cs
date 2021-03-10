using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace BenitosAI
{
    [CreateAssetMenu(menuName = "AI/Consideration Input/Humanoid/Ammo In Weapon", fileName = "Ammo In Weapon")]
    public class CI_HS_AmmoInWeapon : ConsiderationInput
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
            return ((AIController_HumanoidSoldier)decisionContext.aiController).blackboard.GetAmmoRemainingInMagazineRatio(considerationInputParams.weaponID);          
        }
    }
}
