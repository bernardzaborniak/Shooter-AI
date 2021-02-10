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
        public Consideration[] considerations;

        [Header("Momentum")]
        public bool hasMomentum;
        [ConditionalHide()]
        [Tooltip("when selecting this decision, this bonus is added to the rating")]
        public float momentumSelectedBonus;
        [Tooltip("how much does the momentum loose per second?")]
        public float momentumDecayRate;




        public AIState CreateState(AIController aiController, DecisionContext context)
        {
            return correspondingAiStateCreator.CreateState(aiController, context);
        }


        public DecisionContext[] GetRatedDecisionContexts(AIController aiController, float weight, float discardThreshold)
        {
            // Create contexes according to number of targets
            DecisionContext[] contexts = decisionContextCreator.GetDecisionContexts(this, aiController);

            // Score each context
            for (int i = 0; i < contexts.Length; i++)
            {
               // contexts[i].RateContext(considerations, bonusConsiderations, weight, discardThreshold);
                contexts[i].RateContext(considerations, weight, discardThreshold);
            }


            return contexts;
        }

        //THis method is used for visualisation
        /*public Consideration[] GetConsiderations()
        {
            return considerations;
        }*/


    }

}
