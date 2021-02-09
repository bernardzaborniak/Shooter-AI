using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace BenitosAI
{
    [CreateAssetMenu(menuName = "AI/States/AdvanceToPosition", fileName = "AdvanceToPosition")]
    public class SC_HS_AdvanceToPosition : AIStateCreator
    {
        public Vector3 targetPosition;

        public override AIState CreateState(AIController aiController, DecisionContext context)
        {
            St_HS_AdvanceToPosition state = new St_HS_AdvanceToPosition(aiController, context, targetPosition);
            return state;
        }
    }

    public class St_HS_AdvanceToPosition : AIState
    {
        AIController_HumanoidSoldier aiController;
        EC_HumanoidCharacterController charController;
        Vector3 targetPosition;

        public St_HS_AdvanceToPosition(AIController aiController, DecisionContext context, Vector3 targetPosition)
        {
            this.aiController = (AIController_HumanoidSoldier)aiController;
            this.charController = this.aiController.characterController;
            this.targetPosition = targetPosition;
        }

        public override void OnStateEnter()
        {

        }

        public override void OnStateExit()
        {

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

        }
    }
}