using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BenitosAI
{
    public abstract class AIStateCreator : ScriptableObject
    {
        [HideInInspector]
        public AIStateCreatorInputParams.InputParamsType[] inputParamsType;

        // Creates and returns an AIState object.
        public abstract AIState CreateState(AIController aiController, DecisionContext context, AIStateCreatorInputParams inputParams);
    }

    public abstract class AIState
    {
        // Deriving classes will all implement their own Constructor with needed info as parameters.
        //public abstract void SetUpState(AIController aiController, DecisionContext context);

        public abstract void OnStateEnter();

        public abstract void OnStateExit();

        public abstract EntityActionTag[] GetActionTagsToAddOnStateEnter();

        public abstract EntityActionTag[] GetActionTagsToRemoveOnStateExit();

        public abstract void UpdateState();

        public abstract bool ShouldStateBeAborted();
    }
}
