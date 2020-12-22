using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TacticalPointVisibilityInfo : MonoBehaviour
{
    public TacticalPoint tacticalPointAssignedTo;

    public Vector3 GetPointPosition()
    {
        return tacticalPointAssignedTo.transform.position;
    }
}
