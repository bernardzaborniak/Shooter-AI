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

        Queue<SensedEntityInfo> enemiesInfosPool = new Queue<SensedEntityInfo>();
        Dictionary<int,SensedEntityInfo> infosAddedThisFrame = new Dictionary<int, SensedEntityInfo>();
        Dictionary<int,SensedEntityInfo> enemyInfosAddedPreviousFrame = new Dictionary<int, SensedEntityInfo>();
        public SensedEntityInfo[] enemiesInfo = new SensedEntityInfo[0];


        //Friendlies
        Queue<SensedEntityInfo> friiendliesInfoPool = new Queue<SensedEntityInfo>();
        public SensedEntityInfo[] friendliesInfo = new SensedEntityInfo[0];


        //Tactical Points
        Queue<SensedTacticalPointInfo> tPointsCoverInfosPool = new Queue<SensedTacticalPointInfo>();
        public SensedTacticalPointInfo[] tPointsCoverInfo = new SensedTacticalPointInfo[0];


        Queue<SensedTacticalPointInfo> tPointsOpenFieldInfosPool = new Queue<SensedTacticalPointInfo>();
        public SensedTacticalPointInfo[] tPointsOpenFieldInfo = new SensedTacticalPointInfo[0];


        //Infomation Freshness
        public float lastTimeInfoWasUpdated;
        public int lastFrameCountInfoWasUpdated;


        public SensingInfo(int enemiesPoolSize, int firendliesPoolSize, int tPCoverPoolSize, int tPOpenFieldPoolSize)
        {
            // Fill the Pools
            for (int i = 0; i < enemiesPoolSize; i++)
            {
                enemiesInfosPool.Enqueue(new SensedEntityInfo()); //rework the neemy Sensed Info Constructor to not have params
            }
            for (int i = 0; i < firendliesPoolSize; i++)
            {
                friiendliesInfoPool.Enqueue(new SensedEntityInfo()); //rework the neemy Sensed Info Constructor to not have params
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


        public void OnSensedEnemyEntity(EntityVisibilityInfo enemyEntityVisInfo, float squaredDistance)
        {
            int key = enemyEntityVisInfo.entityAssignedTo.GetHashCode();

            SensedEntityInfo sensedEntityInfo;

            // Update older information with new one
            if (enemyInfosAddedPreviousFrame.ContainsKey(key))
            {
                sensedEntityInfo = enemyInfosAddedPreviousFrame[key];
                enemyInfosAddedPreviousFrame.Remove(key);
                infosAddedThisFrame.Add(key, sensedEntityInfo);
            }
            // Else Get An info Object from the pool
            else if(enemiesInfosPool.Count > 0)
            {
                sensedEntityInfo = enemiesInfosPool.Dequeue();
                infosAddedThisFrame.Add(key, sensedEntityInfo);
            }
            //If pool is empty, overwrite and older info
            else
            {
                //just take the first one of the old
                int keyToOverride = 0;
                foreach(int previousKey in enemyInfosAddedPreviousFrame.Keys)
                {
                    keyToOverride = previousKey;
                    break;
                }

                sensedEntityInfo = enemyInfosAddedPreviousFrame[keyToOverride];
                enemyInfosAddedPreviousFrame.Remove(keyToOverride);
                infosAddedThisFrame.Add(keyToOverride, sensedEntityInfo);
            }

            sensedEntityInfo.SetUpInfo(enemyEntityVisInfo, squaredDistance);
        }

        public void OnSensedFriendlyEntity(EntityVisibilityInfo friendlyEntityVisInfo, float squaredDistance)
        {

        }

        public void OnSensedTPCover(TacticalPointVisibilityInfo tPointCoverVisInfo, float squaredDistance)
        {

        }

        public void OnSensedTPOpenField(TacticalPointVisibilityInfo tPointOpenFieldVisInfo, float squaredDistance)
        {

        }


        public void UpdateSensingInfoAfterAddingNewInfo()
        {
            Debug.Log("-----Update After Adding Info ----");
            HashSet<int> deadOnesToRemoveIDs = new HashSet<int>();

            //cheack for dead ones
            foreach (SensedEntityInfo item in enemyInfosAddedPreviousFrame.Values)
            {
                if(!item.IsAlive())
                {
                    deadOnesToRemoveIDs.Add(item.GetHashCode());
                }
            }
            foreach (int key in deadOnesToRemoveIDs)
            {
                enemiesInfosPool.Enqueue(enemyInfosAddedPreviousFrame[key]);
                enemyInfosAddedPreviousFrame.Remove(key);
            }


            //Fill the array,
            enemiesInfo = new SensedEntityInfo[infosAddedThisFrame.Count + enemyInfosAddedPreviousFrame.Count];

            infosAddedThisFrame.Values.CopyTo(enemiesInfo, 0);
            enemyInfosAddedPreviousFrame.Values.CopyTo(enemiesInfo, infosAddedThisFrame.Count);

            //set the nearest just like that for now

            if (enemiesInfo.Length > 0)
            {
                nearestEnemyInfo = enemiesInfo[0];

            }
            else
            {
                nearestEnemyInfo = null;
            }

            // Clear up current, fill the previous
            foreach (SensedEntityInfo item in infosAddedThisFrame.Values)
            {
                Debug.Log("item: " + item);
                Debug.Log("item.entity.GetHashCode(): " + item.GetHashCode()); //TODO NULL here
                enemyInfosAddedPreviousFrame.Add(item.GetHashCode(), item);
            }
            
            infosAddedThisFrame.Clear();


            //Sort it (optionally) 
        }
    }

}
