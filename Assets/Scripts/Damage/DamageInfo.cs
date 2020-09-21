using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageInfo 
{
    public float damage;
    //public bool appliesForce;
    [Tooltip("This force is applied to the corpse/ragdoll upon death.")]
    public Vector3 force; 
    public GameEntity damageGiver;
    public DamageType type;

    public Vector3 damageDealPoint;
    public Vector3 damageDealPointNormal;

    public DamageInfo(float damage)
    {
        this.damage = damage;
        //appliesForce = false;
        this.damageGiver = null;
        this.force = Vector3.zero;
        this.damageDealPoint = Vector3.zero;
        this.damageDealPointNormal = Vector3.zero;

        type = DamageType.Default;   
    }

    public DamageInfo(float damage, GameEntity damageGiver, Vector3 force)
    {
        this.damage = damage;
        //appliesForce = true;
        this.damageGiver = damageGiver;
        this.force = force;
        this.damageDealPoint = Vector3.zero;
        this.damageDealPointNormal = Vector3.zero;

        type = DamageType.Default;
    }

    public DamageInfo(float damage, GameEntity damageGiver, Vector3 force, Vector3 damageDealPoint, Vector3 damageDealPointNormal)
    {
        this.damage = damage;
        this.damageGiver = damageGiver;
        this.force = force;
        this.damageDealPoint = damageDealPoint;
        this.damageDealPointNormal = damageDealPointNormal;
        //appliesForce = false;
       
        type = DamageType.Default;
    }
}
