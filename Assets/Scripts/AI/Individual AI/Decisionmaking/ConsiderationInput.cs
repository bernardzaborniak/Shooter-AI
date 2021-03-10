using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BenitosAI
{

    public class ConsiderationInput : ScriptableObject
    {
        [HideInInspector]
        public ConsiderationInputParams.InputParamsType[] inputParamsType;

        public virtual float GetConsiderationInput(DecisionContext context, ConsiderationInputParams considerationInputParams)
        {
            return 0;
        }
    }

}
