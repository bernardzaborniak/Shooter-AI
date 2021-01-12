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
        public SensingInfo sensingInfo;

        [Header("Physics Search Values")]
        [Tooltip("Assign this to the collider of the unit, which is sensing, so it does not sense itself as a friendly")]
        public Collider myEntityCollider;
        public float sensingRadius;
        //Collider[] collidersInRadius;
        public LayerMask sensingLayerMask;
        public LayerMask postSensingLayerMask;
        [Tooltip("Size of the collider array Physics.OverlapSphere returns - limited for optimisation")]
        public int colliderArraySize = 30;


        //public float sensingInterval = 0.5f;
        //float nextSensingTime;


        [Header("Optimisation")]
        public SensingOptimiser optimiser;

        //sensing info caps
        [SerializeField] int enemiesPoolSize;
        [SerializeField] int friendliesPoolSize;
        [SerializeField] int tPointsCoverPoolSize;
        [SerializeField] int tPointsOpenFieldPoolSize;

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
            sensingInfo = new SensingInfo(enemiesPoolSize, friendliesPoolSize, tPointsCoverPoolSize, tPointsOpenFieldPoolSize);
        }

        public override void UpdateComponent()
        {
            //if (Time.time > nextSensingTime)
            if (optimiser.ShouldSensingBeUpdated() && Time.time - sensingInfo.lastTimeInfoWasUpdated > 0) //only update if more than 0,0 seconds have passed, don't update when time is stopped inside the game
            {
                UnityEngine.Profiling.Profiler.BeginSample("Sensing Profiling");

                optimiser.OnSensingWasUpdated();

                sensingInfo.lastTimeInfoWasUpdated = Time.time;
                sensingInfo.lastFrameCountInfoWasUpdated = Time.frameCount;

                Vector3 myPosition = transform.position;

                #region Scan for other Soldiers

                // fill collections
                Collider[] collidersInRadius = new Collider[colliderArraySize]; //30 is the max numbers this array can have through physics overlap sphere, we need to initialize the array with its size before calling OverlapSphereNonAlloc
                Physics.OverlapSphereNonAlloc(transform.position, sensingRadius, collidersInRadius, sensingLayerMask); //use non alloc to prevent garbage


                for (int i = 0; i < collidersInRadius.Length; i++)
                {
                    if (collidersInRadius[i] != null)
                    {
                        if (collidersInRadius[i] != myEntityCollider)
                        {
                            //here calculation would happen if the entity is even visible
                            bool visible = true;
                            EntityVisibilityInfo visInfo = collidersInRadius[i].GetComponent<EntityVisibilityInfo>();
                            float currentDistance = Vector3.Distance(myPosition, visInfo.GetEntityPosition());

                            //based on distance & other factors calculate visibility 
                            //after this or before the field of view would be taken into account of the calculation, based on the visibilityInfoPosiiton

                            if (visible)
                            {
                                if (visInfo.entityAssignedTo.teamID != myTeamID)
                                {
                                    sensingInfo.OnSensedEnemyEntity(visInfo, currentDistance);
                                }
                                else
                                {
                                    sensingInfo.OnSensedFriendlyEntity(visInfo, currentDistance);
                                }
                            }
                        }
                    }
                }


                #endregion

                #region Scan for Tactical Points

                // fill collections
                collidersInRadius = new Collider[colliderArraySize]; //30 is the max numbers this array can have through physics overlap sphere, we need to initialize the array with its size before calling OverlapSphereNonAlloc
                Physics.OverlapSphereNonAlloc(transform.position, sensingRadius, collidersInRadius, postSensingLayerMask);


                for (int i = 0; i < collidersInRadius.Length; i++)
                {
                    if (collidersInRadius[i] != null)
                    {
                        //here calculation would happen if the entity is even visible
                        bool visible = true;
                        TacticalPointVisibilityInfo visInfo = collidersInRadius[i].GetComponent<TacticalPointVisibilityInfo>();
                        float currentDistance = Vector3.Distance(myPosition, visInfo.GetPointPosition());

                        if (visible)
                        {
                            if (!visInfo.tacticalPointAssignedTo.IsPointFull())
                            {
                                if (visInfo.tacticalPointAssignedTo.tacticalPointType == TacticalPointType.CoverPoint)
                                {
                                    sensingInfo.OnSensedTPCover(visInfo, currentDistance);
                                }
                                else if (visInfo.tacticalPointAssignedTo.tacticalPointType == TacticalPointType.OpenFieldPoint)
                                {
                                    sensingInfo.OnSensedTPOpenField(visInfo, currentDistance);
                                }
                                else
                                {
                                    Debug.LogError("sensed tactical point is not of Type CoverPoint or OpenFieldPoint - dafuck is it then?!");
                                }
                            }
                        }
                    }
                }

                #endregion

                sensingInfo.UpdateSensingInfoAfterAddingNewInfo(transform.position);


                UnityEngine.Profiling.Profiler.EndSample();

            }
        }

        public float GetRemainingHealthToMaxHalthRatio()
        {
            return health.GetRemainingHealthToMaxHalthRatio();
        }

        public float GetAmmoRemainingInMagazineRatio(int weaponID)
        {
            return characterController.GetAmmoRemainingInMagazineRatio(weaponID);
        }
    }

}

