using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BenitosAI
{
    public class DCC_HS_TacticalPointCover : DecisionContextCreator
    {
        public int maxTacticalPointTargetsPerDecision = 10;

        Queue<DecisionContext> contexesPool = new Queue<DecisionContext>();
        DecisionContext[] contexesToReturn;
        private void Awake()
        {
            for (int i = 0; i < maxTacticalPointTargetsPerDecision; i++)
            {
                contexesPool.Enqueue(new DecisionContext());
            }
        }

        public override DecisionContext[] GetDecisionContexes(Decision decision, AIController aiController)
        {
            SensingInfo sensingInfo = ((AIController_HumanoidSoldier)aiController).humanSensing.sensingInfo;

            int coverpointsCountCount = sensingInfo.tPointCoverInfos.Length;
            if (coverpointsCountCount > maxTacticalPointTargetsPerDecision)
            {
                coverpointsCountCount = maxTacticalPointTargetsPerDecision;
            }
            contexesToReturn = new DecisionContext[coverpointsCountCount];

            for (int i = 0; i < coverpointsCountCount; i++)
            {
                contexesToReturn[i] = contexesPool.Dequeue();
                contexesToReturn[i].SetUpContext(decision, aiController, null, sensingInfo.tPointCoverInfos[i]);
            }

            //return them back to the pool
            for (int i = 0; i < coverpointsCountCount; i++)
            {
                contexesPool.Enqueue(contexesToReturn[i]);
            }



            return contexesToReturn;
        }
    }
}
