using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIC_HumanSensing : AIComponent
{
    public GameEntity nearestEnemy;
    public float sensingInterval;

    public override void SetUpComponent(GameEntity entity)
    {
        base.SetUpComponent(entity);
    }

    public override void UpdateComponent()
    {

    }
}
