using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BenitosAI
{
    public class DecisionContextCreator : ScriptableObject
    {
       /* public enum DecisionContextTargetType
        {
            Self,
            Entity, //decision gets seperate context for each entitity
            TacticalPoint
        }

        public DecisionContextTargetType decisionContextTargetType;*/

        public virtual DecisionContext[] GetDecisionContexes(Decision decision, AIController aiController)
        {
            return null;
        }
    }
}