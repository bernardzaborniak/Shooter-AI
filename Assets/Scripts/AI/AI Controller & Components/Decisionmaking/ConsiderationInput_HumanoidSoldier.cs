using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "AI/Consideration/Input", fileName = "Humanoid Soldier Input")]
public class ConsiderationInput_HumanoidSoldier : ScriptableObject
{
    public virtual void SetUpInput(AIController aiController)
    {

    }

    public virtual float GetConsiderationScore()
    {
        return 0;
    }
}

[CreateAssetMenu(menuName = "AI/Consideration/Input", fileName = "Humanoid Soldier Input - My Health")]
public class ConsiderationInput_HumanoidSoldier_MyHealth : ConsiderationInput_HumanoidSoldier
{
    public float min;
    public float max;

    public override void SetUpInput(AIController aiController)
    {

    }

    public override float GetConsiderationScore()
    {
        return 0;
    }

}