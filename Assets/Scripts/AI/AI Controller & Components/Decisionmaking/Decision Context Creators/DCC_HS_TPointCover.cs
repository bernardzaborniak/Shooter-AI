using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BenitosAI
{
    [CreateAssetMenu(menuName = "AI/Decision Context Creator/HumanoidSolder_TacticalPointCover", fileName = "HS_TacticalPointCover")]

    public class DCC_HS_TPointCover : DecisionContextCreator
    {
        public int maxTacticalPointTargetsPerDecision = 10;

        Queue<DecisionContext> contexesPool = new Queue<DecisionContext>();
        DecisionContext[] contexesToReturn;
        private void OnEnable()
        {
            for (int i = 0; i < maxTacticalPointTargetsPerDecision; i++)
            {
                contexesPool.Enqueue(new DecisionContext());
            }
        }

        public override DecisionContext[] GetDecisionContexes(Decision decision, AIController aiController)
        {
            AIController_Blackboard blackboard = ((AIController_HumanoidSoldier)aiController).blackboard;

            int coverPointsCountCount = blackboard.tPCoverInfos.Length;
            if (coverPointsCountCount > maxTacticalPointTargetsPerDecision)
            {
                coverPointsCountCount = maxTacticalPointTargetsPerDecision;
            }
            contexesToReturn = new DecisionContext[coverPointsCountCount];

            for (int i = 0; i < coverPointsCountCount; i++)
            {
                contexesToReturn[i] = contexesPool.Dequeue();
                contexesToReturn[i].SetUpContext(decision, aiController, null, blackboard.tPCoverInfos[i]);
            }

            //return them back to the pool
            for (int i = 0; i < coverPointsCountCount; i++)
            {
                contexesPool.Enqueue(contexesToReturn[i]);
            }



            return contexesToReturn;
        }
    }
}
