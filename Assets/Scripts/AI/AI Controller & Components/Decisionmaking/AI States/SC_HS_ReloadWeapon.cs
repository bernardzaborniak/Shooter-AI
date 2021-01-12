using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace BenitosAI
{
    [CreateAssetMenu(menuName = "AI/States/ReloadWeapon", fileName = "ReloadWeapon")]
    public class SC_HS_ReloadWeapon : AIStateCreator
    {
        public int weaponID;

        public override AIState CreateState(AIController aiController, DecisionContext context)
        {
            St_HS_ReloadWeapon state = new St_HS_ReloadWeapon();
            state.SetUpState(aiController, context);

            return state;
        }
    }

    public class St_HS_ReloadWeapon : AIState //AIState_HumanoidSoldier
    {
        AIController_HumanoidSoldier aiController;
        EC_HumanoidCharacterController charController;
        public Vector3 targetPosition;

        public override void SetUpState(AIController aiController, DecisionContext context)
        {
            this.aiController = (AIController_HumanoidSoldier)aiController;
            charController = this.aiController.characterController;
        }

        public override void OnStateEnter()
        {
           
        }

        public override void OnStateExit()
        {
            charController.AbortReloadingWeapon();
        }

        public override void UpdateState()
        {
            charController.StartReloadingWeapon();
        }
    }
}


