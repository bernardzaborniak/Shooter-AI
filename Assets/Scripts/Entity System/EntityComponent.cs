﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityComponent : MonoBehaviour
{
    protected GameEntity myEntity;

    public virtual void SetUpComponent(GameEntity entity)
    {
        myEntity = entity;
    }

    public virtual void UpdateComponent()
    {

    }

    public virtual void FixedUpdateComponent()
    {

    }

    public virtual void LateUpdateComponent()
    {

    }

    public virtual void OnDie(ref DamageInfo damageInfo)
    {

    }

    public virtual void OnTakeDamage(ref DamageInfo damageInfo)
    {

    }
}
