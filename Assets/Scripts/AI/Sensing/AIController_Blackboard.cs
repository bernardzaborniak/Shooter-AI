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
        [Header("Sensed Information")]

        //Infomation Freshness
        public float lastTimeSensingInfoWasUpdated;
        public int lastFrameCountSensingInfoWasUpdated;


        [NonSerialized] public SensedEntityInfo[] enemyInfos = new SensedEntityInfo[0]; //sorted by distance
        [NonSerialized] public SensedEntityInfo[] friendlyInfos = new SensedEntityInfo[0];//sorted by distance

        [NonSerialized] public (TacticalPoint tPoint, float distance)[] tPCoverInfos = new (TacticalPoint, float)[0]; //sorted by distance
        [NonSerialized] public (TacticalPoint tPoint, float distance)[] tPOpenFieldInfos = new (TacticalPoint, float)[0]; //sorted by distance
        //[NonSerialized] public SensedTacticalPointInfo[] tPCoverPeekInfos = new SensedTacticalPointInfo[0];//not sorted by distance
        [NonSerialized] public (TacticalPoint tPoint, float distance)[] tPCoverPeekInfos = new (TacticalPoint, float)[0]; // sorted by distance
        [NonSerialized] public (EnvironmentalDangerTag dangerTag, float distance)[] environmentalDangerInfos = new (EnvironmentalDangerTag, float)[0];//not sorted by distance

        Dictionary<int, (float rating, float timeWhenRated)> tPRatingsCache = new Dictionary<int, (float rating, float timeWhenRated)>(); //the key is the TPoint hashcode
        public float cleanUpTPRatingsCacheInterval = 0.5f;
        public float timeAfterWhichToRecaulculateRating = 1;
        float nextCleanUpTPRatingsCacheTime;

        [SerializeField] int maxEnemyInfosCount;
        [SerializeField] int maxFriendlyInfosCount;
        // [SerializeField] int maxTPCoverInfosCount;
        //[SerializeField] int maxTPOpenFieldInfosCount;
        // [SerializeField] int maxTPCoverPeekInfosCount;

        #endregion

        [Header("Other Information")]

        [SerializeField] TacticalPoint currentlyUsedTPoint;
        [SerializeField] TacticalPoint currentlyTargetedTPoint;


        //Information Evaluated from Sensing Infos
        public Vector3 meanThreatDirection;
        public Vector3 meanFriendlyDirection;

        public bool visuasliseThreatAndFriendlyDirections;
        public Transform meanThreatDirectionVisualiser;
        public Transform meanFriendlyDirectionVisualiser;

        [Tooltip("How does it seem who has the advantage right now in fighting?")]
        public float currentBalanceOfPower;

        public int numberOfEnemiesShootingAtMeLast3Sec;
        (int numOfEnemies,float time) maxNumberOfEnemiesShootingAtMeMemory;


        #endregion



        public override void SetUpComponent(GameEntity entity)
        {
            base.SetUpComponent(entity);

            nextCleanUpTPRatingsCacheTime = Time.time + UnityEngine.Random.Range(0, cleanUpTPRatingsCacheInterval);

        }

        public override void UpdateComponent()
        {
            //clear ratings cache if needed
            if(Time.time> nextCleanUpTPRatingsCacheTime)
            {
                nextCleanUpTPRatingsCacheTime = Time.time + cleanUpTPRatingsCacheInterval;

                HashSet<int> infosToRemove = new HashSet<int>();

                //Choose which old infos to delete
                foreach (KeyValuePair<int, (float rating, float timeWhenRated)> ratingInfo in tPRatingsCache)
                {
                    if(Time.time - ratingInfo.Value.timeWhenRated > timeAfterWhichToRecaulculateRating)
                    {
                        infosToRemove.Add(ratingInfo.Key);
                    }
                }

                //Delete old infos
                foreach (int ratingInfoKey in infosToRemove)
                {
                    tPRatingsCache.Remove(ratingInfoKey);
                }
            }
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

            if (visuasliseThreatAndFriendlyDirections)
            {
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
            }
            else
            {
                meanThreatDirectionVisualiser.gameObject.SetActive(false);
                meanFriendlyDirectionVisualiser.gameObject.SetActive(false);
            }
           

            #endregion

        }

        public void UpdateTPointInfos( HashSet<(TacticalPoint, float)> newTPCoverInfos,  HashSet<(TacticalPoint, float)> newTPOpenFieldInfos,  HashSet<(TacticalPoint, float)> newTPCoverPeekInfos)
        {
            // UpdateTPointInfos(ref newTPCoverInfos, ref tPCoverInfos, maxTPCoverInfosCount);
            // UpdateTPointInfos(ref newTPOpenFieldInfos, ref tPOpenFieldInfos, maxTPOpenFieldInfosCount);
            // UpdateTPointInfos(ref newTPCoverPeekInfos, ref tPCoverPeekInfos, maxTPCoverPeekInfosCount); //it will hopefully never by more than 3

           // UpdateTPointInfos(ref newTPCoverInfos, ref tPCoverInfos);
            //UpdateTPointInfos(ref newTPOpenFieldInfos, ref tPOpenFieldInfos);
            //UpdateTPointInfos(ref newTPCoverPeekInfos, ref tPCoverPeekInfos); //it will hopefully never by more than 3


            // Cover Points
            tPCoverInfos = new (TacticalPoint, float)[newTPCoverInfos.Count];
            newTPCoverInfos.CopyTo(tPCoverInfos, 0);

            Array.Sort(tPCoverInfos,
            delegate ((TacticalPoint tPoint, float distance) x, (TacticalPoint tPoint, float distance) y) { return x.distance.CompareTo(y.distance); });

            // Open Field Points
            tPOpenFieldInfos = new (TacticalPoint, float)[newTPOpenFieldInfos.Count];
            newTPOpenFieldInfos.CopyTo(tPOpenFieldInfos, 0);
            Array.Sort(tPOpenFieldInfos,
            delegate ((TacticalPoint tPoint, float distance) x, (TacticalPoint tPoint, float distance) y) { return x.distance.CompareTo(y.distance); });

            // Cover Peek Points
            tPCoverPeekInfos = new (TacticalPoint, float)[newTPCoverPeekInfos.Count];
            newTPCoverPeekInfos.CopyTo(tPCoverPeekInfos, 0);
            Array.Sort(tPCoverPeekInfos,
            delegate ((TacticalPoint tPoint, float distance) x, (TacticalPoint tPoint, float distance) y) { return x.distance.CompareTo(y.distance); });

        }

        public void UpdateEnvironmentalDangerInfos(HashSet<(EnvironmentalDangerTag dangerTag, float distance)> dangerInfos)
        {
            environmentalDangerInfos = new (EnvironmentalDangerTag, float)[dangerInfos.Count];

            dangerInfos.CopyTo(environmentalDangerInfos,0);

            Array.Sort(environmentalDangerInfos,
           delegate ((EnvironmentalDangerTag dangerTag, float distance) x, (EnvironmentalDangerTag dangerTag, float distance) y) { return x.distance.CompareTo(y.distance); });
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
            numberOfEnemiesShootingAtMeLast3Sec = maxNumberOfEnemiesShootingAtMeMemory.numOfEnemies;
        }

        // Deprecated
        void UpdateTPointInfos(ref HashSet<(TacticalPoint tPoint, float distanceToPoint)> newTPInfos, ref SensedTacticalPointInfo[] infosToUpdate)//, int maxInfosCount)
        {


            //infosToUpdate = new SensedTacticalPointInfo[newTPInfos.Count];
            /* SensedTacticalPointInfo[] newInfosArray = new SensedTacticalPointInfo[newTPInfos.Count];
             //(TacticalPoint tPoint, float distanceToPoint) [] infosToUpdateTuples = new (TacticalPoint, float)[newTPInfos.Count];
             int counter = 0;
             foreach ((TacticalPoint tPoint, float distanceToPoint) item in newTPInfos)
             {
                 newInfosArray[counter] = new SensedTacticalPointInfo(item.tPoint, item.distanceToPoint);
                 counter++;
             }

             //4. Sort array according to distance
             Array.Sort(newInfosArray,
             delegate (SensedTacticalPointInfo x, SensedTacticalPointInfo y) { return x.lastDistanceMeasured.CompareTo(y.lastDistanceMeasured); });

            //5 Cut array
             int newTPInfoArraySize = newInfosArray.Length;
             if (newTPInfoArraySize > maxInfosCount) newTPInfoArraySize = maxInfosCount;

             infosToUpdate = new SensedTacticalPointInfo[newTPInfoArraySize];
             for (int i = 0; i < newTPInfoArraySize; i++)
             {
                 infosToUpdate[i] = newInfosArray[i];
             }*/

           infosToUpdate = new SensedTacticalPointInfo[newTPInfos.Count];
            //(TacticalPoint tPoint, float distanceToPoint) [] infosToUpdateTuples = new (TacticalPoint, float)[newTPInfos.Count];
            int counter = 0;
            foreach ((TacticalPoint tPoint, float distanceToPoint) item in newTPInfos)
            {
                infosToUpdate[counter] = new SensedTacticalPointInfo(item.tPoint, item.distanceToPoint);
                counter++;
            }

            //4. Sort array according to distance
            Array.Sort(infosToUpdate,
            delegate (SensedTacticalPointInfo x, SensedTacticalPointInfo y) { return x.lastDistanceMeasured.CompareTo(y.lastDistanceMeasured); });









            //1. Create dictionary of current infos        
            /* Dictionary<int, SensedTacticalPointInfo> currentTPInfosDict = new Dictionary<int, SensedTacticalPointInfo>(); //the entity hashSets are the keys
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
             }*/

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

        public void SetCurrentlyTargetedTacticalPoint(TacticalPoint targetedPoint)
        {
            currentlyTargetedTPoint = targetedPoint;
        }

        public TacticalPoint GetCurrentlyTargetedPoint()
        {
            return currentlyTargetedTPoint;
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

        #region Rating TPoints

        public float GetTPTacticalRating(TacticalPoint tPoint)
        {
            float rating = 0;
            //tPoint. hashCode is the dicitionary key

            //check if rating is needed - only check the target point, as all correspnding points are rated together
            if (!tPRatingsCache.ContainsKey(tPoint.GetHashCode()))
            {
                RateTPTogetherWithCorrespondingPoints(tPoint);
            }


            if (tPoint.tacticalPointType == TacticalPointType.CoverPoint)
            {
                //return cover point rating *0.7 + best peek point rating;

                rating = 0.7f * tPRatingsCache[tPoint.GetHashCode()].rating;

                float bestPeekPointRating = 0;
                for (int i = 0; i < tPoint.correspondingCoverPeekPoints.Length; i++)
                {
                    float peekPointRating = tPRatingsCache[tPoint.correspondingCoverPeekPoints[i].GetHashCode()].rating;
                    if (peekPointRating > bestPeekPointRating)
                    {
                        bestPeekPointRating = peekPointRating;
                    }
                }

                rating += 0.3f * bestPeekPointRating;
                return rating;
            }
            else if(tPoint.tacticalPointType == TacticalPointType.CoverPeekPoint)
            {
                //return peek point * 0.7 + cover point *0.3f

                rating = 0.7f * tPRatingsCache[tPoint.GetHashCode()].rating + 0.3f * tPRatingsCache[tPoint.correspondingCoverPoint.GetHashCode()].rating;
                return rating;
            }

            return rating;
        }

        public void RateTPTogetherWithCorrespondingPoints(TacticalPoint tPoint)
        {
            //rates the cover or peek point together with the coresponding cover or peek points

            //1. dertermin coverPoint parent, rate this
            //" go through all peek points, rate them, only calculate distance to enemy once for cover point it there is an enemy

            TacticalPoint coverPoint = null;

            if(tPoint.tacticalPointType == TacticalPointType.CoverPoint)
            {
                coverPoint = tPoint;
            }
            else if(tPoint.tacticalPointType == TacticalPointType.CoverPeekPoint)
            {
                coverPoint = tPoint.correspondingCoverPoint;
            }


            //Rate all 0 when no enemy is visible
            if(enemyInfos.Length == 0)
            {
                tPRatingsCache.Add(coverPoint.GetHashCode(), (0, Time.time));
                for (int i = 0; i < coverPoint.correspondingCoverPeekPoints.Length; i++)
                {
                    tPRatingsCache.Add(coverPoint.correspondingCoverPeekPoints[i].GetHashCode(), (0, Time.time));
                }

                return;
            }
           


            //Rate Cover Point:
            (float distance, float quality) tPCoverRatingForDirection = coverPoint.GetRatingForDirection(meanThreatDirection);

            float coverPointRating;

            if (tPCoverRatingForDirection.distance < 2)
            {
                if (tPCoverRatingForDirection.quality > 0.5f) coverPointRating = 1;
                else coverPointRating = tPCoverRatingForDirection.quality;
            }
            else
            {
                coverPointRating = 0;
            }
            //Save Cover Point Rating

            tPRatingsCache.Add(coverPoint.GetHashCode(), (coverPointRating, Time.time));

            //Rate Cover Peek Points & Save their Ratings


            float distanceToEnemyFromPoint = Vector3.Distance(enemyInfos[0].GetEntityPosition(), coverPoint.transform.position);

            for (int i = 0; i < coverPoint.correspondingCoverPeekPoints.Length; i++)
            {
                (float distance, float quality) tPCoverPeekRatingForDirection = coverPoint.correspondingCoverPeekPoints[i].GetRatingForDirection(meanThreatDirection);

                if (distanceToEnemyFromPoint < tPCoverPeekRatingForDirection.distance)
                {
                    tPRatingsCache.Add(coverPoint.correspondingCoverPeekPoints[i].GetHashCode(), (1, Time.time));
                }
                else
                {
                    tPRatingsCache.Add(coverPoint.correspondingCoverPeekPoints[i].GetHashCode(), (0, Time.time));
                }
            }
        }

        #endregion


    }

}
