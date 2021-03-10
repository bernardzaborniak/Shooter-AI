using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BenitosAI
{
    [CreateAssetMenu(menuName = "AI/Decision Context Creator/HumanoidSolder_TPointCoverPeek", fileName = "HS_TPointCoverPeek")]

    public class DCC_HS_TPointCoverPeek : DecisionContextCreator
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

        public override DecisionContext[] GetDecisionContexts(Decision decision, AIController aiController)
        {
            AIController_Blackboard blackboard = ((AIController_HumanoidSoldier)aiController).blackboard;

            int coverPeekPointsCountCount = blackboard.tPCoverPeekInfos.Length;
            if (coverPeekPointsCountCount > maxTacticalPointTargetsPerDecision)
            {
                coverPeekPointsCountCount = maxTacticalPointTargetsPerDecision;
            }
            contexesToReturn = new DecisionContext[coverPeekPointsCountCount];

            for (int i = 0; i < coverPeekPointsCountCount; i++)
            {
                contexesToReturn[i] = contexesPool.Dequeue();
                contexesToReturn[i].SetUpContext(decision, aiController, blackboard.tPCoverPeekInfos[i]);
            }

            //return them back to the pool
            for (int i = 0; i < coverPeekPointsCountCount; i++)
            {
                contexesPool.Enqueue(contexesToReturn[i]);
            }

            return contexesToReturn;
        }
    }
}

