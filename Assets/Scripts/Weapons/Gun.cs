using System.Collections;
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

    [Header("Visuals")]
    public ParticleSystem shootParticle;


    private void Start()
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
                Projectile projectile = PoolingManager.Instance.SpawnAmmoFromPool(ammoType, shootPoint.position, shootPoint.rotation * Quaternion.Euler(Random.Range(-bloom, bloom), Random.Range(-bloom, bloom), 0f)).GetComponent<Projectile>();

                IMoveable wieldingPlayerMoveable = wieldingEntity.GetComponent<IMoveable>();
                Vector3 weaponHolderMovementSpeed = Vector3.zero;
                if (wieldingPlayerMoveable != null)
                {
                    weaponHolderMovementSpeed = wieldingPlayerMoveable.GetCurrentVelocity();
                }
                else
                {
                    Debug.Log("Warning: No IMoveable could be found on shooting Player- check this to improve realism");
                }
                
                if (!usedByPlayer)
                {
                    projectile.Activate(false, wieldingEntity, this, weaponHolderMovementSpeed, damage, projectileLaunchVelocity);
                }
                else
                {
                    projectile.Activate(true, wieldingEntity, this, weaponHolderMovementSpeed, damage, projectileLaunchVelocity);
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

    public void OnEquipWeapon(GameEntity wieldingEntity)
    {
        this.wieldingEntity = wieldingEntity;
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
