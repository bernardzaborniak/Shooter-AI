using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
namespace BenitosAI
{

    [CreateAssetMenu(menuName = "AI/States/PeekFromCoverPoint", fileName = "PeekFromCoverPoint")]
    public class SC_HS_PeekFromCoverPoint : AIStateCreator
    {
        [Tooltip("if false, cover is taken while standing")]
        public bool takeCoverCrouched;

        public override AIState CreateState(AIController aiController, DecisionContext context)
        {
            St_HS_PeekFromCoverPoint state = new St_HS_PeekFromCoverPoint(aiController, context, takeCoverCrouched);

            return state;
        }
    }

    public class St_HS_PeekFromCoverPoint : AIState
    {
        AIController_HumanoidSoldier aiController;
        EC_HumanoidCharacterController charController;
        TacticalPoint targetPoint;

        bool takeCoverCrouched;

        public St_HS_PeekFromCoverPoint(AIController aiController, DecisionContext context, bool takeCoverCrouched)
        {
            this.aiController = (AIController_HumanoidSoldier)aiController;
            this.charController = this.aiController.characterController;
            this.takeCoverCrouched = takeCoverCrouched;
            targetPoint = context.targetTacticalPointInfo.tPoint;
        }

        public override void OnStateEnter()
        {
            //charController.MoveTo(targetPosition, true);
            //charController.StopAimingSpine();
            //charController.StopAimingWeapon();

            charController.MoveTo(targetPoint.GetPointPosition());
            aiController.OnEnterTPoint(targetPoint);

            if (takeCoverCrouched)
            {
                charController.ChangeCharacterStanceToCrouchingStance();
            }
            else
            {
                charController.ChangeCharacterStanceToStandingCombatStance();
            }
        }

        public override void OnStateExit()
        {
            //leave tPoint
            //targetPoint.tacticalPoint.OnEntityExitsPoint(aiController.humanSensing.GetMyEntity());
            //instead use some method, which also saves current point in sensing
            aiController.OnLeaveTPoint(targetPoint);

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
            //if (targetPoint.lastDistanceMeasured < 0.3f)
            //{
            //    aiController.OnEnterTPoint(targetPoint.tacticalPoint);
                // targetPoint.tacticalPoint.OnEntityEntersPoint(aiController.humanSensing.GetMyEntity());
            //}
        }

        public override bool ShouldStateBeAborted()
        {
            return false;
        }
    }

}*/
