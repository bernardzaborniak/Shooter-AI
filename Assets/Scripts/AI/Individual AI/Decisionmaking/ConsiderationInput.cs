using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BenitosAI
{

    public class ConsiderationInput : ScriptableObject
    {
        [HideInInspector]
        public ConsiderationInputParams.InputParamsType[] inputParamsType;

        //maybe leave consideration out?
        //public virtual float GetConsiderationInput(DecisionContext context, Consideration consideration, ConsiderationInputParams considerationInputParams)
        public virtual float GetConsiderationInput(DecisionContext context, ConsiderationInputParams considerationInputParams)
        {
            return 0;
        }
    }

}
