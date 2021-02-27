using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace BenitosAI
{

    [CreateAssetMenu(menuName = "AI/ConsiderationInput/Humanoid/WillGoingTowardsTPShowMyBackToEnemy ", fileName = "WillGoingTowardsTPShowMyBackToEnemy ")]

    public class CI_HS_WillGoingTowardsTPShowMyBackToEnemy : ConsiderationInput
    {
        // uses consideration min & max - needs range type
        // uses targetTPoint

        public override float GetConsiderationInput(DecisionContext decisionContext, Consideration consideration)
        {
            //TODO


            AIController_Blackboard blackboard = ((AIController_HumanoidSoldier)decisionContext.aiController).blackboard;

            //if thwe if the distance is smaller than 1m, just ignore it
            if (decisionContext.targetTacticalPoint.lastDistanceMeasured > 1)
            {
                //calculate angle between idrection towards tp point and mean threat direction
                Vector3 directionTowardsTPoint = decisionContext.targetTacticalPoint.tacticalPoint.GetPointPosition() - blackboard.GetMyEntity().transform.position;
                directionTowardsTPoint.y = 0;
                Vector3 meanThreatDirection = blackboard.meanThreatDirection;
                meanThreatDirection.y = 0;

                float remappedAngle = Utility.Remap(Vector3.Angle(directionTowardsTPoint, meanThreatDirection), consideration.min, consideration.max, 0, 1);
                return Mathf.Clamp(remappedAngle, 0, 1);
            }
            else
            {
                return 0;
            }
        }
    }
}
