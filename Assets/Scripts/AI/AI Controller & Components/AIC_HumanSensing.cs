using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BenitosAI
{

    // Fills the SensingInfo which is used by the AI 
    public class AIC_HumanSensing : AIComponent
    {

        //the sensing info is filled by the sensingcomponenty using the Physics System
        public AIController_Blackboard blackboard;

        [Header("Physics Search Values")]
        [Tooltip("Assign this to the collider of the unit, which is sensing, so it does not sense itself as a friendly")]
        public Collider myEntityCollider;

        public float visionRadius;
        [Tooltip("for now hearing, just automaticly detects enemies in this radius")]
        public float hearingRadius;
        [Tooltip("tPoints are automaticly sensed in this radius, without any vision checks")]
        public float sensingTPointsRadius;
        //Collider[] collidersInRadius;
        public LayerMask sensingLayerMask;
        public LayerMask postSensingLayerMask;
        [Tooltip("Size of the collider array Physics.OverlapSphere returns - limited for optimisation")]
        public int maxEntitiesSensed = 30;
        Queue<SensedEntityInfo> sensedEntityInfoPool = new Queue<SensedEntityInfo>();
        [Tooltip("Size of the collider array Physics.OverlapSphere returns - limited for optimisation")]
        public int maxTPointsSensed = 30;
        [Tooltip("limit their number, so there will be less cover points ignored")]
        int maxOpenFieldPointsSensed = 15; 

        Queue<SensedTacticalPointInfo> sensedTPointInfoPool = new Queue<SensedTacticalPointInfo>();

        public int maxTCoverShootPointsSensed = 5;
        Queue<SensedTacticalPointInfo> sensedTCoverShootPointInfoPool = new Queue<SensedTacticalPointInfo>();

        public LayerMask visibilityLosTestLayermask;


        [Header("For Line of Sight & local position")]
        public Transform headTransform;




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
            //sensingInfo = new AIController_Blackboard(enemiesPoolSize, friendliesPoolSize, tPointsCoverPoolSize, tPointsOpenFieldPoolSize);//, tPointsCoverShootPoolSize);

            for (int i = 0; i < maxEntitiesSensed; i++)
            {
                sensedEntityInfoPool.Enqueue(new SensedEntityInfo());
            }

            for (int i = 0; i < maxTPointsSensed; i++)
            {
                sensedTPointInfoPool.Enqueue(new SensedTacticalPointInfo());
            }
            //Debug.Log("sensedTPointInfoPool size: " + sensedTPointInfoPool.Count);

            for (int i = 0; i < maxTCoverShootPointsSensed; i++)
            {
                sensedTCoverShootPointInfoPool.Enqueue(new SensedTacticalPointInfo());
            }
        }

        public override void UpdateComponent()
        {
            //if (Time.time > nextSensingTime)
            if (optimiser.ShouldSensingBeUpdated() && Time.time - blackboard.lastTimeSensingInfoWasUpdated > 0) //only update if more than 0,0 seconds have passed, don't update when time is stopped inside the game
            {
                UnityEngine.Profiling.Profiler.BeginSample("Sensing Profiling");

                optimiser.OnSensingWasUpdated();

                blackboard.lastTimeSensingInfoWasUpdated = Time.time;
                blackboard.lastFrameCountSensingInfoWasUpdated = Time.frameCount;

                Vector3 myPosition = transform.position;
                float distanceToTarget;

                #region Scan for other Soldiers

                // fill collections
                Collider[] collidersInRadius = new Collider[maxEntitiesSensed]; //30 is the max numbers this array can have through physics overlap sphere, we need to initialize the array with its size before calling OverlapSphereNonAlloc
                Physics.OverlapSphereNonAlloc(transform.position, visionRadius, collidersInRadius, sensingLayerMask); //use non alloc to prevent garbage

                HashSet<SensedEntityInfo> enemiesSensed = new HashSet<SensedEntityInfo>();
                HashSet<SensedEntityInfo> friendliesSensed = new HashSet<SensedEntityInfo>();
                SensedEntityInfo currentEntityInfo;
                EntitySensingInterface currentEntitySensInterface;
                Vector3 targetLocalPosition;
                bool targetVisible;


                for (int i = 0; i < collidersInRadius.Length; i++)
                {
                    if (collidersInRadius[i] != null)
                    {
                        if (collidersInRadius[i] != myEntityCollider)
                        {
                            currentEntitySensInterface = collidersInRadius[i].GetComponent<EntitySensingInterface>();

                            //Convert to local space
                            targetLocalPosition = headTransform.InverseTransformPoint(currentEntitySensInterface.GetEntityPosition());
                            distanceToTarget = targetLocalPosition.magnitude;

                            targetVisible = false;

                            if (distanceToTarget < hearingRadius)
                            {
                                targetVisible = true;
                            }
                            else if(targetLocalPosition.z>0)
                            {
                                //LOS check
                                RaycastHit hit;
                                if (Physics.Raycast(headTransform.position, currentEntitySensInterface.GetRandomPointForLineOfSightTest() - headTransform.position, out hit, Mathf.Infinity, visibilityLosTestLayermask))
                                {
                                    Hitbox hitbox = hit.collider.GetComponent<Hitbox>();
                                    if (hitbox)
                                    {
                                        if (hitbox.GetEntity() == currentEntitySensInterface.entityAssignedTo)
                                        {
                                            targetVisible = true;
                                        }
                                    }
                                }
                            }

                            if (targetVisible)
                            {
                                currentEntityInfo = sensedEntityInfoPool.Dequeue();
                                currentEntityInfo.SetUpInfo(currentEntitySensInterface, distanceToTarget);


                                if (currentEntitySensInterface.entityAssignedTo.teamID != myTeamID)
                                {
                                    enemiesSensed.Add(currentEntityInfo);
                                }
                                else
                                {
                                    friendliesSensed.Add(currentEntityInfo);
                                }
                            }                         
                        }
                    }
                }

                blackboard.UpdateEntityInfos(enemiesSensed, friendliesSensed);

                //Clean up
                foreach (SensedEntityInfo info in enemiesSensed)
                {
                    sensedEntityInfoPool.Enqueue(info);
                }
                enemiesSensed.Clear();
                foreach (SensedEntityInfo info in friendliesSensed)
                {
                    sensedEntityInfoPool.Enqueue(info);
                }
                friendliesSensed.Clear();

                //dont clear thoose, they get deleted anyway?



                #endregion

                #region Scan for Tactical Points

                // fill collections
                collidersInRadius = new Collider[maxTPointsSensed]; //30 is the max numbers this array can have through physics overlap sphere, we need to initialize the array with its size before calling OverlapSphereNonAlloc
                Physics.OverlapSphereNonAlloc(transform.position, sensingTPointsRadius, collidersInRadius, postSensingLayerMask);

                SensedTacticalPointInfo currentTPInfo;
                HashSet<SensedTacticalPointInfo> coverPointsSensed = new HashSet<SensedTacticalPointInfo>();
                HashSet<SensedTacticalPointInfo> openFieldPointsSensed = new HashSet<SensedTacticalPointInfo>();

                int openFieldPointsAlreadySensed = 0;

                for (int i = 0; i < collidersInRadius.Length; i++)
                {
                    if (collidersInRadius[i] != null)
                    {
                        TacticalPointSensingInterface visInfo = collidersInRadius[i].GetComponent<TacticalPointSensingInterface>();
                        float currentDistance = Vector3.Distance(myPosition, visInfo.GetPointPosition());

                        if (!visInfo.tacticalPointAssignedTo.IsPointFull())
                        {
                            if (visInfo.tacticalPointAssignedTo.tacticalPointType == TacticalPointType.CoverPoint)
                            {
                                currentTPInfo = sensedTPointInfoPool.Dequeue();
                                currentTPInfo.SetUpInfo(visInfo, currentDistance);
                                coverPointsSensed.Add(currentTPInfo);
                            }
                            else if(openFieldPointsAlreadySensed< maxOpenFieldPointsSensed)
                            {
                                if (visInfo.tacticalPointAssignedTo.tacticalPointType == TacticalPointType.OpenFieldPoint)
                                {
                                    openFieldPointsAlreadySensed++;
                                    currentTPInfo = sensedTPointInfoPool.Dequeue();
                                    currentTPInfo.SetUpInfo(visInfo, currentDistance);
                                    openFieldPointsSensed.Add(currentTPInfo);
                                }
                            }
                            //cover shoot points are ignored at this step, they dont have a collider
                        }
                    }
                }

                //Add coverShootPoints if i am inside a point
                SensedTacticalPointInfo currentShootPointInfo;
                HashSet<SensedTacticalPointInfo> coverShootPointInfos = new HashSet<SensedTacticalPointInfo>();

                TacticalPoint currentlyUsedPoint = blackboard.GetCurrentlyUsedTacticalPoint();
                if (currentlyUsedPoint != null)
                {
                    //coverShootPointInfos = new SensedTacticalPointInfo[currentlyUsedPoint.coverShootPoints.Length];
                    for (int i = 0; i < currentlyUsedPoint.coverShootPoints.Length; i++)
                    {
                        TacticalPointSensingInterface visInfo = currentlyUsedPoint.coverShootPoints[i].GetComponent<TacticalPointSensingInterface>();
                        currentShootPointInfo = sensedTCoverShootPointInfoPool.Dequeue();
                        currentShootPointInfo.SetUpInfo(visInfo, Vector3.Distance(myPosition, visInfo.GetPointPosition()));
                        //coverShootPointInfos[i] = sensedTCoverShootPointInfoPool.Dequeue();
                        coverShootPointInfos.Add(currentShootPointInfo);
                    }
                }

                blackboard.UpdateTPointInfos(coverPointsSensed, openFieldPointsSensed, coverShootPointInfos);


                //Clean up
                foreach (SensedTacticalPointInfo info in coverPointsSensed)
                {
                    sensedTPointInfoPool.Enqueue(info);
                }
                enemiesSensed.Clear();
                foreach (SensedTacticalPointInfo info in openFieldPointsSensed)
                {
                    sensedTPointInfoPool.Enqueue(info);
                }
                friendliesSensed.Clear();

                foreach(SensedTacticalPointInfo info in coverShootPointInfos)
                {
                    sensedTCoverShootPointInfoPool.Enqueue(info);
                }
                coverShootPointInfos.Clear();




                #endregion

                //blackboard.UpdateSensingInfoAfterAddingNewInfo(transform.position, currentlyUsedTPoint);


                UnityEngine.Profiling.Profiler.EndSample();

            }
        }

        public void UpdateEntityInfoDistance(ref SensedEntityInfo entityInfo)
        {
            entityInfo.lastDistanceMeasured = Vector3.Distance(transform.position, entityInfo.GetEntityPosition());
        }

       
       
    }

}

