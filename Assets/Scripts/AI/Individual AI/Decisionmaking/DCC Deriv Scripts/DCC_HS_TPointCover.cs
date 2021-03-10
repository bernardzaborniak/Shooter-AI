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

        public override DecisionContext[] GetDecisionContexts(Decision decision, AIController aiController)
        {
            AIController_Blackboard blackboard = ((AIController_HumanoidSoldier)aiController).blackboard;

            int coverPointsCount = blackboard.tPCoverInfos.Length;
            if (coverPointsCount > maxTacticalPointTargetsPerDecision)
            {
                coverPointsCount = maxTacticalPointTargetsPerDecision;
            }
            contexesToReturn = new DecisionContext[coverPointsCount];

            for (int i = 0; i < coverPointsCount; i++)
            {
                contexesToReturn[i] = contexesPool.Dequeue();
                contexesToReturn[i].SetUpContext(decision, aiController, blackboard.tPCoverInfos[i]);
            }

            //return them back to the pool
            for (int i = 0; i < coverPointsCount; i++)
            {
                contexesPool.Enqueue(contexesToReturn[i]);
            }

            return contexesToReturn;
        }
    }
}
