using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public Rigidbody rb;
    public float launchVelocity;

    // Start is called before the first frame update
    void Start()
    {
        rb.velocity = transform.forward * launchVelocity;
    }


}
