using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace BenitosAI
{

    [CreateAssetMenu(menuName = "AI/Consideration Input/Humanoid/Is Reloading Weapon", fileName = "Is Reloading Weapon")]
    public class CI_HS_IsReloadingWeapon : ConsiderationInput
    {
        public override float GetConsiderationInput(DecisionContext decisionContext, Consideration consideration)
        {
            EntityTags tags = ((AIController_HumanoidSoldier)decisionContext.aiController).blackboard.GetMyEntity().entityTags;

            //if(infos[i].entityTags.actionTags)
            foreach (EntityActionTag tag in tags.actionTags)
            {
                if (tag.type == EntityActionTag.Type.ReloadingWeapon)
                {
                    return 1;
                }
            }
            return 0;
        }
    }
}

