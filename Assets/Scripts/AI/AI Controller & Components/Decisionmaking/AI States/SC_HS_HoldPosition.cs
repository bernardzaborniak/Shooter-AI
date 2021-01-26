using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BenitosAI
{
    [CreateAssetMenu(menuName = "AI/States/Hold Position", fileName = "Hold Position")]
    public class SC_HS_HoldPosition : AIStateCreator
    {
        public enum Stance
        {
            StandingIdle,
            StandingCombat,
            Crouching
        }
        public Stance stance;

        public override AIState CreateState(AIController aiController, DecisionContext context)
        {
            //St_HS_HoldPosition state = new St_HS_HoldPosition();
            St_HS_HoldPosition state = new St_HS_HoldPosition(aiController, context, stance);
            //state.SetUpState(aiController, context);
            //state.stance = stance;


            return state;
        }
    }

    public class St_HS_HoldPosition : AIState //AIState_HumanoidSoldier
    {
        AIController_HumanoidSoldier aiController;
        EC_HumanoidCharacterController charController;

        SC_HS_HoldPosition.Stance stance;

        //public override void SetUpState(AIController aiController, DecisionContext context)
        public St_HS_HoldPosition(AIController aiController, DecisionContext context, SC_HS_HoldPosition.Stance stance)
        {
            this.aiController = (AIController_HumanoidSoldier)aiController;
            this.charController = this.aiController.characterController;
            this.stance = stance;
        }

        public override void OnStateEnter()
        {
            //charController.MoveTo(targetPosition, true);
            //charController.StopAimingSpine();
            //charController.StopAimingWeapon();
            charController.StopMoving();

            if (stance == SC_HS_HoldPosition.Stance.StandingIdle)
            {
                charController.ChangeCharacterStanceToIdle();
            }
            else if (stance == SC_HS_HoldPosition.Stance.StandingCombat)
            {
                charController.ChangeCharacterStanceToCombatStance();
            }
            else if (stance == SC_HS_HoldPosition.Stance.Crouching)
            {
                charController.ChangeCharacterStanceToCrouchingStance();
            }
        }

        public override void OnStateExit()
        {

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
            //Debug.Log("updating state: ");
            /*if (charController.IsMoving())
            {
                charController.StopMoving();
            }*/
        }
    }
}
