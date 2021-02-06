using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BenitosAI
{
   // [System.Serializable]
    //If memory is enabled ,info about used decisionContexts is saved in this wrapper class
    public class DecisionContextMemoryItem
    {
        // ------------- Data saved from Decision Context -----------------
        public float rating;

        // Mandatory
        public Decision decision; //what are we trying to do?
        public AIController aiController; //who s asking?

        // Optional
        //public SensedEntityInfo targetEntity; //Who is the target of my action
        public GameEntity targetEntity; //Who is the target of my action
        //public SensedTacticalPointInfo targetTacticalPoint; //Who is the target of my action
        public TacticalPoint targetTacticalPoint; //Who is the target of my action

        // --------------Timing----------------
        public float timeWhenDecided;
        //public int cycles


        [System.Serializable]
        public class ConsiderationMemory
        {
            [SerializeField] string considerationName;
            [SerializeField] float input;
            [SerializeField] float rating;

            public ConsiderationMemory(string considerationName, float input, float rating)
            {
                this.considerationName = considerationName;
                this.input = input;
                this.rating = rating;
            }
        }

        public ConsiderationMemory[] considerationsMemory;


        public DecisionContextMemoryItem(DecisionContext context)
        {
            rating = context.rating;
            decision = context.decision;
            aiController = context.aiController;
            if (context.targetEntity != null)
            {
                targetEntity = context.targetEntity.entity;
            }
            if (context.targetTacticalPoint != null)
            {
                targetTacticalPoint = context.targetTacticalPoint.tacticalPoint;
            }

             timeWhenDecided = Time.time;


            Consideration[] considerations = context.decision.GetConsiderations();
            considerationsMemory = new ConsiderationMemory[considerations.Length];
            for (int i = 0; i < considerations.Length; i++)
            {
                considerationsMemory[i] = new ConsiderationMemory(considerations[i].name, considerations[i].GetConsiderationInput(context), considerations[i].GetConsiderationRating(context));
            }
        }
    }

    //[System.Serializable]
    //This class is a bit dirty to ensure that the decisionmaker works performant without it too
    public class DecisionMakerCycleMemory
    {
        public int frameCountWhenCycleDecided;
        public float timeWhenCycleDecided;


        public List<DecisionContextMemoryItem[]> cycleItems; //= new List<DecisionContextMemoryItem[]>();

        public DecisionMakerCycleMemory()
        {
            Debug.Log("Set up memoryCacly");

                frameCountWhenCycleDecided = Time.frameCount;
                timeWhenCycleDecided = Time.time;

                cycleItems = new List<DecisionContextMemoryItem[]>();

           
        }
        //public  cycleItems;
    }

    // Holds memory for all decision makers used 
    public class DecisionMakerMemory : MonoBehaviour
    {
        [Tooltip("How many cycles of decisions are saved inside memory?")]
        public int memoryDepth;

        //int currentCycleFrame = 0; //to check if new cycle should be added to queue
        DecisionMakerCycleMemory currentMemoryCycle; //= new DecisionMakerCycleMemory();

        //I use a kind of 2 dimensional list instead of a dicitonary to allow memory sorted by rating
        Queue<DecisionMakerCycleMemory> memoryItems = new Queue<DecisionMakerCycleMemory>();

        //for debug only
      // public DecisionMakerCycleMemory[] memoryVisualisation;

        private void Start()
        {
            currentMemoryCycle = new DecisionMakerCycleMemory();
        }

        public void OnDecisionMakerDecided(int decisionLayer, List<DecisionContext> contextsDecidedUpon)
        {
            Debug.Log("contexes before sorting:-----------");
            for (int i = 0; i < contextsDecidedUpon.Count; i++)
            {
                Debug.Log("i: " + i + " " + contextsDecidedUpon[i].decision.name + " rating: " + contextsDecidedUpon[i].rating);
            }

            //sort contextsDecidedUpon according to rating-----------
            contextsDecidedUpon.Sort((p1, p2) => p1.rating.CompareTo(p2.rating));
            contextsDecidedUpon.Reverse();

            //Debug.Log("OnDecisionMakerDecided: decisionLayer: " + decisionLayer);
            //instantiate new cycle item if needed
            if (Time.frameCount != currentMemoryCycle.frameCountWhenCycleDecided)
            {
                currentMemoryCycle = new DecisionMakerCycleMemory();

                memoryItems.Enqueue(currentMemoryCycle);

                //cut the queue
                while(memoryItems.Count > memoryDepth)
                {
                    memoryItems.Dequeue();
                }
            }

            currentMemoryCycle.cycleItems.Insert(decisionLayer, new DecisionContextMemoryItem[contextsDecidedUpon.Count]);
            //add to the current cycle
            Debug.Log("contextsDecidedUpon----------------------------------");
            for (int i = 0; i < contextsDecidedUpon.Count; i++)
            {
                Debug.Log("i: " + i + " " + contextsDecidedUpon[i].decision.name + " rating: "  + contextsDecidedUpon[i].rating);
                currentMemoryCycle.cycleItems[decisionLayer][i] = new DecisionContextMemoryItem(contextsDecidedUpon[i]);
            }


            //visualisation only for debug purposes
           // memoryVisualisation = new DecisionMakerCycleMemory[memoryItems.Count];
            //memoryItems.CopyTo(memoryVisualisation, 0);
            //System.Array.Reverse(memoryVisualisation);
        }


        //used by visualisation
        public DecisionMakerCycleMemory GetCurrentDecisionMakerMemoryCycle()
        {
            return currentMemoryCycle;
        }

    }

}
