using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "AI/Consideration/Humanoid/Consideration", fileName = "new Consideration_Humanoid")]
public class Consideration_HumanoidSoldier : Consideration
{
    public ConsiderationInput_HumanoidSoldier considerationInput;
    float input;

    //for ConsiderationInput_HumanoidSoldier_DistanceToClosestEnemy
    public float min;
    public float max;


    //TODO do an editor with hides and shows parameters depending on the considerationInput assigned

    public override float GetConsiderationRating(AIController aiController)
    {
        //Get Input, already normalized by ConsideraionInput
        input = considerationInput.GetConsiderationInput((AIController_HumanoidSoldier)aiController, this);

        //TODo Refactor, dont save input into another variable, write it directly into the return statement

        //Modify input by curve, curve also normalizes automaticly
        return considerationCurve.GetRemappedValue(input);
    }
}
