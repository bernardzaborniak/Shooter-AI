using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BenitosAI
{
    
    //creates a lot of garbage?
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
            this.targetEntity = context.targetEntity;//new SensedEntityInfo(context.targetEntity);  
            this.targetTacticalPoint = context.targetTacticalPoint;//new SensedTacticalPointInfo(context.targetTacticalPoint);
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
        [Tooltip("When rating a decision, if the score drops below this value -> just discard the decision")]
        public float discardThreshold;

        public DecisionWrapper[] decisions;

       
        //DecisionContext currentDecidedDecisionContext;
        AIState currentState;
        AIController aiController;
       
        [Space(5)]
        public List<DecisionContext> currentDecisionContexts = new List<DecisionContext>();
        [Space(10)]
        public List<DecisionContextVisualiser> currentDecisionContextsVisualisation = new List<DecisionContextVisualiser>();
        [Space(5)]
        public DecisionContextVisualiser lastSelectedDecisionContextVisualiser;
        public DecisionContext lastSelectedDecisionContext = new DecisionContext();


        public void SetUpDecisionLayer(AIController aiController)
        {
            this.aiController = aiController;
        }

        public void Decide(DecisionMakerMemory memory, int decisionMakerLayer) //Decide();
        {
            UnityEngine.Profiling.Profiler.BeginSample("DecisionMaker.Decide");

            currentDecisionContextsVisualisation.Clear();

            //scores all decisions, select the best one, and create new state if this decision is different than the previous one
            float currentRating = 0;
            float bestRatingSoFar = 0;
            DecisionContext bestRatedDecisionContext = null;
            //Debug.Log("Decide========================================================================- ");

            for (int i = 0; i < decisions.Length; i++)
            {
                DecisionContext[] decisionContexesToAdd = decisions[i].decision.GetRatedDecisionContexts(aiController, decisions[i].weigt, discardThreshold);

                for (int j = 0; j < decisionContexesToAdd.Length; j++)
                {
                    //add contexes to all current contexes
                    Debug.Log("adding context to list: " + decisionContexesToAdd[j].decision.name);
                    currentDecisionContexts.Add(decisionContexesToAdd[j]);
                    currentDecisionContextsVisualisation.Add(new DecisionContextVisualiser(decisionContexesToAdd[j]));

                    currentRating = decisionContexesToAdd[j].rating;
                    if (currentRating > bestRatingSoFar)
                    {
                        bestRatingSoFar = currentRating;
                        //the Decision context needs to be copied, as it is a reference to an object from a pool which can change during runtime
                        bestRatedDecisionContext = new DecisionContext(decisionContexesToAdd[j]);
                    }
                    Debug.Log("going through list after adding context to list ---------");
                    for (int x = 0; x < currentDecisionContexts.Count; x++)
                    {
                        Debug.Log("i: " + x + " " + currentDecisionContexts[x].decision.name + " rating: " + currentDecisionContexts[x].rating);
                    }
                }
            }

            if(bestRatedDecisionContext != null)
            {
                StartExecutingDecision(bestRatedDecisionContext);
            }

            if(memory != null)
            {
                Debug.Log("going through list before sending to memory: ---------");
                for (int i = 0; i < currentDecisionContexts.Count; i++)
                {
                    Debug.Log("i: " + i + " " + currentDecisionContexts[i].decision.name + " rating: " + currentDecisionContexts[i].rating);
                }

                memory.OnDecisionMakerDecided(decisionMakerLayer, currentDecisionContexts);
            }

            currentDecisionContexts.Clear();
            // Debug.Log("Decide END========================================================================- ");
            UnityEngine.Profiling.Profiler.EndSample();
        }

        public void StartExecutingDecision(DecisionContext decisionContext)
        {
            // the check here needs to be different - how do we check if a decision context is the same? -> check if the assigned decsision and targets are all the same? -
            // or check if their states are the same - the states are initialised by decisions at runtime or at start?- how are the states parameters set?

            // TODO coulsd it be that because of pooling currentDecidedDecisionContextis already used by somebody else?
            //if (!decisionContext.ContextIsTheSameAs(currentDecidedDecisionContext))
            if (!decisionContext.ContextIsTheSameAs(lastSelectedDecisionContext))
            {
                //currentDecidedDecisionContext = decisionContext;
                //lastSelectedDecisionContextVisualiser = new DecisionContextVisualiser(currentDecidedDecisionContext);

                if (currentState != null)
                {
                    currentState.OnStateExit();
                    aiController.entityTags.RemoveEntityActionTags(currentState.GetActionTagsToRemoveOnStateExit());
                }

                //currentState = currentDecidedDecisionContext.decision.CreateState(aiController, currentDecidedDecisionContext);
                currentState = decisionContext.decision.CreateState(aiController, decisionContext);
                currentState.OnStateEnter();
                Debug.Log("on state Enter");
                if (lastSelectedDecisionContext.decision != null)
                {
                    Debug.Log("previous state: " + lastSelectedDecisionContext.decision.name);
                }
                Debug.Log("new state: " + decisionContext.decision.name);

                aiController.entityTags.AddEntityActionTags(currentState.GetActionTagsToAddOnStateEnter());

                //lastSelectedDecisionContext = Copy decisionContext;
                lastSelectedDecisionContext.SetUpContext(decisionContext);
                lastSelectedDecisionContextVisualiser = new DecisionContextVisualiser(decisionContext);

            }
            else
            {
                //Debug.Log("new decision context: " + decisionContext.decision.name + " was the same as last:" + lastSelectedDecisionContext.decision.name);
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
