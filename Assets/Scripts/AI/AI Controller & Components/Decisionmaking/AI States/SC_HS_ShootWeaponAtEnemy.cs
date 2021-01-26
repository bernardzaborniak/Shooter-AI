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
            St_HS_ShootWeaponAtEnemy state = new St_HS_ShootWeaponAtEnemy(aiController, context, context.targetEntity, weaponID);
            //state.SetUpState(aiController, context, context.targetEntity, weaponID);
            //state.target = context.targetEntity;
            //state.weaponID = weaponID;

            return state;
        }
    }

    public class St_HS_ShootWeaponAtEnemy : AIState //AIState_HumanoidSoldier
    {
        AIController_HumanoidSoldier aiController;
        EC_HumanoidCharacterController charController;
        SensedEntityInfo target;
        int weaponID;

        EntityActionTag[] actionTags;

        //remove set up state completely - have a constructor instead?
        //public override void SetUpState(AIController aiController, DecisionContext context)
        public St_HS_ShootWeaponAtEnemy(AIController aiController, DecisionContext context, SensedEntityInfo target, int weaponID)
        {
            this.aiController = (AIController_HumanoidSoldier)aiController;
            this.charController = this.aiController.characterController;
            this.target = target;
            this.weaponID = weaponID;

            actionTags = new EntityActionTag[1];
            actionTags[0] = new EntityActionTag(EntityActionTagType.ShootingAtTarget);
            actionTags[0].shootAtTarget = target.entity;
        }

        public override void OnStateEnter()
        {

        }

        public override void OnStateExit()
        {
            charController.StopAimingSpine();
            charController.StopAimingWeapon();
        }

        public override EntityActionTag[] GetActionTagsToAddOnStateEnter()
        {
            Debug.Log("shoot weapon state GetTagsToAdd");

            return actionTags;
        }

        public override EntityActionTag[] GetActionTagsToRemoveOnStateExit()
        {
            Debug.Log("shoot weapon state GetTagsToRemove");

            return actionTags;
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


