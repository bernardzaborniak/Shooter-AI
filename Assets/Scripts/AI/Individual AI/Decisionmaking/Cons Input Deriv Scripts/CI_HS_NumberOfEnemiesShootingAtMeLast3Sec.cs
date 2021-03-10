using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace BenitosAI
{

    [CreateAssetMenu(menuName = "AI/Consideration Input/Humanoid/Number of Enemies Shooting at Me - last 3s", fileName = "Number of Enemies Shooting at Me - last 3s")]
    public class CI_HS_NumberOfEnemiesShootingAtMeLast3Sec : ConsiderationInput
    {
        void OnEnable()
        {
            inputParamsType = new ConsiderationInputParams.InputParamsType[]
            {
                ConsiderationInputParams.InputParamsType.Range
            };
        }

        //change it to be more flexible like enemies seen last 3 seconds?
        public override float GetConsiderationInput(DecisionContext decisionContext, ConsiderationInputParams considerationInputParams)
        {
            float input = Utility.Remap(((AIController_HumanoidSoldier)decisionContext.aiController).blackboard.numberOfEnemiesShootingAtMeLast3Sec, considerationInputParams.min, considerationInputParams.max, 0, 1);
            return Mathf.Clamp(input, 0, 1);
        }
    }
}

