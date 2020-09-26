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
    int desiredSelectedItemID;

    public Item[] inventory;

    public enum ItemInteractionState
    {
        Idle,
        PullingOutItemInHand,
        HidingItemInHand,
        ReloadingWeapon,
        ThrowingGrenade,
        //CockBoltAction //doing this lever thingy on snipers
    }

    public ItemInteractionState itemInteractionState;

    float hidingWeaponEndTime;
    float pullingOutWeaponEndTime;

    float currentHideWeaponDuration;
    float currentPullingOutWeaponDuration;

    [Tooltip("the reload time of weapons can be speed up by this value")]
    public float reloadTimeSkillMultiplier = 1;
    float reloadingEndTime;

    //Needs to be able to change later, is calculatet based on current target and throwGrenadeMaxRange
    float currentGrenadeThrowVelocity;
    //public float throwGrenadeMaxRange;
    float throwingGrenadeEndTime;


    public override void SetUpComponent(GameEntity entity)
    {
        base.SetUpComponent(entity);

    }

    public override void UpdateComponent()
    {

        if (itemInteractionState == ItemInteractionState.HidingItemInHand)
        {
            if(Time.time > hidingWeaponEndTime)
            {
                FinishHidingItem();
            }
        }
        else if (itemInteractionState == ItemInteractionState.PullingOutItemInHand)
        {
            if (Time.time > pullingOutWeaponEndTime)
            {
                FinishPullingOutItem();
            }
        }
        else if(itemInteractionState == ItemInteractionState.ReloadingWeapon)
        {
            if(Time.time > reloadingEndTime)
            {
                FinishReloadingWeapon();
            }
        }
        else if(itemInteractionState == ItemInteractionState.ThrowingGrenade)
        {
            if (Time.time > throwingGrenadeEndTime)
            {
                FinishThrowingGrenade();
            }   
        }

        //Debug.Log(gameObject.name + "Update  " + itemInteractionState);

    }

    #region Change Item in Hand

    public void ChangeItemInHand(int newWeaponInventoryID)
    {
        desiredSelectedItemID = newWeaponInventoryID;

        if(itemInteractionState == ItemInteractionState.Idle || itemInteractionState == ItemInteractionState.ReloadingWeapon)
        {
            if (desiredSelectedItemID != currentSelectedItemID)
            {
                if (itemInteractionState == ItemInteractionState.ReloadingWeapon)
                {
                    AbortReloadingWeapon();
                }

                if (inventory[currentSelectedItemID] == null)
                {
                    if (inventory[desiredSelectedItemID] != null)
                    {
                        StartPullingOutItem(desiredSelectedItemID, 1);
                    }
                    else
                    {
                        currentSelectedItemID = desiredSelectedItemID;
                    }
                }
                else
                {
                    StartHidingItem(1);
                }
            }
        }else if(itemInteractionState == ItemInteractionState.PullingOutItemInHand)
        {
            if (desiredSelectedItemID != currentSelectedItemID)
            {
                float percentageAlreadyPulledOut = 1 - (pullingOutWeaponEndTime - Time.time) / inventory[currentSelectedItemID].pullOutItemTime;
                StartHidingItem(percentageAlreadyPulledOut);
            }
        }
        else if(itemInteractionState == ItemInteractionState.HidingItemInHand)
        {
            if(desiredSelectedItemID == currentSelectedItemID)
            {
                float percentageAlreadyHidden = 1 - (hidingWeaponEndTime - Time.time) / inventory[currentSelectedItemID].hideItemTime;
                StartPullingOutItem(desiredSelectedItemID, percentageAlreadyHidden);
            }     
        }

    }

    public void AbortChangingItemInHand()
    {
        ChangeItemInHand(currentSelectedItemID);
    }

    void StartPullingOutItem(int newInventoryID, float percentageAlreadyHidden)
    {

        currentSelectedItemID = newInventoryID;
        inventory[currentSelectedItemID].gameObject.SetActive(true);

        pullingOutWeaponEndTime = Time.time + inventory[currentSelectedItemID].pullOutItemTime * percentageAlreadyHidden;

        itemInteractionState = ItemInteractionState.PullingOutItemInHand;

        animationController.ChangeItemInHand(inventory[currentSelectedItemID].GetItemInteractionTypeID());
        animationController.AdjustPullOutAnimationSpeedAndOffset(inventory[currentSelectedItemID].pullOutItemTime, 1-percentageAlreadyHidden);
        animationController.ChangeWeaponInteractionState(1);

        if(inventory[currentSelectedItemID] is Gun)
        {
            aimingController.OnChangeWeapon(inventory[currentSelectedItemID] as Gun);
        }
        else
        {
            aimingController.OnChangeWeapon(null);
        }
        
    }

    void FinishPullingOutItem()
    {
        itemInteractionState = ItemInteractionState.Idle;

        animationController.ChangeWeaponInteractionState(0);

        handsIKController.OnChangeItemInHand(inventory[currentSelectedItemID]);
    }

    void StartHidingItem(float pulledOutPercentage)
    {
        // pulledOutPercentage is a value which tells us how much the pull out was already executed, so we can shorten the hide time by this amount

        hidingWeaponEndTime = Time.time + inventory[currentSelectedItemID].hideItemTime * pulledOutPercentage;

        itemInteractionState = ItemInteractionState.HidingItemInHand;

        animationController.AdjustHideAnimationSpeedAndOffset(inventory[currentSelectedItemID].hideItemTime, 1 - pulledOutPercentage);
        animationController.ChangeWeaponInteractionState(2);

        handsIKController.DisableIKs();
    }

    void FinishHidingItem()
    {
        inventory[currentSelectedItemID].gameObject.SetActive(false);

        if (inventory[desiredSelectedItemID] != null)
        {
            StartPullingOutItem(desiredSelectedItemID,1);
        }
        else
        {
            currentSelectedItemID = desiredSelectedItemID;

            itemInteractionState = ItemInteractionState.Idle;
            animationController.ChangeWeaponInteractionState(0);
            animationController.ChangeItemInHand(0);

            aimingController.OnChangeWeapon(null);
            handsIKController.OnChangeItemInHand(null);
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
        if(itemInteractionState == ItemInteractionState.Idle)
        {
            if (inventory[currentSelectedItemID] is Gun)
            {
                (inventory[currentSelectedItemID] as Gun).Shoot();
            }
        }
    }

    public void StartReloadingWeapon()
    {
        if(itemInteractionState == ItemInteractionState.Idle)
        {
            if (inventory[currentSelectedItemID] is Gun)
            {
                itemInteractionState = ItemInteractionState.ReloadingWeapon;

                //calculate reload animation duration & speed
                float reloadDuration = (inventory[currentSelectedItemID] as Gun).defaultReloadDuration * reloadTimeSkillMultiplier;

                reloadingEndTime = Time.time + reloadDuration;

                animationController.StartReloadingWeapon(reloadDuration);

                handsIKController.DisableIKs();
            }
        }
    }

    public int GetAmmoRemainingInMagazine()
    {
        if(inventory[currentSelectedItemID] is Gun)
        {
            return (inventory[currentSelectedItemID] as Gun).GetBulletsInMagazineLeft();
        }

        return 0;

    }

    public void AbortReloadingWeapon()
    {
        if (itemInteractionState == ItemInteractionState.ReloadingWeapon)
        {
            if (inventory[currentSelectedItemID] is Gun)
            {
                itemInteractionState = ItemInteractionState.Idle;
                animationController.AbortReloadingWeapon();
                //todo adjust hands IK here

                handsIKController.ReenableIKs();
            }
        }
    }

    public void FinishReloadingWeapon()
    {
        if (itemInteractionState == ItemInteractionState.ReloadingWeapon)
        {
            if (inventory[currentSelectedItemID] is Gun)
            {
                itemInteractionState = ItemInteractionState.Idle;
                (inventory[currentSelectedItemID] as Gun).RefillBulletsInMagazine();
                animationController.AbortReloadingWeapon();
                //todo adjust hands IK here

                handsIKController.ReenableIKs();
            }
        }
    }

    #endregion

    #region Other Item Commands

    public void ThrowGrenade(float currentGrenadeThrowVelocity)
    {
        if(itemInteractionState == ItemInteractionState.Idle)
        {
            if (inventory[currentSelectedItemID] is Grenade)
            {
                this.currentGrenadeThrowVelocity = currentGrenadeThrowVelocity;

                itemInteractionState = ItemInteractionState.ThrowingGrenade;
                throwingGrenadeEndTime = Time.time + (inventory[currentSelectedItemID] as Grenade).throwingTime;

                animationController.StartThrowingGrenade((inventory[currentSelectedItemID] as Grenade).throwingTime);
            }
        }
    }

    void FinishThrowingGrenade()
    {
        itemInteractionState = ItemInteractionState.Idle;
        Debug.Log("throw direction: " + aimingController.GetCurrentSpineAimDirection());
        (inventory[currentSelectedItemID] as Grenade).Throw(aimingController.GetCurrentSpineAimDirection(), currentGrenadeThrowVelocity);
        inventory[currentSelectedItemID] = null; //Remove grenade from inventory
        animationController.AbortThrowingGrenade();
    }

    public void AbortThrowingGrenade()
    {
        if(itemInteractionState == ItemInteractionState.ThrowingGrenade)
        {
            //can only be caused by stagger or flinch or something?
            itemInteractionState = ItemInteractionState.Idle;
            (inventory[currentSelectedItemID] as Grenade).Throw(aimingController.GetCurrentSpineAimDirection(), Random.Range(-3, 3));  //is this the proper way
            inventory[currentSelectedItemID] = null; //Remove grenade from inventory
            animationController.AbortThrowingGrenade();
        }  
    }

    public Item GetCurrentSelectedItem()
    {
        return inventory[currentSelectedItemID];
    }

    public Item GetItemInInventory(int inventoryPosition)
    {
        return inventory[inventoryPosition];
    }

    #endregion

    public bool DoesCurrentItemInteractionStanceAllowAimingWeapon()
    {
        return itemInteractionState == ItemInteractionState.Idle;
    }
}
