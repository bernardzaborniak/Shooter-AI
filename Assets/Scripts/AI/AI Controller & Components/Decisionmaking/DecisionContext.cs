using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BenitosAI
{
    // Package of Information concerning a decision
    [System.Serializable] //Only for Debug purposes
    public class DecisionContext
    {
        public float rating;

        // Mandatory
        public Decision decision; //what are we trying to do?
        public AIController aiController; //who s asking?

        // Optional
        public SensedEntityInfo targetEntity; //Who is the target of my action
        public SensedTacticalPointInfo targetTacticalPoint; //Who is the target of my action
    
        public void SetUpContext(Decision decision, AIController aiController, SensedEntityInfo targetObject, SensedTacticalPointInfo targetTacticalPoint)
        {
            this.decision = decision;
            this.aiController = aiController;
            this.targetEntity = targetObject;
            this.targetTacticalPoint = targetTacticalPoint;
        }
    }
}


