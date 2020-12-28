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

        public DecisionWrapper[] decisions;

        DecisionContext currentDecidedDecisionContext;
        AIState currentState;
        AIController aiController;

        public List<DecisionContext> currentDecisionContexts = new List<DecisionContext>();


        public void SetUpDecisionLayer(AIController aiController)
        {
            this.aiController = aiController;
        }

        public void Decide() //Decide();
        {
            currentDecisionContexts.Clear();

            //scores all decisions, select the best one, and create new state if this decision is different than the previous one
            float currentRating = 0;
            float bestRatingSoFar = 0;
            DecisionContext bestRatedDecisionContext = null;

            for (int i = 0; i < decisions.Length; i++)
            {
                DecisionContext[] decisionContexesToAdd = decisions[i].decision.GetDecisionRating(aiController);
                for (int j = 0; j < decisionContexesToAdd.Length; j++)
                {
                    //add weight
                    decisionContexesToAdd[j].rating *= decisions[i].weigt;
                    //add contexes to all current contexes
                    currentDecisionContexts.Add(decisionContexesToAdd[j]);

                    currentRating = decisionContexesToAdd[j].rating;
                    if (currentRating > bestRatingSoFar)
                    {
                        bestRatingSoFar = currentRating;
                        bestRatedDecisionContext = decisionContexesToAdd[j];
                    }
                }
            }

            if(bestRatedDecisionContext != null)
            {
                StartExecutingDecision(bestRatedDecisionContext);
            }
        }

        public void StartExecutingDecision(DecisionContext decisionContext)
        {
            if (decisionContext != currentDecidedDecisionContext)
            {
                currentDecidedDecisionContext = decisionContext;

                if (currentState != null)
                {
                    currentState.OnStateExit();
                }

                if (currentDecidedDecisionContext.decision.correspondingAIState == AIStateEnum.TestState)
                {
                    currentState = new AIst_HumSol_MovingToZeroPoint();
                    currentState.OnStateEnter();
                }
            }
        }

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
