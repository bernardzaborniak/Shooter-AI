using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "AI/Consideration/Humanoid/Input/Enemy Visible", fileName = "Enemy Visible")]
public class ConsiderationInput_HumanoidSoldier_EnemyVisible : ConsiderationInput_HumanoidSoldier
{
    public override float GetConsiderationInput(AIController_HumanoidSoldier aiController, Consideration_HumanoidSoldier consideration)
    {
        if (aiController.humanSensing.currentSensingInfo.enemiesInSensingRadius.Count > 0)
        {
            return 1;
        }
        else
        {
            return 0;
        }
    }
}
