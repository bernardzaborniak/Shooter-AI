using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace BenitosAI
{
    public struct SensingInfoToAdd
    {
        public EntityVisibilityInfo visInfo;
        public float squaredDistance;

        public SensingInfoToAdd(EntityVisibilityInfo visInfo, float squaredDistance)
        {
            this.visInfo = visInfo;
            this.squaredDistance = squaredDistance;
        }
    }


    // Custom Object used for saving and transfering sensing information, needs Expanding with adding some kind of memory
    public class SensingInfo
    {
       

        //Enemies
        public SensedEntityInfo nearestEnemyInfo;
        float nearestEnemySquaredDistance;
        Queue<SensedEntityInfo> enemiesInfosPool;
        public Dictionary<int,SensedEntityInfo> enemyInfos;// = new HashSet<SensedEntityInfo>();

        HashSet<int> enemyEntitiesToRemoveBecauseTheyDied = new HashSet<int>();
        HashSet<SensingInfoToAdd> enemyInfosToAddThisFrame = new HashSet<SensingInfoToAdd>();
        HashSet<int> enemyEntitiesNotUpdatedThisFrame = new HashSet<int>();
        //SensedEntityInfo[] sensedEnemiesSorted;


        //Friendlies
        Queue<SensedEntityInfo> friendlyInfosPool;
        public Dictionary<int,SensedEntityInfo> friendlyInfos;// = new HashSet<SensedEntityInfo>();

        HashSet<int> friendlyEntitiesToRemoveBecauseTheyDied = new HashSet<int>();
        HashSet<SensingInfoToAdd> friendlyInfosToAddThisFrame = new HashSet<SensingInfoToAdd>();
        HashSet<int> friendlyEntitiesNotUpdatedThisFrame = new HashSet<int>();

        //SensedEntityInfo[] sensedFriendliesSorted;



        //Tactical Points
        Queue<SensedTacticalPointInfo> tPointsCoverInfosPool;// = new HashSet<SensedTacticalPointInfo>();
        public Dictionary<int,SensedTacticalPointInfo> tPointsCoverInfos;// = new HashSet<SensedTacticalPointInfo>();

        Queue<SensedTacticalPointInfo> tPointsOpenFieldInfosPool;// = new HashSet<SensedTacticalPointInfo>();
        public Dictionary<int, SensedTacticalPointInfo> tPointsOpenFieldInfos;// = new HashSet<SensedTacticalPointInfo>();


        //Infomation Freshness
        public float lastTimeInfoWasUpdated;
        public int lastFrameCountInfoWasUpdated;


        public SensingInfo(int enemiesPoolSize, int firendliesPoolSize, int tPCoverPoolSize, int tPOpenFieldPoolSize)
        {
            // Initialise Collections
            enemiesInfosPool = new Queue<SensedEntityInfo>();
            enemyInfos = new Dictionary<int, SensedEntityInfo>();

            friendlyInfosPool = new Queue<SensedEntityInfo>();
            friendlyInfos = new Dictionary<int, SensedEntityInfo>();

            tPointsCoverInfosPool = new Queue<SensedTacticalPointInfo>();
            tPointsCoverInfos = new Dictionary<int, SensedTacticalPointInfo>();

            tPointsOpenFieldInfosPool = new Queue<SensedTacticalPointInfo>();
            tPointsOpenFieldInfos = new Dictionary<int, SensedTacticalPointInfo>();

            // Fill the Pools
            for (int i = 0; i < enemiesPoolSize; i++)
            {
                enemiesInfosPool.Enqueue(new SensedEntityInfo()); //rework the neemy Sensed Info Constructor to not have params
            }
            for (int i = 0; i < firendliesPoolSize; i++)
            {
                friendlyInfosPool.Enqueue(new SensedEntityInfo()); //rework the neemy Sensed Info Constructor to not have params
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

        public void UpdateSensingInfo(HashSet<SensingInfoToAdd> enemyInfoToAdd, HashSet<SensingInfoToAdd> friendlyInfoToAdd)
        {
            //they are being filled together with the death check
            enemyEntitiesNotUpdatedThisFrame.Clear();
            friendlyEntitiesNotUpdatedThisFrame.Clear();

            //1. Delete Dead Ones ---------------------------------------------
            foreach (SensedEntityInfo info in enemyInfos.Values)
            {
                if (!info.IsAlive())
                {
                    enemyEntitiesToRemoveBecauseTheyDied.Add(info.entity.GetInstanceID());
                }
                else
                {
                    enemyEntitiesNotUpdatedThisFrame.Add(info.entity.GetInstanceID());
                }

            }
            foreach (SensedEntityInfo info in friendlyInfos.Values)
            {
                if (!info.IsAlive())
                {
                    friendlyEntitiesToRemoveBecauseTheyDied.Add(info.entity.GetInstanceID());
                }
                else
                {
                    friendlyEntitiesNotUpdatedThisFrame.Add(info.entity.GetInstanceID());
                }
            }

            //remove them from current infos and add them to pool
            foreach (int entityID in enemyEntitiesToRemoveBecauseTheyDied)
            {
                enemiesInfosPool.Enqueue(enemyInfos[entityID]);
                enemyInfos.Remove(entityID);
            }
            foreach (int entityID in friendlyEntitiesToRemoveBecauseTheyDied)
            {
                friendlyInfosPool.Enqueue(friendlyInfos[entityID]);
                friendlyInfos.Remove(entityID);
            }

            //2. Go through the collections of the objects to add
            enemyInfosToAddThisFrame.Clear();
            friendlyInfosToAddThisFrame.Clear();

            foreach (SensingInfoToAdd infoToAdd in enemyInfoToAdd)
            {
                if (enemyInfos.ContainsKey(infoToAdd.visInfo.entityAssignedTo.GetInstanceID()))
                {
                    enemyInfos[infoToAdd.visInfo.entityAssignedTo.GetInstanceID()].SetUpInfo(infoToAdd.visInfo, infoToAdd.squaredDistance);
                    enemyEntitiesNotUpdatedThisFrame.Remove(infoToAdd.visInfo.entityAssignedTo.GetInstanceID());
                }
                else
                {
                    enemyInfosToAddThisFrame.Add(infoToAdd);
                }   
            }
            foreach (SensingInfoToAdd infoToAdd in friendlyInfoToAdd)
            {
                if (friendlyInfos.ContainsKey(infoToAdd.visInfo.entityAssignedTo.GetInstanceID()))
                {
                    friendlyInfos[infoToAdd.visInfo.entityAssignedTo.GetInstanceID()].SetUpInfo(infoToAdd.visInfo, infoToAdd.squaredDistance);
                    friendlyEntitiesNotUpdatedThisFrame.Remove(infoToAdd.visInfo.entityAssignedTo.GetInstanceID());
                }
                else
                {
                    friendlyInfosToAddThisFrame.Add(infoToAdd);
                }
            }

            //if there is not enough room in the pool, make more room






        }

        public void OnSensedEnemyEntity(EntityVisibilityInfo enemyEntityVisInfo, float squaredDistance)
        {
            AddSensedEntity(enemyEntityVisInfo, squaredDistance, enemiesInfosPool, enemyInfos, enemyInfosToAddThisFrame);   
            
            if(squaredDistance < nearestEnemySquaredDistance)
            {
                nearestEnemyInfo = enemyInfos[enemyEntityVisInfo.entityAssignedTo.GetInstanceID()];
            }
        }

        public void OnSensedFriendlyEntity(EntityVisibilityInfo friendlyEntityVisInfo, float squaredDistance)
        {
            AddSensedEntity(friendlyEntityVisInfo, squaredDistance, friendlyInfosPool, friendlyInfos, friendlyInfosToAddThisFrame);
        }

        void AddSensedEntity(EntityVisibilityInfo entityVisInfo, float squaredDistance, Queue<SensedEntityInfo> infoPool, Dictionary<int, SensedEntityInfo> usedInfos, HashSet<SensingInfoToAdd> infosToAdd)
        {

            //1. Check if there already is older information about this enemy saved in the hashSet, if yes, just update the values on this object
            if (usedInfos.ContainsKey(entityVisInfo.entityAssignedTo.GetInstanceID()))
            {
                usedInfos[entityVisInfo.entityAssignedTo.GetInstanceID()].SetUpInfo(entityVisInfo, squaredDistance);
                Debug.Log("entity 1.");
            }
            else
            {
                infosToAdd.Add(new SensingInfoToAdd(entityVisInfo, squaredDistance));
                //2. If no, check if pool count >0 -> 
                /*if (infoPool.Count > 0)
                {
                    //  2.1 if yes dequeue from pool and add to hashset and set values
                    SensedEntityInfo info = infoPool.Dequeue();
                    info.SetUpInfo(entityVisInfo, squaredDistance);
                    usedInfos.Add(info.entity.GetInstanceID(), info);
                    Debug.Log("entity 2.1");
                }
                else
                {
                    Debug.Log("entity 2.2");
                    //  2.2 if no, foreach through whole hashset to find oldest information and overwrite it with the new ne 
                    float oldestTime = Mathf.Infinity;
                    int oldestInfoKey = 0;

                    foreach (SensedEntityInfo info in usedInfos.Values)
                    {
                        if (info.timeWhenLastSeen < oldestTime)
                        {
                            oldestTime = info.timeWhenLastSeen;
                            oldestInfoKey = info.entity.GetInstanceID();
                        }
                    }

                    usedInfos[oldestInfoKey].SetUpInfo(entityVisInfo, squaredDistance);

                }*/
           }
        }

        /*public void UpdateSensingInfo(SensingInfoToAdd[] enemiesSensed, SensingInfoToAdd[] friendliesSensed)
        {
            //the arrays can have null objects

        }*/

        public void OnSensedTPCover(TacticalPointVisibilityInfo tPointCoverVisInfo, float squaredDistance)
        {
            //AddSensedTacticalPoint(tPointCoverVisInfo, squaredDistance, tPointsCoverInfosPool, tPointsCoverInfos);
        }

        public void OnSensedTPOpenField(TacticalPointVisibilityInfo tPointOpenFieldVisInfo, float squaredDistance)
        {
            //AddSensedTacticalPoint(tPointOpenFieldVisInfo, squaredDistance, tPointsOpenFieldInfosPool, tPointsOpenFieldInfos);
        }

        void AddSensedTacticalPoint(TacticalPointVisibilityInfo tPointVisInfo, float squaredDistance, Queue<SensedTacticalPointInfo> infoPool, Dictionary<int, SensedTacticalPointInfo> usedInfos)
        {
            Debug.Log("Add Tacticalpoint");
            //1. Check if there already is older information about this enemy saved in the hashSet, if yes, just update the values on this object
            if (usedInfos.ContainsKey(tPointVisInfo.tacticalPointAssignedTo.GetInstanceID()))
            {
                usedInfos[tPointVisInfo.tacticalPointAssignedTo.GetInstanceID()].SetUpInfo(tPointVisInfo, squaredDistance);

            }
            else
            {
                //2. If no, check if pool count >0 -> 
                if (infoPool.Count > 0)
                {
                    //  2.1 if yes dequeue from pool and add to hashset and set values
                    SensedTacticalPointInfo info = infoPool.Dequeue();
                    info.SetUpInfo(tPointVisInfo, squaredDistance);
                    usedInfos.Add(info.tacticalPoint.GetInstanceID(), info);
                }
                else
                {
                    //  2.2 if no, foreach through whole hashset to find oldest information and overwrite it with the new ne 
                    float oldestTime = Time.time + 0.1f;
                    int oldestInfoKey = 0;

                    // this extra loop makes sensing algorythm from 0n to 0n2 :/
                    foreach (SensedTacticalPointInfo info in usedInfos.Values)
                    {
                        if (info.timeWhenLastSeen < oldestTime)
                        {
                            oldestTime = info.timeWhenLastSeen;
                            oldestInfoKey = info.tacticalPoint.GetInstanceID();
                        }
                    }
                    //Debug.Log("key tried: " + oldestInfoKey + "tpPosition: " + oldestInfoKey.transform.position +  " hash: " + oldestInfoKey.GetHashCode() + " tried by sensing info nr: " + this.GetHashCode());
                    Debug.Log("key tried: " + oldestInfoKey + " hash: " + oldestInfoKey.GetHashCode() + " tried by sensing info nr: " + this.GetHashCode());
                    //Debug.Log("key contains: " + usedInfos.ContainsKey(oldestInfoKey.GetInstanceID()));
                    usedInfos[oldestInfoKey].SetUpInfo(tPointVisInfo, squaredDistance);

                }
            }
        }

        //this update thould be called every frame to simulate forgetting?
        public void UpdateSensingInfoAfterAddingNewInfo()
        {
            //to exapnd later - here forgetting and corrupting memories would be simulated

            //maybe this could cause errors, basue the entity s stille there?

            //for now we scan for dead entities here
            //1. collect all items to delete cause of death--------------------------------------------------
            foreach (SensedEntityInfo info in enemyInfos.Values)
            {
                if (!info.IsAlive())
                {
                    enemyEntitiesToRemoveBecauseTheyDied.Add(info.entity.GetInstanceID());
                }

            }
            foreach (SensedEntityInfo info in friendlyInfos.Values)
            {
                if (!info.IsAlive())
                {
                    friendlyEntitiesToRemoveBecauseTheyDied.Add(info.entity.GetInstanceID());
                }
            }

            //remove them from current infos and add them to pool
            foreach (int entityID in enemyEntitiesToRemoveBecauseTheyDied)
            {
                enemiesInfosPool.Enqueue(enemyInfos[entityID]);
                enemyInfos.Remove(entityID);
            }
            foreach (int entityID in friendlyEntitiesToRemoveBecauseTheyDied)
            {
                friendlyInfosPool.Enqueue(friendlyInfos[entityID]);
                friendlyInfos.Remove(entityID);
            }

            // Use tuples instead of structs?
            foreach (SensingInfoToAdd entityToAdd in enemyInfosToAddThisFrame)
            {
                if (enemiesInfosPool.Count > 0)
                {
                    SensedEntityInfo info = enemiesInfosPool.Dequeue();
                    enemyInfos.Add(info.entity.GetInstanceID(), info);
                }
                //else if()
                //{

                //}
            }

            foreach (SensingInfoToAdd entityToAdd in friendlyInfosToAddThisFrame)
            {

            }
            //sort the infos according to time -> change them to array

            //let the array be the one which is read by all?
            /*sensedEnemiesSorted = new SensedEntityInfo[enemyInfos.Count];
            enemyInfos.Values.CopyTo(sensedEnemiesSorted, 0);
            Array.Sort(sensedEnemiesSorted, delegate(SensedEntityInfo info1, SensedEntityInfo info2) {
                                            return info1.timeWhenLastSeen.CompareTo(info2.timeWhenLastSeen);
                                            });

            sensedFriendliesSorted = new SensedEntityInfo[friendlyInfos.Count];
            friendlyInfos.Values.CopyTo(sensedFriendliesSorted, 0);
            Array.Sort(sensedFriendliesSorted, delegate (SensedEntityInfo info1, SensedEntityInfo info2) {
                                            return info1.timeWhenLastSeen.CompareTo(info2.timeWhenLastSeen);
                                             });*/

            //the sorting could be optimised?

            //use list instead of array?
            //enemyEntitiesNotUpdatedThisFrame

            enemyEntitiesToRemoveBecauseTheyDied.Clear();
            friendlyEntitiesToRemoveBecauseTheyDied.Clear();

            enemyInfosToAddThisFrame.Clear();
            friendlyInfosToAddThisFrame.Clear();

            //enemyEntitiesNotUpdatedThisFrame = new HashSet<int>(enemyInfos.Values);



            /*foreach (GameEntity entity in entitiesToRemoveBecauseTheyDied)
            {
                enemyInfos.Remove(entity.GetInstanceID());
                friendliesInfos.Remove(entity.GetInstanceID());
            }*/

            //2. convert to array ,sort and  collect all items which are too old--------------------------------------------------

            //add the items to delete and the old ones back to the pool

            // add the items to add by taking the unused objects from the pool



            // clear the colections
           

            //TODO 1 after deleting unused , sort everything by information freshness and while sorting, also determine nearest enemy?
        }


    }

}
