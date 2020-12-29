using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BenitosAI
{
    [CreateAssetMenu(menuName = "AI/States/HS MovingToZeroPoint", fileName = "MovingToZeroPoint")]
    public class AISC_HS_MovingToZeroPoint : AIStateCreator
    {
        public override AIState CreateState(AIController aiController, DecisionContext context)
        {
            St_HS_MovingToZeroPoint state = new St_HS_MovingToZeroPoint();
            state.SetUpState(aiController, context);
            return state;
        }
    }

    public class St_HS_MovingToZeroPoint : AIState //AIState_HumanoidSoldier
    {
        AIController_HumanoidSoldier aiController;

        public override void SetUpState(AIController aiController, DecisionContext context)
        {
            this.aiController = (AIController_HumanoidSoldier)aiController;
        }

        public override void OnStateEnter() { }

        public override void OnStateExit() { }

        public override void UpdateState() { }
    }
}

