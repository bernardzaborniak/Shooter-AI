using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class RaycastUsedToGenerateCoverRating
{
    public Vector3 start;
    public Vector3 end;
    public float distance;

    bool infinite;

    public RaycastUsedToGenerateCoverRating(Vector3 start, Vector3 end, bool infinite = false)
    {
        this.start = start;
        this.end = end;

        this.infinite = infinite;

        if (infinite)
        {
            distance = Mathf.Infinity;
        }
        else
        {
            distance = Vector3.Distance(end, start);
        }
    }

    public bool IsInfinite()
    {
        return infinite;
    }
}
