﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIControllerOptimiser : MonoBehaviour, IScriptOptimiser
{
    //instead of directly updating the sensing script, the optimiser just tells the sensing script when an update would be good and performant.

    [SerializeField] bool updateAIControllerNextTimePossible;

    private void OnEnable()
    {
        AIControllerOptimisationManager.Instance.AddOptimiser(this);
    }

    private void OnDisable()
    {
        AIControllerOptimisationManager.Instance.RemoveOptimiser(this);
    }

    public void UpdateOptimiser()
    {
        updateAIControllerNextTimePossible = true;
       // Debug.Log("updateAIControllerNextTimePossible = true ");
    }

    public Vector3 GetPosition()
    {
        return transform.position;
    }

    public bool ShouldAIControllerBeUpdated()
    {
        return updateAIControllerNextTimePossible;
    }

    public void OnAIControllerWasUpdated()
    {
        updateAIControllerNextTimePossible = false;
    }

    public string GetName()
    {
        return transform.parent.name;
    }
}
