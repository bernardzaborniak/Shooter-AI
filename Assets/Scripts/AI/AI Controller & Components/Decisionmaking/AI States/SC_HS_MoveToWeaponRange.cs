using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BenitosAI
{

    [CreateAssetMenu(menuName = "AI/States/MoveToWeaponRange", fileName = "MoveToWeaponRange")]
    public class SC_HS_MoveToWeaponRange : AIStateCreator
    {
        public float desiredRange;
        public bool sprint;
        public enum Stance
        {
            StandingIdle,
            StandingCombat,
            Crouching
        }
        public Stance stance;


        public override AIState CreateState(AIController aiController, DecisionContext context)
        {
            St_HS_MoveToWeaponRange state = new St_HS_MoveToWeaponRange(aiController, context, desiredRange, sprint, stance);
            //state.SetUpState(aiController, context);
            //state.desiredRange = desiredRange;
            //state.sprint = sprint;
            //state.stance = stance;


            return state;
        }
    }

    public class St_HS_MoveToWeaponRange : AIState //AIState_HumanoidSoldier
    {
        AIController_HumanoidSoldier aiController;
        EC_HumanoidCharacterController charController;

        SensedEntityInfo targetEntityInfo;
        float desiredRange;
        bool sprint;
        SC_HS_MoveToWeaponRange.Stance stance;

        float nextIssueMoveOrderTime;
        float issueMoveOrderMinInterval = 0.1f;
        float issueMoveOrderMaxInterval = 0.9f;

        //public override void SetUpState(AIController aiController, DecisionContext context)
        public St_HS_MoveToWeaponRange(AIController aiController, DecisionContext context, float desiredRange, bool sprint, SC_HS_MoveToWeaponRange.Stance stance)
        {
            this.aiController = (AIController_HumanoidSoldier)aiController;
            this.charController = this.aiController.characterController;
            this.desiredRange = desiredRange;
            this.sprint = sprint;
            this.stance = stance;

            targetEntityInfo = context.targetEntity;

            nextIssueMoveOrderTime = 0;

        }

        public override void OnStateEnter()
        {
            //charController.MoveTo(targetPosition, true);
            //charController.StopAimingSpine();
            //charController.StopAimingWeapon();

            if (stance == SC_HS_MoveToWeaponRange.Stance.StandingIdle)
            {
                charController.ChangeCharacterStanceToStandingIdle();
            }
            else if (stance == SC_HS_MoveToWeaponRange.Stance.StandingCombat)
            {
                charController.ChangeCharacterStanceToStandingCombatStance();
            }
            else if(stance == SC_HS_MoveToWeaponRange.Stance.Crouching)
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
            if(Time.time > nextIssueMoveOrderTime)
            {
                nextIssueMoveOrderTime = Time.time + Random.Range(issueMoveOrderMinInterval, issueMoveOrderMaxInterval);

                

                charController.MoveTo(targetEntityInfo.GetEntityPosition() + (charController.transform.position - targetEntityInfo.GetEntityPosition()) * desiredRange, sprint);
            }
            //Debug.Log("updating state: ");
            /*if (charController.IsMoving())
            {
                charController.StopMoving();
            }*/
        }
    }
}

