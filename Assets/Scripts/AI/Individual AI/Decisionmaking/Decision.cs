using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BenitosAI
{

   // [CreateAssetMenu(menuName = "AI/Decision", fileName = "New Decision")]
    [System.Serializable]
    public class Decision //: ScriptableObject
    {
        #region Fields

        [Space(5)]
        public string name;
        public float weight;
        
        [Space(5)]
        [Tooltip("Select an SO for the Desired Decision Target")]
        [SerializeField] DecisionContextCreator decisionContextCreator;
        [Tooltip("Select the Action/State the AI will execute upon selecting this Decision")]
        [SerializeField] AIStateCreator correspondingAiStateCreator;
        [SerializeField] AIStateCreatorInputParams aIStateCreatorInputParams;

        [Header("Momentum")]
        [Tooltip("Simulates Decision Momentum: After selecting a decision, the selected decision will be scored higher in the next Decide() calls, depending on the Bonus & Decay Rate params")]
        public bool hasMomentum;
        [ConditionalHide()]
        [Tooltip("When selecting this decision, this bonus is added to the rating.")]
        public float momentumSelectedBonus;
        [Tooltip("How much momentum is lost per second?")]
        public float momentumDecayRate;


        [Space(5)]
        public Consideration[] considerations;

        #endregion


        public AIState CreateState(AIController aiController, DecisionContext context)
        {
            return correspondingAiStateCreator.CreateState(aiController, context, aIStateCreatorInputParams);
        }

        public DecisionContext[] GetRatedDecisionContexts(AIController aiController, float discardThreshold)
        {
            // Create contexes according to number of targets
            DecisionContext[] contexts = decisionContextCreator.GetDecisionContexts(this, aiController);

            // Score each context
            for (int i = 0; i < contexts.Length; i++)
            {
                contexts[i].RateContext(considerations, weight, discardThreshold);
            }

            return contexts;
        }
    }
}
