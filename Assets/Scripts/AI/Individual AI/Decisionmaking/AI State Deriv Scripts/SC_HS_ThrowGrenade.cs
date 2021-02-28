using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace BenitosAI
{
    [CreateAssetMenu(menuName = "AI/States/ThrowGrenade", fileName = "Throw Grenade")]


    public class SC_HS_ThrowGrenade : AIStateCreator
    {
        public override AIState CreateState(AIController aiController, DecisionContext context)
        {
            St_HS_ThrowGrenade state = new St_HS_ThrowGrenade(aiController, context);

            return state;
        }
    }

    public class St_HS_ThrowGrenade : AIState
    {
        AIController_HumanoidSoldier aiController;
        EC_HumanoidCharacterController charController;
        SensedEntityInfo target;


        public St_HS_ThrowGrenade(AIController aiController, DecisionContext context)
        {
            this.aiController = (AIController_HumanoidSoldier)aiController;
            this.charController = this.aiController.characterController;
            target = context.targetEntity;
        }

        public override void OnStateEnter()
        {
            //charController.MoveTo(targetPosition, true);
            //
            //charController.StopAimingWeapon();

            charController.ChangeSelectedItem(3);

        }

        public override void OnStateExit()
        {
            //leave tPoint
            //targetPoint.tacticalPoint.OnEntityExitsPoint(aiController.humanSensing.GetMyEntity());
            //instead use some method, which also saves current point in sensing
            charController.AbortThrowingGrenade();
            charController.StopAimingSpine();

        }

        public override EntityActionTag[] GetActionTagsToAddOnStateEnter()
        {
            return null;
        }

        public override EntityActionTag[] GetActionTagsToRemoveOnStateExit()
        {
            return null;
        }

        public override void UpdateState()
        {
            //if(targetPoint.lastDistanceMeasured < 0.5f)
            //{
            //   aiController.OnEnterTPoint(targetPoint.tacticalPoint);
            // targetPoint.tacticalPoint.OnEntityEntersPoint(aiController.humanSensing.GetMyEntity());
            //}
            charController.AimSpineAtPosition(target.GetAimPosition());
            charController.ChangeSelectedItem(3);
            charController.StartThrowingGrenade();
        }

        public override bool ShouldStateBeAborted()
        {
            return false;
        }
    }


}
