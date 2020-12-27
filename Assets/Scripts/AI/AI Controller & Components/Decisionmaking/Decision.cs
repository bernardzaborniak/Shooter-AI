using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BenitosAI
{

    [CreateAssetMenu(menuName = "AI/Decision", fileName = "New Decision")]
    public class Decision : ScriptableObject
    {
        [SerializeField] DecisionContextCreator decisionContextCreator;
        //every decision has a list of considerations based on which to decide
        [SerializeField] Consideration[] considerations;
        //how to solve one decision that has to seperate into several ones - if there are more enemies to go to - we need this decision to seperate into x decisions. where x is the number of enemies

        //also has its own codestate initialised on start and passed to the decision layer, if decision was selected

        public AIStateEnum correspondingAIState;

        //AIController aiController;
        //AIState state;
        //several decisions can have the same state?
        /*public AIState GetCorrespondingState()
        {
            return state;
        }*/

        /* public void SetUpDecision(AIController aiController)
         {
             this.aiController = aiController;

             /*for (int i = 0; i < considerations.Length; i++)
             {
                 considerations[i].SetUpConsideration(aiController);
             }
         }*/

        /*public AIState OnDecisionWasSelected(AIController aiController)
        {
            AIState aIState = new AIst_HumSol_MovingToZeroPoint();
            aIState.SetUpState(aiController);
            return aIState;
        }*/

      

       // [Header("Optimisation")]
       // public int maxEntityTargetsPerDecision = 5;
        //public int maxTacticalPointTargetsPerDecision = 10;

        //Queue<DecisionContext> decisionContextesPool = new Queue<DecisionContext>();

        /*private void Awake()
        {
            int poolSize = 0;
            if(decisionContextTargetType == DecisionContextTargetType.Self)
            {
                poolSize = 1;
            }
            else if(decisionContextTargetType == DecisionContextTargetType.Entity)
            {
                poolSize = maxEntityTargetsPerDecision;
            }
            else if (decisionContextTargetType == DecisionContextTargetType.TacticalPoint)
            {
                poolSize = maxTacticalPointTargetsPerDecision;
            }

            for (int i = 0; i < poolSize; i++)
            {
                decisionContextesPool.Enqueue(new DecisionContext());
            }
        }*/

        public float GetDecisionRating(DecisionContext context)
        {
            float score = 1;

            for (int i = 0; i < considerations.Length; i++)
            {
                score *= considerations[i].GetConsiderationRating(context);
            }

            //Debug.Log("decision score before adding makeup value: " + score);

            //Add makeup Value / Compensation Factor - as you multiply normalized values, teh total drops - if we dont do this more considerations will result in a lower weight - according to Mark Dave and a tipp from Ben Sizer

            score += score * ((1 - score) * (1 - (1 / considerations.Length)));

           // Debug.Log("decision score after adding makeup value: " + score);

            return score;
        }

        public DecisionContext[] GetDecisionRating(AIController aiController)
        {
            return decisionContextCreator.GetDecisionContexes(this, aiController);
            //1 set up the contextes

            /* if (decisionContextTargetType == DecisionContextTargetType.Self)
             {
                 //no targets
             }
             else if (decisionContextTargetType == DecisionContextTargetType.Entity)
             {
                 HashSet<SensedEntityInfo> targetEntities = new HashSet<SensedEntityInfo>();
             }
             else if (decisionContextTargetType == DecisionContextTargetType.TacticalPoint)
             {
                 poolSize = maxTacticalPointTargetsPerDecision;
             }

             return decisionContextes;*/
        }


        /*void Start()
        {
            //instantiate the corresponding state
           // if(correspondingAIState == AIStateEnum.TestState)
            //{
             //   state = new AITestState();
           // }
        }

        void Update()
        {

        }*/
    }

}
