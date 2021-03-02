using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace BenitosAI
{

    [CreateAssetMenu(menuName = "AI/Consideration Input/Humanoid/Is In Danger Of Grenade Explosion", fileName = "Is In Danger Of Grenade Explosion")]

    public class CI_HS_IsInDangerOfGrenadeExplosion : ConsiderationInput
    {
        public override float GetConsiderationInput(DecisionContext decisionContext, Consideration consideration)
        {
            (EnvironmentalDangerTag danger, float distance)[] dangersInfos = ((AIController_HumanoidSoldier)decisionContext.aiController).blackboard.environmentalDangerInfos;

            for (int i = 0; i < dangersInfos.Length; i++)
            {
                if(dangersInfos[i].distance < 4.5)
                {
                    return 1;
                }
            }

            return 0;
        }
    }
}
