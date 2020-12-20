using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//maybe we could do without this class?

//[CreateAssetMenu(menuName = "AI/Consideration/Input", fileName = "Humanoid Soldier Input")]
public class ConsiderationInput : ScriptableObject
{
    public virtual float GetConsiderationInput(AIController aiController, Consideration consideration)
    {
        return 0;
    }
}
