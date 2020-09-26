using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grenade : Item
{
    public Rigidbody rigidbody;

    [Tooltip("How long does the throwing animation takes place? - or how long does the delay between ordering the action and the action being executed takes place")]
    public float throwingTime;

    public float maxThrowVelocity;
    //TODo Improve this for it to be a variable


    public void Throw(Vector3 direction, float throwVelocity)
    {
        transform.SetParent(null);
        rigidbody.isKinematic = false;

        rigidbody.velocity = direction.normalized * throwVelocity;
    }
}
