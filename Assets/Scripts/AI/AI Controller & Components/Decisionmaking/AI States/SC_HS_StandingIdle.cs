using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BenitosAI
{
    public class SC_HS_StandingIdle : AIStateCreator
    {
        public override AIState CreateState(AIController aiController, DecisionContext context)
        {
            St_HS_StandingIdle state = new St_HS_StandingIdle();
            state.SetUpState(aiController, context);


            return state;
        }
    }

    public class St_HS_StandingIdle : AIState //AIState_HumanoidSoldier
    {
        AIController_HumanoidSoldier aiController;
        EC_HumanoidCharacterController charController;

        public override void SetUpState(AIController aiController, DecisionContext context)
        {
            this.aiController = (AIController_HumanoidSoldier)aiController;
            charController = this.aiController.characterController;
        }

        public override void OnStateEnter()
        {
            //charController.MoveTo(targetPosition, true);
            charController.StopAimingSpine();
            charController.StopAimingWeapon();
        }

        public override void OnStateExit()
        {
        }

        public override void UpdateState()
        {
            //Debug.Log("updating state: ");
            if (charController.IsMoving())
            {
                charController.StopMoving();
            }
        }
    }
}
