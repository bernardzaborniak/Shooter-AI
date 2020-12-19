using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIController_HumanoidSoldier : AIController
{
    public AIC_HumanSensing humanSensing;
    public AIC_AimingController aimingController;

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
        humanSensing.UpdateComponent();
        aimingController.UpdateComponent();
    }
}
