using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EC_HumanoidInterationController : EntityComponent
{
    // Is responsible for interactions like picking up/ changing weapons/reloading etc, communicates with animator  

    public EC_HumanoidAimingController aimingController;
    public EC_HumanoidAnimationController animationController;

    public Weapon rifle; //itemInHandID 1
    public Weapon pistol; //itemInHandID 2

    public Weapon currentWeapon;

    public override void SetUpComponent(GameEntity entity)
    {
        base.SetUpComponent(entity);

    }

    public override void UpdateComponent()
    {

    }

    public void SelectPistol()
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
    }
}
