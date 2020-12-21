using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BenitosAI
{
    // Package of Information concerning a decision
    public class DecisionContext
    {
        // Mandatory
        public Decision decision; //what are we trying to do?
        public AIController aiController; //who s asking?

        // Optional
        public SensedEntityInfo targetObject; //Who is the target of my action
        public SensedTacticalPointInfo targetTacticalPoint; //Who is the target of my action
    
        public void SetUpContect(Decision decision, AIController aiController, SensedEntityInfo targetObject, SensedTacticalPointInfo targetTacticalPoint)
        {
            this.decision = decision;
            this.aiController = aiController;
            this.targetObject = targetObject;
            this.targetTacticalPoint = targetTacticalPoint;
        }
    }
}


