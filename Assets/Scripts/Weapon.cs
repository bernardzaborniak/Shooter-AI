using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public Transform rightHandIKPosition;
    public Transform leftHandIKPosition;

    [Tooltip("Will be read when by aimingController when equipping a weapon, makes sure the offset between weapon and shoulder is correct")]
    public Vector3 weaponAimParentLocalAdjusterOffset;
}
