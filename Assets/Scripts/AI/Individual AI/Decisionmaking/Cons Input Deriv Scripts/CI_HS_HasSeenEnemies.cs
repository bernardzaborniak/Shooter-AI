using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BenitosAI
{

    [CreateAssetMenu(menuName = "AI/Consideration Input/Humanoid/Has seen Enemies", fileName = "Has seen Enemies")]
    public class CI_HS_HasSeenEnemies : ConsiderationInput
    {
        [Tooltip("If the information about the enemy entity is older than x seconds, ignore it")]
        public float informationFreshnessThreshold = 1f;

        public override float GetConsiderationInput(DecisionContext decisionContext, Consideration consideration)
        {
            //dont forget to check how old an infomration about an enemy is
            SensedEntityInfo[] infos = ((AIController_HumanoidSoldier)decisionContext.aiController).blackboard.enemyInfos;
            if (infos.Length > 0)
            {
                for (int i = 0; i < infos.Length; i++)
                {
                    if (Time.time - infos[i].timeWhenLastSeen < informationFreshnessThreshold)
                    {
                        return 1;
                    }
                }
            }


            return 0;

        }
    }

}
