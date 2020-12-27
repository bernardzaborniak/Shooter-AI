using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BenitosAI
{
    [System.Serializable]
    public class DecisionWrapper
    {
        public Decision decision;
        [Min(0)]
        public float weigt;
    }

    [System.Serializable]
    // Holds an array of all possible decisions, rates them and decides which to execute.
    // Holds an internal statemachine concerning current states executing
    public class DecisionMaker
    {
        public string name = "new Layer";

        public enum DecisionMethod
        {
            BestRated,
            RandomOutOf3BestRated
        }

        public DecisionMethod decisionMethod;
        public float[] decisionRatings; //make it private later

        public DecisionWrapper[] decisions;

        Decision currentDecision;
        AIState currentState;
        AIController aiController;

        //[Header("Debug")]
        //public float decisionRatingsDebug;

      
        // Decision Context Pool
        //Queue<DecisionContext> decisionContextPool = new Queue<DecisionContext>();

        HashSet<DecisionContext> currentDecisionContexes = new HashSet<DecisionContext>();

        public void SetUpDecisionLayer(AIController aiController)
        {

            this.aiController = aiController;

            /* for (int i = 0; i < decisions.Length; i++)
             {
                 decisions[i].decision.SetUpDecision(aiController);
             }*/

            decisionRatings = new float[decisions.Length];

            // Decide the pool length;
            /*int decisionContextPoolLength = 0;
            Decision.DecisionContextTargetType targetType;

            for (int i = 0; i < decisions.Length; i++)
            {
                targetType = decisions[i].decision.decisionContextTargetType;

                if (targetType == Decision.DecisionContextTargetType.Self)
                {
                    decisionContextPoolLength += 1;
                }
                else if(targetType == Decision.DecisionContextTargetType.Entity)
                {
                    decisionContextPoolLength += maxEntityTargetsPerDecision;
                }
                else if (targetType == Decision.DecisionContextTargetType.TacticalPoint)
                {
                    decisionContextPoolLength += maxTacticalPointTargetsPerDecision;
                }
            }

            for (int i = 0; i < decisionContextPoolLength; i++)
            {
                decisionContextPool.Enqueue(new DecisionContext());
            }*/

        }

        public void Decide() //Decide();
        {
            //1 set up the contextes, rate them

            //2 decide which one to execute based on rating


            //Debug.Log("Decide ");
            //scores all decisions, select the best one, and create new state if this decision is different than the previous one
            float currentRating;
            int bestRatedDecision = int.MaxValue;
            float bestRatingSoFar = 0;

            for (int i = 0; i < decisions.Length; i++)
            {
                //Debug.Log("-----------------  Decide decition number: " + i);
                DecisionContext context = new DecisionContext();
                context.SetUpContext(decisions[i].decision, aiController, null, null);
                currentRating = decisions[i].decision.GetDecisionRating(context) * decisions[i].weigt;
               // Debug.Log("current Rating: " + currentRating);
                decisionRatings[i] = currentRating;

                if (currentRating > bestRatingSoFar)
                {
                    bestRatingSoFar = currentRating;
                    bestRatedDecision = i;
                }
            }

            if (bestRatedDecision != int.MaxValue)
            {
                StartExecutingDecision(decisions[bestRatedDecision].decision);
            }

        }

        public void StartExecutingDecision(Decision decision)
        {
            if (decision != currentDecision)
            {
                currentDecision = decision;

                if (currentState != null)
                {
                    currentState.OnStateExit();
                }

                if (currentDecision.correspondingAIState == AIStateEnum.TestState)
                {
                    currentState = new AIst_HumSol_MovingToZeroPoint();
                    currentState.OnStateEnter();
                }
            }




        }

        //also called when adding decisions dynamically 
        /*  void AddDecision(Decision decision)
          {
              //transform the array into a list? or leave this dynamicall adding out for now? - yeah leave it out for now
              decision.SetUpDecision(aiController);
          }*/

        /* void UpdateDecision()
         {
             //scores all decisions, select the best one, and create new state if this decision is different than the previous one
         }*/

        //before rating & decising the decisions have to be set up according to current sensing, if there are more targets for one decision, the decision has to be seperated into several ones

        public void UpdateCurrentState()
        {
            //updates current state
            if (currentState != null)
            {
                currentState.UpdateState();
            }
        }



        //all decisions get a deference to the decision layer they cre called from ,so they can call set state on it
        // every decision has its state in code? - no all states should be declared inside the decision layer? or inside the individual decisions would be better



    }
}
