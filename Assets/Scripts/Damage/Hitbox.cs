using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hitbox : MonoBehaviour, IDamageable<DamageInfo>
{
    GameEntity myEntity;
    EC_Health healthComponent;
    public float damageMultiplier = 1;

    public void SetUp(GameEntity gameEntity, EC_Health healthComponent)
    {
        myEntity = gameEntity;
        this.healthComponent = healthComponent;
    }

    public bool TakeDamage(ref DamageInfo damageInfo)
    {
        damageInfo.damage *= damageMultiplier;
        return healthComponent.TakeDamage(ref damageInfo);
    }

    public int GetTeamID()
    {
        return myEntity.teamID;
    }
}
