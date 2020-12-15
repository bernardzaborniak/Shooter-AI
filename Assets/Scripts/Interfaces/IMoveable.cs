using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IMoveable
{
    Vector3 GetCurrentVelocity();
    Vector3 GetCurrentAngularVelocity();
}
