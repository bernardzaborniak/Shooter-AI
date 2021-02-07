using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace BenitosAI
{

    public class AI_Vis_UI_SensingItem : MonoBehaviour
    {
        public TextMeshProUGUI tmp_sensedThingName;
        public TextMeshProUGUI tmp_sensedThingDistance;
        public TextMeshProUGUI tmp_sensedTimeSinceLastSeen;
        public TextMeshProUGUI tmp_sensedFrameCountLastSeen;

        int frameCountLastSensed;
        float timeLastSensed;

        Transform referencedObjectTransform;
        AIVisualisationManager managerReference;

        public void SetUp(string sensedThingName, float sensedThingDistance, float timeLastSensed, int frameCountLastSensed, Transform referencedObjectTransform, AIVisualisationManager managerReference)
        {
            tmp_sensedThingName.text = sensedThingName;
            //tmp_sensedThingDistance.text = Mathf.Sqrt(sensedThingDistance).ToString("F1");
            tmp_sensedThingDistance.text = sensedThingDistance.ToString("F1");
            this.timeLastSensed = timeLastSensed;
            this.frameCountLastSensed = frameCountLastSensed;

            this.referencedObjectTransform = referencedObjectTransform;
            this.managerReference = managerReference;

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

        public void OnFrameOnObjectButtonClicked()
        {
            Debug.Log("button clicked");
            managerReference.FrameCameraOnObject(referencedObjectTransform);
        }
    }

}
