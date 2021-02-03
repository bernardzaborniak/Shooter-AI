using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BenitosAI
{

    [CreateAssetMenu(menuName = "AI/Decision", fileName = "New Decision")]
    public class Decision : ScriptableObject
    {
        [Space(5)]
        [SerializeField] DecisionContextCreator decisionContextCreator;
        [SerializeField] AIStateCreator correspondingAiStateCreator;
        //every decision has a list of considerations based on which to decide
        [Space(5)]
        [SerializeField] Consideration[] considerations;
        [Space(5)]
        [SerializeField] BonusConsiderationWrapper[] bonusConsiderations;

        //also has its own codestate initialised on start and passed to the decision layer, if decision was selected

        //public AIStateEnum correspondingAIState;
        [System.Serializable]
        public class BonusConsiderationWrapper
        {
            public Consideration consideration;
            public float weight;
        }


        public AIState CreateState(AIController aiController, DecisionContext context)
        {
            return correspondingAiStateCreator.CreateState(aiController, context);
        }


        public DecisionContext[] GetRatedDecisionContexts(AIController aiController, float weight, float discardThreshold)
        {
            // Create contexes according to number of targets
            DecisionContext[] contexts = decisionContextCreator.GetDecisionContexes(this, aiController);

            // Score each context
            for (int i = 0; i < contexts.Length; i++)
            {
                contexts[i].RateContext(considerations, bonusConsiderations, weight, discardThreshold);
                //make a seperate method for rating contexes

                //float score = 1;
                /*float score = weight;

                for (int c = 0; c < considerations.Length; c++)
                {
                    //Debug.Log("new consideration being rated -----------------------------------" + considerations[c].name);


                    //Debug.Log("score: " + score + " *= " + considerations[c].GetConsiderationRating(contexts[i]));
                    score *= considerations[c].GetConsiderationRating(contexts[i]);
                    //Debug.Log("score: " + score);
                    if(score< discardThreshold)
                    {
                        score = -1;
                        contexts[i].rating = score;
                        break;
                    }

                }

                for (int c = 0; c < bonusConsiderations.Length; c++)
                {
                    score += bonusConsiderations[c].consideration.GetConsiderationRating(contexts[i]) * bonusConsiderations[c].weight;
                }

                //Debug.Log("score before makeup: " + score);
                //Add makeup Value / Compensation Factor - as you multiply normalized values, teh total drops - if we dont do this more considerations will result in a lower weight - according to Mark Dave and a tipp from Ben Sizer
                score += score * ((1 - score) * (1 - (1 / considerations.Length)));
                //Debug.Log("score after makeup: " + score);

                contexts[i].rating = score;*/

            }


            return contexts;
        }

        //THis method is used for visualisation
        public Consideration[] GetConsiderations()
        {
            return considerations;
        }
    }

}
