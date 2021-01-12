using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BenitosAI
{
    [CreateAssetMenu(menuName = "AI/States/Example", fileName = "Example")]
    public class SC_HS_Example : AIStateCreator
    {

        public override AIState CreateState(AIController aiController, DecisionContext context)
        {
            St_HS_Example state = new St_HS_Example();
            state.SetUpState(aiController, context);

            return state;
        }
    }

    public class St_HS_Example : AIState //AIState_HumanoidSoldier
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

        }

        public override void OnStateExit()
        {

        }

        public override void UpdateState()
        {

        }
    }

}

