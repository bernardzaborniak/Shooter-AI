using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace BenitosAI
{

    [CreateAssetMenu(menuName = "AI/ConsiderationInput/Humanoid/NumberOfEnemiesShootingAtMeLast3Sec", fileName = "NumberOfEnemiesShootingAtMeLast3Sec")]
    public class CI_HS_NumberOfEnemiesShootingAtMeLast3Sec : ConsiderationInput
    {
        public override float GetConsiderationInput(DecisionContext decisionContext, Consideration consideration)
        {
            float input = Utility.Remap(((AIController_HumanoidSoldier)decisionContext.aiController).blackboard.numberOfEnemiesShootingAtMeLast3Sec, consideration.min, consideration.max, 0, 1);
            return Mathf.Clamp(input, 0, 1);
        }
    }
}

