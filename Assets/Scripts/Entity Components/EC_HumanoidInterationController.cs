using UnityEngine;


public enum WeaponInteractionType
{
    BareHands,
    Pistol,
    Rifle
}

public class EC_HumanoidInterationController : EntityComponent
{
    // Is responsible for interactions like picking up/ changing weapons/reloading etc, communicates with animator  

    public EC_HumanoidAimingController aimingController;
    public EC_HumanoidAnimationController animationController;


    public int currentSelectedWeaponID;
    int desiredSelectedWeaponID;

    public Weapon[] inventory;

    enum WeaponInteractionState
    {
        Idle,
        PullingOutWeapon,
        HidingWeapon,
        ReloadingWeapon,
        CockBoltAction //doing this lever thingy on snipers
    }

    WeaponInteractionState weaponInteractionState;

    float hidingWeaponEndTime;
    float pullingOutWeaponEndTime;

    float currentHideWeaponDuration;
    float currentPullingOutWeaponDuration;

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
                FinishHidingWeapon();
            }
        }
        else if (weaponInteractionState == WeaponInteractionState.PullingOutWeapon)
        {
            if (Time.time > pullingOutWeaponEndTime)
            {
                FinishPullingOutWeapon();
            }
        }
    }

    public void ChangeWeapon(int newWeaponInventoryID)
    {
        desiredSelectedWeaponID = newWeaponInventoryID;

        if(weaponInteractionState == WeaponInteractionState.Idle)
        {
            if (desiredSelectedWeaponID != currentSelectedWeaponID)
            {
                if (inventory[currentSelectedWeaponID] == null)
                {
                    if (inventory[desiredSelectedWeaponID] != null)
                    {
                        StartPullingOutWeapon(desiredSelectedWeaponID, 1);
                    }
                    else
                    {
                        currentSelectedWeaponID = desiredSelectedWeaponID;
                    }
                }
                else
                {
                    StartHidingWeapon(1);
                }
            }
        }else if(weaponInteractionState == WeaponInteractionState.PullingOutWeapon)
        {
            if (desiredSelectedWeaponID != currentSelectedWeaponID)
            {
                float percentageAlreadyPulledOut = 1 - (pullingOutWeaponEndTime - Time.time) / inventory[currentSelectedWeaponID].pullOutWeaponTime;
                StartHidingWeapon(percentageAlreadyPulledOut);
            }
        }
        else if(weaponInteractionState == WeaponInteractionState.HidingWeapon)
        {
            if(desiredSelectedWeaponID == currentSelectedWeaponID)
            {
                float percentageAlreadyHidden = 1 - (hidingWeaponEndTime - Time.time) / inventory[currentSelectedWeaponID].hideWeaponTime;
                StartPullingOutWeapon(desiredSelectedWeaponID, percentageAlreadyHidden);
            }     
        }   
    }

    void StartPullingOutWeapon(int newWeaponInventoryID, float percentageAlreadyHidden)
    {

        currentSelectedWeaponID = newWeaponInventoryID;
        inventory[currentSelectedWeaponID].gameObject.SetActive(true);

        pullingOutWeaponEndTime = Time.time + inventory[currentSelectedWeaponID].pullOutWeaponTime * percentageAlreadyHidden;

        weaponInteractionState = WeaponInteractionState.PullingOutWeapon;

        animationController.ChangeItemInHand(inventory[currentSelectedWeaponID].GetWeaponInteractionTypeID());
        animationController.AdjustPullOutAnimationSpeedAndOffset(inventory[currentSelectedWeaponID].pullOutWeaponTime, 1-percentageAlreadyHidden);
        animationController.ChangeWeaponInteractionState(1);
        
        aimingController.OnChangeWeapon(inventory[currentSelectedWeaponID]);
    }

    void FinishPullingOutWeapon()
    {
        weaponInteractionState = WeaponInteractionState.Idle;

        animationController.ChangeWeaponInteractionState(0);
    }

    void StartHidingWeapon(float pulledOutPercentage)
    {
        // pulledOutPercentage is a value which tells us how much the pull out was already executed, so we can shorten the hide time by this amount

        hidingWeaponEndTime = Time.time + inventory[currentSelectedWeaponID].hideWeaponTime * pulledOutPercentage;

        weaponInteractionState = WeaponInteractionState.HidingWeapon;

        animationController.AdjustHideAnimationSpeedAndOffset(inventory[currentSelectedWeaponID].hideWeaponTime, 1 - pulledOutPercentage);
        animationController.ChangeWeaponInteractionState(2);
    }

    void FinishHidingWeapon()
    {
        inventory[currentSelectedWeaponID].gameObject.SetActive(false);

        if (inventory[desiredSelectedWeaponID] != null)
        {
            StartPullingOutWeapon(desiredSelectedWeaponID,1);
        }
        else
        {
            currentSelectedWeaponID = desiredSelectedWeaponID;
            
            weaponInteractionState = WeaponInteractionState.Idle;
            animationController.ChangeWeaponInteractionState(0);
            animationController.ChangeItemInHand(0);

            aimingController.OnChangeWeapon(null);
        }
    }

    public bool DoesCurrentItemInHandAllowCombatStance()
    {
        if(inventory[currentSelectedWeaponID] == null)
        {
            return false;
        }
        else
        {
            return true;
        }
    }
}
