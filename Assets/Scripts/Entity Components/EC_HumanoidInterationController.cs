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

    public override void SetUpComponent(GameEntity entity)
    {
        base.SetUpComponent(entity);

    }

    public override void UpdateComponent()
    {

    }

    public void SelectPistol()
    {
        rifle.gameObject.SetActive(false);
        pistol.gameObject.SetActive(true);

        animationController.ChangeItemInHand(2);

        aimingController.ChangeWeapon(pistol);
    }

    public void SelectRifle()
    {
        rifle.gameObject.SetActive(true);
        pistol.gameObject.SetActive(false);

        animationController.ChangeItemInHand(1);

        aimingController.ChangeWeapon(rifle);
    }
}
