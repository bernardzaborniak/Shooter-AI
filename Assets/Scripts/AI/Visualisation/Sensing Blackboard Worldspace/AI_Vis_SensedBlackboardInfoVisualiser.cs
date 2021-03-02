using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class AI_Vis_SensedBlackboardInfoVisualiser : MonoBehaviour
{
    public Transform objectToAlignToCamera;
    public TMP_Text tmp_type;
    public TMP_Text tmp_name;
    public TMP_Text tmp_distance;
    public TMP_Text tmp_timeSinceLastSeen;
    public TMP_Text tmp_framesSinceLastSeen;

    float timeWhenLastSeen;
    int frameCountWhenLastSeen;

    public void UpdateVisualiser(Vector3 cameraForward)
    {
        tmp_timeSinceLastSeen.text = (Time.time - timeWhenLastSeen).ToString("F2");
        tmp_framesSinceLastSeen.text = (Time.frameCount - frameCountWhenLastSeen).ToString();

        //objectToAlignToCamera.rotation = Quaternion.LookRotation(-(cameraPosition - objectToAlignToCamera.position));
        objectToAlignToCamera.rotation = Quaternion.LookRotation(cameraForward);
    }

    public void SetUpForEnemyEntityInfo(BenitosAI.SensedEntityInfo entityInfo)
    {
        //tmp_type.gameObject.SetActive(true);
        if (entityInfo.IsAlive())
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

        transform.position = entityInfo.GetEntityPosition();


    }

    public void SetUpForFriendlyEntityInfo(BenitosAI.SensedEntityInfo entityInfo)
    {
        if (entityInfo.IsAlive())
        {
            tmp_type.text = "Friendly";
            tmp_name.text = entityInfo.entity.name + " " + entityInfo.entity.GetHashCode();
        }
        else
        {
            tmp_type.text = "Friendly";
            tmp_name.text = "He Dead";
        }

        tmp_distance.text = entityInfo.lastDistanceMeasured.ToString("F1");

        timeWhenLastSeen = entityInfo.timeWhenLastSeen;
        frameCountWhenLastSeen = entityInfo.frameCountWhenLastSeen;

        transform.position = entityInfo.GetEntityPosition();
    }

    public void SetUpForTPointCoverInfo((TacticalPoint tPoint, float distance) tPointInfo)
    {
        tmp_type.text = "TP Cover";
        tmp_name.text = tPointInfo.tPoint.name + " " + tPointInfo.tPoint.GetHashCode();
        tmp_distance.text = tPointInfo.distance.ToString("F1");

        tmp_timeSinceLastSeen.gameObject.SetActive(false);
        tmp_framesSinceLastSeen.gameObject.SetActive(false);

        transform.position = tPointInfo.tPoint.GetPointPosition();

    }

    public void SetUpForTPointOpenFieldInfo((TacticalPoint tPoint, float distance) tPointInfo)
    {
        tmp_type.text = "TP Open Field";
        tmp_name.text = tPointInfo.tPoint.name + " " + tPointInfo.tPoint.GetHashCode();
        tmp_distance.text = tPointInfo.distance.ToString("F1");

        tmp_timeSinceLastSeen.gameObject.SetActive(false);
        tmp_framesSinceLastSeen.gameObject.SetActive(false);

        transform.position = tPointInfo.tPoint.GetPointPosition();
    }

    public void SetUpForTPointCoverPeekInfo((TacticalPoint tPoint, float distance) tPointInfo)
    {
        tmp_type.text = "TP Cover Peek";
        tmp_name.text = tPointInfo.tPoint.name + " " + tPointInfo.tPoint.GetHashCode();
        tmp_distance.text = tPointInfo.distance.ToString("F1");

        tmp_timeSinceLastSeen.gameObject.SetActive(false);
        tmp_framesSinceLastSeen.gameObject.SetActive(false);

        transform.position = tPointInfo.tPoint.GetPointPosition();
    }

    public void SetUpForEnvironmentalDanger((BenitosAI.EnvironmentalDangerTag dangerTag, float distance) dangerInfo)
    {
        tmp_type.text = "Environmental Danger";
        tmp_name.text = dangerInfo.dangerTag.ToString() + " " + dangerInfo.dangerTag.GetHashCode();
        tmp_distance.text = dangerInfo.distance.ToString("F1");
        tmp_timeSinceLastSeen.gameObject.SetActive(false);
        tmp_framesSinceLastSeen.gameObject.SetActive(false);

        transform.position = dangerInfo.dangerTag.transform.position;
    }


    public void SetUpForCurrentlyUsedTPoint(TacticalPoint tPoint)
    {
        tmp_type.text = "Curr. used TP";
        tmp_name.text = tPoint.name + " " + tPoint.GetHashCode();
        tmp_distance.gameObject.SetActive(false);
        tmp_timeSinceLastSeen.gameObject.SetActive(false);
        tmp_framesSinceLastSeen.gameObject.SetActive(false);

        transform.position = tPoint.GetPointPosition();


    }
}
