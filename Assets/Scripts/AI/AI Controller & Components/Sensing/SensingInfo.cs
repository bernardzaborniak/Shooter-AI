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
        HashSet<SensedEntityInfo> enemiesInSensingRadiusPool;
        HashSet<SensedEntityInfo> enemiesInSensingRadius;// = new HashSet<SensedEntityInfo>();

        //Friendlies
        HashSet<SensedEntityInfo> friendliesInSensingRadiusPool;
        HashSet<SensedEntityInfo> friendliesInSensingRadius;// = new HashSet<SensedEntityInfo>();

        //Tactical Points
        HashSet<SensedTacticalPointInfo> tPointsCoverInSensingRadiusPool;// = new HashSet<SensedTacticalPointInfo>();
        HashSet<SensedTacticalPointInfo> tPointsCoverInSensingRadius;// = new HashSet<SensedTacticalPointInfo>();

        HashSet<SensedTacticalPointInfo> tPointsOpenFieldInSensingRadiusPool;// = new HashSet<SensedTacticalPointInfo>();
        HashSet<SensedTacticalPointInfo> tPointsOpenFieldInSensingRadius;// = new HashSet<SensedTacticalPointInfo>();

        //Infomation Freshness
        public float lastTimeInfoWasUpdated;
        public int lastFrameCountInfoWasUpdated;


        public SensingInfo(int enemiesPoolSize, int firendliesPoolSize, int tPCoverPoolSize, int tPOpenFieldPoolSize)
        {
            enemiesInSensingRadius = new HashSet<SensedEntityInfo>();

            //insitialise all sets - maybe also sort the collection by how recent the infomation is? this way removing or corrupting the old informaiton would be more performant?
        }

        public void OnSensedEnemyEntity(SensedEntityInfo enemyEntityInfo)
        {

        }

        public void OnSensedFriendlyEntity(SensedEntityInfo friendlyEntityInfo)
        {

        }

        public void OnSensedTPCover(SensedTacticalPointInfo tpCoverInfo)
        {

        }

        public void OnSensedTPOpenField(SensedTacticalPointInfo tpOpenFieldInfo)
        {

        }

        //this update thould be called every frame to simulate forgetting?
        public void UpdateSensingInfo()
        {

        }
    }

}
