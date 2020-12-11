using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[System.Serializable]
//custom object uised for saving and transfering sensing information
public class SensingInfo
{
    //Enemeis
    public AIC_S_EntityVisibilityInfo nearestEnemyInfo;
    public HashSet<AIC_S_EntityVisibilityInfo> enemiesInSensingRadius = new HashSet<AIC_S_EntityVisibilityInfo>();

    //Firendlies
    public HashSet<AIC_S_EntityVisibilityInfo> friendliesInSensingRadius = new HashSet<AIC_S_EntityVisibilityInfo>();

    //tacticalPoints
    public HashSet<AIC_S_TacticalPointVisibilityInfo> tacticalPointsInSensingRadius = new HashSet<AIC_S_TacticalPointVisibilityInfo>();

    public float lastTimeInfoWasUpdated;




    public SensingInfo()
    {

    }
}
