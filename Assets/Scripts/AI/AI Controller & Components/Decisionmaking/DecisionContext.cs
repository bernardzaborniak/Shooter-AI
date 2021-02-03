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
    
        public DecisionContext()
        {

        }

        public DecisionContext(DecisionContext objectToCopyValuesFrom)
        {
            SetUpContext(objectToCopyValuesFrom.decision, objectToCopyValuesFrom.aiController, objectToCopyValuesFrom.targetEntity, objectToCopyValuesFrom.targetTacticalPoint);
            rating = objectToCopyValuesFrom.rating;
        }

        public void SetUpContext(Decision decision, AIController aiController, SensedEntityInfo targetEntity, SensedTacticalPointInfo targetTacticalPoint)
        {
            this.decision = decision;
            this.aiController = aiController;
            this.targetEntity = targetEntity;
            this.targetTacticalPoint = targetTacticalPoint;
        }

        // this could be more elegantly solved by making the hashcode dependant on this variables? - but then i would have problems weh saving this objects in a hashSet? or overide the == operator?
        public bool ContextIsTheSameAs(DecisionContext otherContext)
        {
            if (otherContext == null) return false;

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

        public void RateContext(Consideration[] considerations, Decision.BonusConsiderationWrapper[] bonusConsiderations, float weight, float discardThreshold)
        {
            //DecisionContext[] contexts = decisionContextCreator.GetDecisionContexes(this, aiController);

            // Score each context
            //for (int i = 0; i < contexts.Length; i++)
            //{
                //make a seperate method for rating contexes

                //float score = 1;
                float score = weight;

                for (int c = 0; c < considerations.Length; c++)
                {
                    //Debug.Log("new consideration being rated -----------------------------------" + considerations[c].name);


                    //Debug.Log("score: " + score + " *= " + considerations[c].GetConsiderationRating(contexts[i]));
                    score *= considerations[c].GetConsiderationRating(this);
                    //Debug.Log("score: " + score);
                    if (score < discardThreshold)
                    {
                    //Debug.Log("Discard decision Context");
                        score = -1;
                        rating = score;
                        return ;
                    }

                }

                for (int c = 0; c < bonusConsiderations.Length; c++)
                {
                    score += bonusConsiderations[c].consideration.GetConsiderationRating(this) * bonusConsiderations[c].weight;
                }

                //Debug.Log("score before makeup: " + score);
                //Add makeup Value / Compensation Factor - as you multiply normalized values, teh total drops - if we dont do this more considerations will result in a lower weight - according to Mark Dave and a tipp from Ben Sizer
                score += score * ((1 - score) * (1 - (1 / considerations.Length)));
                //Debug.Log("score after makeup: " + score);

                rating = score;

//}


           // return contexts;
        }
    }
}


