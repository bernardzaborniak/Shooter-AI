using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace BenitosAI
{
    [CreateAssetMenu(menuName = "AI/States/Reload Weapon", fileName = "Reload Weapon")]
    public class SC_HS_ReloadWeapon : AIStateCreator
    {
        //public int weaponID;

        void OnEnable()
        {
            inputParamsType = new AIStateCreatorInputParams.InputParamsType[]
            {
                AIStateCreatorInputParams.InputParamsType.WeaponID
            };
        }

        public override AIState CreateState(AIController aiController, DecisionContext context, AIStateCreatorInputParams inputParams)
        {
            St_HS_ReloadWeapon state = new St_HS_ReloadWeapon(aiController, context, inputParams.weaponID);
            return state;
        }
    }

    public class St_HS_ReloadWeapon : AIState 
    {
        AIController_HumanoidSoldier aiController;
        EC_HumanoidCharacterController charController;

        EntityActionTag[] actionTags;
        int weaponID;


        public St_HS_ReloadWeapon(AIController aiController , DecisionContext context, int weaponID)
        {
            this.aiController = (AIController_HumanoidSoldier)aiController;
            charController = this.aiController.characterController;

            actionTags = new EntityActionTag[1];
            actionTags[0] = new EntityActionTag(EntityActionTag.Type.ReloadingWeapon);
            this.weaponID = weaponID;

        }

        public override void OnStateEnter()
        {
            charController.ChangeSelectedItem(weaponID);
        }

        public override void OnStateExit()
        {
            charController.AbortReloadingWeapon();
        }

        public override EntityActionTag[] GetActionTagsToAddOnStateEnter()
        {
            return actionTags;
        }

        public override EntityActionTag[] GetActionTagsToRemoveOnStateExit()
        {
            return actionTags;
        }

        public override void UpdateState()
        {
            charController.StartReloadingWeapon();

        }

        public override bool ShouldStateBeAborted()
        {
            if (charController.GetAmmoRemainingInMagazineRatio() == 1)
            {
                return true;
            }

            return false;
        }
    }
}


