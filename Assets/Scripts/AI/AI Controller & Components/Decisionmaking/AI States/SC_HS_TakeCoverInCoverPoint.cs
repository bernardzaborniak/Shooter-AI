using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BenitosAI
{
    [CreateAssetMenu(menuName = "AI/States/TakeCoverInCoverPoint", fileName = "TakeCoverInCoverPoint")]


    public class SC_HS_TakeCoverInCoverPoint : AIStateCreator
    {
        [Tooltip("if false, cover is taken while standing")]
        public bool takeCoverCrouched;

        public override AIState CreateState(AIController aiController, DecisionContext context)
        {
            St_HS_TakeCoverInCoverPoint state = new St_HS_TakeCoverInCoverPoint(aiController, context, takeCoverCrouched);

            return state;
        }
    }

    public class St_HS_TakeCoverInCoverPoint : AIState 
    {
        AIController_HumanoidSoldier aiController;
        EC_HumanoidCharacterController charController;
        SensedTacticalPointInfo targetPoint;

        bool takeCoverCrouched;

        public St_HS_TakeCoverInCoverPoint(AIController aiController, DecisionContext context, bool takeCoverCrouched)
        {
            this.aiController = (AIController_HumanoidSoldier)aiController;
            this.charController = this.aiController.characterController;
            this.takeCoverCrouched = takeCoverCrouched;
            targetPoint = context.targetTacticalPoint;
        }

        public override void OnStateEnter()
        {
            //charController.MoveTo(targetPosition, true);
            //charController.StopAimingSpine();
            //charController.StopAimingWeapon();

            charController.MoveTo(targetPoint.tacticalPoint.GetPointPosition());
            aiController.OnEnterTPoint(targetPoint.tacticalPoint);

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
            aiController.OnLeaveTPoint(targetPoint.tacticalPoint);

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
        }

        public override bool ShouldStateBeAborted()
        {
            return false;
        }
    }


}
