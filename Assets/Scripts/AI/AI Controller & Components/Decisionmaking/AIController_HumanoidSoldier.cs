using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIController_HumanoidSoldier : AIController
{
    public AIC_HumanSensing humanSensing;
    public AIC_AimingController aimingController;
    public EC_HumanoidCharacterController characterController;


    // Start is called before the first frame update
    public override void SetUpComponent(GameEntity entity)
    {
        
        base.SetUpComponent(entity);

        humanSensing.SetUpComponent(myEntity);
        aimingController.SetUpComponent(myEntity);
    }

    // Update is called once per frame
    public override void UpdateComponent()
    {
        base.UpdateComponent();

        // ---- this will happen later if the new AI Controller fully replaces the old ----
        //humanSensing.UpdateComponent();
        //aimingController.UpdateComponent();
    }
}
