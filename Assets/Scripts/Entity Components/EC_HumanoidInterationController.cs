using System.Collections;
using System.Collections.Generic;
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

    enum WeaponChangingState
    {
        Idle,
        PullingOutWeapon,
        HidingWeapon
    }

    WeaponChangingState weaponChangingState;

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
        /*if(weaponChangingState == WeaponChangingState.Idle)
        {
            if (desiredSelectedWeaponID != currentSelectedWeaponID)
            {
                if (inventory[currentSelectedWeaponID] == null)
                {
                    if (inventory[desiredSelectedWeaponID] != null)
                    {
                        StartPullingOutWeapon(desiredSelectedWeaponID);
                    }
                    else
                    {
                        currentSelectedWeaponID = desiredSelectedWeaponID;
                    }
                }
                else
                {
                    StartHidingWeapon();
                }
            }
           
        }*/

        //else 
        if (weaponChangingState == WeaponChangingState.HidingWeapon)
        {
            if(Time.time > hidingWeaponEndTime)
            {
                //weaponChangingState = WeaponChangingState.Idle;
                FinishHidingWeapon();
            }
        }
        else if (weaponChangingState == WeaponChangingState.PullingOutWeapon)
        {
            if (Time.time > pullingOutWeaponEndTime)
            {
                //weaponChangingState = WeaponChangingState.Idle;
                FinishPullingOutWeapon();
            }
        }

        Debug.Log("current state: " + weaponChangingState);
        Debug.Log("current weapon: " + inventory[currentSelectedWeaponID]);

        Debug.Log("currentSelectedWeaponID: " + currentSelectedWeaponID);
        Debug.Log("desiredSelectedWeaponID: " + desiredSelectedWeaponID);
    }

    public void ChangeWeapon(int newWeaponInventoryID)
    {
        desiredSelectedWeaponID = newWeaponInventoryID;

        if(weaponChangingState == WeaponChangingState.Idle)
        {
            if (desiredSelectedWeaponID != currentSelectedWeaponID)
            {
                if (inventory[currentSelectedWeaponID] == null)
                {
                    if (inventory[desiredSelectedWeaponID] != null)
                    {
                        StartPullingOutWeapon(desiredSelectedWeaponID);
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
        }else if(weaponChangingState == WeaponChangingState.PullingOutWeapon)
        {
            float percentageAlreadyPulledOut = 1 - (pullingOutWeaponEndTime - Time.time) / inventory[currentSelectedWeaponID].pullOutWeaponTime;
            Debug.Log("percentageAlreadyPulledOut: " + percentageAlreadyPulledOut);
            StartHidingWeapon(percentageAlreadyPulledOut);
        }

       
    }

    void StartPullingOutWeapon(int newWeaponInventoryID)
    {
        // if (inventory[currentSelectedWeaponID]) inventory[currentSelectedWeaponID].gameObject.SetActive(false);

        currentSelectedWeaponID = newWeaponInventoryID;
        inventory[currentSelectedWeaponID].gameObject.SetActive(true);
        
        animationController.ChangeItemInHand(inventory[currentSelectedWeaponID].animationID);
        animationController.PullOutWeapon(inventory[currentSelectedWeaponID].animationID);

        aimingController.OnChangeWeapon(inventory[currentSelectedWeaponID]);

        weaponChangingState = WeaponChangingState.PullingOutWeapon;

        pullingOutWeaponEndTime = Time.time + inventory[currentSelectedWeaponID].pullOutWeaponTime;

    }

    void FinishPullingOutWeapon()
    {
        weaponChangingState = WeaponChangingState.Idle;
    }

    void StartHidingWeapon(float pulledOutPercentage)
    {
        // pulledOutPercentage is a value which tells us how much the pull out was already executed, so we can shorten the hide time by this amount

        weaponChangingState = WeaponChangingState.HidingWeapon;
        animationController.HideWeapon(inventory[currentSelectedWeaponID].animationID);

        Debug.Log("----------------------------------------StartHidingWeapon ------------------------------");
        Debug.Log("Time.time: " + Time.time);
        Debug.Log("inventory[currentSelectedWeaponID].hideWeaponTime: " + inventory[currentSelectedWeaponID].hideWeaponTime);

        hidingWeaponEndTime = Time.time + inventory[currentSelectedWeaponID].hideWeaponTime * pulledOutPercentage;

        Debug.Log("hidingWeaponEndTime: " + hidingWeaponEndTime);

    }

    void FinishHidingWeapon()
    {
        Debug.Log("----------------------------------------finish hiding ------------------------------");
        inventory[currentSelectedWeaponID].gameObject.SetActive(false);
        //inventory[currentSelectedWeaponID] = null;

        //animationController.ChangeItemInHand(0);

        if (inventory[desiredSelectedWeaponID] != null)
        {
            StartPullingOutWeapon(desiredSelectedWeaponID);
        }
        else
        {
            currentSelectedWeaponID = desiredSelectedWeaponID;
            animationController.ChangeItemInHand(0);
            weaponChangingState = WeaponChangingState.Idle;
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
