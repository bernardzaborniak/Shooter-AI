using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BenitosAI
{

    // Custom Object used for saving and transfering sensing information, needs Expanding with adding some kind of memory
    public class SensingInfo
    {
        //Enemies
        //SensedEntityInfo nearestEnemyInfo;
        Queue<SensedEntityInfo> enemiesInSensingRadiusPool;
        Dictionary<GameEntity,SensedEntityInfo> enemiesInSensingRadius;// = new HashSet<SensedEntityInfo>();

        //Friendlies
        Queue<SensedEntityInfo> friendliesInSensingRadiusPool;
        Dictionary<GameEntity,SensedEntityInfo> friendliesInSensingRadius;// = new HashSet<SensedEntityInfo>();

        //Tactical Points
        Queue<SensedTacticalPointInfo> tPointsCoverInSensingRadiusPool;// = new HashSet<SensedTacticalPointInfo>();
        Dictionary<TacticalPoint,SensedTacticalPointInfo> tPointsCoverInSensingRadius;// = new HashSet<SensedTacticalPointInfo>();

        Queue<SensedTacticalPointInfo> tPointsOpenFieldInSensingRadiusPool;// = new HashSet<SensedTacticalPointInfo>();
        Dictionary<TacticalPoint, SensedTacticalPointInfo> tPointsOpenFieldInSensingRadius;// = new HashSet<SensedTacticalPointInfo>();

        //Infomation Freshness
        public float lastTimeInfoWasUpdated;
        public int lastFrameCountInfoWasUpdated;


        public SensingInfo(int sensingEntityTeamID, int enemiesPoolSize, int firendliesPoolSize, int tPCoverPoolSize, int tPOpenFieldPoolSize)
        {
            // Initialise Collections
            enemiesInSensingRadiusPool = new Queue<SensedEntityInfo>();
            enemiesInSensingRadius = new Dictionary<GameEntity, SensedEntityInfo>();

            friendliesInSensingRadiusPool = new Queue<SensedEntityInfo>();
            friendliesInSensingRadius = new Dictionary<GameEntity, SensedEntityInfo>();

            tPointsCoverInSensingRadiusPool = new Queue<SensedTacticalPointInfo>();
            tPointsCoverInSensingRadius = new Dictionary<TacticalPoint, SensedTacticalPointInfo>();

            tPointsOpenFieldInSensingRadiusPool = new Queue<SensedTacticalPointInfo>();
            tPointsOpenFieldInSensingRadius = new Dictionary<TacticalPoint, SensedTacticalPointInfo>();

            // Fill the Pools
            for (int i = 0; i < enemiesPoolSize; i++)
            {
                enemiesInSensingRadiusPool.Enqueue(new SensedEntityInfo()); //rework the neemy Sensed Info Constructor to not have params
            }
            for (int i = 0; i < firendliesPoolSize; i++)
            {
                friendliesInSensingRadiusPool.Enqueue(new SensedEntityInfo()); //rework the neemy Sensed Info Constructor to not have params
            }
            for (int i = 0; i < tPCoverPoolSize; i++)
            {
                tPointsCoverInSensingRadiusPool.Enqueue(new SensedTacticalPointInfo()); //rework the neemy Sensed Info Constructor to not have params
            }
            for (int i = 0; i < tPOpenFieldPoolSize; i++)
            {
                tPointsOpenFieldInSensingRadiusPool.Enqueue(new SensedTacticalPointInfo()); //rework the neemy Sensed Info Constructor to not have params
            }

        }

        /*public void OnSensedEntity(EntityVisibilityInfo visInfo)
        {
            //calculate distance based on vis info & check teamID
            int teamID = visInfo.entityAssignedTo.teamID;

            if()
        }*/

        public void OnSensedEnemyEntity(EntityVisibilityInfo enemyEntityVisInfo, float squaredDistance)
        {
            //calculate distance
            //float squaredDistance = (myPosition - collidersInRadius[i].transform.position).sqrMagnitude;

            //1. Check if there already is older information about this enemy saved in the hashSet, if yes, just update the values on this object
            if (enemiesInSensingRadius.ContainsKey(enemyEntityVisInfo.entityAssignedTo))
            {
                enemiesInSensingRadius[enemyEntityVisInfo.entityAssignedTo].SetUpInfo(enemyEntityVisInfo, squaredDistance);
            }
            else
            {
                //2. If no, check if pool count >0 -> 
                if (enemiesInSensingRadiusPool.Count > 0)
                {
                    //  2.1 if yes dequeue from pool and add to hashset and set values
                    SensedEntityInfo info = enemiesInSensingRadiusPool.Dequeue();
                    info.SetUpInfo(enemyEntityVisInfo, squaredDistance);
                }
                else
                {
                    //  2.2 if no, foreach through whole hashset to find oldest information and overwrite it with the new ne 
                }
            }

           
           
           
        }

        public void OnSensedFriendlyEntity(EntityVisibilityInfo friendlyEntityVisInfo, float squaredDistance)
        {

        }

        public void OnSensedTPCover(TacticalPoint tPointCover)
        {

        }

        public void OnSensedTPOpenField(TacticalPoint tPointOpenField)
        {

        }

        //this update thould be called every frame to simulate forgetting?
        public void UpdateSensingInfo()
        {
            //to exapnd later - here forgetting and corrupting memories would be simulated
        }
    }

}
