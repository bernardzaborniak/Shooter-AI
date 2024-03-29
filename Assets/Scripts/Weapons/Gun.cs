﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;




public class Gun : Item, IItemWithIKHandPositions
{
    [SerializeField]
    Transform rightHandIKPosition;
    [SerializeField]
    Transform leftHandIKPosition;

    [Header("For Animation & Constraints")]
    [Tooltip("Will be read when by aimingController when equipping a weapon, makes sure the offset between weapon and shoulder is correct")]
    public Vector3 weaponAimParentLocalAdjusterOffset;
    public float spineOverrideAnimatedYRotationWeight;


    [Header("Shooting")]
    // public GameObject projectile;
    public AmmoType ammoType;
    public Transform shootPoint;
    public float damage;
    public float projectileLaunchVelocity;
    public float bloom;
    //Override Frienldy fire for specific Weapons
    public bool overrideFriendlyFireSetting;
    [ShowWhen("overrideFriendlyFireSetting")]
    public bool overridenFriendlyFireOn;

    [Min(1)]
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

    bool usedByPlayer = false;
    GameEntity wieldingEntity;
    IMoveable wieldingEntityMoveable;

    [Header("Visuals")]
    public ParticleSystem shootParticle;


    //should be set up instead of start, cause start wont execute if this object is inactive inside the inventory hierarchy
    public void SetUp()
    {
        shootInterval = 1 / (rateOfFire / 60);
        bulletsInMagazine = magazineSize;

        //Set up recoil Info
        gunRecoilInfo = new RecoilInfo();
        gunRecoilInfo.SetRecoilForces(recoilStats.recoilUpShootForce, recoilStats.recoilSideShootForce, recoilStats.recoilBackShootForce);
        gunRecoilInfo.SetRecoilUpValues(recoilStats.maxRecoilUpSpeedTwoHanded, recoilStats.maxReduceRecoilUpAccelerationTwoHanded, recoilStats.maxReduceRecoilUpSpeedTwoHanded,recoilStats.maxRotationUpTwoHanded);
        gunRecoilInfo.SetRecoilSideValues(recoilStats.maxRecoilSideSpeedTwoHanded, recoilStats.maxReduceRecoilSideAccelerationTwoHanded, recoilStats.maxReduceRecoilSideSpeedTwoHanded, recoilStats.maxRotationSideTwoHanded);
        gunRecoilInfo.SetRecoilBackValues(recoilStats.maxRecoilBackSpeedTwoHanded, recoilStats.maxReduceRecoilBackAccelerationTwoHanded, recoilStats.maxReduceRecoilBackSpeedTwoHanded, recoilStats.maxPositionBackTwoHanded);
    }

    //"kind of animation played for this item - 0 is bare hands, 1 is rifle, 2 is pistol"
    

    public bool Shoot()
    {
        if (bulletsInMagazine > 0)
        {
            if(Time.time> nextShootTime)
            {
                #region Spawn & Activate Bullet
                Projectile projectile = PoolingManager.Instance.SpawnAmmoFromPool(ammoType, shootPoint.position, Utility.CalculateRandomBloomInConeShapeAroundTransformForward(shootPoint,bloom)).GetComponent<Projectile>();

                //IMoveable wieldingEntityMoveable = wieldingEntity.GetComponent<IMoveable>();
                Vector3 weaponHolderMovementSpeed = Vector3.zero;
                if (wieldingEntityMoveable != null)
                {
                    weaponHolderMovementSpeed = wieldingEntityMoveable.GetCurrentVelocity();
                }
                else
                {
                    Debug.Log("Warning: No IMoveable could be found on shooting Player- check this to improve realism");
                }
                
                if (!usedByPlayer)
                {
                    projectile.Activate(false, wieldingEntity, this, weaponHolderMovementSpeed, damage, projectileLaunchVelocity, overrideFriendlyFireSetting, overridenFriendlyFireOn);
                }
                else
                {
                    projectile.Activate(true, wieldingEntity, this, weaponHolderMovementSpeed, damage, projectileLaunchVelocity, overrideFriendlyFireSetting, overridenFriendlyFireOn);
                }

                #endregion

                nextShootTime = Time.time + shootInterval;
                bulletsInMagazine--;

                shootParticle.Play();

                return true;
            }
        }

        return false;
    }

    public void OnEquipWeapon(GameEntity wieldingEntity, IMoveable wieldingEntityMoveable)
    {
        this.wieldingEntity = wieldingEntity;
        this.wieldingEntityMoveable = wieldingEntityMoveable;
        /*wieldingEntityMoveable = null;
        if (wieldingEntity.gameObject.GetComponent<VisibilityInfo>())
        {

        }*/
        // here special things could be triggered
        // gunVisualsAndAudioManager.OnGrabWeapon();
    }

    public void OnReleaseWeapon()
    {
        wieldingEntity = null;
        //gunVisualsAndAudioManager.OnReleaseWeapon();
    }


    public int GetBulletsInMagazineLeft()
    {
        return bulletsInMagazine;
    }

    public float GetBulletsInMagazineLeftRatio()
    {
        //Debug.Log("bullets in mag ration: " + 1f * bulletsInMagazine / magazineSize);
        return 1f * bulletsInMagazine/magazineSize;
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
