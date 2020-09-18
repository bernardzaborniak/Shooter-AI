using UnityEngine;


public enum ItemInteractionType
{
    BareHands,
    Rifle,
    Pistol,
    Grenade
}

public class EC_HumanoidInterationController : EntityComponent
{
    // Is responsible for interactions like picking up/ changing weapons/reloading etc, communicates with animator  

    public EC_HumanoidAimingController aimingController;
    public EC_HumanoidAnimationController animationController;
    public EC_HumanoidHandsIKController handsIKController;


    public int currentSelectedItemID;
    int desiredSelectedWeaponID;

    public Item[] inventory;

    enum WeaponInteractionState
    {
        Idle,
        PullingOutWeapon,
        HidingWeapon,
        ReloadingWeapon,
        ThrowingGrenade,
        //CockBoltAction //doing this lever thingy on snipers
    }

    WeaponInteractionState weaponInteractionState;

    float hidingWeaponEndTime;
    float pullingOutWeaponEndTime;

    float currentHideWeaponDuration;
    float currentPullingOutWeaponDuration;

    [Tooltip("the reload time of weapons can be speed up by this value")]
    public float reloadTimeSkillMultiplier = 1;
    float reloadingEndTime;


    public override void SetUpComponent(GameEntity entity)
    {
        base.SetUpComponent(entity);

    }

    public override void UpdateComponent()
    {

        if (weaponInteractionState == WeaponInteractionState.HidingWeapon)
        {
            if(Time.time > hidingWeaponEndTime)
            {
                FinishHidingItem();
            }
        }
        else if (weaponInteractionState == WeaponInteractionState.PullingOutWeapon)
        {
            if (Time.time > pullingOutWeaponEndTime)
            {
                FinishPullingOutItem();
            }
        }else if(weaponInteractionState == WeaponInteractionState.ReloadingWeapon)
        {
            Debug.Log("reloading");
            if(Time.time > reloadingEndTime)
            {
                FinishReloadingWeapon();
            }
        }
    }

    #region Change Item in Hand

    public void ChangeItemInHand(int newWeaponInventoryID)
    {
        desiredSelectedWeaponID = newWeaponInventoryID;

        if(weaponInteractionState == WeaponInteractionState.Idle || weaponInteractionState == WeaponInteractionState.ReloadingWeapon)
        {
            if(weaponInteractionState == WeaponInteractionState.ReloadingWeapon)
            {
                AbortReloadingWeapon();
            }

            if (desiredSelectedWeaponID != currentSelectedItemID)
            {
                if (inventory[currentSelectedItemID] == null)
                {
                    if (inventory[desiredSelectedWeaponID] != null)
                    {
                        StartPullingOutItem(desiredSelectedWeaponID, 1);
                    }
                    else
                    {
                        currentSelectedItemID = desiredSelectedWeaponID;
                    }
                }
                else
                {
                    StartHidingItem(1);
                }
            }
        }else if(weaponInteractionState == WeaponInteractionState.PullingOutWeapon)
        {
            if (desiredSelectedWeaponID != currentSelectedItemID)
            {
                float percentageAlreadyPulledOut = 1 - (pullingOutWeaponEndTime - Time.time) / inventory[currentSelectedItemID].pullOutItemTime;
                StartHidingItem(percentageAlreadyPulledOut);
            }
        }
        else if(weaponInteractionState == WeaponInteractionState.HidingWeapon)
        {
            if(desiredSelectedWeaponID == currentSelectedItemID)
            {
                float percentageAlreadyHidden = 1 - (hidingWeaponEndTime - Time.time) / inventory[currentSelectedItemID].hideItemTime;
                StartPullingOutItem(desiredSelectedWeaponID, percentageAlreadyHidden);
            }     
        }

    }

