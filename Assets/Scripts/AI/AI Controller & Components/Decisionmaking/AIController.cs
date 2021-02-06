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
        [Tooltip("[optional]if memory is assigned, previous decisions will be saved inside of it -> only for debugging cause of performance impact")]
        public DecisionMakerMemory memory;
        public DecisionMaker[] decisionLayers;
        [Space(10)]
        public float decisionInterval;
        float nextDecisionTime;

        public override void SetUpComponent(GameEntity entity)
        {
            //entityAttachedTo = entity;
            base.SetUpComponent(entity);



            nextDecisionTime = Time.time + Random.Range(0, nextDecisionTime);

            for (int i = 0; i < decisionLayers.Length; i++)
            {
                decisionLayers[i].SetUpDecisionLayer(this, memory, i);
            }

        }

        public override void UpdateComponent()
        {
            // Update Current States
            for (int i = 0; i < decisionLayers.Length; i++)
            {
                decisionLayers[i].UpdateCurrentState();
            }

            // Decide every x seconds
            if (Time.time > nextDecisionTime)
            {
                //Debug.Log(" ----------------------------------------------  Decisionmaker.Decide() " + myEntity.GetHashCode());

                nextDecisionTime = Time.time + decisionInterval;

                for (int i = 0; i < decisionLayers.Length; i++)
                {
                    decisionLayers[i].Decide();
                }
            }
        }

       /* public GameEntity GetEntity()
        {
            return myEntity;
        }*/
    }

}
