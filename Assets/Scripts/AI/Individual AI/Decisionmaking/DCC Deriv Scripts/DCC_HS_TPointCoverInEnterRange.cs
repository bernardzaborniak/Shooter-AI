using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BenitosAI
{
    [CreateAssetMenu(menuName = "AI/Decision Context Creator/HumanoidSolder_TPointCoverInEnterRange", fileName = "HS_TPointCoverInEnterRange")]
    public class DCC_HS_TPointCoverInEnterRange : DecisionContextCreator
    {
       /* public float enterCoverPointRange = 0.5f;
        public int maxTacticalPointTargetsPerDecision = 3;

        Queue<DecisionContext> contexesPool = new Queue<DecisionContext>();
        HashSet<DecisionContext> contexesToReturnSet = new HashSet<DecisionContext>();
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
            contexesToReturnSet.Clear();
            //SensingInfo sensingInfo = ((AIController_HumanoidSoldier)aiController).humanSensing.sensingInfo;
            SensedTacticalPointInfo[] coverPointInfos = ((AIController_HumanoidSoldier)aiController).humanSensing.sensingInfo.tPointCoverInfos;

            for (int i = 0; i < coverPointInfos.Length; i++)
            {
                if(coverPointInfos[i].lastDistanceMeasured < 0.5f)
                {
                    if (contexesPool.Count > 0)
                    {
                        DecisionContext context = contexesPool.Dequeue();
                        context.SetUpContext(decision, aiController, null, coverPointInfos[i]);
                        contexesToReturnSet.Add(context);

                    }
                }
                //contexesToReturn[i] = 
                //contexesToReturn[i].SetUpContext(decision, aiController, null, sensingInfo.tPointCoverInfos[i]);
            }

            //return them back to the pool
            for (int i = 0; i < coverPointsCountCount; i++)
            {
                contexesPool.Enqueue(contexesToReturn[i]);
            }



            return contexesToReturn;
        }*/
    }
}
