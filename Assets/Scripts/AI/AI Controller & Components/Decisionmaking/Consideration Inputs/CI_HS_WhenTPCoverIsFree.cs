using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace BenitosAI
{

    [CreateAssetMenu(menuName = "AI/Consideration/Input/Humanoid/WhenTPCoverIsFree", fileName = "WhenTPCoverIsFree")]

    public class CI_HS_WhenTPCoverIsFree : ConsiderationInput
    {
        public override float GetConsiderationInput(DecisionContext decisionContext, Consideration consideration)
        {

            if(decisionContext.targetTacticalPoint.tacticalPoint.tacticalPointType == TacticalPointType.CoverPoint)
            {
                if (!decisionContext.targetTacticalPoint.tacticalPoint.IsPointFull())
                {
                    return 1;
                }
            }

            return 0;
        }
    }
}

