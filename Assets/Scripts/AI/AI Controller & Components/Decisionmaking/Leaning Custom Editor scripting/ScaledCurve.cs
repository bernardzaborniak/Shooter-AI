using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ScaledCurve 
{
    public float scale = 1;
    public AnimationCurve curve = AnimationCurve.Linear(0, 0, 1, 1);
}
