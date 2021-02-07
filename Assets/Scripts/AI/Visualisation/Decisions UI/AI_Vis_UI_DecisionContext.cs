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
        AIVisualisationManager managerReference;

        public void SetUp(DecisionMemoryItem memoryItem, AIVisualisationManager managerReference, bool selectedDecision = false)
        {
            tmp_decisionName.text = memoryItem.decision.name;
            tmp_weight.text = memoryItem.decisionWeight.ToString("F2");
            if (memoryItem.rating == -1)
            {
                tmp_rating.text = "reject";
            }
            else if (selectedDecision)
            {
                tmp_rating.text = memoryItem.rating.ToString("F2") + " [S]";
            }
            else
            {
                tmp_rating.text = memoryItem.rating.ToString("F2");
            }
            
            tmp_timeSinceDecided.text = memoryItem.timeWhenDecided.ToString("F2");
            tmp_considerationNumber.text = memoryItem.decision.GetConsiderations().Length.ToString();


            timeWhenDecided = memoryItem.timeWhenDecided;

            // Set up for camera framing
            this.managerReference = managerReference;
            if(memoryItem.targetEntity != null)
            {
                referencedObjectTransform = memoryItem.targetEntity.transform;
                tmp_targetName.text = memoryItem.targetEntity.name + memoryItem.targetEntity.GetHashCode();

            }
            else if(memoryItem.targetTacticalPoint != null)
            {
                referencedObjectTransform = memoryItem.targetTacticalPoint.transform;
                tmp_targetName.text = memoryItem.targetTacticalPoint.name + memoryItem.targetTacticalPoint.GetHashCode();

            }
            else
            {
                tmp_targetName.text = "no Target";
            }

            Update();


            //Set Up Considerations:

            for (int i = 0; i < memoryItem.considerationsMemory.Length; i++)
            {
                AI_Vis_UI_Consideration spawnedConsiderationItem = Instantiate(considerationPrefab, considerationParent).GetComponent<AI_Vis_UI_Consideration>();
                spawnedConsiderationItem.SetUp(memoryItem.considerationsMemory[i]);
            }


        }

        /*public void UpdateTimeSinceLastSeen(float sensedTimeSinceLastSeen)
        {
            tmp_sensedTimeSinceLastSeen.text = sensedTimeSinceLastSeen.ToString("F1");
        }*/

        private void Update()
        {
            //just update the time
            tmp_timeSinceDecided.text = (Time.time - timeWhenDecided).ToString("F2");
            //tmp_sensedFrameCountLastSeen.text = (Time.frameCount - frameCountLastSensed).ToString("F1");
        }

        public void OnFrameOnObjectButtonClicked()
        {
            Debug.Log("button clicked");
            if(referencedObjectTransform != null)
            {
                managerReference.FrameCameraOnObject(referencedObjectTransform);
            }
        }
    }
}
