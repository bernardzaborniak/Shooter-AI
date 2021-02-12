using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public Rigidbody rb;
    public float launchVelocity;

    int projectileTeamID;
    GameEntity shooterEntity;
    public float damage;

    //override FF
    public bool overrideFriendlyFireSetting;
    [ShowWhen("overrideFriendlyFireSetting")]
    public bool overridenFriendlyFireOn;

    Vector3 velocityLastFrame;

    public ProjectileImpactEffectController impactEffectDisableController;

    // Start is called before the first frame update
    void Start()
    {
        //rb.velocity = transform.forward * launchVelocity;
    }

    private void FixedUpdate()
    {
        velocityLastFrame = rb.velocity;
    }

    public void Activate(bool shotByPlayer, GameEntity shooterEntity, Gun gunFromWhichItWasShot, Vector3 shootersMovementVelocity, float damage, float launchVelocity, bool overrideFriendlyFireSetting = false, bool overridenFriendlyFireOn = false)
    {
        //this.shotByPlayer = shotByPlayer;

        this.damage = damage;

        rb.velocity = transform.forward * launchVelocity + shootersMovementVelocity;

        if (shooterEntity)
        {
            projectileTeamID = shooterEntity.teamID;
        }
        else
        {
            Debug.Log("Warning: no shooter Entity set in Projectile");
        }
        this.shooterEntity = shooterEntity;

        this.overrideFriendlyFireSetting = overrideFriendlyFireSetting;
        this.overridenFriendlyFireOn = overridenFriendlyFireOn;

        impactEffectDisableController.Reset(transform);

        /*if (audioSource != null)
        {
            impactSoundDisableController.Reset(transform);

            if (flySound != null)
            {
                audioSource.loop = true;
                audioSource.clip = flySound;
                audioSource.Play();

            }
        }*/
    }

    private void OnCollisionEnter(Collision collision)
    {
        IDamageable<DamageInfo> damageable = collision.gameObject.GetComponent<IDamageable<DamageInfo>>();

        if(damageable != null)
        {
            bool damageBlockedCauseOfFriendlyFire = false;

            if (overrideFriendlyFireSetting)
            {
                if (!overridenFriendlyFireOn)
                {
                    if (damageable.GetTeamID() == projectileTeamID)
                    {
                        damageBlockedCauseOfFriendlyFire = true;
                    }

                }
            }
            else
            {
                if (!SceneSettings.Instance.allowFriendlyFire)
                {
                    if (damageable.GetTeamID() == projectileTeamID)
                    {
                        damageBlockedCauseOfFriendlyFire = true;
                    }
                }
            }

            if (!damageBlockedCauseOfFriendlyFire)
            {
                DamageInfo damageInfo = new DamageInfo(damage, null, velocityLastFrame * rb.mass, collision.contacts[0].point, collision.contacts[0].normal);

                damageable.TakeDamage(ref damageInfo);
            }          
        }

        impactEffectDisableController.EnableImpactEffect(collision.contacts[0].point, collision.contacts[0].normal);

        gameObject.SetActive(false);
    }



}
