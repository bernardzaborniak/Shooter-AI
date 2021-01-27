using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BenitosAI
{
    

    [System.Serializable]
    public class DecisionContextVisualiser        //as decision context objects are reused, this visualiser shows which context was use for this specific instance
    {
        [HideInInspector]
        public string name;
        public float rating;

        public Decision decision; //what are we trying to do?
        public AIController aiController; //who s asking?

        public SensedEntityInfo targetEntity; //Who is the target of my action
        public SensedTacticalPointInfo targetTacticalPoint; //Who is the target of my action

        [System.Serializable]
        public class ConsiderationVisualiser
        {
            [SerializeField] string considerationName;
            [SerializeField] float input;
            [SerializeField] float rating;

            public ConsiderationVisualiser(string considerationName, float input, float rating)
            {
                this.considerationName = considerationName;
                this.input = input;
                this.rating = rating;
            }
        }

        [SerializeField] ConsiderationVisualiser[] considerationVisualisers;

        public DecisionContextVisualiser(DecisionContext context)
        {
            this.name = context.decision.name;
            this.decision = context.decision;
            this.aiController = context.aiController;
            this.targetEntity = context.targetEntity;
            this.targetTacticalPoint = context.targetTacticalPoint;
            rating = context.rating;

            Consideration[] considerations;
            considerations = context.decision.GetConsiderations();
            considerationVisualisers = new ConsiderationVisualiser[considerations.Length];

            for (int i = 0; i < considerationVisualisers.Length; i++)
            {
                considerationVisualisers[i] = new ConsiderationVisualiser(considerations[i].name, considerations[i].GetConsiderationInput(context), considerations[i].GetConsiderationRating(context));
            }
        }
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
       
        [Space(5)]
        public List<DecisionContext> currentDecisionContexts = new List<DecisionContext>();
        [Space(10)]
        public List<DecisionContextVisualiser> currentDecisionContextsVisualisation = new List<DecisionContextVisualiser>();
        [Space(5)]
        public DecisionContextVisualiser lastSelectedDecisionContext;


        public void SetUpDecisionLayer(AIController aiController)
        {
            this.aiController = aiController;
        }

        public void Decide() //Decide();
        {
            currentDecisionContextsVisualisation.Clear();

            //scores all decisions, select the best one, and create new state if this decision is different than the previous one
            float currentRating = 0;
            float bestRatingSoFar = 0;
            DecisionContext bestRatedDecisionContext = null;
            //Debug.Log("Decide========================================================================- ");

            for (int i = 0; i < decisions.Length; i++)
            {
               // Debug.Log("Current Decision: " + decisions[i].decision.name + "--------------------]");
                DecisionContext[] decisionContexesToAdd = decisions[i].decision.GetRatedDecisionContexts(aiController);
                //Debug.Log("decision ocntext size in decision mker: " + decisionContexesToAdd.Length);
                for (int j = 0; j < decisionContexesToAdd.Length; j++)
                {
                    //add weight
                    decisionContexesToAdd[j].rating *= decisions[i].weigt;
                    //add contexes to all current contexes
                    currentDecisionContexts.Add(decisionContexesToAdd[j]);
                    currentDecisionContextsVisualisation.Add(new DecisionContextVisualiser(decisionContexesToAdd[j]));

                    currentRating = decisionContexesToAdd[j].rating;
                    //Debug.Log("current Decision Context: " + decisionContexesToAdd[j].decision + " rating: " + currentRating);
                    if (currentRating > bestRatingSoFar)
                    {
                       // Debug.Log("rating was higher than best rating so far, which was: " + bestRatingSoFar);

                        bestRatingSoFar = currentRating;
                        //bestRatedDecisionContext = decisionContexesToAdd[j];
                        //the Decision context needs to be copied, as it is a reference to an object from a pool which can change during runtime
                        bestRatedDecisionContext = new DecisionContext(decisionContexesToAdd[j]);
                        //Debug.Log("-> set new - best rated Decision context: " + bestRatedDecisionContext.decision);
                    }
                }
               // Debug.Log("Current Decision END: " + decisions[i].decision.name + "--------------------]");
            }
            //Debug.Log("best rated Decision context: " + bestRatedDecisionContext.decision);

            if(bestRatedDecisionContext != null)
            {
                StartExecutingDecision(bestRatedDecisionContext);
            }

            currentDecisionContexts.Clear();
           // Debug.Log("Decide END========================================================================- ");

        }

        public void StartExecutingDecision(DecisionContext decisionContext)
        {
            // the check here needs to be different - how do we check if a decision context is the same? -> check if the assigned decsision and targets are all the same? -
            // or check if their states are the same - the states are initialised by decisions at runtime or at start?- how are the states parameters set?

            //TODO Rethink

            //if (decisionContext != currentDecidedDecisionContext)
            if (!decisionContext.ContextIsTheSameAs(currentDecidedDecisionContext))
            {
                currentDecidedDecisionContext = decisionContext;
                lastSelectedDecisionContext = new DecisionContextVisualiser(currentDecidedDecisionContext);

                if (currentState != null)
                {
                    currentState.OnStateExit();
                    aiController.entityTags.RemoveEntityActionTags(currentState.GetActionTagsToRemoveOnStateExit());
                }

                currentState = currentDecidedDecisionContext.decision.CreateState(aiController, currentDecidedDecisionContext);
                currentState.OnStateEnter();
                aiController.entityTags.AddEntityActionTags(currentState.GetActionTagsToAddOnStateEnter());

                /*if (currentDecidedDecisionContext.decision.correspondingAIState == AIStateEnum.TestState)
                {
                    currentState = new St_HS_MovingToZeroPoint();
                    currentState.OnStateEnter();
                }*/
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
