using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIComponent : MonoBehaviour
{
    GameEntity myEntity;

    public virtual void SetUpComponent(GameEntity entity)
    {
        myEntity = entity;
    }

    public virtual void UpdateComponent()
    {

    }
}
