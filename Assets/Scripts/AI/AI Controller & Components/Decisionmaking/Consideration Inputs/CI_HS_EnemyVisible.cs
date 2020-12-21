﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BenitosAI
{

    [CreateAssetMenu(menuName = "AI/Consideration/Input/Humanoid/Enemy Visible", fileName = "Enemy Visible")]
    public class CI_HS_EnemyVisible : ConsiderationInput
    {
        public override float GetConsiderationInput(DecisionContext decisionContext, Consideration consideration)
        {
            if (((AIController_HumanoidSoldier)decisionContext.aiController).humanSensing.currentSensingInfo.enemiesInSensingRadius.Count > 0)
            {
                return 1;
            }
            else
            {
                return 0;
            }
        }
    }

}
