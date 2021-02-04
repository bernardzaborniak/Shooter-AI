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
            DecisionContext[] contexts = decisionContextCreator.GetDecisionContexts(this, aiController);

            // Score each context
            for (int i = 0; i < contexts.Length; i++)
            {
                contexts[i].RateContext(considerations, bonusConsiderations, weight, discardThreshold);
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
