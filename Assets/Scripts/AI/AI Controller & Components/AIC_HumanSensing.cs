using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BenitosAI
{

    // Fills the SensingInfo which is used by the AI 
    public class AIC_HumanSensing : AIComponent
    {
        //some values are recieved by other scripts
        [Header("Scripts to Get Values From")]
        [Tooltip("Reference for cecking things like ammo ")]
        public EC_HumanoidCharacterController characterController;
        [Tooltip("Reference for cecking health ")]
        public EC_Health health;
        //the sensing info is filled by the sensingcomponenty using the Physics System
        public SensingInfo currentSensingInfo = new SensingInfo();

        [Header("Physics Search Values")]
        public float sensingRadius;
        Collider[] collidersInRadius;
        public LayerMask sensingLayerMask;
        public LayerMask postSensingLayerMask;
        [Tooltip("Size of the collider array Physics.OverlapSphere returns - limited for optimisation")]
        public int colliderArraySize = 30;
        //public float sensingInterval = 0.5f;
        //float nextSensingTime;


        [Header("Optimisation")]
        public SensingOptimiser optimiser;

        int myTeamID;

        //#region Pooling



        //#endregion

        #region Variables only cached if called repeatedly in one frame

        float myHealthRatio;

        #endregion


        public override void SetUpComponent(GameEntity entity)
        {
            base.SetUpComponent(entity);

            // nextSensingTime = Time.time + UnityEngine.Random.Range(0, sensingInterval);
            myTeamID = myEntity.teamID; // cached for optimisation.


        }

        public override void UpdateComponent()
        {
            //if (Time.time > nextSensingTime)
            if (optimiser.ShouldSensingBeUpdated() && Time.time - currentSensingInfo.lastTimeInfoWasUpdated > 0) //only update if more than 0,0 seconds have passed, don't update when time is stopped inside the game
            {
                UnityEngine.Profiling.Profiler.BeginSample("Sensing Profiling");

                optimiser.OnSensingWasUpdated();

                currentSensingInfo.lastTimeInfoWasUpdated = Time.time;
                currentSensingInfo.lastFrameCountInfoWasUpdated = Time.frameCount;

                #region Scan for other Soldiers

                // variables 
                currentSensingInfo.enemiesInSensingRadius.Clear();
                currentSensingInfo.friendliesInSensingRadius.Clear();
                float smallestDistanceSqr = Mathf.Infinity;
                Vector3 myPosition = transform.position;
                currentSensingInfo.nearestEnemyInfo = null;

                // fill collections
                collidersInRadius = new Collider[colliderArraySize]; //30 is the max numbers this array can have through physics overlap sphere, we need to initialize the array with its size before calling OverlapSphereNonAlloc
                Physics.OverlapSphereNonAlloc(transform.position, sensingRadius, collidersInRadius, sensingLayerMask); //use non alloc to prevent garbage


                for (int i = 0; i < collidersInRadius.Length; i++)
                {
                    if (collidersInRadius[i] != null)
                    {
                        SensedEntityInfo entityVisInfo = new SensedEntityInfo(collidersInRadius[i].GetComponent<EntityVisibilityInfo>());   //to optimise garbage collection i could pool this opjects thus limiting the total number of visible enemies at once - which seems like a good idea
                        float currentDistanceSqr = (myPosition - collidersInRadius[i].transform.position).sqrMagnitude;
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
                            if (entityVisInfo.entity != myEntity)
                            {
                                currentSensingInfo.friendliesInSensingRadius.Add(entityVisInfo);
                            }
                        }
                    }
                }

                #endregion

                #region Scan for Tactical Points

                // variables 
                currentSensingInfo.tPointsCoverInSensingRadius.Clear();
                currentSensingInfo.tPointsOpenFieldInSensingRadius.Clear();
                smallestDistanceSqr = Mathf.Infinity;
                myPosition = transform.position;

                // fill collections
                collidersInRadius = new Collider[30]; //30 is the max numbers this array can have through physics overlap sphere, we need to initialize the array with its size before calling OverlapSphereNonAlloc
                Physics.OverlapSphereNonAlloc(transform.position, sensingRadius, collidersInRadius, postSensingLayerMask);
                //collidersInRadius = Physics.OverlapSphere(transform.position, sensingRadius, postSensingLayerMask);

                for (int i = 0; i < collidersInRadius.Length; i++)
                {
                    if (collidersInRadius[i] != null)
                    {
                        TacticalPoint currentTPoint = collidersInRadius[i].GetComponent<TacticalPoint>();

                        if (!currentTPoint.IsPointFull())
                        {
                            SensedTacticalPointInfo tPointVisInfo = new SensedTacticalPointInfo(collidersInRadius[i].GetComponent<TacticalPointVisibilityInfo>());
                            float currentDistanceSqr = (myPosition - collidersInRadius[i].transform.position).sqrMagnitude;
                            tPointVisInfo.lastSquaredDistanceMeasured = currentDistanceSqr;
                            if (tPointVisInfo.point.tacticalPointType == TacticalPointType.CoverPoint)
                            {
                                currentSensingInfo.tPointsCoverInSensingRadius.Add(tPointVisInfo);
                            }
                            else if (tPointVisInfo.point.tacticalPointType == TacticalPointType.OpenFieldPoint)
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

        public float GetRemainingHealthToMaxHalthRatio()
        {
            return health.GetRemainingHealthToMaxHalthRatio();
        }
    }

}

