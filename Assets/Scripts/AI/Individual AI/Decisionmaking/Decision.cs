﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BenitosAI
{

   // [CreateAssetMenu(menuName = "AI/Decision", fileName = "New Decision")]
    [System.Serializable]
    public class Decision //: ScriptableObject
    {
        [Space(5)]
        public string name;
        public float weight;
        [Space(5)]
        [SerializeField] DecisionContextCreator decisionContextCreator;
        [SerializeField] AIStateCreator correspondingAiStateCreator;
        [SerializeField] AIStateCreatorInputParams aIStateCreatorInputParams;
        //every decision has a list of considerations based on which to decide



        [Header("Momentum")]
        public bool hasMomentum;
        [ConditionalHide()]
        [Tooltip("when selecting this decision, this bonus is added to the rating")]
        public float momentumSelectedBonus;
        [Tooltip("how much does the momentum loose per second?")]
        public float momentumDecayRate;

        [Space(5)]
        //[Header("Considerations")]
        public Consideration[] considerations;

        /*public Decision()
        {
            aIStateCreatorInputParams = ScriptableObject.CreateInstance<AIStateCreatorInputParams>();
        }*/




        public AIState CreateState(AIController aiController, DecisionContext context)
        {
            
            return correspondingAiStateCreator.CreateState(aiController, context);
        }


        //public DecisionContext[] GetRatedDecisionContexts(AIController aiController, float weight, float discardThreshold)
        public DecisionContext[] GetRatedDecisionContexts(AIController aiController, float discardThreshold)
        {
            //if (name == "Hold Position Combat Stance Standing")
            //{
                Debug.Log(name + " aIStateCreatorInputParams: " + aIStateCreatorInputParams.position1);

            //}


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

/*#if UNITY_EDITOR
        public List<UnityEditor.SerializedProperty> GetCorrespondingParams(UnityEditor.SerializedProperty inputParamsProp, AIStateCreatorInputParams.InputParamsType inputParamsType)//AIStateCreator aiStateCreator, ref UnityEditor.SerializedObject serializedObject)
        {
            return aIStateCreatorInputParams.GetCorrespondingParams(inputParamsProp, inputParamsType);
        }
#endif*/

        //THis method is used for visualisation
        /*public Consideration[] GetConsiderations()
        {
            return considerations;
        }*/


    }

}
