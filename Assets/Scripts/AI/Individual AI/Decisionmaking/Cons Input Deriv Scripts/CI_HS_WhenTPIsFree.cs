using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace BenitosAI
{

    [CreateAssetMenu(menuName = "AI/Consideration Input/Humanoid/When TP is Free", fileName = "When TP is Free")]

    public class CI_HS_WhenTPIsFree : ConsiderationInput
    {
        public override float GetConsiderationInput(DecisionContext decisionContext, Consideration consideration)
        {
            //not needed anymore? - sensing takes care of this

            /*if (!decisionContext.targetTacticalPoint.tacticalPoint.IsPointUsedByAnotherEntity())
            {
                return 1;
            }
            else if (decisionContext.targetTacticalPoint.tacticalPoint.entityUsingThisPoint == ((AIController_HumanoidSoldier)decisionContext.aiController).blackboard.GetMyEntity())
            {
                //else if i am already using it
                return 1;
            }*/

            return 0;
        }
    }
}

