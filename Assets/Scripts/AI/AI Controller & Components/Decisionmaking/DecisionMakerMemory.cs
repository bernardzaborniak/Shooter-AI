using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BenitosAI
{
    //If memory is enabled ,info about used decisionContexts is saved in this wrapper class
    public class DecisionMemoryItem
    {
        // ------------- Data saved from Decision Context -----------------
        public float rating;

        // Mandatory
        public Decision decision; //what are we trying to do?
        public AIController aiController; //who s asking?

        // Optional
        public GameEntity targetEntity; //Who is the target of my action
        public TacticalPoint targetTacticalPoint; //Who is the target of my action

        // --------------Timing----------------
        public float timeWhenDecided;


        //[System.Serializable]
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


        public DecisionMemoryItem(DecisionContext context)
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

            //timeWhenDecided = context.time
            timeWhenDecided = Time.time;


            Consideration[] considerations = context.decision.GetConsiderations();
            considerationsMemory = new ConsiderationMemory[considerations.Length];
            for (int i = 0; i < considerations.Length; i++)
            {
                considerationsMemory[i] = new ConsiderationMemory(considerations[i].name, considerations[i].GetConsiderationInput(context), considerations[i].GetConsiderationRating(context));
            }
        }
    }

    public class DecisionMemoryLayer
    {
        public DecisionMemoryItem selectedItem;
        public List<DecisionMemoryItem> decisionMemoryItems = new List<DecisionMemoryItem>();
    }

    public class DecisionMakerMemoryCycle
    {
        public int frameCountWhenCycleDecided;
        public float timeWhenCycleDecided;

        

        public DecisionMemoryLayer[] decisionMemoryLayers;

        public DecisionMakerMemoryCycle(int numberOfDecisionLayers)
        {

            frameCountWhenCycleDecided = Time.frameCount;
            timeWhenCycleDecided = Time.time;

            decisionMemoryLayers = new DecisionMemoryLayer[numberOfDecisionLayers];
            for (int i = 0; i < decisionMemoryLayers.Length; i++)
            {
                decisionMemoryLayers[i] = new DecisionMemoryLayer();
            }
        }
    }


    // Holds memory for all decision makers used 
    public class DecisionMakerMemory : MonoBehaviour
    {
        [Tooltip("How many cycles of decisions are saved inside memory?")]
        public int memoryDepth;
        [Tooltip("needs to be assigned the same number as in decision layers")]
        public int numberOfDecisionLayers;

        DecisionMakerMemoryCycle currentMemoryCycle; 
        DecisionMakerMemoryCycle previousMemoryCycle; 

        Queue<DecisionMakerMemoryCycle> memoryCycles = new Queue<DecisionMakerMemoryCycle>();


        private void Start()
        {
            currentMemoryCycle = new DecisionMakerMemoryCycle(numberOfDecisionLayers);
        }

        public void RememberContext(int decisionLayer, DecisionContext context)
        {
            InstantateNewCycleIfNeeded();

            try
            {
                currentMemoryCycle.decisionMemoryLayers[decisionLayer].decisionMemoryItems.Add(new DecisionMemoryItem(context));
            }
            catch(System.Exception e)
            {
                throw new System.Exception("DecisionMakerMemory.numberOfDecisionLayers is not set properly");
            }


        }

        public void RememberSelectedContext(int decisionLayer, DecisionContext context)
        {
            InstantateNewCycleIfNeeded();

            try
            {
                currentMemoryCycle.decisionMemoryLayers[decisionLayer].selectedItem = new DecisionMemoryItem(context);
            }
            catch (System.Exception e)
            {
                throw new System.Exception("DecisionMakerMemory.numberOfDecisionLayers is not set properly");
            }

        }

        public void RememberSelectedContextSameAsLastCycle(int decisionLayer)
        {
            InstantateNewCycleIfNeeded();

            if(previousMemoryCycle != null)
            {
                currentMemoryCycle.decisionMemoryLayers[decisionLayer].selectedItem = previousMemoryCycle.decisionMemoryLayers[decisionLayer].selectedItem;
            }

           
        }

        void InstantateNewCycleIfNeeded()
        {
            if (Time.frameCount != currentMemoryCycle.frameCountWhenCycleDecided)
            {
                if (currentMemoryCycle != null)
                {
                    previousMemoryCycle = currentMemoryCycle;
                }
                currentMemoryCycle = new DecisionMakerMemoryCycle(numberOfDecisionLayers);

                memoryCycles.Enqueue(currentMemoryCycle);

                //cut the queue if too long
                while (memoryCycles.Count > memoryDepth)
                {
                    memoryCycles.Dequeue();
                }
            }
        }

        public List<DecisionMemoryItem> GetCurrentCycleDecisions(int decisionLayer)
        {
            return currentMemoryCycle.decisionMemoryLayers[decisionLayer].decisionMemoryItems;
        }

       public DecisionMemoryItem GetCurrentlySelectedItem(int decisionLayer)
       {
            return currentMemoryCycle.decisionMemoryLayers[decisionLayer].selectedItem;
       }

        public int GetFrameCountCycleDecided()
        {
            return currentMemoryCycle.frameCountWhenCycleDecided;
        }

    }

}
