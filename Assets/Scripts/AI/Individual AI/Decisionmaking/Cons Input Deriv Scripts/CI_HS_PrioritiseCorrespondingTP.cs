using System.Collections;
using System.Collections.Generic;
using UnityEngine;



namespace BenitosAI
{

    [CreateAssetMenu(menuName = "AI/Consideration Input/Humanoid/Prioritise Corresponding TP", fileName = "Prioritise Corresponding TP")]

    public class CI_HS_PrioritiseCorrespondingTP : ConsiderationInput
    {
        // uses consideration min & max - needs range type
        // uses targetTPoint

        public override float GetConsiderationInput(DecisionContext decisionContext, Consideration consideration)
        {
            TacticalPoint currentlyUsedTP = ((AIController_HumanoidSoldier)decisionContext.aiController).blackboard.GetCurrentlyUsedTacticalPoint();
            if (currentlyUsedTP == null) return 1;

            if(decisionContext.targetTacticalPointInfo.tPoint.tacticalPointType == TacticalPointType.CoverPeekPoint)
            {
                if (currentlyUsedTP.tacticalPointType == TacticalPointType.CoverPoint)
                {
                    for (int i = 0; i < currentlyUsedTP.correspondingCoverPeekPoints.Length; i++)
                    {
                        if (currentlyUsedTP.correspondingCoverPeekPoints[i] == decisionContext.targetTacticalPointInfo.tPoint) return 1;
                    }
                }
                else if(currentlyUsedTP.tacticalPointType == TacticalPointType.CoverPeekPoint)
                {
                    for (int i = 0; i < currentlyUsedTP.correspondingCoverPoint.correspondingCoverPeekPoints.Length; i++)
                    {
                        if (currentlyUsedTP.correspondingCoverPoint.correspondingCoverPeekPoints[i] == decisionContext.targetTacticalPointInfo.tPoint) return 1;
                    }
                }
            }
            else if(decisionContext.targetTacticalPointInfo.tPoint.tacticalPointType == TacticalPointType.CoverPoint)
            {
                if (currentlyUsedTP.tacticalPointType == TacticalPointType.CoverPeekPoint)
                {
                    if (currentlyUsedTP.correspondingCoverPoint == decisionContext.targetTacticalPointInfo.tPoint) return 1;

                }
            }


            return 0;
        }
    }
}
