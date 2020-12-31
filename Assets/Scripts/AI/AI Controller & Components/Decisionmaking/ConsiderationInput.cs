using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BenitosAI
{
    //maybe we could do without this class?

    public class ConsiderationInput : ScriptableObject
    {
        public enum InputParamsType
        {
            None,
            Range,
            RangeAndDesiredFloatValue,
            Direction //usefull for things like prioritise targets in front of me
                      //Buff Status
                      //Assigned Tag
        }

        public InputParamsType inputParamsType;

        public virtual float GetConsiderationInput(DecisionContext context, Consideration consideration)
        {
            return 0;
        }
    }

}
