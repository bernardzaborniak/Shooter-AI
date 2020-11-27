using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TacticalPointsGeneratorBox : MonoBehaviour
{
    HashSet<TacticalPoint> tacticalPointsGenerated;
    [Range(0,5)]
    public int testInt;

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    public void Generate()
    {
        Debug.Log("Genrate clicked");
    }
}
