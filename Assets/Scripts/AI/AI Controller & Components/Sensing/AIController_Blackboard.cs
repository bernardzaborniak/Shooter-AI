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

        //Enemies
        [NonSerialized] public SensedEntityInfo[] enemyInfos = new SensedEntityInfo[0]; //sorted by distance
        Queue<SensedEntityInfo> enemyInfoPool = new Queue<SensedEntityInfo>();

        //Friendlies
        [NonSerialized] public SensedEntityInfo[] friendlyInfos = new SensedEntityInfo[0];//sorted by distance
        Queue<SensedEntityInfo> friendlyInfosPool = new Queue<SensedEntityInfo>();


        //Tactical Points Cover
        //Queue<SensedTacticalPointInfo> tPointCoverInfoPool = new Queue<SensedTacticalPointInfo>();
        [NonSerialized] public SensedTacticalPointInfo[] tPointCoverInfos = new SensedTacticalPointInfo[0];//sorted by distance

        //Tactical Points Open Field
        //Queue<SensedTacticalPointInfo> tPointOpenFieldInfoPool = new Queue<SensedTacticalPointInfo>();
        [NonSerialized] public SensedTacticalPointInfo[] tPointOpenFieldInfos = new SensedTacticalPointInfo[0];//sorted by distance

        //Tactical Points Cover Shoot
        //Queue<SensedTacticalPointInfo> tPointCoverShootInfoPool = new Queue<SensedTacticalPointInfo>();
        [NonSerialized] public SensedTacticalPointInfo[] tPointCoverShootInfos = new SensedTacticalPointInfo[0];//not sorted by distance


        [SerializeField] int enemyInfosPoolSize;
        [SerializeField] int friendlyInfosPoolSize;
        [SerializeField] int tPCoverInfosPoolSize;
        [SerializeField] int tPOpenFieldInfosPoolSize;
        //int tPCoverShootInfosPoolSize = 5;

        #endregion

        [SerializeField] TacticalPoint currentlyUsedTPoint;

        #endregion


        public override void SetUpComponent(GameEntity entity)
        {
            base.SetUpComponent(entity);

            //myTeamID = myEntity.teamID; // cached for optimisation.

            // Fill the Pools
            for (int i = 0; i < enemyInfosPoolSize; i++)
            {
                enemyInfoPool.Enqueue(new SensedEntityInfo()); //rework the neemy Sensed Info Constructor to not have params
            }
            for (int i = 0; i < friendlyInfosPoolSize; i++)
            {
                friendlyInfosPool.Enqueue(new SensedEntityInfo()); //rework the neemy Sensed Info Constructor to not have params
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

        #region Updating Sensing Information

        public void UpdateEntityInfos(HashSet<SensedEntityInfo> newEnemyInfos, HashSet<SensedEntityInfo> newFriendlyInfos)
        {
            //Remember, newEnemyInfos & newFriendlyInfos are readonly, dont change them or the two pools of SensedEntityInfo inside blackboard & sensing will start messing with each other :(

            //-> The new entities are registered, while the older ones arent forgotten completely.

            #region Register new Enemies

            HashSet<SensedEntityInfo> oldEnemiesToHoldOn = new HashSet<SensedEntityInfo>();

            //Carry over old values which arent overriden to the new values set
            for (int i = 0; i < enemyInfos.Length; i++)
            {
                if (!newEnemyInfos.Contains(enemyInfos[i]))
                {
                    //If hasnt died yet
                    if (enemyInfos[i].IsAlive())
                    {
                        //Update distance inside info, the unit can roughly predit how far the unit will be by now.
                        sensing.UpdateEntityInfoDistance(ref enemyInfos[i]);
                        oldEnemiesToHoldOn.Add(enemyInfos[i]);
                    }
                    
                }

                //Return the infos to the pool again
                enemyInfoPool.Enqueue(enemyInfos[i]);
            }


            //Convert the HashSet to Array & Sort it
            SensedEntityInfo[] newEnemyInfosSortedArray = new SensedEntityInfo[newEnemyInfos.Count + oldEnemiesToHoldOn.Count];
            newEnemyInfos.CopyTo(newEnemyInfosSortedArray,0);
            oldEnemiesToHoldOn.CopyTo(newEnemyInfosSortedArray, newEnemyInfos.Count);


            Array.Sort(newEnemyInfosSortedArray,
            delegate (SensedEntityInfo x, SensedEntityInfo y) { return x.lastDistanceMeasured.CompareTo(y.lastDistanceMeasured); });


            int newEnemyInfoArraySize = newEnemyInfosSortedArray.Length;
            if(newEnemyInfoArraySize > enemyInfosPoolSize) newEnemyInfoArraySize = enemyInfosPoolSize;

            enemyInfos = new SensedEntityInfo[newEnemyInfoArraySize];

            for (int i = 0; i < newEnemyInfoArraySize; i++)
            {
                enemyInfos[i] = enemyInfoPool.Dequeue();
                enemyInfos[i].CopyInfo(newEnemyInfosSortedArray[i]);  //The info is copied - this prevents referencing objects from the sensing pool, we only want objects from the blackboard pool here.
            }

            #endregion

            #region Register new Friendlies

            HashSet<SensedEntityInfo> oldFriendliesToHoldOn = new HashSet<SensedEntityInfo>();

            //Carry over old values which arent overriden to the new values set
            for (int i = 0; i < friendlyInfos.Length; i++)
            {
                if (!newFriendlyInfos.Contains(friendlyInfos[i]))
                {
                    //if hasnt died yet
                    if (friendlyInfos[i].IsAlive())
                    {
                        //Update distance inside info, the unit can roughly predit how far the unit will be by now.
                        sensing.UpdateEntityInfoDistance(ref friendlyInfos[i]);
                        oldFriendliesToHoldOn.Add(friendlyInfos[i]);
                    }

                }

                //Return the infos to the pool again
                friendlyInfosPool.Enqueue(friendlyInfos[i]);
            }


            //convert the HashSet to Array & Sort it
            SensedEntityInfo[] newFriendlyInfosSortedArray = new SensedEntityInfo[newFriendlyInfos.Count + oldFriendliesToHoldOn.Count];
            newFriendlyInfos.CopyTo(newFriendlyInfosSortedArray, 0);
            oldFriendliesToHoldOn.CopyTo(newFriendlyInfosSortedArray, newFriendlyInfos.Count);


            Array.Sort(newFriendlyInfosSortedArray,
            delegate (SensedEntityInfo x, SensedEntityInfo y) { return x.lastDistanceMeasured.CompareTo(y.lastDistanceMeasured); });


            int newFriendlyInfoArraySize = newFriendlyInfosSortedArray.Length;
            if (newFriendlyInfoArraySize > friendlyInfosPoolSize) newFriendlyInfoArraySize = friendlyInfosPoolSize;

            friendlyInfos = new SensedEntityInfo[newFriendlyInfoArraySize];

            for (int i = 0; i < newFriendlyInfoArraySize; i++)
            {
                friendlyInfos[i] = friendlyInfosPool.Dequeue();
                friendlyInfos[i].CopyInfo(newFriendlyInfosSortedArray[i]); //The info is copied - this prevents referencing objects from the sensing pool, we only want objects from the blackboard pool here.

            }

            #endregion
        }

        public void UpdateTPointInfos(HashSet<SensedTacticalPointInfo> newCoverPointInfos, HashSet<SensedTacticalPointInfo> newOpenFieldInfos, HashSet<SensedTacticalPointInfo> coverShootPointInfos)
        {
            //For TPoints, we just override them, the references here are referencing SensedTacticalPointInfo objects inside the SensingPool.

            tPointCoverInfos = new SensedTacticalPointInfo[newCoverPointInfos.Count];
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
