using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace BenitosAI
{

    [CreateAssetMenu(menuName = "AI/Consideration Input/Humanoid/Will Going Towards TP Show My Back To Enemy [DCC_TPoint] ", fileName = "Will Going Towards TP Show My Back To Enemy [DCC_TPoint]")]

    public class CI_HS_WillGoingTowardsTPShowMyBackToEnemy_DCCTPoint : ConsiderationInput
    {
        void OnEnable()
        {
            inputParamsType = new ConsiderationInputParams.InputParamsType[]
            {
                ConsiderationInputParams.InputParamsType.Range
            };
        }

        public override float GetConsiderationInput(DecisionContext decisionContext, ConsiderationInputParams considerationInputParams)
        {
            //if thwe if the distance is smaller than 1m, just ignore it
            if (((System.ValueTuple<TacticalPoint, float>)decisionContext.target).Item2 < 1) return 0;

            AIController_Blackboard blackboard = ((AIController_HumanoidSoldier)decisionContext.aiController).blackboard;


            //calculate angle between idrection towards tp point and mean threat direction
            Vector3 directionTowardsTPoint = ((System.ValueTuple<TacticalPoint, float>)decisionContext.target).Item1.GetPointPosition() - blackboard.GetMyEntity().transform.position;
            directionTowardsTPoint.y = 0;
            Vector3 meanThreatDirection = blackboard.meanThreatDirection;
            meanThreatDirection.y = 0;

            return Utility.Remap(Vector3.Angle(directionTowardsTPoint, meanThreatDirection), considerationInputParams.min, considerationInputParams.max, 0, 1, true);
            //float remappedAngle = Utility.Remap(Vector3.Angle(directionTowardsTPoint, meanThreatDirection), consideration.min, consideration.max, 0, 1, true);
            //return Mathf.Clamp(remappedAngle, 0, 1);
        }
    }
}
