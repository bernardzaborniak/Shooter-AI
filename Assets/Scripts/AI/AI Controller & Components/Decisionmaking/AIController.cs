using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIController : EntityComponent
{
   // protected GameEntity entityAttachedTo;
    //public AIComponent[] aIComponents;

    public DecisionMaker[] decisionLayers;

    public float decisionInterval;
    float nextDecisionTime;

    public override void SetUpComponent(GameEntity entity)
    {
        //entityAttachedTo = entity;
        base.SetUpComponent(entity);

        nextDecisionTime = Time.time + Random.Range(0, nextDecisionTime);

        for (int i = 0; i < decisionLayers.Length; i++)
        {
            decisionLayers[i].SetUpDecisionLayer(this);
        }
        
    }

    public override void UpdateComponent()
    {
        // Decide every x seconds
        if(Time.time> nextDecisionTime)
        {
            nextDecisionTime = Time.time + decisionInterval;

            for (int i = 0; i < decisionLayers.Length; i++)
            {
                decisionLayers[i].Decide();
            }
        }

        // Update Current States
        for (int i = 0; i < decisionLayers.Length; i++)
        {
            decisionLayers[i].UpdateCurrentState();
        }

    }
}
