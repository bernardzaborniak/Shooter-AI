using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIController : MonoBehaviour
{
    public DecisionMaker[] decisionLayers;

    void Start()
    {
        for (int i = 0; i < decisionLayers.Length; i++)
        {
            decisionLayers[i].SetUpDecisionLayer(this);
        }
        
    }

    void Update()
    {
        for (int i = 0; i < decisionLayers.Length; i++)
        {
            decisionLayers[i].Decide();
        }
    }
}
