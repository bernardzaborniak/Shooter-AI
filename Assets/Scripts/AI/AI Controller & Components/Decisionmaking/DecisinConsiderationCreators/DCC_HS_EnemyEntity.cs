﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BenitosAI
{
    [CreateAssetMenu(menuName = "AI/Decision Context Creator/HumanoidSolder_EnemyEntity", fileName = "HS_EnemyEntity")]
    public class DCC_HS_EnemyEntity : DecisionContextCreator
    {
        public int maxEntityTargetsPerDecision = 5;

        Queue<DecisionContext> contextsPool = new Queue<DecisionContext>();
        DecisionContext[] contextsToReturn;
        private void OnEnable()
        {
            for (int i = 0; i < maxEntityTargetsPerDecision; i++)
            {
                contextsPool.Enqueue(new DecisionContext());
            }   
        }

        public override DecisionContext[] GetDecisionContexes(Decision decision, AIController aiController)
        {
            SensingInfo sensingInfo = ((AIController_HumanoidSoldier)aiController).humanSensing.sensingInfo;

            int enemyEntitiesCount = sensingInfo.enemyInfos.Length;
            if (enemyEntitiesCount > maxEntityTargetsPerDecision)
            {
                enemyEntitiesCount = maxEntityTargetsPerDecision;
            }
            contextsToReturn = new DecisionContext[enemyEntitiesCount];

            for (int i = 0; i < enemyEntitiesCount; i++)
            {
                contextsToReturn[i] = contextsPool.Dequeue();
                contextsToReturn[i].SetUpContext(decision, aiController, sensingInfo.enemyInfos[i], null);
            }

            //return them back to the pool
            for (int i = 0; i < enemyEntitiesCount; i++)
            {
                contextsPool.Enqueue(contextsToReturn[i]);
            }
            


            return contextsToReturn;
        }
    }
}
