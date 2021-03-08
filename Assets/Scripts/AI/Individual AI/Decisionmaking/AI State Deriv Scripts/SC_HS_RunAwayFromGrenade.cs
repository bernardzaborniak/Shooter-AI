using System.Collections;
using System.Collections.Generic;
using UnityEngine;



namespace BenitosAI
{
    [CreateAssetMenu(menuName = "AI/States/RunAwayFromGrenade", fileName = "RunAwayFromGrenade")]
    public class SC_HS_RunAwayFromGrenade : AIStateCreator
    {

        public override AIState CreateState(AIController aiController, DecisionContext context)
        {
            St_HS_RunAwayFromGrenade state = new St_HS_RunAwayFromGrenade(aiController, context);
            return state;
        }
    }

    public class St_HS_RunAwayFromGrenade : AIState
    {
        AIController_HumanoidSoldier aiController;
        EC_HumanoidCharacterController charController;

        //EntityActionTag[] actionTags;

        Vector3 directionTowardsGrenadeDanger;


        public St_HS_RunAwayFromGrenade(AIController aiController, DecisionContext context)
        {
            this.aiController = (AIController_HumanoidSoldier)aiController;
            charController = this.aiController.characterController;

            //actionTags = new EntityActionTag[1];
            //actionTags[0] = new EntityActionTag(EntityActionTag.Type.ReloadingWeapon);

        }

        public override void OnStateEnter()
        {
            //this code prevents the bug, wehre the soldier is stuck in the throwing grenade or reloading state - seems to be a problem with the charackter ocntroller
           // charController.AbortThrowingGrenade();
            //charController.AbortReloadingWeapon();
        }

        public override void OnStateExit()
        {
            charController.StopMoving();
        }

        public override EntityActionTag[] GetActionTagsToAddOnStateEnter()
        {
            return null;
        }

        public override EntityActionTag[] GetActionTagsToRemoveOnStateExit()
        {
            return null;
        }

        public override void UpdateState()
        {
            // charController.StartReloadingWeapon();
            if (aiController.blackboard.environmentalDangerInfos.Length > 0)
            {
                directionTowardsGrenadeDanger = aiController.blackboard.environmentalDangerInfos[0].dangerTag.transform.position - charController.transform.position;
            }
            else
            {
                directionTowardsGrenadeDanger = Vector3.zero;
            }

            charController.ChangeCharacterStanceToStandingCombatStance();
            charController.MoveTo(charController.transform.position - directionTowardsGrenadeDanger.normalized * 2, true);

        }

        public override bool ShouldStateBeAborted()
        {
            /*if (directionTowardsGrenadeDanger == Vector3.zero)
            {
                return true;
            }*/

            return false;
        }
    }
}
