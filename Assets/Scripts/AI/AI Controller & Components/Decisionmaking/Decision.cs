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

        //also has its own codestate initialised on start and passed to the decision layer, if decision was selected

        //public AIStateEnum correspondingAIState;
        [SerializeField] AIStateCreator correspondingAiStateCreator;

        public AIState CreateState(AIController aiController, DecisionContext context)
        {
            return correspondingAiStateCreator.CreateState(aiController, context);
        }


        public DecisionContext[] GetRatedDecisionContext(AIController aiController)
        {
            // Create contexes according to number of targets
            DecisionContext[] contexts = decisionContextCreator.GetDecisionContexes(this, aiController);

            // Score each context
            for (int i = 0; i < contexts.Length; i++)
            {
                Debug.Log("new consideration being rated -----------------------------------");

                float score = 1;

                for (int c = 0; c < considerations.Length; c++)
                {
                    Debug.Log("score: " + score + " += " + considerations[c].GetConsiderationRating(contexts[i]));
                    score *= considerations[c].GetConsiderationRating(contexts[i]);
                }

                Debug.Log("score before makeup: " + score);
                //Add makeup Value / Compensation Factor - as you multiply normalized values, teh total drops - if we dont do this more considerations will result in a lower weight - according to Mark Dave and a tipp from Ben Sizer
                score += score * ((1 - score) * (1 - (1 / considerations.Length)));
                Debug.Log("score after makeup: " + score);

                contexts[i].rating = score;

            }


            return contexts;
        }
    }

}
