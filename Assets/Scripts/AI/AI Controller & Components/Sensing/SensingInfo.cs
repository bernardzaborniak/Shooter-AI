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

        Queue<SensedEntityInfo> enemyInfoPool = new Queue<SensedEntityInfo>();
        Dictionary<int,SensedEntityInfo> enemyInfosAddedThisFrame = new Dictionary<int, SensedEntityInfo>();
        Dictionary<int,SensedEntityInfo> enemyInfosAddedPreviousFrame = new Dictionary<int, SensedEntityInfo>();
        public SensedEntityInfo[] enemyInfos = new SensedEntityInfo[0];


        //Friendlies
        Queue<SensedEntityInfo> friendlyInfoPool = new Queue<SensedEntityInfo>();
        Dictionary<int, SensedEntityInfo> friendlyInfosAddedThisFrame = new Dictionary<int, SensedEntityInfo>();
        Dictionary<int, SensedEntityInfo> friendlyInfosAddedPreviousFrame = new Dictionary<int, SensedEntityInfo>();
        public SensedEntityInfo[] friendlyInfos = new SensedEntityInfo[0];


        //Tactical Points
        Queue<SensedTacticalPointInfo> tPointCoverInfoPool = new Queue<SensedTacticalPointInfo>();
        Dictionary<int, SensedTacticalPointInfo> tPointCoverInfosAddedThisFrame = new Dictionary<int, SensedTacticalPointInfo>();
        Dictionary<int, SensedTacticalPointInfo> tPointCoverInfosAddedPreviousFrame = new Dictionary<int, SensedTacticalPointInfo>();
        public SensedTacticalPointInfo[] tPointCoverInfos = new SensedTacticalPointInfo[0];


        Queue<SensedTacticalPointInfo> tPointOpenFieldInfoPool = new Queue<SensedTacticalPointInfo>();
        Dictionary<int, SensedTacticalPointInfo> tPointOpenFieldInfosAddedThisFrame = new Dictionary<int, SensedTacticalPointInfo>();
        Dictionary<int, SensedTacticalPointInfo> tPointOpenFieldInfosAddedPreviousFrame = new Dictionary<int, SensedTacticalPointInfo>();
        public SensedTacticalPointInfo[] tPointOpenFieldInfos = new SensedTacticalPointInfo[0];


        //Infomation Freshness
        public float lastTimeInfoWasUpdated;
        public int lastFrameCountInfoWasUpdated;


        public SensingInfo(int enemiesPoolSize, int firendliesPoolSize, int tPCoverPoolSize, int tPOpenFieldPoolSize)
        {
            // Fill the Pools
            for (int i = 0; i < enemiesPoolSize; i++)
            {
                enemyInfoPool.Enqueue(new SensedEntityInfo()); //rework the neemy Sensed Info Constructor to not have params
            }
            for (int i = 0; i < firendliesPoolSize; i++)
            {
                friendlyInfoPool.Enqueue(new SensedEntityInfo()); //rework the neemy Sensed Info Constructor to not have params
            }
            for (int i = 0; i < tPCoverPoolSize; i++)
            {
                tPointCoverInfoPool.Enqueue(new SensedTacticalPointInfo()); //rework the neemy Sensed Info Constructor to not have params
            }
            for (int i = 0; i < tPOpenFieldPoolSize; i++)
            {
                tPointOpenFieldInfoPool.Enqueue(new SensedTacticalPointInfo()); //rework the neemy Sensed Info Constructor to not have params
            }
        }

        #region Adding Entities

        public void OnSensedEnemyEntity(EntityVisibilityInfo enemyEntityVisInfo, float squaredDistance)
        {
            OnSensedEntity(enemyEntityVisInfo, squaredDistance, ref enemyInfoPool, ref enemyInfosAddedThisFrame, ref enemyInfosAddedPreviousFrame);
            /*int key = enemyEntityVisInfo.entityAssignedTo.GetHashCode();

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

            sensedEntityInfo.SetUpInfo(enemyEntityVisInfo, squaredDistance);*/
        }

        public void OnSensedFriendlyEntity(EntityVisibilityInfo friendlyEntityVisInfo, float squaredDistance)
        {
            OnSensedEntity(friendlyEntityVisInfo, squaredDistance, ref enemyInfoPool, ref friendlyInfosAddedThisFrame, ref friendlyInfosAddedPreviousFrame);
        }

        void OnSensedEntity(EntityVisibilityInfo entityVisInfo, float squaredDistance, ref Queue<SensedEntityInfo> infosPool, ref Dictionary<int, SensedEntityInfo> infosAddedThisFrame, ref Dictionary<int, SensedEntityInfo> infosAddedPreviousFrames)
        {
            int key = entityVisInfo.entityAssignedTo.GetHashCode();

           SensedEntityInfo sensedEntityInfo;

           // Update older information with new one
           if (infosAddedPreviousFrames.ContainsKey(key))
           {
               sensedEntityInfo = infosAddedPreviousFrames[key];
               infosAddedPreviousFrames.Remove(key);
               infosAddedThisFrame.Add(key, sensedEntityInfo);
           }
           // Else Get An info Object from the pool
           else if(infosPool.Count > 0)
           {
               sensedEntityInfo = infosPool.Dequeue();
               infosAddedThisFrame.Add(key, sensedEntityInfo);
           }
           //If pool is empty, overwrite and older info
           else
           {
               //just take the first one of the old
               int keyToOverride = 0;
               foreach(int previousKey in infosAddedPreviousFrames.Keys)
               {
                   keyToOverride = previousKey;
                   break;
               }

               sensedEntityInfo = infosAddedPreviousFrames[keyToOverride];
                infosAddedPreviousFrames.Remove(keyToOverride);
               infosAddedThisFrame.Add(keyToOverride, sensedEntityInfo);
           }

           sensedEntityInfo.SetUpInfo(entityVisInfo, squaredDistance);
        }

        #endregion

        #region Add Tactical Points

        public void OnSensedTPCover(TacticalPointVisibilityInfo tPointCoverVisInfo, float squaredDistance)
        {
            OnSensedTPoint(tPointCoverVisInfo, squaredDistance, ref tPointCoverInfoPool, ref tPointCoverInfosAddedThisFrame, ref tPointCoverInfosAddedPreviousFrame);
        }

        public void OnSensedTPOpenField(TacticalPointVisibilityInfo tPointOpenFieldVisInfo, float squaredDistance)
        {
            OnSensedTPoint(tPointOpenFieldVisInfo, squaredDistance, ref tPointOpenFieldInfoPool, ref tPointOpenFieldInfosAddedThisFrame, ref tPointOpenFieldInfosAddedPreviousFrame);
        }

        void OnSensedTPoint(TacticalPointVisibilityInfo tPointVisInfo, float squaredDistance, ref Queue<SensedTacticalPointInfo> infosPool, ref Dictionary<int, SensedTacticalPointInfo> infosAddedThisFrame, ref Dictionary<int, SensedTacticalPointInfo> infosAddedPreviousFrames)
        {
            int key = tPointVisInfo.tacticalPointAssignedTo.GetHashCode();

            SensedTacticalPointInfo sensedTPointInfo;
            Debug.Log("one sensed TPoint");
            // Update older information with new one
            if (infosAddedPreviousFrames.ContainsKey(key))
            {
                Debug.Log("one sensed 1");

                sensedTPointInfo = infosAddedPreviousFrames[key];
                infosAddedPreviousFrames.Remove(key);
                infosAddedThisFrame.Add(key, sensedTPointInfo);

                sensedTPointInfo.SetUpInfo(tPointVisInfo, squaredDistance);

            }
            // Else Get An info Object from the pool
            else if (infosPool.Count > 0)
            {
                Debug.Log("one sensed 2");

                sensedTPointInfo = infosPool.Dequeue();
                infosAddedThisFrame.Add(key, sensedTPointInfo);

                sensedTPointInfo.SetUpInfo(tPointVisInfo, squaredDistance);

            }
            //If pool is empty, overwrite and older info
            else
            {
                Debug.Log("one sensed 3");
                if (infosAddedPreviousFrames.Count > 0)
                {
                    //just take the first one of the old
                    int keyToOverride = 0;
                    foreach (int previousKey in infosAddedPreviousFrames.Keys)
                    {
                        keyToOverride = previousKey;
                        break;
                    }

                    sensedTPointInfo = infosAddedPreviousFrames[keyToOverride];
                    infosAddedPreviousFrames.Remove(keyToOverride);
                    infosAddedThisFrame.Add(keyToOverride, sensedTPointInfo);

                    sensedTPointInfo.SetUpInfo(tPointVisInfo, squaredDistance);
                }
                //else: it can happen that there are more infos added this frame than the pool size, thoose extra infos are just ignored, maybe also adjust the collider pool SIze inside sensing

            }

        }

        #endregion

        #region Updating Info after adding

        public void UpdateSensingInfoAfterAddingNewInfo()
        {
            Debug.Log("-----Update After Adding Info ----");
            UpdateEntities(ref enemyInfos, ref enemyInfoPool, ref enemyInfosAddedThisFrame, ref enemyInfosAddedPreviousFrame);
            UpdateEntities(ref friendlyInfos, ref friendlyInfoPool, ref friendlyInfosAddedThisFrame, ref friendlyInfosAddedPreviousFrame);

            UpdateTPoints(ref tPointCoverInfos, ref tPointCoverInfoPool, ref tPointCoverInfosAddedThisFrame, ref tPointCoverInfosAddedPreviousFrame);
            UpdateTPoints(ref tPointOpenFieldInfos, ref tPointOpenFieldInfoPool, ref tPointOpenFieldInfosAddedThisFrame, ref tPointOpenFieldInfosAddedPreviousFrame);
            /*HashSet<int> deadOnesToRemoveIDs = new HashSet<int>();

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
                enemyInfosPool.Enqueue(enemyInfosAddedPreviousFrame[key]);
                enemyInfosAddedPreviousFrame.Remove(key);
            }


            //Fill the array,
            enemyInfo = new SensedEntityInfo[enemyInfosAddedThisFrame.Count + enemyInfosAddedPreviousFrame.Count];

            enemyInfosAddedThisFrame.Values.CopyTo(enemyInfo, 0);
            enemyInfosAddedPreviousFrame.Values.CopyTo(enemyInfo, enemyInfosAddedThisFrame.Count);

          
            // Clear up current, fill the previous
            foreach (SensedEntityInfo item in enemyInfosAddedThisFrame.Values)
            {
                Debug.Log("item: " + item);
                Debug.Log("item.entity.GetHashCode(): " + item.GetHashCode()); //TODO NULL here
                enemyInfosAddedPreviousFrame.Add(item.GetHashCode(), item);
            }
            
            enemyInfosAddedThisFrame.Clear();

            //Sort the array by time when updated (optionally) 


            //set the nearest just like that for now
            */
            if (enemyInfos.Length > 0)
            {
                nearestEnemyInfo = enemyInfos[0];

            }
            else
            {
                nearestEnemyInfo = null;
            }
        }

        void UpdateEntities(ref SensedEntityInfo[] infoArray, ref Queue<SensedEntityInfo> infosPool, ref Dictionary<int, SensedEntityInfo> infosAddedThisFrame, ref Dictionary<int, SensedEntityInfo> infosAddedPreviousFrames)
        {
            //TODO 
            HashSet<int> deadOnesToRemoveIDs = new HashSet<int>();

            //cheack for dead ones
            foreach (SensedEntityInfo item in infosAddedPreviousFrames.Values)
            {
                if (!item.IsAlive())
                {
                    deadOnesToRemoveIDs.Add(item.GetHashCode());
                }
            }
            foreach (int key in deadOnesToRemoveIDs)
            {
                infosPool.Enqueue(infosAddedPreviousFrames[key]);
                infosAddedPreviousFrames.Remove(key);
            }


            //Fill the array,
            infoArray = new SensedEntityInfo[infosAddedThisFrame.Count + infosAddedPreviousFrames.Count];

            infosAddedThisFrame.Values.CopyTo(infoArray, 0);
            infosAddedPreviousFrames.Values.CopyTo(infoArray, infosAddedThisFrame.Count);


            // Clear up current, fill the previous
            foreach (SensedEntityInfo item in infosAddedThisFrame.Values)
            {
                infosAddedPreviousFrames.Add(item.GetHashCode(), item);
            }

            infosAddedThisFrame.Clear();

            //Sort the array by time when updated (optionally) 
        }

        void UpdateTPoints(ref SensedTacticalPointInfo[] infoArray, ref Queue<SensedTacticalPointInfo> infosPool, ref Dictionary<int, SensedTacticalPointInfo> infosAddedThisFrame, ref Dictionary<int, SensedTacticalPointInfo> infosAddedPreviousFrames)
        {
            //TODO 
            HashSet<int> invalidOnesToRemoveIDs = new HashSet<int>();

            //cheack for dead ones
            foreach (SensedTacticalPointInfo item in infosAddedPreviousFrames.Values)
            {
                if (!item.IsValid())
                {
                    invalidOnesToRemoveIDs.Add(item.GetHashCode());
                }
            }
            foreach (int key in invalidOnesToRemoveIDs)
            {
                infosPool.Enqueue(infosAddedPreviousFrames[key]);
                infosAddedPreviousFrames.Remove(key);
            }


            //Fill the array,
            infoArray = new SensedTacticalPointInfo[infosAddedThisFrame.Count + infosAddedPreviousFrames.Count];

            infosAddedThisFrame.Values.CopyTo(infoArray, 0);
            infosAddedPreviousFrames.Values.CopyTo(infoArray, infosAddedThisFrame.Count);


            // Clear up current, fill the previous
            foreach (SensedTacticalPointInfo item in infosAddedThisFrame.Values)
            {
                infosAddedPreviousFrames.Add(item.GetHashCode(), item);
            }

            infosAddedThisFrame.Clear();

            //Sort the array by time when updated (optionally) 
        }

#endregion
    }

}
