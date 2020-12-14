using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class AI_VIS_UI_SensingItem : MonoBehaviour
{
    public TextMeshProUGUI tmp_sensedThingName;
    public TextMeshProUGUI tmp_sensedThingDistance;
    public TextMeshProUGUI tmp_sensedTimeSinceLastSeen;
    public TextMeshProUGUI tmp_sensedFrameCountLastSeen;

    int frameCountLastSensed;
    float timeLastSensed;

    public void SetUp(string sensedThingName, float sensedThingDistance, float timeLastSensed, int frameCountLastSensed)
    {
        tmp_sensedThingName.text = sensedThingName;
        tmp_sensedThingDistance.text = Mathf.Sqrt(sensedThingDistance).ToString("F1");
        this.timeLastSensed = timeLastSensed;
        this.frameCountLastSensed = frameCountLastSensed;

        Update();
    }

    /*public void UpdateTimeSinceLastSeen(float sensedTimeSinceLastSeen)
    {
        tmp_sensedTimeSinceLastSeen.text = sensedTimeSinceLastSeen.ToString("F1");
    }*/

    private void Update()
    {
        //just update the time
        tmp_sensedTimeSinceLastSeen.text = (Time.time - timeLastSensed).ToString("F2");
        tmp_sensedFrameCountLastSeen.text = (Time.frameCount - frameCountLastSensed).ToString("F1");
    }
}
