using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "AI/Consideration", fileName = "Humanoid Soldier Consideration")]
public class Consideration_HumanoidSoldier : Consideration
{
    public ConsiderationInput_HumanoidSoldier considerationInput;
    AIController aiController;

    public override void SetUpConsideration(AIController aiController)
    {
        this.aiController = aiController;

        //Set Up Input

        considerationInput.SetUpInput(aiController);
    }

    public override float GetConsiderationRating()
    {

    }
}
