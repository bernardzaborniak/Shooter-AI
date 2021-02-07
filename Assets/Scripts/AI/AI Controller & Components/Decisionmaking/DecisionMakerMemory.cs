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
        public float decisionWeight;
        public AIController aiController; //who s asking?

        // Optional
        public GameEntity targetEntity; //Who is the target of my action
        public TacticalPoint targetTacticalPoint; //Who is the target of my action

        // --------------Timing----------------
        public float timeWhenDecided;

        public bool wasSelected;


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


        public DecisionMemoryItem(DecisionContext context, float decisionWeight)
        {
            rating = context.rating;
            this.decisionWeight = decisionWeight;
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

            // Set Up Considerations
            Consideration[] considerations = context.decision.GetConsiderations();
            considerationsMemory = new ConsiderationMemory[considerations.Length];
            for (int i = 0; i < considerations.Length; i++)
            {
                considerationsMemory[i] = new ConsiderationMemory(considerations[i].name, considerations[i].GetConsiderationInput(context), considerations[i].GetConsiderationRating(context));
            }
        }

        public bool IsTheSameAsContext(DecisionContext context)
        {
            DecisionMemoryItem testItem = new DecisionMemoryItem(context, 0); //weight is ignored when checking for identity

            bool isTheSame = false;

            if(rating == testItem.rating)
            {
                if(decision == testItem.decision)
                {
                    if(aiController == testItem.aiController)
                    {
                        if(targetEntity == testItem.targetEntity)
                        {
                            if(targetTacticalPoint == testItem.targetTacticalPoint)
                            {
                                isTheSame = true;
                            }
                        }
                    }
                }
            }


            return isTheSame;
        }


    }

    public class DecisionMemoryLayer
    {
        public DecisionMemoryItem selectedItem;
        public List<DecisionMemoryItem> decisionMemoryItems = new List<DecisionMemoryItem>();

        public void SortItems()
        {
            decisionMemoryItems.Sort((p1, p2) => p1.rating.CompareTo(p2.rating));
            decisionMemoryItems.Reverse();
        }

        public void SetSelectedItem(DecisionContext context)
        {
            for (int i = 0; i < decisionMemoryItems.Count; i++)
            {
                if (decisionMemoryItems[i].IsTheSameAsContext(context))
                {
                    selectedItem = decisionMemoryItems[i];
                    decisionMemoryItems[i].wasSelected = true;
                }
            }
        }
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

        public void RememberContext(int decisionLayer, DecisionContext context, float decisionWeight)
        {
            InstantateNewCycleIfNeeded();

            try
            {
                currentMemoryCycle.decisionMemoryLayers[decisionLayer].decisionMemoryItems.Add(new DecisionMemoryItem(context, decisionWeight));
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
               // currentMemoryCycle.decisionMemoryLayers[decisionLayer].selectedItem = new DecisionMemoryItem(context);
                currentMemoryCycle.decisionMemoryLayers[decisionLayer].SetSelectedItem(context);
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
                //currentMemoryCycle.decisionMemoryLayers[decisionLayer].selectedItem = previousMemoryCycle.decisionMemoryLayers[decisionLayer].selectedItem;
                currentMemoryCycle.decisionMemoryLayers[decisionLayer].decisionMemoryItems.Add(previousMemoryCycle.decisionMemoryLayers[decisionLayer].selectedItem);
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
            currentMemoryCycle.decisionMemoryLayers[decisionLayer].SortItems();
            return currentMemoryCycle.decisionMemoryLayers[decisionLayer].decisionMemoryItems;
        }

        public List<DecisionMemoryItem> GetPreviousCycleDecisions(int decisionLayer, int stepsBackward)
        {
            DecisionMakerMemoryCycle cycle = null;
            Queue<DecisionMakerMemoryCycle> memoryCyclesToOpen = new Queue<DecisionMakerMemoryCycle>(memoryCycles);
            for (int i = 0; i < stepsBackward; i++)
            {
                cycle = memoryCyclesToOpen.Dequeue();
            }

            cycle.decisionMemoryLayers[decisionLayer].SortItems();
            return cycle.decisionMemoryLayers[decisionLayer].decisionMemoryItems;
        }

        public DecisionMemoryItem GetCurrentlySelectedItem(int decisionLayer)
       {
            return currentMemoryCycle.decisionMemoryLayers[decisionLayer].selectedItem;
       }

        public int GetFrameCountCycleDecided()
        {
            return currentMemoryCycle.frameCountWhenCycleDecided;
        }

        public float GetCurrentCycleTimeWhenCycleDecided()
        {
            return currentMemoryCycle.timeWhenCycleDecided;
        }

        public float GetPreviousCycleTimeWhenCycleDecided(int stepsBackward)
        {
            float currentTimeWhenCycleDecided;
            Queue<DecisionMakerMemoryCycle> memoryCyclesToOpen = new Queue<DecisionMakerMemoryCycle>(memoryCycles);
            for (int i = 0; i < stepsBackward; i++)
            {
                currentTimeWhenCycleDecided = memoryCyclesToOpen.Dequeue().timeWhenCycleDecided;
            }

            return currentMemoryCycle.timeWhenCycleDecided;
        }

    }

}
