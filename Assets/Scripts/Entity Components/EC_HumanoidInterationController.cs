using UnityEngine;

public class EC_HumanoidInterationController : EntityComponent
{
    // Is responsible for interactions like picking up/ changing weapons/reloading etc, communicates with animator  

    public EC_HumanoidAimingController aimingController;
    public EC_HumanoidAnimationController animationController;

    //public Weapon rifle; //itemInHandID 1
   // public Weapon pistol; //itemInHandID 2

    public int currentSelectedWeaponID;
    int desiredSelectedWeaponID;

    public Weapon[] inventory;

    enum WeaponInteractionState
    {
        Idle,
        PullingOutWeapon,
        HidingWeapon
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
                //weaponChangingState = WeaponChangingState.Idle;
                FinishHidingWeapon();
            }
        }
        else if (weaponInteractionState == WeaponInteractionState.PullingOutWeapon)
        {
            if (Time.time > pullingOutWeaponEndTime)
            {
                //weaponChangingState = WeaponChangingState.Idle;
                FinishPullingOutWeapon();
            }
        }

        //Debug.Log("current state: " + weaponInteractionState);
        //Debug.Log("current weapon: " + inventory[currentSelectedWeaponID]);

        //Debug.Log("currentSelectedWeaponID: " + currentSelectedWeaponID);
       // Debug.Log("desiredSelectedWeaponID: " + desiredSelectedWeaponID);
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
                Debug.Log("percentageAlreadyPulledOut: " + percentageAlreadyPulledOut);
                StartHidingWeapon(percentageAlreadyPulledOut);
            }
        }
        else if(weaponInteractionState == WeaponInteractionState.HidingWeapon)
        {
            if(desiredSelectedWeaponID == currentSelectedWeaponID)
            {
                float percentageAlreadyHidden = 1 - (hidingWeaponEndTime - Time.time) / inventory[currentSelectedWeaponID].hideWeaponTime;
                Debug.Log("percentageAlreadyHidden: " + percentageAlreadyHidden);
                StartPullingOutWeapon(desiredSelectedWeaponID, percentageAlreadyHidden);
            }
            
            
        }

       
    }

    void StartPullingOutWeapon(int newWeaponInventoryID, float percentageAlreadyHidden)
    {
        // if (inventory[currentSelectedWeaponID]) inventory[currentSelectedWeaponID].gameObject.SetActive(false);

        currentSelectedWeaponID = newWeaponInventoryID;
        inventory[currentSelectedWeaponID].gameObject.SetActive(true);

        pullingOutWeaponEndTime = Time.time + inventory[currentSelectedWeaponID].pullOutWeaponTime * percentageAlreadyHidden;

        weaponInteractionState = WeaponInteractionState.PullingOutWeapon;
        animationController.ChangeItemInHand(inventory[currentSelectedWeaponID].animationID);
        animationController.AdjustPullOutAnimationSpeedAndOffset(inventory[currentSelectedWeaponID].pullOutWeaponTime, 1-percentageAlreadyHidden);
        animationController.ChangeWeaponInteractionState(1);
        
        //animationController.StartPullingOutWeapon(inventory[currentSelectedWeaponID].animationID, inventory[currentSelectedWeaponID].pullOutWeaponTime);
        

        aimingController.OnChangeWeapon(inventory[currentSelectedWeaponID]);


       

    }

    void FinishPullingOutWeapon()
    {
        weaponInteractionState = WeaponInteractionState.Idle;
        animationController.ChangeWeaponInteractionState(0);


        //animationController.FinishPullingOutWeapon();
    }

    void StartHidingWeapon(float pulledOutPercentage)
    {
        // pulledOutPercentage is a value which tells us how much the pull out was already executed, so we can shorten the hide time by this amount

        hidingWeaponEndTime = Time.time + inventory[currentSelectedWeaponID].hideWeaponTime * pulledOutPercentage;


        weaponInteractionState = WeaponInteractionState.HidingWeapon;
        animationController.AdjustHideAnimationSpeedAndOffset(inventory[currentSelectedWeaponID].hideWeaponTime, 1 - pulledOutPercentage);
        animationController.ChangeWeaponInteractionState(2);

        // Debug.Log("----------------------------------------StartHidingWeapon ------------------------------");
        //Debug.Log("Time.time: " + Time.time);
        //Debug.Log("inventory[currentSelectedWeaponID].hideWeaponTime: " + inventory[currentSelectedWeaponID].hideWeaponTime);


        //animationController.StartHidingWeapon(inventory[currentSelectedWeaponID].animationID, inventory[currentSelectedWeaponID].hideWeaponTime , 1- pulledOutPercentage);

        //Debug.Log("hidingWeaponEndTime: " + hidingWeaponEndTime);

    }

    void FinishHidingWeapon()
    {
        //animationController.FinishHidingWeapon();

        //Debug.Log("----------------------------------------finish hiding ------------------------------");
        inventory[currentSelectedWeaponID].gameObject.SetActive(false);
        //inventory[currentSelectedWeaponID] = null;

        //animationController.ChangeItemInHand(0);

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
        }
    }

    

   /* public void SelectPistol()
    {
        currentWeapon = pistol;
        rifle.gameObject.SetActive(false);
        pistol.gameObject.SetActive(true);

        animationController.ChangeItemInHand(currentWeapon.animationID);

        aimingController.OnChangeWeapon(pistol);
    }

    public void SelectRifle()
    {
        currentWeapon = rifle;
        rifle.gameObject.SetActive(true);
        pistol.gameObject.SetActive(false);

        animationController.ChangeItemInHand(currentWeapon.animationID);

        aimingController.OnChangeWeapon(rifle);
    }

    public void SelectNothing()
    {
        Debug.Log("select nothing");
        currentWeapon = null;
        rifle.gameObject.SetActive(false);
        pistol.gameObject.SetActive(false);

        animationController.ChangeItemInHand(0);
    }*/

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
