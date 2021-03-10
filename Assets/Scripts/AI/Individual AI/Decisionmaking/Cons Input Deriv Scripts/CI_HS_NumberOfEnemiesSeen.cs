using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace BenitosAI
{

    [CreateAssetMenu(menuName = "AI/Consideration Input/Humanoid/Number of Enemies Seen", fileName = "Number of Enemies Seen")]
    public class CI_HS_NumberOfEnemiesSeen : ConsiderationInput
    {
        void OnEnable()
        {
            inputParamsType = new ConsiderationInputParams.InputParamsType[]
            {
                ConsiderationInputParams.InputParamsType.Range,
                //ConsiderationInputParams.InputParamsType.InformationFreshness
            };
        }



        public override float GetConsiderationInput(DecisionContext decisionContext, ConsiderationInputParams considerationInputParams)
        {
            //dont forget to check how old an infomration about an enemy is?
            float input = Utility.Remap(((AIController_HumanoidSoldier)decisionContext.aiController).blackboard.enemyInfos.Length, considerationInputParams.min, considerationInputParams.max, 0, 1);
            return Mathf.Clamp(input, 0, 1);
        }
    }

}