    void StartPullingOutItem(int newInventoryID, float percentageAlreadyHidden)
    {

        currentSelectedItemID = newInventoryID;
        inventory[currentSelectedItemID].gameObject.SetActive(true);

        pullingOutWeaponEndTime = Time.time + inventory[currentSelectedItemID].pullOutItemTime * percentageAlreadyHidden;

        weaponInteractionState = WeaponInteractionState.PullingOutWeapon;

        animationController.ChangeItemInHand(inventory[currentSelectedItemID].GetItemInteractionTypeID());
        animationController.AdjustPullOutAnimationSpeedAndOffset(inventory[currentSelectedItemID].pullOutItemTime, 1-percentageAlreadyHidden);
        animationController.ChangeWeaponInteractionState(1);

        if(inventory[currentSelectedItemID] is Gun)
        {
            aimingController.OnChangeWeapon(inventory[currentSelectedItemID] as Gun);
            handsIKController.OnChangeWeapon(inventory[currentSelectedItemID] as Gun);
        }
        else
        {
            aimingController.OnChangeWeapon(null);
            handsIKController.OnChangeWeapon(null);
        }
        
    }

    void FinishPullingOutItem()
    {
        weaponInteractionState = WeaponInteractionState.Idle;

        animationController.ChangeWeaponInteractionState(0);
    }

    void StartHidingItem(float pulledOutPercentage)
    {
        // pulledOutPercentage is a value which tells us how much the pull out was already executed, so we can shorten the hide time by this amount

        hidingWeaponEndTime = Time.time + inventory[currentSelectedItemID].hideItemTime * pulledOutPercentage;

        weaponInteractionState = WeaponInteractionState.HidingWeapon;

        animationController.AdjustHideAnimationSpeedAndOffset(inventory[currentSelectedItemID].hideItemTime, 1 - pulledOutPercentage);
        animationController.ChangeWeaponInteractionState(2);
    }

    void FinishHidingItem()
    {
        inventory[currentSelectedItemID].gameObject.SetActive(false);

        if (inventory[desiredSelectedWeaponID] != null)
        {
            StartPullingOutItem(desiredSelectedWeaponID,1);
        }
        else
        {
            currentSelectedItemID = desiredSelectedWeaponID;
            
            weaponInteractionState = WeaponInteractionState.Idle;
            animationController.ChangeWeaponInteractionState(0);
            animationController.ChangeItemInHand(0);

            aimingController.OnChangeWeapon(null);
            handsIKController.OnChangeWeapon(null);
        }
    }

    public bool DoesCurrentItemInHandAllowCombatStance()
    {
        if(inventory[currentSelectedItemID] == null)
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    #endregion

    #region Gun Commands

    public void ShootWeapon()
    {
        if(weaponInteractionState == WeaponInteractionState.Idle)
        {
            if (inventory[currentSelectedItemID] is Gun)
            {
                (inventory[currentSelectedItemID] as Gun).Shoot();
            }
        }
    }

    public void StartReloadingWeapon()
    {
        if(weaponInteractionState == WeaponInteractionState.Idle)
        {
            if (inventory[currentSelectedItemID] is Gun)
            {
                weaponInteractionState = WeaponInteractionState.ReloadingWeapon;

                //calculate reload animation duration & speed
                float reloadDuration = (inventory[currentSelectedItemID] as Gun).defaultReloadDuration * reloadTimeSkillMultiplier;

                reloadingEndTime = Time.time + reloadDuration;

                animationController.StartReloadingWeapon(reloadDuration);

                //todo djust hands IK here
                handsIKController.DisableIKs();
            }
        }


    }

    public void AbortReloadingWeapon()
    {
        if (weaponInteractionState == WeaponInteractionState.ReloadingWeapon)
        {
            if (inventory[currentSelectedItemID] is Gun)
            {
                weaponInteractionState = WeaponInteractionState.Idle;
                animationController.AbortReloadingWeapon();
                //todo adjust hands IK here

                handsIKController.ReenableIKs();
            }
        }
    }

    public void FinishReloadingWeapon()
    {
        if (weaponInteractionState == WeaponInteractionState.ReloadingWeapon)
        {
            if (inventory[currentSelectedItemID] is Gun)
            {
                weaponInteractionState = WeaponInteractionState.Idle;
                (inventory[currentSelectedItemID] as Gun).RefillBulletsInMagazine();
                animationController.AbortReloadingWeapon();
                //todo adjust hands IK here

                handsIKController.ReenableIKs();
            }
        }
    }

    #endregion
}
