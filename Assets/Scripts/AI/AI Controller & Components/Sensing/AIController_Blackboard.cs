using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace BenitosAI
{

    public class AIController_Blackboard: AIComponent
    {
        //some values are recieved by other scripts
        [Header("Scripts to Get Values From")]
        [Tooltip("Reference for cecking things like ammo ")]
        public EC_HumanoidCharacterController characterController;
        [Tooltip("Reference for cecking health ")]
        public EC_Health health;

        #region For Storing Sensing Information
        [Header("For Storing Sensing Information")]
        //Enemies
        public SensedEntityInfo[] enemyInfos = new SensedEntityInfo[0]; //sorted by distance
        Queue<SensedEntityInfo> enemyInfoPool = new Queue<SensedEntityInfo>();

        //Friendlies
        public SensedEntityInfo[] friendlyInfos = new SensedEntityInfo[0];//sorted by distance
        Queue<SensedEntityInfo> friendlyInfoPool = new Queue<SensedEntityInfo>();


        //Tactical Points Cover
        //Queue<SensedTacticalPointInfo> tPointCoverInfoPool = new Queue<SensedTacticalPointInfo>();
        public SensedTacticalPointInfo[] tPointCoverInfos = new SensedTacticalPointInfo[0];//sorted by distance

        //Tactical Points Open Field
        //Queue<SensedTacticalPointInfo> tPointOpenFieldInfoPool = new Queue<SensedTacticalPointInfo>();
        public SensedTacticalPointInfo[] tPointOpenFieldInfos = new SensedTacticalPointInfo[0];//sorted by distance

        //Tactical Points Cover Shoot
        //Queue<SensedTacticalPointInfo> tPointCoverShootInfoPool = new Queue<SensedTacticalPointInfo>();
        public SensedTacticalPointInfo[] tPointCoverShootInfos = new SensedTacticalPointInfo[0];//not sorted by distance

        //Infomation Freshness
        public float lastTimeSensingInfoWasUpdated;
        public int lastFrameCountSensingInfoWasUpdated;

        [SerializeField] int enemyInfosPoolSize;
        [SerializeField] int firendlyInfosPoolSize;
        [SerializeField] int tPCoverInfosPoolSize;
        [SerializeField] int tPOpenFieldInfosPoolSize;
        //int tPCoverShootInfosPoolSize = 5;

        #endregion

        TacticalPoint currentlyUsedTPoint;

        [Tooltip("occasionally used to update some values")]
        public AIC_HumanSensing sensing;

        public override void SetUpComponent(GameEntity entity)
        {
            base.SetUpComponent(entity);

            //myTeamID = myEntity.teamID; // cached for optimisation.


            // Fill the Pools
            for (int i = 0; i < enemyInfosPoolSize; i++)
            {
                enemyInfoPool.Enqueue(new SensedEntityInfo()); //rework the neemy Sensed Info Constructor to not have params
            }
            for (int i = 0; i < firendlyInfosPoolSize; i++)
            {
                friendlyInfoPool.Enqueue(new SensedEntityInfo()); //rework the neemy Sensed Info Constructor to not have params
            }
            /*for (int i = 0; i < tPCoverInfosPoolSize; i++)
            {
                tPointCoverInfoPool.Enqueue(new SensedTacticalPointInfo()); //rework the neemy Sensed Info Constructor to not have params
            }
            for (int i = 0; i < tPOpenFieldInfosPoolSize; i++)
            {
                tPointOpenFieldInfoPool.Enqueue(new SensedTacticalPointInfo()); //rework the neemy Sensed Info Constructor to not have params
            }*/
            //for (int i = 0; i < tPCoverShootInfosPoolSize; i++)
            //{
            //   tPointCoverShootInfoPool.Enqueue(new SensedTacticalPointInfo()); //rework the neemy Sensed Info Constructor to not have params
            //}
        }

        public void UpdateEntityInfos(HashSet<SensedEntityInfo> newEnemyInfos, HashSet<SensedEntityInfo> newFriendlyInfos)
        {
            //the new entities are registered, while the older ones arent forgotten completely

            //first Enemies

            //Add old values which arent overriden to the new values set
            /*for (int i = 0; i < enemyInfos.Length; i++)
            {
                if (!newEnemyInfos.Contains(enemyInfos[i]))
                {
                    //if hasnt died yet
                    if (enemyInfos[i].IsAlive())
                    {
                        //update distance inside info
                        sensing.UpdateEntityInfoDistance(ref enemyInfos[i]);
                        newEnemyInfos.Add(enemyInfos[i]);
                    }
                    
                }

                //Return the infos to the pool again
                enemyInfoPool.Enqueue(enemyInfos[i]);
            }

            //convert the HashSet to Array & Sort it
            SensedEntityInfo[] newEnemyInfosArray = new SensedEntityInfo[newEnemyInfos.Count];
            newEnemyInfos.CopyTo(newEnemyInfosArray);

            Array.Sort(newEnemyInfosArray,
            delegate (SensedEntityInfo x, SensedEntityInfo y) { return x.lastDistanceMeasured.CompareTo(y.lastDistanceMeasured); });


            int newEnemyInfoArraySize = enemyInfoPool.Count;
            if(newEnemyInfos.Count < newEnemyInfoArraySize) newEnemyInfoArraySize = newEnemyInfos.Count;

            enemyInfos = new SensedEntityInfo[newEnemyInfoArraySize];

            for (int i = 0; i < newEnemyInfoArraySize; i++)
            {
                enemyInfos[i] = enemyInfoPool.Dequeue();
                enemyInfos[i].SetUpInfo(newEnemyInfosArray[i].visInfo, newEnemyInfosArray[i].lastDistanceMeasured);
            }


            //Friendlies

            //Add old values which arent overriden to the new values set
            for (int i = 0; i < friendlyInfos.Length; i++)
            {
                if (!newFriendlyInfos.Contains(friendlyInfos[i]))
                {
                    //if hasnt died yet
                    if (friendlyInfos[i].IsAlive())
                    {
                        //update distance inside info
                        sensing.UpdateEntityInfoDistance(ref friendlyInfos[i]);
                        newFriendlyInfos.Add(friendlyInfos[i]);
                    }

                }

                //Return the infos to the pool again
                friendlyInfoPool.Enqueue(friendlyInfos[i]);
            }

            //convert the HashSet to Array & Sort it
            SensedEntityInfo[] newFriendlyInfosArray = new SensedEntityInfo[newFriendlyInfos.Count];
            newFriendlyInfos.CopyTo(newFriendlyInfosArray);

            Array.Sort(newFriendlyInfosArray,
            delegate (SensedEntityInfo x, SensedEntityInfo y) { return x.lastDistanceMeasured.CompareTo(y.lastDistanceMeasured); });


            int newFriendlyInfoArraySize = friendlyInfoPool.Count;
            if (newFriendlyInfos.Count < newFriendlyInfoArraySize) newFriendlyInfoArraySize = newFriendlyInfos.Count;

            enemyInfos = new SensedEntityInfo[newFriendlyInfoArraySize];

            for (int i = 0; i < newFriendlyInfoArraySize; i++)
            {
                friendlyInfos[i] = friendlyInfoPool.Dequeue();
                friendlyInfos[i].SetUpInfo(newFriendlyInfosArray[i].visInfo, newFriendlyInfosArray[i].lastDistanceMeasured);
            }*/
        }

        public void UpdateTPointInfos(HashSet<SensedTacticalPointInfo> newCoverPointInfos, HashSet<SensedTacticalPointInfo> newOpenFieldInfos, HashSet<SensedTacticalPointInfo> coverShootPointInfos)
        {
            //now go through cover and open field points, also just override it?
            tPointCoverInfos = new SensedTacticalPointInfo[newCoverPointInfos.Count];
            newCoverPointInfos.CopyTo(tPointCoverInfos);
            
            Array.Sort(tPointCoverInfos,
            delegate (SensedTacticalPointInfo x, SensedTacticalPointInfo y) { return x.lastDistanceMeasured.CompareTo(y.lastDistanceMeasured); });

            tPointOpenFieldInfos = new SensedTacticalPointInfo[newOpenFieldInfos.Count];
            newOpenFieldInfos.CopyTo(tPointOpenFieldInfos);

            Array.Sort(tPointOpenFieldInfos,
            delegate (SensedTacticalPointInfo x, SensedTacticalPointInfo y) { return x.lastDistanceMeasured.CompareTo(y.lastDistanceMeasured); });


            //the cover shoot points are just overriden
            tPointCoverShootInfos = new SensedTacticalPointInfo[coverShootPointInfos.Count];
            coverShootPointInfos.CopyTo(tPointCoverShootInfos);

            Array.Sort(tPointCoverShootInfos,
            delegate (SensedTacticalPointInfo x, SensedTacticalPointInfo y) { return x.lastDistanceMeasured.CompareTo(y.lastDistanceMeasured); });

        }


        public void SetCurrentlyUsedTacticalPoint(TacticalPoint usedPoint)
        {
            currentlyUsedTPoint = usedPoint;
        }

        public TacticalPoint GetCurrentlyUsedTacticalPoint()
        {
            return currentlyUsedTPoint;
        }

        public float GetRemainingHealthToMaxHalthRatio()
        {
            return health.GetRemainingHealthToMaxHalthRatio();
        }

        public float GetAmmoRemainingInMagazineRatio(int weaponID)
        {
            return characterController.GetAmmoRemainingInMagazineRatio(weaponID);
        }

        public GameEntity GetMyEntity()
        {
            return myEntity;
        }

        public EntityTags GetMyTags()
        {
            return myEntity.entityTags;
        }


    }

}
