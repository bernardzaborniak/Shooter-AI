using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BenitosAI
{
    public abstract class AIStateCreator : ScriptableObject
    {
        public abstract AIState CreateState(AIController aiController, DecisionContext context);
    }

    public abstract class AIState
    {
        //public abstract void SetUpState(AIController aiController, DecisionContext context);

        public abstract void OnStateEnter();

        public abstract void OnStateExit();

        public abstract EntityActionTag[] GetActionTagsToAddOnStateEnter();

        public abstract EntityActionTag[] GetActionTagsToRemoveOnStateExit();

        public abstract void UpdateState();


    }
}
