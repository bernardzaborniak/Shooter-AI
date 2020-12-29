using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace BenitosAI
{
    // Package of Information concerning a decision
   // [System.Serializable] //Only for Debug purposes
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

        // this could be more elegantly solved by making the hashcode dependant on this variables? - but then i would have problems weh saving this objects in a hashSet? or overide the == operator?
        public bool ContextIsTheSameAs(DecisionContext otherContext)
        {
            if(decision == otherContext.decision)
            {
                if(targetEntity == otherContext.targetEntity)
                {
                    if(targetTacticalPoint == otherContext.targetTacticalPoint)
                    {
                        return true;
                    }
                }
            }

            return false;
        }
    }
}


