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
        //[NonSerialized] public SensedTacticalPointInfo[] tPCoverPeekInfos = new SensedTacticalPointInfo[0];//not sorted by distance
        public SensedTacticalPointInfo[] tPCoverPeekInfos = new SensedTacticalPointInfo[0];//not sorted by distance

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

        public void UpdateEntityInfos( HashSet<(EntitySensingInterface, float)> newEnemyInfos,  HashSet<(EntitySensingInterface, float)> newFriendlyInfos)
        {
            //-> The new entities are registered, while the older ones arent forgotten completely.

            UpdateEntityInfos(ref newEnemyInfos, ref enemyInfos, maxEnemyInfosCount);
            UpdateEntityInfos(ref newFriendlyInfos, ref friendlyInfos, maxFriendlyInfosCount);

            #region------------Enemies------------------

            //1. Create dictionary of current infos        
            /*Dictionary<int,SensedEntityInfo> currentEnemyInfosDict = new Dictionary<int,SensedEntityInfo>(); //the entity hashSets are the keys
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
            }*/

            #endregion --------------------------------
        }

        public void UpdateTPointInfos( HashSet<(TacticalPoint, float)> newTPCoverInfos,  HashSet<(TacticalPoint, float)> newTPOpenFieldInfos,  HashSet<(TacticalPoint, float)> newTPCoverPeekInfos)
        {
            UpdateTPointInfos(ref newTPCoverInfos, ref tPCoverInfos, maxTPCoverInfosCount);
            UpdateTPointInfos(ref newTPOpenFieldInfos, ref tPOpenFieldInfos, maxTPOpenFieldInfosCount);
            UpdateTPointInfos(ref newTPCoverPeekInfos, ref tPCoverPeekInfos, 10); //it will hopefully never by more than 3
        }

        void UpdateEntityInfos(ref HashSet<(EntitySensingInterface, float)> newEntityInfos, ref SensedEntityInfo[] infosToUpdate, int maxInfosCount)
        {
            //1. Create dictionary of current infos        
            Dictionary<int, SensedEntityInfo> currentEntityInfosDict = new Dictionary<int, SensedEntityInfo>(); //the entity hashSets are the keys
            for (int i = 0; i < infosToUpdate.Length; i++)
            {
                currentEntityInfosDict.Add(infosToUpdate[i].entity.GetHashCode(), infosToUpdate[i]);
            }

            //2. Go through new infos, update the infos in idcitionary if ocntains, or create and add new elements to dictionary
            foreach ((EntitySensingInterface entitySensInterface, float distanceToEntity) newEntityInfoTuple in newEntityInfos)
            {
                int currentKey = newEntityInfoTuple.entitySensInterface.entityAssignedTo.GetHashCode();
                if (currentEntityInfosDict.ContainsKey(currentKey))
                {
                    // update the current info
                    currentEntityInfosDict[currentKey].UpdateInfo(newEntityInfoTuple.distanceToEntity);
                }
                else
                {
                    // or create & add new info
                    currentEntityInfosDict.Add(currentKey, new SensedEntityInfo(newEntityInfoTuple.entitySensInterface, newEntityInfoTuple.distanceToEntity));
                }
            }

            //3. Convert dictionary back to array -->dict.Values.CopyTo(foos, 0);
            SensedEntityInfo[] newInfosSortedArray = new SensedEntityInfo[currentEntityInfosDict.Count];
            currentEntityInfosDict.Values.CopyTo(newInfosSortedArray, 0);

            //4. Sort array according to distance
            Array.Sort(newInfosSortedArray,
            delegate (SensedEntityInfo x, SensedEntityInfo y) { return x.lastDistanceMeasured.CompareTo(y.lastDistanceMeasured); });

            //5 Cut array
            int newInfosArraySize = newInfosSortedArray.Length;
            if (newInfosArraySize > maxInfosCount) newInfosArraySize = maxInfosCount;

            infosToUpdate = new SensedEntityInfo[newInfosArraySize];

            for (int i = 0; i < newInfosArraySize; i++)
            {
                infosToUpdate[i] = newInfosSortedArray[i];

                //if we still have old info, not updated this frame, "predict" its distance
                if (infosToUpdate[i].frameCountWhenLastSeen != Time.frameCount)
                {
                    sensing.UpdateEntityInfoDistance(ref infosToUpdate[i]);
                }
            }

        }

        void UpdateTPointInfos(ref HashSet<(TacticalPoint, float)> newTPInfos, ref SensedTacticalPointInfo[] infosToUpdate, int maxInfosCount)
        {
            //1. Create dictionary of current infos        
            Dictionary<int, SensedTacticalPointInfo> currentTPInfosDict = new Dictionary<int, SensedTacticalPointInfo>(); //the entity hashSets are the keys
            for (int i = 0; i < infosToUpdate.Length; i++)
            {
                currentTPInfosDict.Add(infosToUpdate[i].tacticalPoint.GetHashCode(), infosToUpdate[i]);
            }

            //2. Go through new infos, update the infos in idcitionary if ocntains, or create and add new elements to dictionary
            foreach ((TacticalPoint tPoint, float distanceToPoint) newTPInfoTuple in newTPInfos)
            {
                int currentKey = newTPInfoTuple.tPoint.GetHashCode();
                if (currentTPInfosDict.ContainsKey(currentKey))
                {
                    // update the current info
                    currentTPInfosDict[currentKey].UpdateInfo(newTPInfoTuple.distanceToPoint);
                }
                else
                {
                    // or create & add new info
                    currentTPInfosDict.Add(currentKey, new SensedTacticalPointInfo(newTPInfoTuple.tPoint, newTPInfoTuple.distanceToPoint));
                }
            }

            //3. Convert dictionary back to array -->dict.Values.CopyTo(foos, 0);
            SensedTacticalPointInfo[] newTPInfosSortedArray = new SensedTacticalPointInfo[currentTPInfosDict.Count];
            currentTPInfosDict.Values.CopyTo(newTPInfosSortedArray, 0);

            //4. Sort array according to distance
            Array.Sort(newTPInfosSortedArray,
            delegate (SensedTacticalPointInfo x, SensedTacticalPointInfo y) { return x.lastDistanceMeasured.CompareTo(y.lastDistanceMeasured); });

            //5 Cut array
            int newTPInfoArraySize = newTPInfosSortedArray.Length;
            if (newTPInfoArraySize > maxEnemyInfosCount) newTPInfoArraySize = maxEnemyInfosCount;

            infosToUpdate = new SensedTacticalPointInfo[newTPInfoArraySize];

            for (int i = 0; i < newTPInfoArraySize; i++)
            {
                infosToUpdate[i] = newTPInfosSortedArray[i];

                //if we still have old info, not updated this frame, "predict" its distance
                if (infosToUpdate[i].frameCountWhenLastSeen != Time.frameCount)
                {
                    sensing.UpdateTPInfoDistance(ref infosToUpdate[i]);
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

        public Transform GetCurrentUsedGunShootPoint()
        {
            return characterController.GetCurrentWeaponShootPoint();
        }


    }

}
