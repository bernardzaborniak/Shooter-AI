using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BenitosAI
{
    [CreateAssetMenu(menuName = "AI/Decision Context Creator/HumanoidSolder_TPointCoverPeek", fileName = "HS_TPointCoverPeek")]

    public class DCC_HS_TPointCoverPeek : DecisionContextCreator
    {
        public int maxTacticalPointTargetsPerDecision = 4;
        Queue<DecisionContext> contexesPool = new Queue<DecisionContext>();

        DecisionContext[] contexesToReturn;
        SensedTacticalPointInfo[] coverShootPoints;

        private void OnEnable()
        {
            for (int i = 0; i < maxTacticalPointTargetsPerDecision; i++)
            {
                contexesPool.Enqueue(new DecisionContext());
            }
        }

        public override DecisionContext[] GetDecisionContexts(Decision decision, AIController aiController)
        {
            coverShootPoints = ((AIController_HumanoidSoldier)aiController).blackboard.tPCoverPeekInfos;

            contexesToReturn = new DecisionContext[coverShootPoints.Length];

            for (int i = 0; i < contexesToReturn.Length; i++)
            {
                contexesToReturn[i] = contexesPool.Dequeue();
                contexesToReturn[i].SetUpContext(decision, aiController, null,  coverShootPoints[i]);
            }

            //return them back to the pool
            for (int i = 0; i < contexesToReturn.Length; i++)
            {
                contexesPool.Enqueue(contexesToReturn[i]);
            }



            return contexesToReturn;
        }
    }
}
