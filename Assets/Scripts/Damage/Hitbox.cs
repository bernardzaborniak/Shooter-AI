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

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Keypad1))
        {
            Debug.Log("Damage Input 1");
            DamageInfo di = new DamageInfo(5);
            TakeDamage(ref di);
        }
        if (Input.GetKeyDown(KeyCode.Keypad2))
        {
            Debug.Log("Damage Input 2");
            DamageInfo di = new DamageInfo(15);
            TakeDamage(ref di);
        }
        if (Input.GetKeyDown(KeyCode.Keypad3))
        {
            Debug.Log("Damage Input 3");
            DamageInfo di = new DamageInfo(55);
            TakeDamage(ref di);
        }
    }


}
