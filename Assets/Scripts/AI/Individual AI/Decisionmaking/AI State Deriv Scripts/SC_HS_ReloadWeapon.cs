using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace BenitosAI
{
    [CreateAssetMenu(menuName = "AI/States/ReloadWeapon", fileName = "ReloadWeapon")]
    public class SC_HS_ReloadWeapon : AIStateCreator
    {
        public int weaponID;

        public override AIState CreateState(AIController aiController, DecisionContext context)
        {
            St_HS_ReloadWeapon state = new St_HS_ReloadWeapon(aiController, context, weaponID);
            return state;
        }
    }

    public class St_HS_ReloadWeapon : AIState 
    {
        AIController_HumanoidSoldier aiController;
        EC_HumanoidCharacterController charController;

        EntityActionTag[] actionTags;


        public St_HS_ReloadWeapon(AIController aiController , DecisionContext context, int weaponID)
        {
            this.aiController = (AIController_HumanoidSoldier)aiController;
            charController = this.aiController.characterController;

            actionTags = new EntityActionTag[1];
            actionTags[0] = new EntityActionTag(EntityActionTag.Type.ReloadingWeapon);

        }

        public override void OnStateEnter()
        {
           
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


