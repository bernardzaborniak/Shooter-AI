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
            St_HS_HoldWeaponIdle state = new St_HS_HoldWeaponIdle(aiController, context, weaponID);
            //state.SetUpState(aiController, context);
            //state.weaponID = weaponID;

            return state;
        }
    }

    public class St_HS_HoldWeaponIdle : AIState
    {
        AIController_HumanoidSoldier aiController;
        EC_HumanoidCharacterController charController;

        int weaponID;

       // public override void SetUpState(AIController aiController, DecisionContext context)
        public St_HS_HoldWeaponIdle(AIController aiController, DecisionContext context, int weaponID)
        {
            this.aiController = (AIController_HumanoidSoldier)aiController;
            this.charController = this.aiController.characterController;
            this.weaponID = weaponID;
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
            charController.ChangeSelectedItem(weaponID);
            //Debug.Log("updating state: ");
            /*if (charController.IsMoving())
            {
                charController.StopMoving();
            }*/
        }
    }

}

