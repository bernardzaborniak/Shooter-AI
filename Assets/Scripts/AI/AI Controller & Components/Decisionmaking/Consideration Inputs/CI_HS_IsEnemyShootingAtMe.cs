using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace BenitosAI
{

    [CreateAssetMenu(menuName = "AI/Consideration/Input/Humanoid/IsEnemyShootingAtMe", fileName = "IsEnemyShootingAtMe")]
    public class CI_HS_IsEnemyShootingAtMe : ConsiderationInput
    {
        public override float GetConsiderationInput(DecisionContext decisionContext, Consideration consideration)
        {
            AIController_Blackboard blackboard = ((AIController_HumanoidSoldier)decisionContext.aiController).blackboard;
            SensedEntityInfo[] infos = blackboard.enemyInfos;

            for (int i = 0; i < infos.Length; i++)
            {
                //if(infos[i].entityTags.actionTags)
                foreach (EntityActionTag tag in infos[i].entityTags.actionTags)
                {
                    if(tag.type == EntityActionTagType.ShootingAtTarget)
                    {
                        if(tag.shootAtTarget == blackboard.GetMyEntity())
                        {
                            return 1;
                        }
                    }
                }
            }

            return 0;
        }
    }
}

