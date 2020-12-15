using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GarbageCollectionOptimiser : MonoBehaviour
{
    public int frameInterval;

    void Update()
    {
        //for now this makes garabage collection bigger?ikes

        if (Time.frameCount % frameInterval == 0)
        {
            System.GC.Collect();
        }
    }
}
