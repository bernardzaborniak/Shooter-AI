using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "AI/Consideration/Humanoid/Input/Enemy Visible", fileName = "Enemy Visible")]
public class ConsiderationInput_HumanoidSoldier_EnemyVisible : ConsiderationInput
{

    public override float GetConsiderationInput(AIController aiController, Consideration consideration)
    {
        if (((AIController_HumanoidSoldier)aiController).humanSensing.currentSensingInfo.enemiesInSensingRadius.Count > 0)
        {
            return 1;
        }
        else
        {
            return 0;
        }
    }
}
