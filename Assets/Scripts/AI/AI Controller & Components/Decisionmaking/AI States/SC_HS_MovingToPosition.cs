using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BenitosAI
{
    [CreateAssetMenu(menuName = "AI/States/Moving To Position", fileName = "MovingToPosition")]
    public class SC_HS_MovingToPosition : AIStateCreator
    {
        public Vector3 targetPosition;

        public override AIState CreateState(AIController aiController, DecisionContext context)
        {
            St_HS_MovingToPosition state = new St_HS_MovingToPosition();
            state.SetUpState(aiController, context);

            state.targetPosition = targetPosition;

            return state;
        }
    }

    public class St_HS_MovingToPosition : AIState //AIState_HumanoidSoldier
    {
        AIController_HumanoidSoldier aiController;
        EC_HumanoidCharacterController charController;
        public Vector3 targetPosition;

        public override void SetUpState(AIController aiController, DecisionContext context)
        {
            this.aiController = (AIController_HumanoidSoldier)aiController;
            charController = this.aiController.characterController;
        }

        public override void OnStateEnter() 
        {
            charController.MoveTo(targetPosition, true);
        }

        public override void OnStateExit()
        {
            charController.StopMoving();
        }

        public override void UpdateState() 
        {
            //Debug.Log("updating state: ");
            if (!charController.IsMoving())
            {
                charController.MoveTo(targetPosition, true);
            }
        }
    }
}

