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
        public AI_SI_EntityVisibilityInfo targetObject; //Who is the target of my action
        public AI_SI_TacticalPointVisibilityInfo targetTacticalPoint; //Who is the target of my action
    
        public void SetUpContect(Decision decision, AIController aiController, AI_SI_EntityVisibilityInfo targetObject, AI_SI_TacticalPointVisibilityInfo targetTacticalPoint)
        {
            this.decision = decision;
            this.aiController = aiController;
            this.targetObject = targetObject;
            this.targetTacticalPoint = targetTacticalPoint;
        }
    }
}


