using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIC_HumanSensing : AIComponent
{
    public SensingOptimiser optimiser;
    public AIC_S_EntityVisibilityInfo nearestEnemyInfo;
   // public HashSet<(SensingEntityVisibilityInfo, float)> enemiesInSensingRadius = new HashSet<(SensingEntityVisibilityInfo, float)>();
    public HashSet<AIC_S_EntityVisibilityInfo> enemiesInSensingRadius = new HashSet<AIC_S_EntityVisibilityInfo>();
    public HashSet<AIC_S_EntityVisibilityInfo> friendliesInSensingRadius = new HashSet<AIC_S_EntityVisibilityInfo>();
    Collider[] collidersInRadius;

    public float sensingInterval = 0.5f;
    float nextSensingTime;
    public float sensingRadius;
    public LayerMask sensingLayerMask;

    //public HashSet<(SensingTacticalPointVisibilityInfo, float)> postsInSensingRadius = new HashSet<(SensingTacticalPointVisibilityInfo, float)>();
    public HashSet<AIC_S_TacticalPointVisibilityInfo> postsInSensingRadius = new HashSet<AIC_S_TacticalPointVisibilityInfo>();
    public LayerMask postSensingLayerMask;

    int myTeamID;


    #region Variables cached to optimise garbage collection

    //Cached for UpdateComponent() Method
    AIC_S_EntityVisibilityInfo entityVisInfo;
    AIC_S_TacticalPointVisibilityInfo tPointVisInfo;

    float smallestDistanceSqr;
    float currentDistanceSqr;
    Vector3 myPosition;

    //GameEntity currentEntity;
    TacticalPoint currentTPoint;

    #endregion


    public override void SetUpComponent(GameEntity entity)
    {
        base.SetUpComponent(entity);

        nextSensingTime = Time.time + UnityEngine.Random.Range(0, sensingInterval);
        myTeamID = myEntity.teamID;
    }

    public override void UpdateComponent()
    {
        //if (Time.time > nextSensingTime)
        if(optimiser.ShouldSensingBeUpdated())
        {
            UnityEngine.Profiling.Profiler.BeginSample("Sensing Profiling");

            optimiser.OnSensingWasUpdated();

            #region scan for enemies

            collidersInRadius = Physics.OverlapSphere(transform.position, sensingRadius, sensingLayerMask);
            
            enemiesInSensingRadius.Clear();
            friendliesInSensingRadius.Clear();

            smallestDistanceSqr = Mathf.Infinity;
            myPosition = transform.position;

            nearestEnemyInfo = null;

            for (int i = 0; i < collidersInRadius.Length; i++)
            {
                // currentEntity = collidersInRadius[i].GetComponent<GameEntity>();
                // if (currentEntity)
                //{
                entityVisInfo = new AIC_S_EntityVisibilityInfo(collidersInRadius[i].GetComponent<EntityVisibilityInfo>());   //to optimise garbage collection i could pool this opjects thus limiting the total number of visible enemies at once - which seems like a good idea
                currentDistanceSqr = (myPosition - collidersInRadius[i].transform.position).sqrMagnitude;
                entityVisInfo.lastSquaredDistanceMeasured = currentDistanceSqr;

                // if (currentEntity.teamID != myTeamID)
                if (entityVisInfo.entityTeamID != myTeamID)
                {  
                    enemiesInSensingRadius.Add(entityVisInfo);

                    if (currentDistanceSqr < smallestDistanceSqr)
                    {
                        smallestDistanceSqr = currentDistanceSqr;

                        nearestEnemyInfo = entityVisInfo;
                    }
                }
                else
                {
                    friendliesInSensingRadius.Add(entityVisInfo);
                }
                // }
            }

            #endregion

            #region Scan for Tactical Points

            collidersInRadius = Physics.OverlapSphere(transform.position, sensingRadius, postSensingLayerMask);

            postsInSensingRadius.Clear();

            smallestDistanceSqr = Mathf.Infinity;
            currentDistanceSqr = 0;
   
            myPosition = transform.position;

            for (int i = 0; i < collidersInRadius.Length; i++)
            {
                currentTPoint = collidersInRadius[i].GetComponent<TacticalPoint>();

                if (!currentTPoint.IsPointFull())
                {
                    tPointVisInfo = new AIC_S_TacticalPointVisibilityInfo(collidersInRadius[i].GetComponent<TacticalPointVisibilityInfo>());
                    currentDistanceSqr = (myPosition - collidersInRadius[i].transform.position).sqrMagnitude;
                    tPointVisInfo.lastSquaredDistanceMeasured = currentDistanceSqr;
                    postsInSensingRadius.Add(tPointVisInfo);
                    
                }


                //if (currentPost)
                //{
                //if (!tPointVisInfo.point.IsPointFull())
                //{
                //currentDistanceSqr = (myPosition - currentPost.transform.position).sqrMagnitude;
                //postsInSensingRadius.Add((new AIC_S_TacticalPointVisibilityInfo(currentPost.GetComponent<TacticalPointVisibilityInfo>()), currentDistanceSqr));
                //}
                //}
            }

            #endregion

            UnityEngine.Profiling.Profiler.EndSample();

        }
    }
}
