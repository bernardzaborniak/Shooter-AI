using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIC_HumanSensing : AIComponent
{
    public GameEntity nearestEnemy;
    public HashSet<GameEntity> enemiesInSensingRadius = new HashSet<GameEntity>();
    Collider[] collidersInRadius;

    public float sensingInterval;
    float nextSensingTime;
    public float sensingRadius;
    public LayerMask sensingLayerMask;

    int myTeamID;

    public override void SetUpComponent(GameEntity entity)
    {
        base.SetUpComponent(entity);

        nextSensingTime = Time.time + Random.Range(0, sensingInterval);
        myTeamID = myEntity.teamID;
    }

    public override void UpdateComponent()
    {
        if(Time.time > nextSensingTime)
        {
            nextSensingTime = Time.time + sensingInterval;

            collidersInRadius = Physics.OverlapSphere(transform.position, sensingRadius, sensingLayerMask);
            
            enemiesInSensingRadius.Clear();

            float smallestDistance = Mathf.Infinity;
            float currentDistance;
            GameEntity currentEntity;
            Vector3 myPosition = transform.position;

            for (int i = 0; i < collidersInRadius.Length; i++)
            {
                currentEntity = collidersInRadius[i].GetComponent<GameEntity>();
                if (currentEntity)
                {
                    if(currentEntity.teamID != myTeamID)
                    {
                        enemiesInSensingRadius.Add(currentEntity);

                        currentDistance = (myPosition - currentEntity.transform.position).sqrMagnitude;
                        if (currentDistance < smallestDistance)
                        {
                            smallestDistance = currentDistance;
                            nearestEnemy = currentEntity;
                        }
                    }           
                }
            }

        }
    }
}
