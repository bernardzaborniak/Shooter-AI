using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


namespace BenitosAI
{
    


    public class AI_Vis_UI_Consideration : MonoBehaviour
    {
        public TextMeshProUGUI tmp_considerationName;
        public TextMeshProUGUI tmp_considerationInput;
        public TextMeshProUGUI tmp_considerationOutput;

        public void SetUp(DecisionMemoryItem.ConsiderationMemory considerationMemory)
        {
            tmp_considerationName.text = considerationMemory.considerationName;
            tmp_considerationInput.text = considerationMemory.input.ToString("F");
            tmp_considerationOutput.text = considerationMemory.rating.ToString("F");
        }

    }
}
