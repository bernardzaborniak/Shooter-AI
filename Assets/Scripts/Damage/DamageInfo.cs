using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageInfo 
{
    public float damage;
    public bool appliesForce;
    [Tooltip("This force is applied to the corpse/ragdoll upon death.")]
    public Vector3 killPushForce; 
    public GameEntity damageGiver;
    public DamageType type;

    public Vector3 damageDealPoint;
    public Vector3 damageDealPointNormal;

    public DamageInfo(float damage)
    {
        this.damage = damage;
        appliesForce = false;
        damageGiver = null;
        type = DamageType.Default;
    }

    public DamageInfo(float damage, Vector3 damageForce, GameEntity damageGiver)
    {
        this.damage = damage;
        appliesForce = true;
        this.killPushForce = damageForce;
        this.damageGiver = damageGiver;
        type = DamageType.Default;
    }

    public DamageInfo(float damage, GameEntity damageGiver, Vector3 damageDealPoint, Vector3 damageDealPointNormal)
    {
        this.damageDealPoint = damageDealPoint;
        this.damage = damage;
        this.damageDealPointNormal = damageDealPointNormal;
        appliesForce = false;
        this.damageGiver = damageGiver;
        type = DamageType.Default;
    }
}
