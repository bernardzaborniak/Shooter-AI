using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextToCameraAligner : MonoBehaviour
{
    //isnt updated by the visualisation manager but is a subsystem of visualiser componenents

    //You can add the script to the singleton manager either on start or on enable, 
    //if you use onEnable, make sure the manager is executed before the managedObjects 
    //in the scriptExecution Order serrings to prevent null Exceptions

    /*private void OnEnable()
    {
        VisualisationManager.Instance.AddTextAlignedToCamera(this);
    }*/

    /*void Start()
    {
        VisualisationManager.Instance.AddTextAlignedToCamera(this);
    }*/

    /*void OnDisable()
    {
        VisualisationManager.Instance.RemoveTextAlignedToCamera(this);
    }*/

    public void UpdateTextAligner(Vector3 cameraForward)
    {
        transform.forward = cameraForward;
    }
}
