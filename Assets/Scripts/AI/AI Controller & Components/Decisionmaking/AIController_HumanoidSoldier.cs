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
        public AIController_Blackboard blackboard;
        public AIC_HumanSensing humanSensing;
        public AIC_AimingController aimingController;
        //change the components to just an array?




        // Start is called before the first frame update
        public override void SetUpComponent(GameEntity entity)
        {

            base.SetUpComponent(entity);

            blackboard.SetUpComponent(myEntity);
            humanSensing.SetUpComponent(myEntity);
            aimingController.SetUpComponent(myEntity);
        }

        // Update is called once per frame
        public override void UpdateComponent()
        {
            base.UpdateComponent();

            //Update Components
            blackboard.UpdateComponent();
            humanSensing.UpdateComponent();
            aimingController.UpdateComponent();
        }

        public void OnEnterTPoint(TacticalPoint tPoint)
        {
            blackboard.SetCurrentlyUsedTacticalPoint(tPoint);
            tPoint.OnEntityEntersPoint(blackboard.GetMyEntity());
        }

        public void OnLeaveTPoint(TacticalPoint tPoint)
        {
            blackboard.SetCurrentlyUsedTacticalPoint(null);
            tPoint.OnEntityExitsPoint(blackboard.GetMyEntity());


        }
    }

}
