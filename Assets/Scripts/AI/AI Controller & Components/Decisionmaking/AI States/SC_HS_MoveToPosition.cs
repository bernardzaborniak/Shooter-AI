using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BenitosAI
{
    public class SC_HS_MoveToPosition : AIStateCreator
    {
        float preferredDistanceToEnemy;

        public override AIState CreateState(AIController aiController, DecisionContext context)
        {
            St_HS_MovingToPosition state = new St_HS_MovingToPosition();
            state.SetUpState(aiController, context);

            //state.targetPosition = targetPosition;

            return state;
        }
    }
}

