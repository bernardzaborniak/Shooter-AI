using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace BenitosAI
{

    public class AI_Vis_UI_DecisionContext : MonoBehaviour
    {
        public TextMeshProUGUI tmp_decisionName;
        public TextMeshProUGUI tmp_rating;
        public TextMeshProUGUI tmp_weight;
        public TextMeshProUGUI tmp_timeSinceDecided;
        public TextMeshProUGUI tmp_targetName;
        public TextMeshProUGUI tmp_considerationNumber;

        [Space(5)]
        public RectTransform considerationParent;
        public GameObject considerationPrefab;


        float timeWhenDecided;

        Transform referencedObjectTransform;
        AIVisualisationManager managerReference; //Needed for framing camera - would be better of as a singleton?

        public void SetUp(DecisionMaker.Memory.DecisionContextMemory memoryItem, AIVisualisationManager managerReference)//, bool selectedDecision = false)
        {
            tmp_decisionName.text = memoryItem.decision.name;
            tmp_weight.text = memoryItem.weight.ToString("F2");
            if (memoryItem.rating == -1)
            {
                tmp_rating.text = "reject";
            }
            else
            {
                tmp_rating.text = memoryItem.rating.ToString("F2");
            }
            
            tmp_timeSinceDecided.text = memoryItem.timeOfDecison.ToString("F2");
            tmp_considerationNumber.text = memoryItem.decision.considerations.Length.ToString();


            timeWhenDecided = memoryItem.timeOfDecison;

            // Set up for camera framing
            this.managerReference = managerReference;
            
            if(memoryItem.targetEntity != null)
            {
                referencedObjectTransform = memoryItem.targetEntity.transform;
                tmp_targetName.text = memoryItem.targetEntityName;
            }    
            else if(memoryItem.targetTacticalPoint != null)
            {
                referencedObjectTransform = memoryItem.targetTacticalPoint.transform;
                tmp_targetName.text = memoryItem.targetTacticalPoint.name + memoryItem.targetTacticalPoint.GetHashCode();

            }
            else
            {
                tmp_targetName.text = memoryItem.targetEntityName;
            }



            Update();


            //Set Up Considerations:

            for (int i = 0; i < memoryItem.considerationsMemory.Length; i++)
            {
                AI_Vis_UI_Consideration spawnedConsiderationItem = Instantiate(considerationPrefab, considerationParent).GetComponent<AI_Vis_UI_Consideration>();
                spawnedConsiderationItem.SetUp(memoryItem.considerationsMemory[i]);
            }


        }

        private void Update()
        {
            //just update the time
            tmp_timeSinceDecided.text = (Time.time - timeWhenDecided).ToString("F2");
        }

        public void OnFrameOnObjectButtonClicked()
        {
            Debug.Log("OnFrame button clicked");
            if(referencedObjectTransform != null)
            {
                managerReference.FrameCameraOnObject(referencedObjectTransform);
            }
        }
    }
}
