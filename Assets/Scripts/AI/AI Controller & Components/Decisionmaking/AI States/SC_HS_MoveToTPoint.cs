using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BenitosAI
{
    [CreateAssetMenu(menuName = "AI/States/MoveToTPoint", fileName = "MoveToTPoint")]

    public class SC_HS_MoveToTPoint : AIStateCreator
    {
        public bool sprint;
        /*public enum Stance
        {
            StandingIdle,
            StandingCombat,
            Crouching
        }*/
        public EC_HumanoidCharacterController.CharacterStance stance;

        public override AIState CreateState(AIController aiController, DecisionContext context)
        {
            St_HS_MoveToTPoint state = new St_HS_MoveToTPoint(aiController, context, sprint, stance);
            return state;
        }
    }

    public class St_HS_MoveToTPoint : AIState //AIState_HumanoidSoldier
    {
        AIController_HumanoidSoldier aiController;
        EC_HumanoidCharacterController charController;

        SensedTacticalPointInfo targetTPInfo;
        bool sprint;
        EC_HumanoidCharacterController.CharacterStance stance;

        float nextIssueMoveOrderTime;


        //public override void SetUpState(AIController aiController, DecisionContext context)
        public St_HS_MoveToTPoint(AIController aiController, DecisionContext context, bool sprint, EC_HumanoidCharacterController.CharacterStance stance)
        {
            this.aiController = (AIController_HumanoidSoldier)aiController;
            this.charController = this.aiController.characterController;
            this.sprint = sprint;
            this.stance = stance;

            targetTPInfo = context.targetTacticalPoint;

        }

        public override void OnStateEnter()
        {
            //charController.MoveTo(targetPosition, true);
            //charController.StopAimingSpine();
            //charController.StopAimingWeapon();

            if (stance == EC_HumanoidCharacterController.CharacterStance.StandingIdle)
            {
                charController.ChangeCharacterStanceToStandingIdle();
            }
            else if (stance == EC_HumanoidCharacterController.CharacterStance.StandingCombatStance)
            {
                charController.ChangeCharacterStanceToStandingCombatStance();
            }
            else if (stance == EC_HumanoidCharacterController.CharacterStance.Crouching)
            {
                charController.ChangeCharacterStanceToCrouchingStance();
            }

            charController.MoveTo(targetTPInfo.tacticalPoint.GetPointPosition(), sprint);
        }

        public override void OnStateExit()
        {
            charController.StopMoving();
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

        }

        public override bool ShouldStateBeAborted()
        {
            return false;
        }
    }

}
