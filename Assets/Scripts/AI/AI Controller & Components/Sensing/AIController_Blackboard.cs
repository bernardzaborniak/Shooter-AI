using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace BenitosAI
{

    public class AIController_Blackboard: MonoBehaviour
    {
        //Enemies
        Queue<SensedEntityInfo> enemyInfoPool = new Queue<SensedEntityInfo>();
        public SensedEntityInfo[] enemyInfos = new SensedEntityInfo[0]; //sorted by distance

        //Friendlies
        Queue<SensedEntityInfo> friendlyInfoPool = new Queue<SensedEntityInfo>();
        public SensedEntityInfo[] friendlyInfos = new SensedEntityInfo[0];//sorted by distance

        //Tactical Points Cover
        Queue<SensedTacticalPointInfo> tPointCoverInfoPool = new Queue<SensedTacticalPointInfo>();
        public SensedTacticalPointInfo[] tPointCoverInfos = new SensedTacticalPointInfo[0];//sorted by distance

        //Tactical Points Open Field
        Queue<SensedTacticalPointInfo> tPointOpenFieldInfoPool = new Queue<SensedTacticalPointInfo>();
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

        TacticalPoint currentlyUsedTPoint;


        void Start()
        //public AIController_Blackboard(int enemyInfosPoolSize, int firendlyInfosPoolSize, int tPCoverInfosPoolSize, int tPOpenFieldInfosPoolSize, int tPCoverShootInfosPoolSize)
        {
            // Fill the Pools
            for (int i = 0; i < enemyInfosPoolSize; i++)
            {
                enemyInfoPool.Enqueue(new SensedEntityInfo()); //rework the neemy Sensed Info Constructor to not have params
            }
            for (int i = 0; i < firendlyInfosPoolSize; i++)
            {
                friendlyInfoPool.Enqueue(new SensedEntityInfo()); //rework the neemy Sensed Info Constructor to not have params
            }
            for (int i = 0; i < tPCoverInfosPoolSize; i++)
            {
                tPointCoverInfoPool.Enqueue(new SensedTacticalPointInfo()); //rework the neemy Sensed Info Constructor to not have params
            }
            for (int i = 0; i < tPOpenFieldInfosPoolSize; i++)
            {
                tPointOpenFieldInfoPool.Enqueue(new SensedTacticalPointInfo()); //rework the neemy Sensed Info Constructor to not have params
            }
            //for (int i = 0; i < tPCoverShootInfosPoolSize; i++)
            //{
             //   tPointCoverShootInfoPool.Enqueue(new SensedTacticalPointInfo()); //rework the neemy Sensed Info Constructor to not have params
            //}
        }

        public void UpdateEntityInfos(HashSet<SensedEntityInfo> newEnemyInfos, HashSet<SensedEntityInfo> newFriendlyInfos)
        {
           // HashSet<(EntitySensingInterface enemyEntitySensInterface, float distance)> oldEnemyInfoTuples = new HashSet<(EntitySensingInterface enemyEntitySensInterface, float distance)>();

            //Add old values which arent overriden to the new values set
            for (int i = 0; i < enemyInfos.Length; i++)
            {
                if (!newEnemyInfos.Contains(enemyInfos[i]))
                {
                    newEnemyInfos.Add(enemyInfos[i]);
                }

                //Return the infos to the pool again
                enemyInfoPool.Enqueue(enemyInfos[i]);
            }

            //convert the HashSet to Array & Sort it
            SensedEntityInfo[] newEnemyInfosArray = new SensedEntityInfo[newEnemyInfos.Count];
            newEnemyInfos.CopyTo(newEnemyInfosArray);

            Array.Sort(newEnemyInfosArray,
            delegate (SensedEntityInfo x, SensedEntityInfo y) { return x.lastDistanceMeasured.CompareTo(y.lastDistanceMeasured); });


            int newInfoArraySize = enemyInfoPool.Count;
            if(newEnemyInfos.Count < newInfoArraySize) newInfoArraySize = newEnemyInfos.Count;

            infoArray = new SensedEntityInfo[newInfoArraySize];

            for (int i = 0; i < newInfoArraySize; i++)
            {
                infoArray[i] = enemyInfoPool.Dequeue();
                infoArray[i].SetUp(newEnemyInfosArray[i]);
            }
        }

        public void UpdateTPointInfos(HashSet<SensedTacticalPointInfo> newCoverPointInfos, HashSet<SensedTacticalPointInfo> newOpenFieldInfos, HashSet<SensedTacticalPointInfo> coverShootPointInfos)
        {
            //the cover shoot points are just overriden
            tPointCoverShootInfos = new SensedTacticalPointInfo[coverShootPointInfos.Count];
            coverShootPointInfos.CopyTo(tPointCoverShootInfos);
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
