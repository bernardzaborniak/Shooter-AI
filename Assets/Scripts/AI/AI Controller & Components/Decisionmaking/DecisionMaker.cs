using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BenitosAI
{
    
   

    [System.Serializable]
    // Holds an array of all possible decisions, rates them and decides which to execute.
    // Holds an internal statemachine concerning current states executing
    public class DecisionMaker
    {
        #region Fields

        // For now only used by visualisation, but could also play a part in deciding 
        // - creates a lot of garbage?
        [System.Serializable]
        public class Memory
        {
            [System.Serializable]
            public class DecisionContextMemory
            {
                [HideInInspector]
                public string name;
                public float rating;
                public float weight;

                public Decision decision; //what are we trying to do?
                public AIController aiController; //who s asking?

                public GameEntity targetEntity; //Who is the target of my action
                public string targetEntityName; //Used to still know the target if the entity was already destroyed
                public TacticalPoint targetTacticalPoint; //Who is the target of my action

                public float timeOfDecison;

                [System.Serializable]
                public class ConsiderationMemory
                {
                    public string considerationName;
                    public float input;
                    public float rating;

                    public ConsiderationMemory(string considerationName, float input, float rating)
                    {
                        this.considerationName = considerationName;
                        this.input = input;
                        this.rating = rating;
                    }
                }

                public ConsiderationMemory[] considerationsMemory;

                public DecisionContextMemory(DecisionContext context, float weight)
                {
                    this.name = context.decision.name;
                    this.rating = context.rating;
                    this.weight = weight;
                    this.decision = context.decision;
                    this.aiController = context.aiController;
                    timeOfDecison = Time.time;

                    try{  this.targetEntity = context.targetEntity.entity;  }
                    catch (System.Exception e) { }

                    try { targetEntityName = targetEntity.name + " " + targetEntity.GetHashCode(); }
                    catch (System.Exception e) { targetEntityName = "no Target"; }

                    try { this.targetTacticalPoint = context.targetTacticalPoint.tacticalPoint; }
                    catch (System.Exception e) { }

                    Consideration[] considerations;
                    considerations = context.decision.considerations;
                    considerationsMemory = new ConsiderationMemory[considerations.Length];

                    for (int i = 0; i < considerationsMemory.Length; i++)
                    {
                        considerationsMemory[i] = new ConsiderationMemory(considerations[i].name, considerations[i].GetConsiderationInput(context), considerations[i].GetConsiderationRating(context));
                    }
                }


            }

            Queue<DecisionContextMemory> selectedDecisionsRememberedQueue = new Queue<DecisionContextMemory>();
            public int numberOfPriorSelectedDecisionsRemembered = 8;
            public DecisionContextMemory[] selectedDecisionsRemembered; //visualised as sorted array

            public List<DecisionContextMemory> lastDecisionsRemembered = new List<DecisionContextMemory>();


            //1
            public void CleanUpLastDecisionRemembered()
            {
                lastDecisionsRemembered.Clear();
            }

            //2
            public void AddToLastDecisionsRemembered(DecisionContext context, float weight)
            {
                lastDecisionsRemembered.Add(new DecisionContextMemory(context, weight));
            }

            //3
            public void SortLastDecisionsRemembered()
            {
                //sorted by weight
                lastDecisionsRemembered.Sort((p1, p2) => p1.rating.CompareTo(p2.rating));
                lastDecisionsRemembered.Reverse();
            }

            public void OnSelectedNewDecision(DecisionContext context)
            {
                //add to queue
                selectedDecisionsRememberedQueue.Enqueue(new DecisionContextMemory(context, 0));
                //cut queue if necessary
                while(selectedDecisionsRememberedQueue.Count> numberOfPriorSelectedDecisionsRemembered)
                {
                    selectedDecisionsRememberedQueue.Dequeue();
                }
                //convert queue to array
                selectedDecisionsRemembered = new DecisionContextMemory[selectedDecisionsRememberedQueue.Count];
                selectedDecisionsRememberedQueue.CopyTo(selectedDecisionsRemembered, 0);
                System.Array.Reverse(selectedDecisionsRemembered);
            }

        }

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

        AIState currentState;
        AIController aiController;

        Memory.DecisionContextMemory lastSelectedDecisionContextMemory; //= new DecisionContext(); //was public before - does it cause errors?

        [Header("Memory")]
        [SerializeField] bool useMemory;
        public Memory memory;

        //Momentum
        float currentMomentum;

        #endregion

        public void SetUpDecisionLayer(AIController aiController)
        {
            this.aiController = aiController;
        }

        public void Decide() 
        {
            UnityEngine.Profiling.Profiler.BeginSample("DecisionMaker.Decide");

            if (useMemory) memory.CleanUpLastDecisionRemembered();

            //scores all decisions, select the best one, and create new state if this decision is different than the previous one
            float currentRating = 0;
            float bestRatingSoFar = 0;
            DecisionContext bestRatedDecisionContext = null;

            for (int i = 0; i < decisions.Length; i++)
            {
                DecisionContext[] decisionContexesToAdd = decisions[i].decision.GetRatedDecisionContexts(aiController, decisions[i].weigt, discardThreshold);

                for (int j = 0; j < decisionContexesToAdd.Length; j++)
                {                 
                    //If its the same as the current selected context - add momentum
                    if (decisionContexesToAdd[j].ContextIsTheSameAs(lastSelectedDecisionContextMemory))
                    {
                        //Debug.Log("context the same as selected");
                        //update momentum
                        currentMomentum -= (Time.time - lastSelectedDecisionContextMemory.timeOfDecison) * lastSelectedDecisionContextMemory.decision.momentumDecayRate;
                        //Debug.Log("current momentum + " + currentMomentum + " sold: " + aiController.transform.parent.name + aiController.transform.parent.gameObject.GetHashCode());

                        //if resulting value would be smaller than current rating, ignore momentum
                        if(currentMomentum > decisionContexesToAdd[j].rating)
                        {
                            //add momentum to the current rating
                            decisionContexesToAdd[j].rating = currentMomentum;
                        }

                    }
                    /*else
                    {
                        currentMomentum = 0;
                    }*/

                    currentRating = decisionContexesToAdd[j].rating;

                    if (currentRating > bestRatingSoFar)
                    {
                        bestRatingSoFar = currentRating;
                        //the Decision context needs to be copied, as it is a reference to an object from a pool which can change during runtime
                        bestRatedDecisionContext = new DecisionContext(decisionContexesToAdd[j]);  
                    }

                    if (useMemory) memory.AddToLastDecisionsRemembered(decisionContexesToAdd[j], decisions[i].weigt);

                }
            }

            if (useMemory) memory.SortLastDecisionsRemembered();

            if(bestRatedDecisionContext != null)
            {
                StartExecutingDecision(bestRatedDecisionContext);
            }

            UnityEngine.Profiling.Profiler.EndSample();
        }

        public void StartExecutingDecision(DecisionContext decisionContext)
        {
            // the check here needs to be different - how do we check if a decision context is the same? -> check if the assigned decsision and targets are all the same? -
            // or check if their states are the same - the states are initialised by decisions at runtime or at start?- how are the states parameters set?

            // TODO coulsd it be that because of pooling currentDecidedDecisionContextis already used by somebody else?
            if (!decisionContext.ContextIsTheSameAs(lastSelectedDecisionContextMemory))
            {
                if (currentState != null)
                {
                    currentState.OnStateExit();
                    aiController.entityTags.RemoveEntityActionTags(currentState.GetActionTagsToRemoveOnStateExit());
                }

                currentState = decisionContext.decision.CreateState(aiController, decisionContext);
                currentState.OnStateEnter();

                aiController.entityTags.AddEntityActionTags(currentState.GetActionTagsToAddOnStateEnter());

                //lastSelectedDecisionContextMemory.SetUpContext(decisionContext);
                lastSelectedDecisionContextMemory = new Memory.DecisionContextMemory(decisionContext, 0); // weight doesnt matter for this memory

                if (useMemory) memory.OnSelectedNewDecision(decisionContext);

                //set the momentum
                if (decisionContext.decision.hasMomentum) currentMomentum = decisionContext.rating + decisionContext.decision.momentumSelectedBonus;
            }
        }

        public void UpdateCurrentState()
        {
            //updates current state
            if (currentState != null)
            {
                //state update return a bool if this state should be continued or aborted
                if (currentState.ShouldStateBeAborted())
                {
                    AbortCurrentDecision();
                }
                else
                {
                    currentState.UpdateState();
                }
            }
        }

        //can be called by the selected deicison execution logic- if it deciedes it isnt a valid decision anymore
        public void AbortCurrentDecision()
        {
            if (currentState != null)
            {
                //Debug.Log("aborting from inside: " + currentState.ToString());
                currentState.OnStateExit();
                aiController.entityTags.RemoveEntityActionTags(currentState.GetActionTagsToRemoveOnStateExit());
            }

            lastSelectedDecisionContextMemory = null;
            currentMomentum = 0;
        }


        //all decisions get a deference to the decision layer they cre called from ,so they can call set state on it
        // every decision has its state in code? - no all states should be declared inside the decision layer? or inside the individual decisions would be better
    }
}
