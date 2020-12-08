using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SensingOptimiser : MonoBehaviour, IScriptOptimiser
{
    //instead of directly updating the sensing script, the optimiser just tells the sensing script when an update would be good and performant.

    [SerializeField] bool updateSensingNextTimePossible;

    //float lastSenseTime;
    //int lastSenseFrameCount;

    private void OnEnable()
    {
         SensingOptimisationManager.Instance.AddOptimiser(this);
        //lastSenseTime = Time.time;
        //lastSenseFrameCount = Time.frameCount;
    }

    private void OnDisable()
    {
         SensingOptimisationManager.Instance.RemoveOptimiser(this);
    }

    public void UpdateOptimiser()
    {
        updateSensingNextTimePossible = true;

       // Debug.Log("updated after " + (Time.time - lastSenseTime) + " s & " + (Time.frameCount - lastSenseFrameCount) + " frames");
        //Debug.Log("hash: " + gameObject.GetHashCode());
        //lastSenseTime = Time.time;
        //lastSenseFrameCount = Time.frameCount;
    }

    public Vector3 GetPosition()
    {
        return transform.position;
    }

    public bool ShouldSensingBeUpdated()
    {
        return updateSensingNextTimePossible;
    }

    public void OnSensingWasUpdated()
    {
        updateSensingNextTimePossible = false;
    }

    public string GetName()
    {
        return transform.parent.name;
    }
}
