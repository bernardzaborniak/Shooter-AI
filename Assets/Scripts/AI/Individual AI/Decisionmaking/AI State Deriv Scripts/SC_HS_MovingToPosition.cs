﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BenitosAI
{
    [CreateAssetMenu(menuName = "AI/States/Moving To Position", fileName = "MovingToPosition")]
    public class SC_HS_MovingToPosition : AIStateCreator
    {
        public Vector3 targetPosition;

        public override AIState CreateState(AIController aiController, DecisionContext context)
        {
            St_HS_MovingToPosition state = new St_HS_MovingToPosition(aiController, context, targetPosition);
            return state;
        }
    }

    public class St_HS_MovingToPosition : AIState
    {
        AIController_HumanoidSoldier aiController;
        EC_HumanoidCharacterController charController;
        Vector3 targetPosition;

        public St_HS_MovingToPosition(AIController aiController, DecisionContext context, Vector3 targetPosition)
        {
            this.aiController = (AIController_HumanoidSoldier)aiController;
            this.charController = this.aiController.characterController;
            this.targetPosition = targetPosition;
        }

        public override void OnStateEnter() 
        {
            charController.MoveTo(targetPosition, true);
            charController.ChangeCharacterStanceToStandingCombatStance();
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
            //Debug.Log("updating state: ");
           /* if (!charController.IsMoving())
            {
                charController.MoveTo(targetPosition, true);
            }*/
        }

        public override bool ShouldStateBeAborted()
        {
            return false;
        }
    }
}
