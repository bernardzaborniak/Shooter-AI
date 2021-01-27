using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BenitosAI
{

    [CreateAssetMenu(menuName = "AI/Consideration/Input/Humanoid/TPointCoverQualityForCurrentSituation", fileName = "TPointCoverQualityForCurrentSituation")]
    public class CI_HS_TPointCoverQualityForCurrentSituation : ConsiderationInput
    {
        public override float GetConsiderationInput(DecisionContext decisionContext, Consideration consideration)
        {
            //get  nearest enemies - check if crocuhed cover direction towards him has quality >0.7 - if yes -> cover quality = 1, else cover qulity = 0



            //float input = Utility.Remap(decisionContext.targetEntity.lastDistanceMeasured, consideration.min, consideration.max, 0, 1);
            //return Mathf.Clamp(input, 0, 1);
            return 0;

        }
    }

}

