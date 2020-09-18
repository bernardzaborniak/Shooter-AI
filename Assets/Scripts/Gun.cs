using System.Collections;
using System.Collections.Generic;
using UnityEngine;




public class Gun : Item
{
    public Transform rightHandIKPosition;
    public Transform leftHandIKPosition;

    [Tooltip("Will be read when by aimingController when equipping a weapon, makes sure the offset between weapon and shoulder is correct")]
    public Vector3 weaponAimParentLocalAdjusterOffset;


    [Header("Shooting")]
    public GameObject projectile;
    public Transform shootPoint;
    public int magazineSize;
    int bulletsInMagazine;
    [Tooltip("How long is this weapon being reloaded in seconds?")]
    public float defaultReloadDuration;

    public float rateOfFire;
    float shootInterval;
    float nextShootTime;


    private void Start()
    {
        shootInterval = 1 / (rateOfFire / 60);
        bulletsInMagazine = magazineSize;
    }

    //"kind of animation played for this item - 0 is bare hands, 1 is rifle, 2 is pistol"
   

    public void Shoot()
    {
        if (bulletsInMagazine > 0)
        {
            if(Time.time> nextShootTime)
            {
                Instantiate(projectile, shootPoint.position, shootPoint.rotation);
                nextShootTime = Time.time + shootInterval;
                bulletsInMagazine--;
            }
        }
    }

    public int GetBulletsInMagazineLeft()
    {
        return bulletsInMagazine;
    }

    public bool AreBulletsLeftInMagazine()
    {
        if (bulletsInMagazine > 0)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public void RefillBulletsInMagazine()
    {
        bulletsInMagazine = magazineSize;
    }
}
