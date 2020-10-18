using System;
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

    public HashSet<Tuple<Post,float>> postsInSensingRadius = new HashSet<Tuple<Post, float>>();
    public LayerMask postSensingLayerMask;

    int myTeamID;




    public override void SetUpComponent(GameEntity entity)
    {
        base.SetUpComponent(entity);

        nextSensingTime = Time.time + UnityEngine.Random.Range(0, sensingInterval);
        myTeamID = myEntity.teamID;
    }

    public override void UpdateComponent()
    {
        if(Time.time > nextSensingTime)
        {
            nextSensingTime = Time.time + sensingInterval;

            #region scan for enemies

            collidersInRadius = Physics.OverlapSphere(transform.position, sensingRadius, sensingLayerMask);
            
            enemiesInSensingRadius.Clear();

            float smallestDistanceSqr = Mathf.Infinity;
            float currentDistanceSqr;
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

                        currentDistanceSqr = (myPosition - currentEntity.transform.position).sqrMagnitude;
                        if (currentDistanceSqr < smallestDistanceSqr)
                        {
                            smallestDistanceSqr = currentDistanceSqr;
                            nearestEnemy = currentEntity;
                        }
                    }           
                }
            }

            #endregion

            #region Scan for Posts

            collidersInRadius = Physics.OverlapSphere(transform.position, sensingRadius, postSensingLayerMask);

            postsInSensingRadius.Clear();

            smallestDistanceSqr = Mathf.Infinity;
            currentDistanceSqr = 0;
            Post currentPost;
            myPosition = transform.position;

            for (int i = 0; i < collidersInRadius.Length; i++)
            {
                currentPost = collidersInRadius[i].GetComponent<Post>();
                if (currentPost)
                {
                    if (!currentPost.used)
                    {
                        currentDistanceSqr = (myPosition - currentPost.transform.position).sqrMagnitude;
                        postsInSensingRadius.Add(new Tuple<Post, float>(currentPost, currentDistanceSqr));
                    }
                }
            }

            #endregion

        }
    }
}
