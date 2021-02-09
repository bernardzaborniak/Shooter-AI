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

        //Information Evaluated from Sensing Infos
        public Vector3 meanThreatDirection;
        public Transform meanThreatDirectionVisualiser;
        public Vector3 meanFriendlyDirection;
        public Transform meanFriendlyDirectionVisualiser;

        [Tooltip("How does it seem who has the advantage right now in fighting?")]
        public float currentBalanceOfPower;

        public int numberOfEnemiesWhichWereShootingAtMeInTheLast3Seconds;
        (int numOfEnemies,float time) maxNumberOfEnemiesShootingAtMeMemory;
        


        public override void SetUpComponent(GameEntity entity)
        {
            base.SetUpComponent(entity);

        }

    #region Updating Sensing Information

        public void UpdateEntityInfos( HashSet<(EntitySensingInterface, float)> newEnemyInfos,  HashSet<(EntitySensingInterface, float)> newFriendlyInfos)
        {
            //-> The new entities are registered, while the older ones arent forgotten completely.

            UpdateEntityInfos(ref newEnemyInfos, ref enemyInfos, maxEnemyInfosCount, ref meanThreatDirection);
            UpdateEntityInfos(ref newFriendlyInfos, ref friendlyInfos, maxFriendlyInfosCount, ref meanFriendlyDirection);

            UpdateCurrentBalanceOfPower();
            UpdateNumberOfEnemiesWhichWereShootingAtMeInTheLast3Seconds();

            #region Debug visualisation -> delete later
            if (meanThreatDirection != Vector3.zero)
            {
                meanThreatDirectionVisualiser.forward = meanThreatDirection;
                meanThreatDirectionVisualiser.gameObject.SetActive(true);
            }
            else
            {
                meanThreatDirectionVisualiser.gameObject.SetActive(false);

            }
            if (meanFriendlyDirection != Vector3.zero)
            {
                meanFriendlyDirectionVisualiser.forward = meanFriendlyDirection;
                meanFriendlyDirectionVisualiser.gameObject.SetActive(true);
            }
            else
            {
                meanFriendlyDirectionVisualiser.gameObject.SetActive(false);

            }

            #endregion

        }

        public void UpdateTPointInfos( HashSet<(TacticalPoint, float)> newTPCoverInfos,  HashSet<(TacticalPoint, float)> newTPOpenFieldInfos,  HashSet<(TacticalPoint, float)> newTPCoverPeekInfos)
        {
            UpdateTPointInfos(ref newTPCoverInfos, ref tPCoverInfos, maxTPCoverInfosCount);
            UpdateTPointInfos(ref newTPOpenFieldInfos, ref tPOpenFieldInfos, maxTPOpenFieldInfosCount);
            UpdateTPointInfos(ref newTPCoverPeekInfos, ref tPCoverPeekInfos, 10); //it will hopefully never by more than 3
        }

        void UpdateEntityInfos(ref HashSet<(EntitySensingInterface, float)> newEntityInfos, ref SensedEntityInfo[] infosToUpdate, int maxInfosCount, ref Vector3 meanDirectionToUpdate)
        {
            //1. Create dictionary of current infos        
            Dictionary<int, SensedEntityInfo> currentEntityInfosDict = new Dictionary<int, SensedEntityInfo>(); //the entity hashSets are the keys
            for (int i = 0; i < infosToUpdate.Length; i++)
            {
                if (infosToUpdate[i].IsAlive())
                {
                    //update its distance - the ai is "predicting" it
                    sensing.UpdateEntityInfoDistance(ref infosToUpdate[i]);
                    currentEntityInfosDict.Add(infosToUpdate[i].entity.GetHashCode(), infosToUpdate[i]);
                }
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

            //6. create the new updated array
            infosToUpdate = new SensedEntityInfo[newInfosArraySize];
            meanDirectionToUpdate = Vector3.zero;

            for (int i = 0; i < newInfosArraySize; i++)
            {
                infosToUpdate[i] = newInfosSortedArray[i];
                //Normalize the direction and multiply by an inverse of distance, so closer objects are weighted more heavily.
                meanDirectionToUpdate += (infosToUpdate[i].entity.transform.position - myEntity.transform.position).normalized * (1000 - infosToUpdate[i].lastDistanceMeasured);
            }
            meanDirectionToUpdate.Normalize();
        }

        void UpdateCurrentBalanceOfPower()
        {
            float enemyCombinedStrength = 0;
            float friendlyCombinedStrength = 0;

            for (int i = 0; i < enemyInfos.Length; i++)
            {
                enemyCombinedStrength += enemyInfos[i].entityTags.strengthLevel;
            }

            for (int i = 0; i < friendlyInfos.Length; i++)
            {
                friendlyCombinedStrength += friendlyInfos[i].entityTags.strengthLevel;
            }
            friendlyCombinedStrength += myEntity.entityTags.strengthLevel;

            if (enemyCombinedStrength == 0)
            {
                currentBalanceOfPower = 1;
            }
            else 
            {
                currentBalanceOfPower = friendlyCombinedStrength / enemyCombinedStrength;
            }
        }

        void UpdateNumberOfEnemiesWhichWereShootingAtMeInTheLast3Seconds()
        {
            //1 if the maxNumberOfEnemiesShootingAtMeMemory is older than 3 seconds, reset it to num of enemies 0
            if(Time.time- maxNumberOfEnemiesShootingAtMeMemory.time > 3)
            {
                maxNumberOfEnemiesShootingAtMeMemory.time = Time.time;
                maxNumberOfEnemiesShootingAtMeMemory.numOfEnemies = 0;
            }
            //2. check how mayn enemies are shooting at me this frame
            int numOfEnemiesShootingAtMeNow = 0;
            for (int i = 0; i < enemyInfos.Length; i++)
            {
                foreach (EntityActionTag tag in enemyInfos[i].entityTags.actionTags)
                {
                    if (tag.type == EntityActionTag.Type.ShootingAtTarget)
                    {
                        if (tag.shootAtTarget == GetMyEntity())
                        {
                            numOfEnemiesShootingAtMeNow++;
                        }
                    }
                }
            }

            //3. if memory is num0 or the current number of enemies is bigger than memory
            //-> update max memory

            if (numOfEnemiesShootingAtMeNow > maxNumberOfEnemiesShootingAtMeMemory.numOfEnemies)
            {
                maxNumberOfEnemiesShootingAtMeMemory.time = Time.time;
                maxNumberOfEnemiesShootingAtMeMemory.numOfEnemies = numOfEnemiesShootingAtMeNow;
            }

            //4.set the current to max memory
            numberOfEnemiesWhichWereShootingAtMeInTheLast3Seconds = maxNumberOfEnemiesShootingAtMeMemory.numOfEnemies;
        }

        void UpdateTPointInfos(ref HashSet<(TacticalPoint, float)> newTPInfos, ref SensedTacticalPointInfo[] infosToUpdate, int maxInfosCount)
        {
            //1. Create dictionary of current infos        
            Dictionary<int, SensedTacticalPointInfo> currentTPInfosDict = new Dictionary<int, SensedTacticalPointInfo>(); //the entity hashSets are the keys
            for (int i = 0; i < infosToUpdate.Length; i++)
            {
                sensing.UpdateTPInfoDistance(ref infosToUpdate[i]);
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
