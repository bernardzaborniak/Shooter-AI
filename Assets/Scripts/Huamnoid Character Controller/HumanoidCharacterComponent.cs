using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HumanoidCharacterComponent : MonoBehaviour
{
    protected GameEntity myEntity;

    public virtual void SetUpComponent(GameEntity entity)
    {
        myEntity = entity;
    }

    public virtual void UpdateComponent()
    {

    }
}
