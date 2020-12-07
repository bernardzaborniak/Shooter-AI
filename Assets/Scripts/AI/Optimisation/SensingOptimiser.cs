using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SensingOptimiser : MonoBehaviour, IScriptOptimiser
{
    //instead of directly updating the sensing script, the optimiser just tells the sensing script when an update would be good and performant.

    [SerializeField] bool updateSensingNextTimePossible;

    private void OnEnable()
    {
         SensingOptimisationManager.Instance.AddOptimiser(this);
    }

    private void OnDisable()
    {
         SensingOptimisationManager.Instance.RemoveOptimiser(this);
    }

    public void UpdateOptimiser()
    {
        updateSensingNextTimePossible = true;
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
