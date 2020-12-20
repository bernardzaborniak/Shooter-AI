using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "AI/Consideration/Humanoid/Input/DistanceToClosestEnemy", fileName = "DistanceToClosestEnemy")]
public class ConsiderationInput_HumanoidSoldier_DistanceToClosestEnemy : ConsiderationInput
{
    AIController_HumanoidSoldier aiControllerHuman;

    public override float GetConsiderationInput(AIController aiController, Consideration consideration)
    {
        aiControllerHuman = (AIController_HumanoidSoldier)aiController;

        if (aiControllerHuman.humanSensing.currentSensingInfo.nearestEnemyInfo != null)
        {
            //normalize
            float input = Utility.Remap(aiControllerHuman.humanSensing.currentSensingInfo.nearestEnemyInfo.lastSquaredDistanceMeasured, consideration.minSquared, consideration.maxSquared, 0, 1);
            //remap Squared Distance
            //float input = 0

            return Mathf.Clamp(input, 0, 1);
        }
        else
        {
            return Mathf.Infinity;
        }

    }
}
