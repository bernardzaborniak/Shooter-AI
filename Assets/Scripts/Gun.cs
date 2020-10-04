using System.Collections;
using System.Collections.Generic;
using UnityEngine;




public class Gun : Item, IItemWithIKHandPositions
{
    [SerializeField]
    Transform rightHandIKPosition;
    [SerializeField]
    Transform leftHandIKPosition;

    [Tooltip("Will be read when by aimingController when equipping a weapon, makes sure the offset between weapon and shoulder is correct")]
    public Vector3 weaponAimParentLocalAdjusterOffset;


    [Header("Shooting")]
    public GameObject projectile;
    public Transform shootPoint;
    public float projectileLaunchVelocity;
    public int magazineSize;
    int bulletsInMagazine;
    [Tooltip("How long is this weapon being reloaded in seconds?")]
    public float defaultReloadDuration;

    public float rateOfFire;
    float shootInterval;
    float nextShootTime;

    [Tooltip("If the bullet drops fast due to gravity, have this ticked as true, then the aiming Ai will calculate the aiming direction according to the projectile flight arc")]
    public bool aimWithAngledShotCalculation;

    public RecoilStatsGun recoilStats;
    RecoilInfo gunRecoilInfo;


    private void Start()
    {
        shootInterval = 1 / (rateOfFire / 60);
        bulletsInMagazine = magazineSize;

        //Set up recoil Info
        gunRecoilInfo = new RecoilInfo();
        gunRecoilInfo.SetRecoilForces(recoilStats.recoilUpShootForce, recoilStats.recoilSideShootForce, recoilStats.recoilBackShootForce);
        gunRecoilInfo.SetRecoilUpValues(recoilStats.maxReduceRecoilUpSpeedTwoHanded, recoilStats.maxReduceRecoilUpSpeedTwoHanded, recoilStats.maxReduceRecoilUpSpeedTwoHanded,recoilStats.maxRotationUpTwoHanded);
        gunRecoilInfo.SetRecoilSideValues(recoilStats.maxReduceRecoilSideSpeedTwoHanded, recoilStats.maxReduceRecoilSideSpeedTwoHanded, recoilStats.maxReduceRecoilSideSpeedTwoHanded, recoilStats.maxRotationSideTwoHanded);
        gunRecoilInfo.SetRecoilBackValues(recoilStats.maxReduceRecoilBackSpeedTwoHanded, recoilStats.maxReduceRecoilBackSpeedTwoHanded, recoilStats.maxReduceRecoilBackSpeedTwoHanded, recoilStats.maxPositionBackTwoHanded);
        Debug.Log("Gun recoil info set up");
    }

    //"kind of animation played for this item - 0 is bare hands, 1 is rifle, 2 is pistol"
    

    public bool Shoot()
    {
        if (bulletsInMagazine > 0)
        {
            if(Time.time> nextShootTime)
            {
                GameObject projectileGO = Instantiate(projectile, shootPoint.position, shootPoint.rotation);
                Rigidbody projectileGORB = projectileGO.GetComponent<Rigidbody>();
                projectileGORB.velocity = projectileGORB.transform.forward * projectileLaunchVelocity;
                nextShootTime = Time.time + shootInterval;
                bulletsInMagazine--;

                return true;
            }
        }

        return false;
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

    public Vector3 GetRightHandIKPosition()
    {
        return rightHandIKPosition.position;
    }

    public Vector3 GetLeftHandIKPosition()
    {
        return leftHandIKPosition.position;
    }

    public Quaternion GetRightHandIKRotation()
    {
        return rightHandIKPosition.rotation;
    }

    public Quaternion GetLeftHandIKRotation()
    {
        return leftHandIKPosition.rotation;
    }

    public RecoilInfo GetRecoilInfo()
    {
        return gunRecoilInfo;
    }
}
