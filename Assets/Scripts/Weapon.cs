using System.Collections;
using System.Collections.Generic;
using UnityEngine;




public class Weapon : MonoBehaviour
{
    public WeaponInteractionType weaponInteractionType;

    public Transform rightHandIKPosition;
    public Transform leftHandIKPosition;

    //[Tooltip()]
    //public int animationID;

    [Tooltip("Will be read when by aimingController when equipping a weapon, makes sure the offset between weapon and shoulder is correct")]
    public Vector3 weaponAimParentLocalAdjusterOffset;

 
    [Tooltip("some weapons like rpg or machine gun takes longer to get out of the inventory than other weapons")]
    public float pullOutWeaponTime;
    public float hideWeaponTime;

    //"kind of animation played for this item - 0 is bare hands, 1 is rifle, 2 is pistol"
    public int GetWeaponInteractionTypeID()
    {
        if(weaponInteractionType == WeaponInteractionType.BareHands)
        {
            return 0;
        }
        else if (weaponInteractionType == WeaponInteractionType.Rifle)
        {
            return 1;
        }
        else if (weaponInteractionType == WeaponInteractionType.Pistol)
        {
            return 2;
        }

        return 0;
    }
}
