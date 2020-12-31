using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BenitosAI
{

    [CreateAssetMenu(menuName = "AI/States/MoveToWeaponRange", fileName = "MoveToWeaponRange")]
    public class SC_HS_MoveToWeaponRange : AIStateCreator
    {
        public float desiredRange;


        public override AIState CreateState(AIController aiController, DecisionContext context)
        {
            St_HS_MoveToWeaponRange state = new St_HS_MoveToWeaponRange();
            state.SetUpState(aiController, context);
            state.desiredRange = desiredRange;


            return state;
        }
    }

    public class St_HS_MoveToWeaponRange : AIState //AIState_HumanoidSoldier
    {
        AIController_HumanoidSoldier aiController;
        EC_HumanoidCharacterController charController;

        SensedEntityInfo targetEntityInfo;
        public float desiredRange;

        public override void SetUpState(AIController aiController, DecisionContext context)
        {
            this.aiController = (AIController_HumanoidSoldier)aiController;
            charController = this.aiController.characterController;

            targetEntityInfo = context.targetEntity;
        }

        public override void OnStateEnter()
        {
            //charController.MoveTo(targetPosition, true);
            //charController.StopAimingSpine();
            //charController.StopAimingWeapon();
        }

        public override void OnStateExit()
        {
        }

        public override void UpdateState()
        {
            charController.MoveTo(targetEntityInfo.GetEntityPosition() + (charController.transform.position - targetEntityInfo.GetEntityPosition()) * desiredRange);
            //Debug.Log("updating state: ");
            /*if (charController.IsMoving())
            {
                charController.StopMoving();
            }*/
        }
    }
}

