using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace BenitosAI
{

    public class AI_Vis_SelectedDecisionsVisualiser : MonoBehaviour
    {
        public Transform objectToAlignToCamera;
        
        [Space(5)]
        public TMP_Text tmp_decision1;
        public TMP_Text tmp_timeSinceSelectedDecision1;
        public TMP_Text tmp_decision1Rating;

        [Space(5)]
        public TMP_Text tmp_decision2;
        public TMP_Text tmp_timeSinceSelectedDecision2;
        public TMP_Text tmp_decision2Rating;

        [Space(5)]
        public TMP_Text tmp_healthRatio;
        public TMP_Text tmp_ammoRatio;


        public void UpdateVisualiser(Vector3 cameraForward, DecisionMaker.Memory.DecisionContextMemory selectedDecision1, DecisionMaker.Memory.DecisionContextMemory selectedDecision2)
        {
            objectToAlignToCamera.rotation = Quaternion.LookRotation(cameraForward);

            tmp_decision1.text = selectedDecision1.decision.name;
            tmp_timeSinceSelectedDecision1.text = (Time.time - selectedDecision1.timeOfDecison).ToString("F2");
            tmp_decision1Rating.text = selectedDecision1.rating.ToString("F2");

            tmp_decision2.text = selectedDecision2.decision.name;
            tmp_timeSinceSelectedDecision2.text = (Time.time - selectedDecision2.timeOfDecison).ToString("F2");
            tmp_decision2Rating.text = selectedDecision2.rating.ToString("F2");

            tmp_healthRatio.text = "";
            tmp_ammoRatio.text = "";
        }

        // Also updates ammo & health info
        public void UpdateVisualiser(Vector3 cameraForward, DecisionMaker.Memory.DecisionContextMemory selectedDecision1, DecisionMaker.Memory.DecisionContextMemory selectedDecision2, float healthRatio, float ammoRatio)
        {
            objectToAlignToCamera.rotation = Quaternion.LookRotation(cameraForward);

            tmp_decision1.text = selectedDecision1.decision.name;
            tmp_timeSinceSelectedDecision1.text = (Time.time - selectedDecision1.timeOfDecison).ToString("F2");
            tmp_decision1Rating.text = selectedDecision1.rating.ToString("F2");

            tmp_decision2.text = selectedDecision2.decision.name;
            tmp_timeSinceSelectedDecision2.text = (Time.time - selectedDecision2.timeOfDecison).ToString("F2");
            tmp_decision2Rating.text = selectedDecision2.rating.ToString("F2");

            tmp_healthRatio.text = "H: " + healthRatio.ToString("F2");
            tmp_ammoRatio.text = "A: " + ammoRatio.ToString("F2");
        }
    }
}

