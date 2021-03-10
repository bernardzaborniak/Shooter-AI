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
        public GameObject targetButton;


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
            tmp_targetName.text = memoryItem.targetName;


            if (memoryItem.target != null)
            {
                targetButton.SetActive(true);

                if (memoryItem.target is SensedEntityInfo)
                {
                    if (((SensedEntityInfo)memoryItem.target).entity)
                    {
                        referencedObjectTransform = ((SensedEntityInfo)memoryItem.target).entity.transform;
                    }
                }
                else if(memoryItem.target is System.ValueTuple<TacticalPoint, float>)
                {
                    referencedObjectTransform = ((System.ValueTuple<TacticalPoint, float>)memoryItem.target).Item1.transform;
                }
            }
            else
            {
                targetButton.SetActive(false);
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
