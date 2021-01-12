using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BenitosAI
{

    [CreateAssetMenu(menuName = "AI/States/ShootWeaponAtEnemy", fileName = "ShootWeaponAtEnemy")]
    public class SC_HS_ShootWeaponAtEnemy : AIStateCreator
    {
        public int weaponID;


        public override AIState CreateState(AIController aiController, DecisionContext context)
        {
            St_HS_ShootWeaponAtEnemy state = new St_HS_ShootWeaponAtEnemy();
            state.SetUpState(aiController, context);
            state.target = context.targetEntity;
            state.weaponID = weaponID;

            return state;
        }
    }

    public class St_HS_ShootWeaponAtEnemy : AIState //AIState_HumanoidSoldier
    {
        AIController_HumanoidSoldier aiController;
        EC_HumanoidCharacterController charController;
        public SensedEntityInfo target;
        public int weaponID;

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
            charController.StopAimingSpine();
            charController.StopAimingWeapon();
        }

        public override void UpdateState()
        {
            if (target.IsAlive())
            {
                charController.ChangeSelectedItem(weaponID);

                charController.AimSpineAtPosition(target.GetAimPosition());
                charController.AimWeaponAtPosition(target.GetAimPosition());

                if (charController.GetCurrentWeaponAimingErrorAngle(false) < 15f)
                {
                    charController.ShootWeapon();
                }
            }
        }
    }
}


