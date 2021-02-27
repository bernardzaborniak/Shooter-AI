using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace BenitosAI
{

    [CreateAssetMenu(menuName = "AI/ConsiderationInput/Humanoid/BalanceOfPower", fileName = "BalanceOfPower")]
    public class CI_HS_BalanceOfPower : ConsiderationInput
    {
        public override float GetConsiderationInput(DecisionContext decisionContext, Consideration consideration)
        {
            //AIController_Blackboard blackboard = ((AIController_HumanoidSoldier)decisionContext.aiController).blackboard;

            float input = Utility.Remap(((AIController_HumanoidSoldier)decisionContext.aiController).blackboard.currentBalanceOfPower, consideration.min, consideration.max, 0, 1);
            return Mathf.Clamp(input, 0, 1);

            //return blackboard.currentBalanceOfPower;
            /*SensedEntityInfo[] infos = blackboard.enemyInfos;

            for (int i = 0; i < infos.Length; i++)
            {
                //if(infos[i].entityTags.actionTags)
                foreach (EntityActionTag tag in infos[i].entityTags.actionTags)
                {
                    if (tag.type == EntityActionTag.Type.ShootingAtTarget)
                    {
                        if (tag.shootAtTarget == blackboard.GetMyEntity())
                        {
                            return 1;
                        }
                    }
                }
            }

            return 0;*/
        }
    }
}
