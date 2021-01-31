using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BenitosAI
{
    public class AIController_HumanoidSoldier : AIController
    {
        [Header("Charackter to Control")]
        public EC_HumanoidCharacterController characterController;

        [Header("AI Components")]
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
            base.UpdateComponent();

            //Update Components
            humanSensing.UpdateComponent();
            aimingController.UpdateComponent();
        }

        public void OnEnterTPoint(TacticalPoint tPoint)
        {
            humanSensing.SetCurrentlyUsedTacticalPoint(tPoint);
            tPoint.OnEntityEntersPoint(humanSensing.GetMyEntity());
        }

        public void OnLeaveTPoint(TacticalPoint tPoint)
        {
            humanSensing.SetCurrentlyUsedTacticalPoint(null);
            tPoint.OnEntityExitsPoint(humanSensing.GetMyEntity());


        }
    }

}
