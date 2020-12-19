using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "AI/Consideration/Humanoid/Input/My Health Ratio", fileName = "My Health Ratio")]
public class ConsiderationInput_HumanoidSoldier_MyHealthRatio : ConsiderationInput_HumanoidSoldier
{
    public override float GetConsiderationInput(AIController_HumanoidSoldier aiController, Consideration_HumanoidSoldier consideration)
    {
        return aiController.humanSensing.GetRemainingHealthToMaxHalthRatio();
    }

}
