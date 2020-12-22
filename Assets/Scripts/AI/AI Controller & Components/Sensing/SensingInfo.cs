using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BenitosAI
{

    // Custom Object used for saving and transfering sensing information, needs Expanding with adding some kind of memory
    public class SensingInfo
    {
        //Enemies
        public SensedEntityInfo nearestEnemyInfo;
        float nearestEnemySquaredDistance;
        Queue<SensedEntityInfo> enemiesInfosPool;
        public Dictionary<GameEntity,SensedEntityInfo> enemyInfos;// = new HashSet<SensedEntityInfo>();

        //Friendlies
        Queue<SensedEntityInfo> friendliesInfosPool;
        public Dictionary<GameEntity,SensedEntityInfo> friendliesInfos;// = new HashSet<SensedEntityInfo>();

        //Tactical Points
        Queue<SensedTacticalPointInfo> tPointsCoverInfosPool;// = new HashSet<SensedTacticalPointInfo>();
        public Dictionary<TacticalPoint,SensedTacticalPointInfo> tPointsCoverInfos;// = new HashSet<SensedTacticalPointInfo>();

        Queue<SensedTacticalPointInfo> tPointsOpenFieldInfosPool;// = new HashSet<SensedTacticalPointInfo>();
        public Dictionary<TacticalPoint, SensedTacticalPointInfo> tPointsOpenFieldInfos;// = new HashSet<SensedTacticalPointInfo>();

        HashSet<GameEntity> entitiesToRemoveBecauseTheyDied = new HashSet<GameEntity>();

        //Infomation Freshness
        public float lastTimeInfoWasUpdated;
        public int lastFrameCountInfoWasUpdated;


        public SensingInfo(int enemiesPoolSize, int firendliesPoolSize, int tPCoverPoolSize, int tPOpenFieldPoolSize)
        {
            // Initialise Collections
            enemiesInfosPool = new Queue<SensedEntityInfo>();
            enemyInfos = new Dictionary<GameEntity, SensedEntityInfo>();

            friendliesInfosPool = new Queue<SensedEntityInfo>();
            friendliesInfos = new Dictionary<GameEntity, SensedEntityInfo>();

            tPointsCoverInfosPool = new Queue<SensedTacticalPointInfo>();
            tPointsCoverInfos = new Dictionary<TacticalPoint, SensedTacticalPointInfo>();

            tPointsOpenFieldInfosPool = new Queue<SensedTacticalPointInfo>();
            tPointsOpenFieldInfos = new Dictionary<TacticalPoint, SensedTacticalPointInfo>();

            // Fill the Pools
            for (int i = 0; i < enemiesPoolSize; i++)
            {
                enemiesInfosPool.Enqueue(new SensedEntityInfo()); //rework the neemy Sensed Info Constructor to not have params
            }
            for (int i = 0; i < firendliesPoolSize; i++)
            {
                friendliesInfosPool.Enqueue(new SensedEntityInfo()); //rework the neemy Sensed Info Constructor to not have params
            }
            for (int i = 0; i < tPCoverPoolSize; i++)
            {
                tPointsCoverInfosPool.Enqueue(new SensedTacticalPointInfo()); //rework the neemy Sensed Info Constructor to not have params
            }
            for (int i = 0; i < tPOpenFieldPoolSize; i++)
            {
                tPointsOpenFieldInfosPool.Enqueue(new SensedTacticalPointInfo()); //rework the neemy Sensed Info Constructor to not have params
            }

        }

        //this update thould be called every frame to simulate forgetting?
        public void UpdateSensingInfo()
        {
            //to exapnd later - here forgetting and corrupting memories would be simulated

            //maybe this could cause errors, basue the entity s stille there?

            //for now we scan for dead entities here
            foreach (SensedEntityInfo info in enemyInfos.Values)
            {
                if (!info.IsAlive())
                {
                    entitiesToRemoveBecauseTheyDied.Remove(info.entity);

                    if(nearestEnemySquaredDistance == info.lastSquaredDistanceMeasured)
                    {
                        DetermineNearestEnemy();
                    }
                }

            }
            foreach (SensedEntityInfo info in friendliesInfos.Values)
            {
                if (!info.IsAlive())
                {
                    entitiesToRemoveBecauseTheyDied.Remove(info.entity);
                }
            }

            foreach (GameEntity entity in entitiesToRemoveBecauseTheyDied)
            {
                enemyInfos.Remove(entity);
                friendliesInfos.Remove(entity);
            }

            //TODO 1 after deleting unused , sort everything by information freshness and while sorting, also determine nearest enemy?
        }

        void DetermineNearestEnemy()
        {
            nearestEnemySquaredDistance = Mathf.Infinity;
            nearestEnemyInfo = null;

            foreach (SensedEntityInfo info in enemyInfos.Values)
            {
                if(info.lastSquaredDistanceMeasured < nearestEnemySquaredDistance)
                {
                    nearestEnemySquaredDistance = info.lastSquaredDistanceMeasured;
                    nearestEnemyInfo = info;
                }
            }


        }


        public void OnSensedEnemyEntity(EntityVisibilityInfo enemyEntityVisInfo, float squaredDistance)
        {
            AddSensedEntity(enemyEntityVisInfo, squaredDistance, enemiesInfosPool, enemyInfos);   
            
            if(squaredDistance < nearestEnemySquaredDistance)
            {
                nearestEnemyInfo = enemyInfos[enemyEntityVisInfo.entityAssignedTo];
            }
        }

        public void OnSensedFriendlyEntity(EntityVisibilityInfo friendlyEntityVisInfo, float squaredDistance)
        {
            AddSensedEntity(friendlyEntityVisInfo, squaredDistance, friendliesInfosPool, friendliesInfos);
        }

        void AddSensedEntity(EntityVisibilityInfo entityVisInfo, float squaredDistance, Queue<SensedEntityInfo> infoPool, Dictionary<GameEntity, SensedEntityInfo> usedInfos)
        {
            int writeInfoID;

            //1. Check if there already is older information about this enemy saved in the hashSet, if yes, just update the values on this object
            if (usedInfos.ContainsKey(entityVisInfo.entityAssignedTo))
            {
                usedInfos[entityVisInfo.entityAssignedTo].SetUpInfo(entityVisInfo, squaredDistance);
                Debug.Log("entity 1.");
            }
            else
            {
                //2. If no, check if pool count >0 -> 
                if (infoPool.Count > 0)
                {
                    //  2.1 if yes dequeue from pool and add to hashset and set values
                    SensedEntityInfo info = infoPool.Dequeue();
                    info.SetUpInfo(entityVisInfo, squaredDistance);
                    usedInfos.Add(info.entity, info);
                    Debug.Log("entity 2.1");
                }
                else
                {
                    Debug.Log("entity 2.2");
                    //  2.2 if no, foreach through whole hashset to find oldest information and overwrite it with the new ne 
                    float oldestTime = Mathf.Infinity;
                    GameEntity oldestInfoKey = null;

                    foreach (SensedEntityInfo info in usedInfos.Values)
                    {
                        if (info.timeWhenLastSeen < oldestTime)
                        {
                            oldestTime = info.timeWhenLastSeen;
                            oldestInfoKey = info.entity;
                        }
                    }

                    usedInfos[oldestInfoKey].SetUpInfo(entityVisInfo, squaredDistance);

                }
            }
        }

        public void OnSensedTPCover(TacticalPointVisibilityInfo tPointCoverVisInfo, float squaredDistance)
        {
            AddSensedTacticalPoint(tPointCoverVisInfo, squaredDistance, tPointsCoverInfosPool, tPointsCoverInfos);
        }

        public void OnSensedTPOpenField(TacticalPointVisibilityInfo tPointOpenFieldVisInfo, float squaredDistance)
        {
            AddSensedTacticalPoint(tPointOpenFieldVisInfo, squaredDistance, tPointsOpenFieldInfosPool, tPointsOpenFieldInfos);
        }

        void AddSensedTacticalPoint(TacticalPointVisibilityInfo tPointVisInfo, float squaredDistance, Queue<SensedTacticalPointInfo> infoPool, Dictionary<TacticalPoint, SensedTacticalPointInfo> usedInfos)
        {
            Debug.Log("Add Tacticalpoint");
            //1. Check if there already is older information about this enemy saved in the hashSet, if yes, just update the values on this object
            if (usedInfos.ContainsKey(tPointVisInfo.tacticalPointAssignedTo))
            {
                usedInfos[tPointVisInfo.tacticalPointAssignedTo].SetUpInfo(tPointVisInfo, squaredDistance);

            }
            else
            {
                //2. If no, check if pool count >0 -> 
                if (infoPool.Count > 0)
                {
                    //  2.1 if yes dequeue from pool and add to hashset and set values
                    SensedTacticalPointInfo info = infoPool.Dequeue();
                    info.SetUpInfo(tPointVisInfo, squaredDistance);
                    usedInfos.Add(info.tacticalPoint, info);
                }
                else
                {
                    //  2.2 if no, foreach through whole hashset to find oldest information and overwrite it with the new ne 
                    float oldestTime = Time.time + 0.1f;
                    TacticalPoint oldestInfoKey = null;

                    // this extra loop makes sensing algorythm from 0n to 0n2 :/
                    foreach (SensedTacticalPointInfo info in usedInfos.Values)
                    {
                        if (info.timeWhenLastSeen < oldestTime)
                        {
                            oldestTime = info.timeWhenLastSeen;
                            oldestInfoKey = info.tacticalPoint;
                        }
                    }
                    Debug.Log("key tried: " + oldestInfoKey + "tpPosition: " + oldestInfoKey.transform.position +  " hash: " + oldestInfoKey.GetHashCode() + " tried by sensing info nr: " + this.GetHashCode());
                    Debug.Log("key contains: " + usedInfos.ContainsKey(oldestInfoKey));
                    usedInfos[oldestInfoKey].SetUpInfo(tPointVisInfo, squaredDistance);

                }
            }
        }

     
    }

}
