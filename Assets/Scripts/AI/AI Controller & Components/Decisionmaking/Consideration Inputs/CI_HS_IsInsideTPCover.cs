using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BenitosAI
{

    [CreateAssetMenu(menuName = "AI/Consideration/Input/Humanoid/IsInsideTPCover", fileName = "Is Inside TP Cover")]

    public class CI_HS_IsInsideTPCover : ConsiderationInput
    {
        public override float GetConsiderationInput(DecisionContext decisionContext, Consideration consideration)
        {
            //get raycast start posiiton & direction -> how? - get the position and direction from the head - position is head and direction is direction from head to target - aim posiiton
            //- check if hitbox entity is target entity

            // get head posiiton from sensing, target from context
            TacticalPoint tPoint = ((AIController_HumanoidSoldier)decisionContext.aiController).humanSensing.GetCurrentlyUsedTacticalPoint();
            if (tPoint != null)
            {
                if(tPoint.tacticalPointType == TacticalPointType.CoverPoint)
                {
                    return 1;
                }
            }
            return 0;
        }
    }
}
