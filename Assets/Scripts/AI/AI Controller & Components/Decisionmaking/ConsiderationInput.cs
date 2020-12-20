using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//maybe we could do without this class?

public class ConsiderationInput : ScriptableObject
{
    public virtual float GetConsiderationInput(AIController aiController, Consideration consideration)
    {
        return 0;
    }
}
