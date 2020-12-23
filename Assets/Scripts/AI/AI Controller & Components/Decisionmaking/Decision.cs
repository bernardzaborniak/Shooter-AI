using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BenitosAI
{

    [CreateAssetMenu(menuName = "AI/Decision", fileName = "New Decision")]
    public class Decision : ScriptableObject
    {
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
