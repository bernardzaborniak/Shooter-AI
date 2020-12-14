using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//fills its SensingInfo which is used by the AI 
public class AIC_HumanSensing : AIComponent
{
    public SensingInfo currentSensingInfo = new SensingInfo();

    //public AIC_S_EntityVisibilityInfo nearestEnemyInfo;
    //public HashSet<AIC_S_EntityVisibilityInfo> enemiesInSensingRadius = new HashSet<AIC_S_EntityVisibilityInfo>();
    //public HashSet<AIC_S_EntityVisibilityInfo> friendliesInSensingRadius = new HashSet<AIC_S_EntityVisibilityInfo>();
    Collider[] collidersInRadius;

    //public float sensingInterval = 0.5f;
    //float nextSensingTime;
    public float sensingRadius;
    public LayerMask sensingLayerMask;

    //public HashSet<AIC_S_TacticalPointVisibilityInfo> postsInSensingRadius = new HashSet<AIC_S_TacticalPointVisibilityInfo>();
    public LayerMask postSensingLayerMask;


    int myTeamID;

    [Header("Optimisation")]
    public SensingOptimiser optimiser;

    #region Variables cached to optimise garbage collection

    // Cached for UpdateComponent() Method---------------
    AIC_S_EntityVisibilityInfo entityVisInfo;
    AIC_S_TacticalPointVisibilityInfo tPointVisInfo;

    float smallestDistanceSqr;
    float currentDistanceSqr;
    Vector3 myPosition;

    //GameEntity currentEntity;
    TacticalPoint currentTPoint;
    // ---------------------------------------------------

    #endregion


    public override void SetUpComponent(GameEntity entity)
    {
        base.SetUpComponent(entity);

       // nextSensingTime = Time.time + UnityEngine.Random.Range(0, sensingInterval);
        myTeamID = myEntity.teamID;

    }

    public override void UpdateComponent()
    {
        //if (Time.time > nextSensingTime)
        if(optimiser.ShouldSensingBeUpdated() && Time.time - currentSensingInfo.lastTimeInfoWasUpdated > 0) //only update if more than 0,0 seconds have passed, dont update when time is stopped inside the game
        {
            UnityEngine.Profiling.Profiler.BeginSample("Sensing Profiling");

            optimiser.OnSensingWasUpdated();

            currentSensingInfo.lastTimeInfoWasUpdated = Time.time;
            currentSensingInfo.lastFrameCountInfoWasUpdated = Time.frameCount;

            #region Scan for other Soldiers

            // cache variables 
            currentSensingInfo.enemiesInSensingRadius.Clear();
            currentSensingInfo.friendliesInSensingRadius.Clear();
            smallestDistanceSqr = Mathf.Infinity;
            myPosition = transform.position;
            currentSensingInfo.nearestEnemyInfo = null;

            // fill collections
            collidersInRadius = new Collider[30]; //30 is the max numbers this array can have through physics overlap sphere, we need to initialize the array with its size before calling OverlapSphereNonAlloc
            Physics.OverlapSphereNonAlloc(transform.position, sensingRadius, collidersInRadius, sensingLayerMask);
            //collidersInRadius = Physics.OverlapSphere(transform.position, sensingRadius, sensingLayerMask);


            for (int i = 0; i < collidersInRadius.Length; i++)
            {
                if(collidersInRadius[i] != null)
                {
                    entityVisInfo = new AIC_S_EntityVisibilityInfo(collidersInRadius[i].GetComponent<EntityVisibilityInfo>());   //to optimise garbage collection i could pool this opjects thus limiting the total number of visible enemies at once - which seems like a good idea
                    currentDistanceSqr = (myPosition - collidersInRadius[i].transform.position).sqrMagnitude;
                    entityVisInfo.lastSquaredDistanceMeasured = currentDistanceSqr;

                    if (entityVisInfo.entityTeamID != myTeamID)
                    {
                        currentSensingInfo.enemiesInSensingRadius.Add(entityVisInfo);

                        if (currentDistanceSqr < smallestDistanceSqr)
                        {
                            smallestDistanceSqr = currentDistanceSqr;

                            currentSensingInfo.nearestEnemyInfo = entityVisInfo;
                        }
                    }
                    else
                    {
                        if(entityVisInfo.entity != myEntity)
                        {
                            currentSensingInfo.friendliesInSensingRadius.Add(entityVisInfo);
                        }
                    }
                }            
            }

            #endregion

            #region Scan for Tactical Points

            // cache variables 
            currentSensingInfo.tPointsCoverInSensingRadius.Clear();
            currentSensingInfo.tPointsOpenFieldInSensingRadius.Clear();
            smallestDistanceSqr = Mathf.Infinity;
            currentDistanceSqr = 0;
            myPosition = transform.position;

            // fill collections
            collidersInRadius = new Collider[30]; //30 is the max numbers this array can have through physics overlap sphere, we need to initialize the array with its size before calling OverlapSphereNonAlloc
            Physics.OverlapSphereNonAlloc(transform.position, sensingRadius, collidersInRadius, postSensingLayerMask);
            //collidersInRadius = Physics.OverlapSphere(transform.position, sensingRadius, postSensingLayerMask);

            for (int i = 0; i < collidersInRadius.Length; i++)
            {
                if (collidersInRadius[i] != null)
                {
                    currentTPoint = collidersInRadius[i].GetComponent<TacticalPoint>();

                    if (!currentTPoint.IsPointFull())
                    {
                        tPointVisInfo = new AIC_S_TacticalPointVisibilityInfo(collidersInRadius[i].GetComponent<TacticalPointVisibilityInfo>());
                        currentDistanceSqr = (myPosition - collidersInRadius[i].transform.position).sqrMagnitude;
                        tPointVisInfo.lastSquaredDistanceMeasured = currentDistanceSqr;
                        if(tPointVisInfo.point.tacticalPointType == TacticalPointType.CoverPoint)
                        {
                            currentSensingInfo.tPointsCoverInSensingRadius.Add(tPointVisInfo);
                        }
                        else if(tPointVisInfo.point.tacticalPointType == TacticalPointType.OpenFieldPoint)
                        {
                            currentSensingInfo.tPointsOpenFieldInSensingRadius.Add(tPointVisInfo);
                        }
                        else
                        {
                            Debug.Log("sensed tactical point is not of Type CoverPoint or OpenFieldPoint - dafuck is it then?!");
                        }

                    }
                }
            }

            #endregion

            UnityEngine.Profiling.Profiler.EndSample();

        }
    }
}
