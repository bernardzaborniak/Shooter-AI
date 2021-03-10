using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace BenitosAI
{

    [CreateAssetMenu(menuName = "AI/Consideration Input/Humanoid/Are Enemies Shooting at Me", fileName = "Are Enemies Shooting at Me")]
    public class CI_HS_AreEnemiesShootingAtMe : ConsiderationInput
    {
        public override float GetConsiderationInput(DecisionContext decisionContext, ConsiderationInputParams considerationInputParams)
        {
            AIController_Blackboard blackboard = ((AIController_HumanoidSoldier)decisionContext.aiController).blackboard;
            SensedEntityInfo[] infos = blackboard.enemyInfos;

            for (int i = 0; i < infos.Length; i++)
            {
                //if(infos[i].entityTags.actionTags)
                foreach (EntityActionTag tag in infos[i].entityTags.actionTags)
                {
                    if(tag.type == EntityActionTag.Type.ShootingAtTarget)
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

