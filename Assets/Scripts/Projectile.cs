using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public Rigidbody rb;
    public float launchVelocity;

    public float damage;

    Vector3 velocityLastFrame;

    // Start is called before the first frame update
    void Start()
    {
        //rb.velocity = transform.forward * launchVelocity;
    }

    private void FixedUpdate()
    {
        velocityLastFrame = rb.velocity;
    }

    private void OnCollisionEnter(Collision collision)
    {
        IDamageable<DamageInfo> damageable = collision.gameObject.GetComponent<IDamageable<DamageInfo>>();

        if(damageable != null)
        {
            DamageInfo damageInfo = new DamageInfo(damage, null, velocityLastFrame * rb.mass, collision.contacts[0].point, collision.contacts[0].normal);

            damageable.TakeDamage(ref damageInfo);

        }

        Destroy(gameObject);
    }


}
