using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grenade : Item
{
    [Header("Damage & Explosion Logic")]
    public float explosionRadius;
    //damage is descending linearly for now
    public float explosionDamageAtCenter;
    public float explosionForceAtCenter;
    public enum GrenadeType
    {
        TimeDelay,
        Impact
    }
    public GrenadeType grenadeType;
    public DamageType damageType;
    //Expand later so frag grenade has a numer of pieces and thus resulting a percentage of how many pieces hit you where and their damage
    bool armed;// rename it when i introduce proper grenade logic for vr
    float explosionTime;
    public float grenadeExplosionTimeDelay;
    bool exploded;

    [Tooltip("walls & soldiers should be inside this layermask")]
    public LayerMask grenadeExplosionBlockingUnitsAndBlockingSurfacesLayermask;
    [Tooltip("This Layers block grenade damage")]
    public LayerMask grenadeExplosionBlockingLayerMask;

    [Header ("Throwing & Collision")]

    public Rigidbody rigidbody;
    [Tooltip("How long does the throwing animation takes place? - or how long does the delay between ordering the action and the action being executed takes place")]
    public float throwingTime;
    public float throwVelocityAt10mDistance;
    [Tooltip("I apply my own friction")]
    public float customFriction;

    [Header("Explosion Effect")]
    public ParticleSystem explosionEffect;
    public GameObject model;

    [Header("AI")]
    public float dangerTagActivationDelayAfterThrow = 1;
    float nextActivateDangerTagTime;
    bool thrownOrDropped = false;
    public BenitosAI.EnvironmentalDangerTag environmentalDangerTag;

    public void Throw(Vector3 direction, float throwVelocity)
    {
        transform.SetParent(null);
        rigidbody.isKinematic = false;

        Debug.Log("Grenade Throw: " + direction + " " + throwVelocity);
        rigidbody.velocity = direction.normalized * throwVelocity;
        Debug.Log("Grenade Throw: vel: " + rigidbody.velocity );


        rigidbody.interpolation = RigidbodyInterpolation.Interpolate;

        nextActivateDangerTagTime = Time.time + dangerTagActivationDelayAfterThrow;
        thrownOrDropped = true;
    }

    public void ArmGrenade()
    {
        armed = true;
        //environmentalDangerTag.dangerActive = true;

        if (grenadeType == GrenadeType.TimeDelay)
        {
            explosionTime = Time.time + grenadeExplosionTimeDelay;
        }
    }

    private void Update()
    {
        if (!exploded)
        {
            if (thrownOrDropped)
            {
                //Debug.Log("Grenade flying: " + rigidbody.velocity);

                if (Time.time > nextActivateDangerTagTime)
                {
                    environmentalDangerTag.dangerActive = true;
                }
            }
           
            if (armed)
            {
                if (grenadeType == GrenadeType.TimeDelay)
                {
                    if (Time.time > explosionTime)
                    {
                        Explode();
                    }
                }
            }
           
        }  
    }

    private void OnCollisionStay(Collision collision)
    {
        //Debug.Log("grenade collides with : " + collision.collider.name);

        Vector3 velocityHorizontalOnly = new Vector3(rigidbody.velocity.x, 0, rigidbody.velocity.z);
        float velocityMagnitude = velocityHorizontalOnly.magnitude;
        //rigidbody.velocity += -rigidbody.velocity.normalized * Mathf.Clamp(velocityMagnitude, -customFriction * Time.time, customFriction * Time.time);
        rigidbody.velocity += Vector3.ClampMagnitude(-velocityHorizontalOnly.normalized * customFriction * Time.time, velocityMagnitude);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (armed)
        {
            if (grenadeType == GrenadeType.Impact)
            {
                Explode();
            }
        } 
    }

    void Explode()
    {
        exploded = true;
        environmentalDangerTag.dangerActive = false;
        model.SetActive(false);
        explosionEffect.Play();

        //damage
        Collider[] collidersInExplosionRange = Physics.OverlapSphere(transform.position, explosionRadius, grenadeExplosionBlockingUnitsAndBlockingSurfacesLayermask);
        HashSet<GameEntity> entitiesWhichAlreadyRecievedDamage = new HashSet<GameEntity>();

        Vector3 grenadePosition = transform.position;
        Debug.Log("Grewnae Explositon Damage -------------------------------------------------------");


        for (int i = 0; i < collidersInExplosionRange.Length; i++)
        {
            IDamageable<DamageInfo> damageable = collidersInExplosionRange[i].gameObject.GetComponent<IDamageable<DamageInfo>>();

            if (damageable != null)
            {
                if (!entitiesWhichAlreadyRecievedDamage.Contains(damageable.GetGameEntity()))
                {
                    Vector3 directionFromGrenade = collidersInExplosionRange[i].gameObject.transform.position - grenadePosition;

                    //check if no obstruction is there
                    bool grenadeFragmentBlockedByObstruction = false;

                    RaycastHit hit;
                    if (Physics.Raycast(transform.position, directionFromGrenade, out hit, explosionRadius, grenadeExplosionBlockingLayerMask))
                    {
                        if (hit.collider.gameObject != collidersInExplosionRange[i].gameObject)
                        {
                            grenadeFragmentBlockedByObstruction = true;
                        }
                    }

                    if (!grenadeFragmentBlockedByObstruction)
                    {
                        float distance = directionFromGrenade.magnitude;
                        Debug.Log("damageoble give damage: " + collidersInExplosionRange[i].name + "-----");

                        Debug.Log("distance to target was: " + distance);
                        float damageAndForceModifier = 1 - (distance / explosionRadius);
                        if (damageAndForceModifier < 0) damageAndForceModifier = 0;
                        Debug.Log("damageAndForceModifier: " + damageAndForceModifier);
                        Debug.Log("damage: " + (explosionDamageAtCenter * damageAndForceModifier));

                        DamageInfo damInfo = new DamageInfo(explosionDamageAtCenter * damageAndForceModifier, null, directionFromGrenade.normalized * explosionForceAtCenter * damageAndForceModifier);
                        damageable.TakeDamage(ref damInfo);

                        entitiesWhichAlreadyRecievedDamage.Add(damageable.GetGameEntity());
                    }
                }
            }
        }

        Destroy(gameObject, 5);

    }
}
