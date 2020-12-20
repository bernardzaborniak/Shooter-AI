using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//maybe we could do without this class?

public class ConsiderationInput : ScriptableObject
{
    public enum InputParamsType
    {
        None,
        Range,
        Direction //usefull for things like prioritise targets in front of me
        //Buff Status
        //Assigned Tag
    }

    public InputParamsType inputParamsType;

    public virtual float GetConsiderationInput(AIController aiController, Consideration consideration)
    {
        return 0;
    }
}
