using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace BenitosAI
{
    [CreateAssetMenu(menuName = "AI/States/ThrowGrenade", fileName = "Throw Grenade")]


    public class SC_HS_ThrowGrenade : AIStateCreator
    {
        public override AIState CreateState(AIController aiController, DecisionContext context, AIStateCreatorInputParams inputParams)
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
        AIC_AimingController aimingController;

        Grenade equippedGrenade;

        // Saved for the nan exception in the angle calculation
        Vector3 grenadeAimSpineDirectionLastFrame;


        public St_HS_ThrowGrenade(AIController aiController, DecisionContext context)
        {
            this.aiController = (AIController_HumanoidSoldier)aiController;
            this.charController = this.aiController.characterController;
            target = context.targetEntityInfo;

            aimingController = this.aiController.aimingController;
            equippedGrenade = this.charController.GetItemInInventory(3) as Grenade;
            grenadeAimSpineDirectionLastFrame = target.GetAimPosition() - charController.transform.position;
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
            charController.ChangeSelectedItem(3);
            charController.StartThrowingGrenade();

            float grenadeThrowingVelocity = aimingController.DetermineThrowingObjectVelocity(equippedGrenade, target.lastDistanceMeasured);

            Vector3 aimDirection = aimingController.GetDirectionToAimAtTarget(target.GetEntityPosition(), target.GetCurrentVelocity(), true, grenadeThrowingVelocity, false);
            if (float.IsNaN(aimDirection.x))
            {
                Debug.Log("aiming spine was nan");
                aimDirection = grenadeAimSpineDirectionLastFrame;
            }

            Vector3 aimSpineDirection = aimDirection;
            aimSpineDirection.y = 0; // it looked starnge when there were looking up
            charController.AimSpineInDirection(aimSpineDirection);


            charController.UpdateVelocityWhileThrowingGrenade(grenadeThrowingVelocity, aimDirection);

            grenadeAimSpineDirectionLastFrame = aimDirection;
        }


        public override bool ShouldStateBeAborted()
        {
            if (charController.GetItemInInventory(3) == null)
            {
                return true;
            }

            return false;
        }
    }


}
