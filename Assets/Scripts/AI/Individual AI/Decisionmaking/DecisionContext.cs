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
        public (TacticalPoint tPoint, float distance) targetTacticalPoint; //Who is the target of my action

        public DecisionContext()
        {

        }

        public DecisionContext(DecisionContext objectToCopyValuesFrom)
        {
            SetUpContext(objectToCopyValuesFrom.decision, objectToCopyValuesFrom.aiController, objectToCopyValuesFrom.targetEntity, objectToCopyValuesFrom.targetTacticalPoint);
            rating = objectToCopyValuesFrom.rating;
        }

        public void SetUpContext(DecisionContext objectToCopyValuesFrom)
        {
            this.decision = objectToCopyValuesFrom.decision;
            this.aiController = objectToCopyValuesFrom.aiController;
            this.targetEntity = objectToCopyValuesFrom.targetEntity;
            this.targetTacticalPoint = objectToCopyValuesFrom.targetTacticalPoint;

            this.rating = objectToCopyValuesFrom.rating;
        }

        public void SetUpContext(Decision decision, AIController aiController, SensedEntityInfo targetEntity, (TacticalPoint tPoint, float distance) targetTacticalPoint)
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

            if (decision == otherContext.decision)
            {
                if (targetEntity == otherContext.targetEntity)
                {
                    if (targetTacticalPoint == otherContext.targetTacticalPoint)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public bool ContextIsTheSameAs(DecisionMaker.Memory.DecisionContextMemory contextMemory)
        {
            if (contextMemory == null) return false;

            if (decision == contextMemory.decision)
            {
                try
                {
                    if (targetEntity.entity == contextMemory.targetEntity)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                } catch (System.Exception e) { }
                try
                {
                    if (targetTacticalPoint.tPoint == contextMemory.targetTacticalPoint)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                } catch (System.Exception e) { }

                return true;
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


