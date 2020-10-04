using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum DiplomacyStatus
{
    War,
    Neutral,
    Friendly
}

public class SceneSettings : MonoBehaviour
{
    public static SceneSettings Instance;
    public bool allowFriendlyFire;
    public GameEntity playerEntity;
    public int objectsInHandPhysicsLayer;
    public int weaponsInHandPhysicsLayer;
    public int physicalObjectsPhysicsLayer;
    [Tooltip(" After something is thrown, collision between this object and the hand which has thrown it is disabled for a short moment, to preven physics glitches resulting from thoose two objects collidiong")]
    public float enableThrowableAndThrowingHandCollisionDelay;

    // TODO Refactor - shoudl we use this physics multipliers?
    /*
    [Header("Physics")]
    [Tooltip("Only applies to units with ecMovement or PlayerMovement")]
    public float gravityMultiplier = 1;
    [Tooltip("Because with changing the gravityMultiplier we would need to change all forces - we do it here . If gravityMult is 1, this should also be one , if gravity is 8 ,this hsoulkd be 2,6667 or 1/3 of 8")]
    public float forceMultiplier = 1;*/


    void Awake()
    {
        if (Instance != null)
        {
            DestroyImmediate(Instance);
        }
        else
        {
            Instance = this;
        }
    }

    public DiplomacyStatus GetDiplomacyStatus(int myTeam, int otherTeam)
    {
        if (myTeam != otherTeam) return DiplomacyStatus.War;
        else return DiplomacyStatus.Friendly;
    }

    public bool AllowDamage(int myTeamID, int otherTeamID)
    {
        if (!allowFriendlyFire)
        {
            if (myTeamID != otherTeamID)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        else
        {
            return true;
        }
    }

    public GameEntity GetPlayerEntity()
    {
        return playerEntity;
    }


}
