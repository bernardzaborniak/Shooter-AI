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
        if (healthComponent == null) Debug.Log("health component null in hitbox: " + gameObject.name);
        return healthComponent.TakeDamage(ref damageInfo);
    }

    public int GetTeamID()
    {
        return myEntity.teamID;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            DamageInfo info = new DamageInfo(100);
            TakeDamage(ref info);
        }
    }
}
