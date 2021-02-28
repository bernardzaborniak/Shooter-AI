using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BenitosAI
{

    [CreateAssetMenu(menuName = "AI/Consideration Input/Humanoid/Is inside TPCoverPeek", fileName = "Is inside TPCoverPeek")]

    public class CI_HS_IsInsideTPCoverPeek : ConsiderationInput
    {
        public override float GetConsiderationInput(DecisionContext decisionContext, Consideration consideration)
        {
            //get raycast start posiiton & direction -> how? - get the position and direction from the head - position is head and direction is direction from head to target - aim posiiton
            //- check if hitbox entity is target entity

            // get head posiiton from sensing, target from context
            TacticalPoint tPoint = ((AIController_HumanoidSoldier)decisionContext.aiController).blackboard.GetCurrentlyUsedTacticalPoint();
            if (tPoint != null)
            {
                if (tPoint.tacticalPointType == TacticalPointType.CoverPeekPoint)
                {
                    return 1;
                }
            }
            return 0;
        }
    }
}
