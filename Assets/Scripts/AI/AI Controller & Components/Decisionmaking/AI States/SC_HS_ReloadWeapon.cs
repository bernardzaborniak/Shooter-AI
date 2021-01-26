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
            //St_HS_ReloadWeapon state = new St_HS_ReloadWeapon();
            St_HS_ReloadWeapon state = new St_HS_ReloadWeapon(aiController, context, weaponID);
            //state.SetUpState(aiController, context);

            return state;
        }
    }

    public class St_HS_ReloadWeapon : AIState //AIState_HumanoidSoldier
    {
        AIController_HumanoidSoldier aiController;
        EC_HumanoidCharacterController charController;
       // public Vector3 targetPosition;

        EntityActionTag[] actionTags;


       // public override void SetUpState(AIController aiController, DecisionContext context)
        public St_HS_ReloadWeapon(AIController aiController, DecisionContext context, int weaponID)
        {
            this.aiController = (AIController_HumanoidSoldier)aiController;
            charController = this.aiController.characterController;

            actionTags = new EntityActionTag[1];
            actionTags[0] = new EntityActionTag(EntityActionTagType.ReloadingWeapon);
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
            Debug.Log("reload weapon state GetTagsToAdd: " + actionTags[0].type);
            return actionTags;
        }

        public override EntityActionTag[] GetActionTagsToRemoveOnStateExit()
        {
            Debug.Log("reload weapon state GetTagsToRemove");

            return actionTags;
        }

        public override void UpdateState()
        {
            charController.StartReloadingWeapon();
        }
    }
}


