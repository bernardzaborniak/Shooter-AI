using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BenitosAI
{
    [CreateAssetMenu(menuName = "AI/States/HoldWeaponIdle", fileName = "HoldWeaponIdle")]
    public class SC_HS_HoldWeaponIdle : AIStateCreator
    {
        public int weaponID;

        public override AIState CreateState(AIController aiController, DecisionContext context)
        {
            St_HS_HoldWeaponIdle state = new St_HS_HoldWeaponIdle();
            state.SetUpState(aiController, context);
            state.weaponID = weaponID;

            return state;
        }
    }

    public class St_HS_HoldWeaponIdle : AIState
    {
        AIController_HumanoidSoldier aiController;
        EC_HumanoidCharacterController charController;

        public int weaponID;

        public override void SetUpState(AIController aiController, DecisionContext context)
        {
            this.aiController = (AIController_HumanoidSoldier)aiController;
            charController = this.aiController.characterController;
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
            charController.ChangeSelectedItem(weaponID);
            //Debug.Log("updating state: ");
            /*if (charController.IsMoving())
            {
                charController.StopMoving();
            }*/
        }
    }

}

