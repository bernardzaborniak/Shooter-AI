using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BenitosAI
{

    // Updates the blackboard info every interval. Has A Pool of EntitySensedInfos.
    public class AIC_HumanSensing : AIComponent
    {
        #region Fields

        [SerializeField] AIController_Blackboard blackboard;

        [Header("Physics Search Values")]
        [Tooltip("Assign this to the collider of the unit, which is sensing, so it does not sense itself as a friendly")]
        [SerializeField] Collider myEntityCollider;

        [Header("For Line of Sight & local position")]
        public Transform headTransform;

        [SerializeField] float visionRadius;
        [Tooltip("for now hearing, just automaticly detects enemies in this radius")]
        [SerializeField] float hearingRadius;
        [Tooltip("tPoints are automaticly sensed in this radius, without any vision checks")]
        [SerializeField] float sensingTPointsRadius;
        [SerializeField] LayerMask sensingLayerMask;
        [SerializeField] LayerMask postSensingLayerMask;
        [SerializeField] LayerMask visibilityLosTestLayerMask;

        [Tooltip("Size of the collider array Physics.OverlapSphere returns - limited for optimisation")]
        [SerializeField] int maxEntitiesSensed = 30;
        
        [Tooltip("Size of the collider array Physics.OverlapSphere returns - limited for optimisation")]
        [SerializeField] int maxTPointsSensed = 30;
        [Tooltip("limit their number, so there will be less cover points ignored")]
        [SerializeField] int maxOpenFieldPointsSensed = 15;
        [SerializeField] int maxTCoverShootPointsSensed = 5;


        [Header("Optimisation")]
        public SensingOptimiser optimiser;

        int myTeamID;

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

                HashSet<(EntitySensingInterface, float)> enemiesSensed = new HashSet<(EntitySensingInterface, float)>();
                HashSet<(EntitySensingInterface,float)> friendliesSensed = new HashSet<(EntitySensingInterface, float)>();
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
                                if (Physics.Raycast(headTransform.position, currentEntitySensInterface.GetRandomPointForLineOfSightTest() - headTransform.position, out hit, Mathf.Infinity, visibilityLosTestLayerMask))
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
                                if (currentEntitySensInterface.entityAssignedTo.teamID != myTeamID)
                                {
                                    enemiesSensed.Add((currentEntitySensInterface, distanceToTarget));
                                }
                                else
                                {
                                    friendliesSensed.Add((currentEntitySensInterface, distanceToTarget));
                                }
                            }                         
                        }
                    }
                }

                blackboard.UpdateEntityInfos(enemiesSensed, friendliesSensed);




                #endregion

                #region Scan for Tactical Points

                // fill collections
                collidersInRadius = new Collider[maxTPointsSensed]; //30 is the max numbers this array can have through physics overlap sphere, we need to initialize the array with its size before calling OverlapSphereNonAlloc
                Physics.OverlapSphereNonAlloc(transform.position, sensingTPointsRadius, collidersInRadius, postSensingLayerMask);

                HashSet<(TacticalPoint,float)> coverPointsSensed = new HashSet<(TacticalPoint, float)>();
                HashSet<(TacticalPoint, float)> openFieldPointsSensed = new HashSet<(TacticalPoint, float)>();

                int openFieldPointsAlreadySensed = 0;

                for (int i = 0; i < collidersInRadius.Length; i++)
                {
                    if (collidersInRadius[i] != null)
                    {
                        TacticalPoint tPoint = collidersInRadius[i].GetComponent<TacticalPoint>();
                        float currentDistance = Vector3.Distance(myPosition, tPoint.GetPointPosition());

                        if (!tPoint.IsPointFull())
                        {
                            if (tPoint.tacticalPointType == TacticalPointType.CoverPoint)
                            {
                                coverPointsSensed.Add((tPoint, currentDistance));
                            }
                            else if(openFieldPointsAlreadySensed< maxOpenFieldPointsSensed)
                            {
                                if (tPoint.tacticalPointType == TacticalPointType.OpenFieldPoint)
                                {
                                    openFieldPointsAlreadySensed++;
                                    openFieldPointsSensed.Add((tPoint, currentDistance));
                                }
                            }
                            //cover shoot points are ignored at this step, they dont have a collider
                        }
                    }
                }

                //Add coverPeekPoints if i am inside a point
                HashSet<(TacticalPoint, float)> coverPeekPointInfos = new HashSet<(TacticalPoint, float)>();

                TacticalPoint currentlyUsedPoint = blackboard.GetCurrentlyUsedTacticalPoint();
                TacticalPoint[] currentCoverPeekPoints = new TacticalPoint[0];
                if (currentlyUsedPoint != null)
                {
                    //Debug.Log("I am inside Cover Point Point");
                    if(currentlyUsedPoint.tacticalPointType == TacticalPointType.CoverPoint) 
                    {
                        currentCoverPeekPoints = currentlyUsedPoint.coverPeekPoints;
                    }
                    else if(currentlyUsedPoint.tacticalPointType == TacticalPointType.CoverPeekPoint)
                    {
                        //Debug.Log("I am inside Cover Peek Point");
                        currentCoverPeekPoints = currentlyUsedPoint.coverPointAssignedTo.coverPeekPoints;
                    }

                    for (int i = 0; i < currentCoverPeekPoints.Length; i++)
                    {
                        TacticalPoint tPoint = currentCoverPeekPoints[i];
                        //Debug.Log("Sensed new Cover Peek Point: " + tPoint.GetHashCode());
                        coverPeekPointInfos.Add((tPoint, Vector3.Distance(myPosition, tPoint.GetPointPosition())));
                    }

                }

                blackboard.UpdateTPointInfos(coverPointsSensed, openFieldPointsSensed, coverPeekPointInfos);

                #endregion

                UnityEngine.Profiling.Profiler.EndSample();

            }
        }

        public void UpdateEntityInfoDistance(ref SensedEntityInfo entityInfo)
        {
            entityInfo.lastDistanceMeasured = Vector3.Distance(transform.position, entityInfo.GetEntityPosition());
        }

        public void UpdateTPInfoDistance(ref SensedTacticalPointInfo tPointInfo)
        {
            tPointInfo.lastDistanceMeasured = Vector3.Distance(transform.position, tPointInfo.tacticalPoint.GetPointPosition());
        }

        

    }

}

