using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace BenitosAI
{
    // Holds informion, acts like the memory of the AI. Has A Pool of EntitySensedInfos.
    public class AIController_Blackboard: AIComponent
    {
        #region Fields

        //some values are recieved by other scripts
        [Header("Scripts to Get Values From")]
        [Tooltip("Reference for cecking things like ammo ")]
        [SerializeField] EC_HumanoidCharacterController characterController;
        [Tooltip("Reference for cecking health ")]
        [SerializeField] EC_Health health;

        [Tooltip("occasionally used to update some values")]
        public AIC_HumanSensing sensing;

        #region For Storing Sensing Information
        [Header("For Storing Sensing Information")]

        //Infomation Freshness
        public float lastTimeSensingInfoWasUpdated;
        public int lastFrameCountSensingInfoWasUpdated;


        [NonSerialized] public SensedEntityInfo[] enemyInfos = new SensedEntityInfo[0]; //sorted by distance
        [NonSerialized] public SensedEntityInfo[] friendlyInfos = new SensedEntityInfo[0];//sorted by distance
        [NonSerialized] public SensedTacticalPointInfo[] tPCoverInfos = new SensedTacticalPointInfo[0];//sorted by distance
        [NonSerialized] public SensedTacticalPointInfo[] tPOpenFieldInfos = new SensedTacticalPointInfo[0];//sorted by distance
        [NonSerialized] public SensedTacticalPointInfo[] tPCoverPeekInfos = new SensedTacticalPointInfo[0];//not sorted by distance

        [SerializeField] int maxEnemyInfosCount;
        [SerializeField] int maxFriendlyInfosCount;
        [SerializeField] int maxTPCoverInfosCount;
        [SerializeField] int maxTPOpenFieldInfosCount;

        #endregion

        [SerializeField] TacticalPoint currentlyUsedTPoint;

        #endregion


        public override void SetUpComponent(GameEntity entity)
        {
            base.SetUpComponent(entity);

        }

    #region Updating Sensing Information

        public void UpdateEntityInfos(HashSet<(EntitySensingInterface, float)> newEnemyInfos, HashSet<(EntitySensingInterface, float)> newFriendlyInfos)
        {
            //-> The new entities are registered, while the older ones arent forgotten completely.

            #region------------Enemies------------------

            //1. Create dictionary of current infos        
            Dictionary<int,SensedEntityInfo> currentEnemyInfosDict = new Dictionary<int,SensedEntityInfo>(); //the entity hashSets are the keys
            for (int i = 0; i < enemyInfos.Length; i++)
            {
                currentEnemyInfosDict.Add(enemyInfos[i].entity.GetHashCode(), enemyInfos[i]);
            }

            //2. Go through new infos, update the infos in idcitionary if ocntains, or create and add new elements to dictionary
            foreach ((EntitySensingInterface entitySensInterface, float distanceToEntity) newEntityInfoTuple in newEnemyInfos)
            {
                int currentKey = newEntityInfoTuple.entitySensInterface.entityAssignedTo.GetHashCode();
                if (currentEnemyInfosDict.ContainsKey(currentKey))
                {
                    // update the current info
                    currentEnemyInfosDict[currentKey].UpdateInfo(newEntityInfoTuple.distanceToEntity);
                }
                else
                {
                    // or create & add new info
                    currentEnemyInfosDict.Add(currentKey, new SensedEntityInfo(newEntityInfoTuple.entitySensInterface, newEntityInfoTuple.distanceToEntity));
                }
            }

            //3. Convert dictionary back to array -->dict.Values.CopyTo(foos, 0);
            SensedEntityInfo[] newEnemyInfosSortedArray = new SensedEntityInfo[currentEnemyInfosDict.Count];
            currentEnemyInfosDict.Values.CopyTo(newEnemyInfosSortedArray,0);

            //4. Sort array according to distance
            Array.Sort(newEnemyInfosSortedArray,
            delegate (SensedEntityInfo x, SensedEntityInfo y) { return x.lastDistanceMeasured.CompareTo(y.lastDistanceMeasured); });

            //5 Cut array
            int newEnemyInfoArraySize = newEnemyInfosSortedArray.Length;
            if (newEnemyInfoArraySize > maxEnemyInfosCount) newEnemyInfoArraySize = maxEnemyInfosCount;

            enemyInfos = new SensedEntityInfo[newEnemyInfoArraySize];

            for (int i = 0; i < newEnemyInfoArraySize; i++)
            {
                enemyInfos[i] = newEnemyInfosSortedArray[i];

                //if we still have old info, not updated this frame, "predict" its distance
                if (enemyInfos[i].frameCountWhenLastSeen != Time.frameCount)
                {
                    sensing.UpdateEntityInfoDistance(ref enemyInfos[i]);
                }
            }

            #endregion ------------------------------

            #region------------Friendlies------------------

            //1. Create dictionary of current infos        
            Dictionary<int, SensedEntityInfo> currentFriendlyInfosDict = new Dictionary<int, SensedEntityInfo>(); //the entity hashSets are the keys
            for (int i = 0; i < friendlyInfos.Length; i++)
            {
                currentFriendlyInfosDict.Add(friendlyInfos[i].entity.GetHashCode(), friendlyInfos[i]);
            }

            //2. Go through new infos, update the infos in idcitionary if ocntains, or create and add new elements to dictionary
            foreach ((EntitySensingInterface entitySensInterface, float distanceToEntity) newEntityInfoTuple in newFriendlyInfos)
            {
                int currentKey = newEntityInfoTuple.entitySensInterface.entityAssignedTo.GetHashCode();
                if (currentFriendlyInfosDict.ContainsKey(currentKey))
                {
                    // update the current info
                    currentFriendlyInfosDict[currentKey].UpdateInfo(newEntityInfoTuple.distanceToEntity);
                }
                else
                {
                    // or create & add new info
                    currentFriendlyInfosDict.Add(currentKey, new SensedEntityInfo(newEntityInfoTuple.entitySensInterface, newEntityInfoTuple.distanceToEntity));
                }
            }

            //3. Convert dictionary back to array -->dict.Values.CopyTo(foos, 0);
            SensedEntityInfo[] newFriendlyInfosSortedArray = new SensedEntityInfo[currentFriendlyInfosDict.Count];
            currentFriendlyInfosDict.Values.CopyTo(newFriendlyInfosSortedArray, 0);

            //4. Sort array according to distance
            Array.Sort(newFriendlyInfosSortedArray,
            delegate (SensedEntityInfo x, SensedEntityInfo y) { return x.lastDistanceMeasured.CompareTo(y.lastDistanceMeasured); });

            //5 Cut array
            int newFriendlyInfoArraySize = newFriendlyInfosSortedArray.Length;
            if (newFriendlyInfoArraySize > maxFriendlyInfosCount) newFriendlyInfoArraySize = maxFriendlyInfosCount;

            friendlyInfos = new SensedEntityInfo[newFriendlyInfoArraySize];

            for (int i = 0; i < newFriendlyInfoArraySize; i++)
            {
                friendlyInfos[i] = newFriendlyInfosSortedArray[i];

                //if we still have old info, not updated this frame, "predict" its distance
                if (friendlyInfos[i].frameCountWhenLastSeen != Time.frameCount)
                {
                    sensing.UpdateEntityInfoDistance(ref friendlyInfos[i]);
                }
            }

            #endregion --------------------------------
        }

        public void UpdateTPointInfos(HashSet<(TacticalPoint, float)> newTPCoverInfos, HashSet<(TacticalPoint, float)> newTPOpenFieldInfos, HashSet<(TacticalPoint, float)> newTPCoverPeekInfos)
        {
            //-----Eureka?-----
            //1. create dictionary of current infos
            //2. go through new infos, update the infos in idcitionary if ocntains, or create and add new elements to dictionary
            //3. convert fictionary back to array -->dict.Values.CopyTo(foos, 0);
            //4. sort array
            //5 cut array

            UpdateTPointInfos(newTPCoverInfos, tPCoverInfos, maxTPCoverInfosCount);
            UpdateTPointInfos(newTPOpenFieldInfos, tPOpenFieldInfos, maxTPOpenFieldInfosCount);
            UpdateTPointInfos(newTPCoverPeekInfos, tPCoverPeekInfos, 10); //it will hopefully never by more than 3

            #region------------Cover Points------------------

            //1. Create dictionary of current infos        
            /* Dictionary<int, SensedTacticalPointInfo> currentTPCoverInfosDict = new Dictionary<int, SensedTacticalPointInfo>(); //the entity hashSets are the keys
             for (int i = 0; i < tPCoverInfos.Length; i++)
             {
                 currentTPCoverInfosDict.Add(tPCoverInfos[i].tacticalPoint.GetHashCode(), tPCoverInfos[i]);
             }

             //2. Go through new infos, update the infos in idcitionary if ocntains, or create and add new elements to dictionary
             foreach ((TacticalPoint tPoint, float distanceToPoint) newTPInfoTuple in newTPCoverInfos)
             {
                 int currentKey = newTPInfoTuple.tPoint.GetHashCode();
                 if (currentTPCoverInfosDict.ContainsKey(currentKey))
                 {
                     // update the current info
                     currentTPCoverInfosDict[currentKey].UpdateInfo(newTPInfoTuple.distanceToPoint);
                 }
                 else
                 {
                     // or create & add new info
                     currentTPCoverInfosDict.Add(currentKey, new SensedTacticalPointInfo(newTPInfoTuple.tPoint, newTPInfoTuple.distanceToPoint));
                 }
             }

             //3. Convert dictionary back to array -->dict.Values.CopyTo(foos, 0);
             SensedEntityInfo[] newEnemyInfosSortedArray = new SensedEntityInfo[currentEnemyInfosDict.Count];
             currentEnemyInfosDict.Values.CopyTo(newEnemyInfosSortedArray, 0);

             //4. Sort array according to distance
             Array.Sort(newEnemyInfosSortedArray,
             delegate (SensedEntityInfo x, SensedEntityInfo y) { return x.lastDistanceMeasured.CompareTo(y.lastDistanceMeasured); });

             //5 Cut array
             int newEnemyInfoArraySize = newEnemyInfosSortedArray.Length;
             if (newEnemyInfoArraySize > maxEnemyInfosCount) newEnemyInfoArraySize = maxEnemyInfosCount;

             enemyInfos = new SensedEntityInfo[newEnemyInfoArraySize];

             for (int i = 0; i < newEnemyInfoArraySize; i++)
             {
                 enemyInfos[i] = newEnemyInfosSortedArray[i];

                 //if we still have old info, not updated this frame, "predict" its distance
                 if (enemyInfos[i].frameCountWhenLastSeen != Time.frameCount)
                 {
                     sensing.UpdateEntityInfoDistance(ref enemyInfos[i]);
                 }
             }

             #endregion ------------------------------*/


            //For TPoints, we just override them, the references here are referencing SensedTacticalPointInfo objects inside the SensingPool.

            /*tPointCoverInfos = new SensedTacticalPointInfo[newCoverPointInfos.Count];
            newCoverPointInfos.CopyTo(tPointCoverInfos);
            
            Array.Sort(tPointCoverInfos,
            delegate (SensedTacticalPointInfo x, SensedTacticalPointInfo y) { return x.lastDistanceMeasured.CompareTo(y.lastDistanceMeasured); });


            tPointOpenFieldInfos = new SensedTacticalPointInfo[newOpenFieldInfos.Count];
            newOpenFieldInfos.CopyTo(tPointOpenFieldInfos);

            Array.Sort(tPointOpenFieldInfos,
            delegate (SensedTacticalPointInfo x, SensedTacticalPointInfo y) { return x.lastDistanceMeasured.CompareTo(y.lastDistanceMeasured); });


            tPointCoverShootInfos = new SensedTacticalPointInfo[coverShootPointInfos.Count];
            coverShootPointInfos.CopyTo(tPointCoverShootInfos);

            Array.Sort(tPointCoverShootInfos,
            delegate (SensedTacticalPointInfo x, SensedTacticalPointInfo y) { return x.lastDistanceMeasured.CompareTo(y.lastDistanceMeasured); });
            */
        }

        void UpdateTPointInfos(HashSet<(TacticalPoint, float)> newTPInfos, SensedTacticalPointInfo[] infosToUpdate, int maxInfosCount)
        {
            //1. Create dictionary of current infos        
            Dictionary<int, SensedTacticalPointInfo> currentTPInfosDict = new Dictionary<int, SensedTacticalPointInfo>(); //the entity hashSets are the keys
            for (int i = 0; i < infosToUpdate.Length; i++)
            {
                currentTPInfosDict.Add(infosToUpdate[i].tacticalPoint.GetHashCode(), infosToUpdate[i]);
            }

            //2. Go through new infos, update the infos in idcitionary if ocntains, or create and add new elements to dictionary
            foreach ((TacticalPoint tPoint, float distanceToPoint) newTPInfoTuple in newTPCoverInfos)
            {
                int currentKey = newTPInfoTuple.tPoint.GetHashCode();
                if (currentTPCoverInfosDict.ContainsKey(currentKey))
                {
                    // update the current info
                    currentTPCoverInfosDict[currentKey].UpdateInfo(newTPInfoTuple.distanceToPoint);
                }
                else
                {
                    // or create & add new info
                    currentTPCoverInfosDict.Add(currentKey, new SensedTacticalPointInfo(newTPInfoTuple.tPoint, newTPInfoTuple.distanceToPoint));
                }
            }

            //3. Convert dictionary back to array -->dict.Values.CopyTo(foos, 0);
            SensedEntityInfo[] newEnemyInfosSortedArray = new SensedEntityInfo[currentEnemyInfosDict.Count];
            currentEnemyInfosDict.Values.CopyTo(newEnemyInfosSortedArray, 0);

            //4. Sort array according to distance
            Array.Sort(newEnemyInfosSortedArray,
            delegate (SensedEntityInfo x, SensedEntityInfo y) { return x.lastDistanceMeasured.CompareTo(y.lastDistanceMeasured); });

            //5 Cut array
            int newEnemyInfoArraySize = newEnemyInfosSortedArray.Length;
            if (newEnemyInfoArraySize > maxEnemyInfosCount) newEnemyInfoArraySize = maxEnemyInfosCount;

            enemyInfos = new SensedEntityInfo[newEnemyInfoArraySize];

            for (int i = 0; i < newEnemyInfoArraySize; i++)
            {
                enemyInfos[i] = newEnemyInfosSortedArray[i];

                //if we still have old info, not updated this frame, "predict" its distance
                if (enemyInfos[i].frameCountWhenLastSeen != Time.frameCount)
                {
                    sensing.UpdateEntityInfoDistance(ref enemyInfos[i]);
                }
            }

        }

        #endregion


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
