using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GarbageCollectionOptimiser : MonoBehaviour
{

    void Update()
    {
        //for now this makes garabage collection bigger?
        if (Time.frameCount % 30 == 0)
        {
            System.GC.Collect();
        }
    }
}
