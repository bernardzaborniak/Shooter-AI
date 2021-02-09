using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace BenitosAI
{
    [CreateAssetMenu(menuName = "AI/States/FallBackToPosition", fileName = "FallBackToPosition")]
    public class SC_HS_FallBackToPosition : AIStateCreator
    {
        public Vector3 targetPosition;

        public override AIState CreateState(AIController aiController, DecisionContext context)
        {
            St_HS_FallBackToPosition state = new St_HS_FallBackToPosition(aiController, context, targetPosition);
            return state;
        }
    }

    public class St_HS_FallBackToPosition : AIState
    {
        AIController_HumanoidSoldier aiController;
        EC_HumanoidCharacterController charController;
        Vector3 targetPosition;

        public St_HS_FallBackToPosition(AIController aiController, DecisionContext context, Vector3 targetPosition)
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
