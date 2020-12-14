using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Custom Object used for saving and transfering sensing information, needs Expanding with adding some kind of memory
public class AI_SensingInfo
{
    //Enemies
    public AI_SI_EntityVisibilityInfo nearestEnemyInfo;
    public HashSet<AI_SI_EntityVisibilityInfo> enemiesInSensingRadius = new HashSet<AI_SI_EntityVisibilityInfo>();

    //Friendlies
    public HashSet<AI_SI_EntityVisibilityInfo> friendliesInSensingRadius = new HashSet<AI_SI_EntityVisibilityInfo>();

    //Tactical Points
    public HashSet<AI_SI_TacticalPointVisibilityInfo> tPointsCoverInSensingRadius = new HashSet<AI_SI_TacticalPointVisibilityInfo>();
    public HashSet<AI_SI_TacticalPointVisibilityInfo> tPointsOpenFieldInSensingRadius = new HashSet<AI_SI_TacticalPointVisibilityInfo>();

    //Infomation Freshness
    public float lastTimeInfoWasUpdated;
    public int lastFrameCountInfoWasUpdated;


    public AI_SensingInfo()
    {

    }
}
