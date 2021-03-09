using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BenitosAI
{
    [CreateAssetMenu(menuName = "AI/States/Hold Position", fileName = "Hold Position")]
    public class SC_HS_HoldPosition : AIStateCreator
    {
        /*public enum Stance
        {
            StandingIdle,
            StandingCombat,
            Crouching
        }
        public Stance stance;*/

        void OnEnable()
        {
            inputParamsType = new AIStateCreatorInputParams.InputParamsType[]
            {
                AIStateCreatorInputParams.InputParamsType.CharacterStance
            };
        }

        public override AIState CreateState(AIController aiController, DecisionContext context, AIStateCreatorInputParams inputParams)
        {
            St_HS_HoldPosition state = new St_HS_HoldPosition(aiController, context, inputParams.characterStance);
            return state;
        }
    }

    public class St_HS_HoldPosition : AIState //AIState_HumanoidSoldier
    {
        AIController_HumanoidSoldier aiController;
        EC_HumanoidCharacterController charController;

        EC_HumanoidCharacterController.CharacterStance stance;

        public St_HS_HoldPosition(AIController aiController, DecisionContext context, EC_HumanoidCharacterController.CharacterStance stance)
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

        public override bool ShouldStateBeAborted()
        {
            return false;
        }
    }
}
