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
        [System.Serializable]
        public class Memory
        {
            //creates a lot of garbage?
            [System.Serializable]
            public class DecisionContextMemory        //as decision context objects are reused, this visualiser shows which context was use for this specific instance
            {
                [HideInInspector]
                public string name;
                public float rating;
                public float weight;

                public Decision decision; //what are we trying to do?
                public AIController aiController; //who s asking?

                public GameEntity targetEntity; //Who is the target of my action
                public TacticalPoint targetTacticalPoint; //Who is the target of my action

                public float timeOfDecison;
                //public bool wasSelected;

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
                    rating = context.rating;
                    this.weight = weight;
                    this.decision = context.decision;
                    this.aiController = context.aiController;
                    timeOfDecison = Time.time;

                    try{  this.targetEntity = context.targetEntity.entity;  }
                    catch (System.Exception e) { }

                    try { this.targetTacticalPoint = context.targetTacticalPoint.tacticalPoint; }
                    catch (System.Exception e) { }

                    Consideration[] considerations;
                    considerations = context.decision.GetConsiderations();
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

       
        //DecisionContext currentDecidedDecisionContext;
        AIState currentState;
        AIController aiController;
       
        //[Space(5)]
       // public List<DecisionContext> currentDecisionContexts = new List<DecisionContext>();
        //[Space(10)]
        //public List<DecisionContextVisualiser> currentDecisionContextsVisualisation = new List<DecisionContextVisualiser>();
       // [Space(5)]
        //public DecisionContextVisualiser lastSelectedDecisionContextVisualiser;
        public DecisionContext lastSelectedDecisionContext = new DecisionContext();

        //only used if memory is used
        //DecisionMakerMemory memory;
        [Header("Memory")]
        [SerializeField] bool useMemory;
        public Memory memory;
       // int decisionMakerLayer;

        //public void SetUpDecisionLayer(AIController aiController, DecisionMakerMemory memory, int decisionMakerLayer)
        public void SetUpDecisionLayer(AIController aiController)//, int decisionMakerLayer)
        {
            this.aiController = aiController;

            //this.memory = memory;
            //this.decisionMakerLayer = decisionMakerLayer;
        }

        public void Decide()  //the parameters are only used if memory is used
        {
            UnityEngine.Profiling.Profiler.BeginSample("DecisionMaker.Decide");

            //currentDecisionContextsVisualisation.Clear();
            if (useMemory) memory.CleanUpLastDecisionRemembered();


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


                   // if (useMemory) memory.RememberContext(decisionMakerLayer, decisionContexesToAdd[j], decisions[i].weigt);
                    if (useMemory) memory.AddToLastDecisionsRemembered(decisionContexesToAdd[j], decisions[i].weigt);

                    //currentDecisionContextsVisualisation.Add(new DecisionContextVisualiser(decisionContexesToAdd[j]));

                    currentRating = decisionContexesToAdd[j].rating;
                    if (currentRating > bestRatingSoFar)
                    {
                        bestRatingSoFar = currentRating;
                        //the Decision context needs to be copied, as it is a reference to an object from a pool which can change during runtime
                        bestRatedDecisionContext = new DecisionContext(decisionContexesToAdd[j]);  
                    }
                }
            }

            if (useMemory) memory.SortLastDecisionsRemembered();

            if(bestRatedDecisionContext != null)
            {
                StartExecutingDecision(bestRatedDecisionContext);
            }

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
               
                /*Debug.Log("on state Enter");
                if (lastSelectedDecisionContext.decision != null)
                {
                    Debug.Log("previous state: " + lastSelectedDecisionContext.decision.name);
                }
                Debug.Log("new state: " + decisionContext.decision.name);*/

                aiController.entityTags.AddEntityActionTags(currentState.GetActionTagsToAddOnStateEnter());

                //lastSelectedDecisionContext = Copy decisionContext;
                lastSelectedDecisionContext.SetUpContext(decisionContext);
                //lastSelectedDecisionContextVisualiser = new DecisionContextVisualiser(decisionContext);

                if (useMemory) memory.OnSelectedNewDecision(decisionContext);
                //if (memory != null)
                //{
                    //send current context to memory
                 //   memory.RememberSelectedContext(decisionMakerLayer, decisionContext);
                //}

            }
            /*else
            {
                if(memory != null)
                {
                    //send current context to memory
                    memory.RememberSelectedContextSameAsLastCycle(decisionMakerLayer);
                }
                //Debug.Log("new decision context: " + decisionContext.decision.name + " was the same as last:" + lastSelectedDecisionContext.decision.name);
            }*/

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
