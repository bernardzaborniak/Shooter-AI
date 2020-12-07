using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIC_HumanSensing : AIComponent
{
    public SensingOptimiser optimiser;
    //public GameEntity nearestEnemy;
    public SensingEntityVisibilityInfo nearestEnemyInfo;
    public HashSet<GameEntity> enemiesInSensingRadius = new HashSet<GameEntity>();
    Collider[] collidersInRadius;

    public float sensingInterval = 0.5f;
    float nextSensingTime;
    public float sensingRadius;
    public LayerMask sensingLayerMask;

    public HashSet<Tuple<TacticalPoint,float>> postsInSensingRadius = new HashSet<Tuple<TacticalPoint, float>>();
    public LayerMask postSensingLayerMask;

    int myTeamID;

    //for debug only:
    float lastSenseTime = 0;




    public override void SetUpComponent(GameEntity entity)
    {
        base.SetUpComponent(entity);

        nextSensingTime = Time.time + UnityEngine.Random.Range(0, sensingInterval);
        myTeamID = myEntity.teamID;

        lastSenseTime = Time.unscaledTime;
    }

    public override void UpdateComponent()
    {
        if (Time.time > nextSensingTime)
        //if(optimiser.ShouldSensingBeUpdated())
        {
            UnityEngine.Profiling.Profiler.BeginSample("Sensing Profiling");

            Debug.Log("update sensing, interval: " + (Time.unscaledTime - lastSenseTime));
            lastSenseTime = Time.unscaledTime;

            //optimiser.OnSensingWasUpdated();
            nextSensingTime = Time.time + sensingInterval;

            for (int i = 0; i < 5; i++)
            {
                Debug.Log("I sense :)");
            }

            #region scan for enemies

            collidersInRadius = Physics.OverlapSphere(transform.position, sensingRadius, sensingLayerMask);
            
            enemiesInSensingRadius.Clear();

            float smallestDistanceSqr = Mathf.Infinity;
            float currentDistanceSqr;
            GameEntity currentEntity;
            Vector3 myPosition = transform.position;

            nearestEnemyInfo = null;

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

                            VisibilityInfo visInfo = currentEntity.GetComponent<VisibilityInfo>();
                            nearestEnemyInfo = new SensingEntityVisibilityInfo();
                            nearestEnemyInfo.SetUpInfo(visInfo);
                           // nearestEnemyInfo.visibilityInfo = currentEntity.GetComponent<VisibilityInfo>();
                           // nearestEnemyInfo.timeWhenLastSeen = Time.time;
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
            TacticalPoint currentPost;
            myPosition = transform.position;

            for (int i = 0; i < collidersInRadius.Length; i++)
            {
                currentPost = collidersInRadius[i].GetComponent<TacticalPoint>();
                if (currentPost)
                {
                    if (!currentPost.IsPointFull())
                    {
                        currentDistanceSqr = (myPosition - currentPost.transform.position).sqrMagnitude;
                        postsInSensingRadius.Add(new Tuple<TacticalPoint, float>(currentPost, currentDistanceSqr));
                    }
                }
            }

            #endregion

            UnityEngine.Profiling.Profiler.EndSample();

        }
    }
}
