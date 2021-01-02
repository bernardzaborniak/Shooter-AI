using UnityEngine;


public enum ItemInteractionType
{
    BareHands,
    Rifle,
    Pistol,
    Grenade
}

public class HCC_HumanoidInterationController : HumanoidCharacterComponent
{
    // Is responsible for interactions like picking up/ changing weapons/reloading etc, communicates with animator  

    [Header("References")]
    public HCC_HumanoidAimingController aimingController;
    public HCC_HumanoidAnimationController animationController;
    public HCC_HumanoidHandsIKController handsIKController;
    public HCC_HumanoidMovementController movementController; //used to ally movement force to things like bullets n grenades
    
    public RecoilManager recoilManager;


    public enum ItemInteractionState
    {
        Idle,
        PullingOutItemInHand,
        HidingItemInHand,
        ReloadingWeapon,
        ThrowingGrenade,
        //CockBoltAction //doing this lever thingy on snipers
    }
    [Space(10)]
    public ItemInteractionState itemInteractionState;

    [Header("Inventory Management")]
    public int currentSelectedItemID;
    int desiredSelectedItemID;

    public Item[] inventory;

    [Space(10)]
    public Transform rightHandItemParent;
    public Transform inventoryItemParent;

    float currentHideWeaponDuration;
    float currentPullingOutWeaponDuration;

    float hidingWeaponEndTime;
    float pullingOutWeaponEndTime;

    //items are not ativated with the beginning of pull out or end of hide animation, pulling out shows weapon after a delay after pulling out started, hiding animation hides weapon, some time before the aniamtion is finished
    public float percentageOfFinishedAnimationWhenItemIsHidden = 0.8f;  
    public float percentageOfFinishedAnimationWhenItemIsActivated = 0.2f;

    [Header("Reloading")]
    [Tooltip("the reload time of weapons can be speed up by this value")]
    public float reloadTimeSkillMultiplier = 1;
    float reloadingEndTime;

    [Header("Grenade Throwing")]
    //Needs to be able to change later, is calculatet based on current target and throwGrenadeMaxRange
    float currentGrenadeThrowVelocity;
    Vector3 currentGrenadeThrowDirection;
    //public float throwGrenadeMaxRange;
    float throwingGrenadeEndTime;
    [Tooltip("the grenade is thrown in the spine direction")]
    public Transform spineAimTransform;


    public override void SetUpComponent(GameEntity entity)
    {
        base.SetUpComponent(entity);

    }

    public override void UpdateComponent()
    {

        if (itemInteractionState == ItemInteractionState.HidingItemInHand)
        {
            if (Time.time > hidingWeaponEndTime - inventory[currentSelectedItemID].hideItemTime * (1 - percentageOfFinishedAnimationWhenItemIsHidden))
            {
                DeactivateItemBeingHidden();
            }
            if (Time.time > hidingWeaponEndTime)
            {
                FinishHidingItem();
            }
            
        }
        else if (itemInteractionState == ItemInteractionState.PullingOutItemInHand)
        {
            if (Time.time > pullingOutWeaponEndTime - inventory[currentSelectedItemID].pullOutItemTime * (1 - percentageOfFinishedAnimationWhenItemIsActivated))
            {
                ActivateItemBeingPulledOut();
            }
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

    public bool ChangeItemInHand(int newWeaponInventoryID) //returns true if item starts to change
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

                return true;
            }
        }else if(itemInteractionState == ItemInteractionState.PullingOutItemInHand)
        {
            if (desiredSelectedItemID != currentSelectedItemID)
            {
                float percentageAlreadyPulledOut = 1 - (pullingOutWeaponEndTime - Time.time) / inventory[currentSelectedItemID].pullOutItemTime;
                StartHidingItem(percentageAlreadyPulledOut);

                return true;
            }
        }
        else if(itemInteractionState == ItemInteractionState.HidingItemInHand)
        {
            if(desiredSelectedItemID == currentSelectedItemID)
            {
                float percentageAlreadyHidden = 1 - (hidingWeaponEndTime - Time.time) / inventory[currentSelectedItemID].hideItemTime;
                StartPullingOutItem(desiredSelectedItemID, percentageAlreadyHidden);

                return true;
            }     
        }

