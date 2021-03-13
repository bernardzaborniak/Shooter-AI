using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BenitosAI //maybe instead having the ai controller be in benitos namespace, just use decision context classes?
{

    public class AIController : EntityComponent
    {
        // protected GameEntity entityAttachedTo;
        //public AIComponent[] aIComponents;
        [Tooltip("the current action tags are being set by the states which are currently executed")]
        public EntityTags entityTags;
        public DecisionMaker[] decisionLayers;
        [Space(10)]
        //public float decisionInterval;
        //float nextDecisionTime;

        [Header("Optimisation")]
        public AIControllerOptimiser optimiser;

        public override void SetUpComponent(GameEntity entity)
        {
            //entityAttachedTo = entity;
            base.SetUpComponent(entity);



            //nextDecisionTime = Time.time + Random.Range(0, nextDecisionTime);

            for (int i = 0; i < decisionLayers.Length; i++)
            {
                decisionLayers[i].SetUpDecisionLayer(this);
            }

        }

        public override void UpdateComponent()
        {
            if (optimiser.ShouldAIControllerBeUpdated())
            {
                optimiser.OnAIControllerWasUpdated();

                UpdateDecisionMakers();
            }

            for (int i = 0; i < decisionLayers.Length; i++)
            {
                decisionLayers[i].UpdateCurrentState();
            }
        }

        protected virtual void UpdateDecisionMakers()
        {
            for (int i = 0; i < decisionLayers.Length; i++)
            {
                decisionLayers[i].Decide();
            }
        }

        /* public GameEntity GetEntity()
         {
             return myEntity;
         }*/
        public override void OnDie(ref DamageInfo damageInfo)
        {
            for (int i = 0; i < decisionLayers.Length; i++)
            {
                decisionLayers[i].AbortCurrentDecision();
            }
        }

        public void AddDecision(int decisionLayer, Decision newDecision)
        {
            decisionLayers[decisionLayer].AddDecision(newDecision);
        }

        public void RemoveDecision(int decisionLayer, Decision decisionToRemove)
        {
            decisionLayers[decisionLayer].RemoveDecision(decisionToRemove);
        }
    }

}
