using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "AI/Consideration/Humanoid/Input/My Health Ratio", fileName = "My Health Ratio")]
public class ConsiderationInput_HumanoidSoldier_MyHealthRatio : ConsiderationInput
{
    public override float GetConsiderationInput(AIController aiController, Consideration consideration)
    {
        return ((AIController_HumanoidSoldier)aiController).humanSensing.GetRemainingHealthToMaxHalthRatio();
    }

}
