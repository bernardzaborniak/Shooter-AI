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
        public object target; //same as System.Object

        public DecisionContext()
        {
            
        }

        public DecisionContext(DecisionContext objectToCopyValuesFrom)
        {
            SetUpContext(objectToCopyValuesFrom.decision, objectToCopyValuesFrom.aiController, objectToCopyValuesFrom.target);
            rating = objectToCopyValuesFrom.rating;

        }

        public bool IsContextValid()
        {
            return (decision != null && aiController != null);
        }

        public void SetUpContext(DecisionContext objectToCopyValuesFrom)
        {
            this.decision = objectToCopyValuesFrom.decision;
            this.aiController = objectToCopyValuesFrom.aiController;

            this.target = objectToCopyValuesFrom.target;

            this.rating = objectToCopyValuesFrom.rating;
        }

        public void SetUpContext(Decision decision, AIController aiController, object target)
        {
            this.decision = decision;
            this.aiController = aiController;

            this.target = target;
        }

        // this could be more elegantly solved by making the hashcode dependant on this variables? - but then i would have problems weh saving this objects in a hashSet? or overide the == operator?
        public bool ContextIsTheSameAs(DecisionContext otherContext)
        {
            if (otherContext == null) return false;

            if (decision == otherContext.decision)
            {
                if (target is System.ValueTuple<TacticalPoint, float>)
                {
                    if (((System.ValueTuple<TacticalPoint, float>)target).Item1 == ((System.ValueTuple<TacticalPoint, float>)otherContext.target).Item1) return true;
                }
                else
                {
                    if (target == otherContext.target) return true;
                }
            }

            return false;
        }

        public bool ContextIsTheSameAs(DecisionMaker.Memory.DecisionContextMemory contextMemory)
        {
            if (contextMemory == null) return false;

            if (decision == contextMemory.decision)
            {
                if(target is System.ValueTuple<TacticalPoint, float>)
                {
                    if (((System.ValueTuple<TacticalPoint, float>)target).Item1 == ((System.ValueTuple<TacticalPoint, float>)contextMemory.target).Item1) return true;
                }
                else
                {
                    if (target == contextMemory.target) return true;
                }

                /*if (target == contextMemory.target)
                {
                    Debug.Log(target + " is the same as " + contextMemory.target);
                    return true;
                }
                else
                {
                    Debug.Log(target + " is NOT the same as " + contextMemory.target);
                }*/
            }

            return false;
        }


        public void RateContext(Consideration[] considerations, float weight, float discardThreshold)
        {
            float score = weight;

            for (int c = 0; c < considerations.Length; c++)
            {
                score *= considerations[c].GetConsiderationRating(this);


                if (score < discardThreshold)
                {
                    score = -1;
                    rating = score;
                    return;
                }
            }

            //Add makeup Value / Compensation Factor - as you multiply normalized values, teh total drops - if we dont do this more considerations will result in a lower weight - according to Mark Dave and a tipp from Ben Sizer
            score /= weight;    
            score += score * ((1 - score) * (1 - (1 / considerations.Length)));
            score *= weight;

            rating = score;
        }
    }
}