        return false;

    }

    public void AbortChangingItemInHand()
    {
        ChangeItemInHand(currentSelectedItemID);
    }

    void StartPullingOutItem(int newInventoryID, float percentageAlreadyHidden)
    {

        currentSelectedItemID = newInventoryID;

        //EquipItem();

        pullingOutWeaponEndTime = Time.time + inventory[currentSelectedItemID].pullOutItemTime * percentageAlreadyHidden;

        itemInteractionState = ItemInteractionState.PullingOutItemInHand;

        animationController.ChangeItemInHand(inventory[currentSelectedItemID].GetItemInteractionTypeID());
        animationController.AdjustPullOutAnimationSpeedAndOffset(inventory[currentSelectedItemID].pullOutItemTime, 1-percentageAlreadyHidden);
        animationController.ChangeWeaponInteractionState(1);

        


        //added
        handsIKController.OnChangeItemInHand(inventory[currentSelectedItemID]);
        handsIKController.OnStartPullingOutWeapon(inventory[currentSelectedItemID].pullOutItemTime * percentageAlreadyHidden);

    }

    void FinishPullingOutItem()
    {
        itemInteractionState = ItemInteractionState.Idle;

        animationController.ChangeWeaponInteractionState(0);

        handsIKController.OnStopPullingOutWeapon();
       // handsIKController.OnChangeItemInHand(inventory[currentSelectedItemID]);
    }

    void StartHidingItem(float pulledOutPercentage)
    {
        // pulledOutPercentage is a value which tells us how much the pull out was already executed, so we can shorten the hide time by this amount

        hidingWeaponEndTime = Time.time + inventory[currentSelectedItemID].hideItemTime * pulledOutPercentage;

        itemInteractionState = ItemInteractionState.HidingItemInHand;

        animationController.AdjustHideAnimationSpeedAndOffset(inventory[currentSelectedItemID].hideItemTime, 1 - pulledOutPercentage);
        animationController.ChangeWeaponInteractionState(2);

        //handsIKController.DisableIKs();
        handsIKController.OnStartHidingWeapon(inventory[currentSelectedItemID].hideItemTime * pulledOutPercentage);
    }

    void FinishHidingItem()
    {
        //HolsterItem();

        handsIKController.OnStopHidingWeapon();

        // What next? - depending on which item is desired
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

    void ActivateItemBeingPulledOut()
    {
        inventory[currentSelectedItemID].transform.SetParent(rightHandItemParent);
        inventory[currentSelectedItemID].transform.localPosition = Vector3.zero;
        inventory[currentSelectedItemID].transform.localRotation = Quaternion.identity;
        inventory[currentSelectedItemID].gameObject.SetActive(true);

        if (inventory[currentSelectedItemID] is Gun)
        {
            Gun gun = (inventory[currentSelectedItemID] as Gun);
            gun.OnEquipWeapon(myEntity, movementController);
            aimingController.OnChangeWeapon(gun);
        }
        else
        {
            aimingController.OnChangeWeapon(null);
        }
    }

    void DeactivateItemBeingHidden()
    {
        // Reset Position & Visibility
        inventory[currentSelectedItemID].gameObject.SetActive(false);
        inventory[currentSelectedItemID].transform.SetParent(inventoryItemParent);
        inventory[currentSelectedItemID].transform.localPosition = Vector3.zero;
        inventory[currentSelectedItemID].transform.localRotation = Quaternion.identity;

        //If Gun - adjust Gun
        if (inventory[currentSelectedItemID] is Gun)
        {
            (inventory[currentSelectedItemID] as Gun).OnReleaseWeapon();
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
                Gun currentGun = (inventory[currentSelectedItemID] as Gun);
                RecoilInfo currentRecoilInfo = currentGun.GetRecoilInfo();
                if (currentGun.Shoot())
                {
                    recoilManager.AddRecoil(ref currentRecoilInfo);
                }
                //TODO if Shoot returns true - > apply recoil to recoil manager 
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

                //handsIKController.DisableIKs();
                handsIKController.OnStartReloadingWeapon();
            }
        }
    }

    public int GetAmmoRemainingInMagazine()
    {
        if(inventory[currentSelectedItemID] is Gun)
        {
            
            return (inventory[currentSelectedItemID] as Gun).GetBulletsInMagazineLeft();
        }
        Debug.Log("!inventory[currentSelectedItemID] is Gun + interaction");
        return 0;

    }

    public float GetAmmoRemainingInMagazineRatio()
    {
        if (inventory[currentSelectedItemID] is Gun)
        {
            return (inventory[currentSelectedItemID] as Gun).GetBulletsInMagazineLeftRatio();
        }
        Debug.Log("!inventory[currentSelectedItemID] is Gun + interaction");
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

                //handsIKController.EnableIKs();
                handsIKController.OnStopReloadingWeapon();
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

                //handsIKController.EnableIKs();
                handsIKController.OnStopReloadingWeapon();
            }
        }
    }

    #endregion

    #region Other Item Commands

    public void StartThrowingGrenade()//float currentGrenadeThrowVelocity, Vector3 currentGrenadeThrowDirection)
    {
        if(itemInteractionState == ItemInteractionState.Idle)
        {
            if (inventory[currentSelectedItemID] is Grenade)
            {
                //this.currentGrenadeThrowVelocity = currentGrenadeThrowVelocity;
                //this.currentGrenadeThrowDirection = currentGrenadeThrowDirection;

                itemInteractionState = ItemInteractionState.ThrowingGrenade;
                throwingGrenadeEndTime = Time.time + (inventory[currentSelectedItemID] as Grenade).throwingTime;

                animationController.StartThrowingGrenade((inventory[currentSelectedItemID] as Grenade).throwingTime);
            }
        }
    }

    public void UpdateVelocityWhileThrowingGrenade(float currentGrenadeThrowVelocity, Vector3 currentGrenadeThrowDirection) 
    {
        //althrough the throw direction is the spine, the grenade is thrown slightly correcter by having this eact direction
        if (itemInteractionState == ItemInteractionState.ThrowingGrenade)
        {
            this.currentGrenadeThrowVelocity = currentGrenadeThrowVelocity;
            this.currentGrenadeThrowDirection = currentGrenadeThrowDirection;
            //throw direction is just spine direction
        }
    }



    void FinishThrowingGrenade()
    {
        itemInteractionState = ItemInteractionState.Idle;
        (inventory[currentSelectedItemID] as Grenade).Throw(currentGrenadeThrowDirection, currentGrenadeThrowVelocity);
        inventory[currentSelectedItemID] = null; //Remove grenade from inventory
        animationController.AbortThrowingGrenade();

    }

    public void AbortThrowingGrenadeAfterTriggeringFuze()
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

    public bool IsThrowingGrenade()
    {
        return itemInteractionState == ItemInteractionState.ThrowingGrenade;
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
