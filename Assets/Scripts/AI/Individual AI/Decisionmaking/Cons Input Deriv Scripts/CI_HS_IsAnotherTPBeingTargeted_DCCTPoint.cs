using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace BenitosAI
{

    [CreateAssetMenu(menuName = "AI/Consideration Input/Humanoid/Is Another TP Being Targeted [DCC_TPoint]", fileName = "Is Another TP Being Targeted [DCC_TPoint]")]
    public class CI_HS_IsAnotherTPBeingTargeted_DCCTPoint : ConsiderationInput
    {
        //returns 1 if the ai is targeting another tpoint
        public override float GetConsiderationInput(DecisionContext decisionContext, ConsiderationInputParams considerationInputParams)
        {
            TacticalPoint targetedPoint = ((AIController_HumanoidSoldier)decisionContext.aiController).blackboard.GetCurrentlyTargetedPoint();

            if (targetedPoint != null)
            {
                if(targetedPoint != decisionContext.targetTacticalPointInfo.tPoint)
                {
                    return 1;
                }
            }

            return 0;

        }
    }
}
