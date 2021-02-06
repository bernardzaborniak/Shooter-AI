using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


public class AI_Vis_SelectedDecisionsVisualiser : MonoBehaviour
{
    public Transform objectToAlignToCamera;

    public TMP_Text tmp_decision1;
    public TMP_Text tmp_timeSinceSelectedDecision1;

    //float timeWhenDecision1Selected;

    public TMP_Text tmp_decision2;
    public TMP_Text tmp_timeSinceSelectedDecision2;

    //float timeWhenDecision2Selected;


    public void UpdateVisualiser(Vector3 cameraForward, string decisionLayer1Name, float decisionLayer1SelectedTime, string decisionLayer2Name, float decisionLayer2SelectedTime)
    {
        objectToAlignToCamera.rotation = Quaternion.LookRotation(cameraForward);

        tmp_decision1.text = decisionLayer1Name;
        tmp_timeSinceSelectedDecision1.text = (Time.time - decisionLayer1SelectedTime).ToString("F2");

        tmp_decision2.text = decisionLayer2Name;
        tmp_timeSinceSelectedDecision2.text = (Time.time - decisionLayer2SelectedTime).ToString("F2");

    }

    /*public void UpdateVisualiser(Vector3 cameraForward)
    {

        tmp_timeSinceSelectedDecision1.text = (Time.time - timeWhenDecision1Selected).ToString("F2");
        tmp_timeSinceSelectedDecision2.text = (Time.frameCount - timeWhenDecision2Selected).ToString("F2");

        //objectToAlignToCamera.rotation = Quaternion.LookRotation(-(cameraPosition - objectToAlignToCamera.position));
        objectToAlignToCamera.rotation = Quaternion.LookRotation(cameraForward);
    }

    public void SetUpForDecisionSelected(BenitosAI.DecisionContext decisionContext)
    {
        //tmp_type.gameObject.SetActive(true);
       /* if (entityInfo.IsAlive())
        {
            tmp_type.text = "Enemy";
            tmp_name.text = entityInfo.entity.name + " " + entityInfo.entity.GetHashCode();
        }
        else
        {
            tmp_type.text = "Enemy";
            tmp_name.text = "He Dead";
        }

        tmp_distance.text = entityInfo.lastDistanceMeasured.ToString("F1");

        timeWhenLastSeen = entityInfo.timeWhenLastSeen;
        frameCountWhenLastSeen = entityInfo.frameCountWhenLastSeen;

        transform.position = entityInfo.GetEntityPosition();*/


   // }
}
