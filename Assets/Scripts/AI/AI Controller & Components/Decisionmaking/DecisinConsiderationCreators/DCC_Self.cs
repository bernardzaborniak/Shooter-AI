using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace BenitosAI
{
    [CreateAssetMenu(menuName = "AI/Decision Context Creator/Self", fileName = "Self")]
    public class DCC_Self : DecisionContextCreator
    {
        DecisionContext[] myselfContext;
        private void OnEnable()
        {
            myselfContext = new DecisionContext[1];
            myselfContext[0] = new DecisionContext();
        }

        public override DecisionContext[] GetDecisionContexes(Decision decision, AIController aiController)
        {
            myselfContext[0].SetUpContext(decision, aiController, null, null);
            return myselfContext;
        }
    }
}
