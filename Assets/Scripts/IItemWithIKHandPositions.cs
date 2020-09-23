using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IItemWithIKHandPositions 
{
    Vector3 GetRightHandIKPosition();
    Vector3 GetLeftHandIKPosition();
    Quaternion GetRightHandIKRotation();

    Quaternion GetLeftHandIKRotation();
}
