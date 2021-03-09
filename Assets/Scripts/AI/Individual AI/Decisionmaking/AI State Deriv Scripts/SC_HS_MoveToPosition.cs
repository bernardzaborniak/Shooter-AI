using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BenitosAI
{
    [CreateAssetMenu(menuName = "AI/States/Move To Position", fileName = "MoveToPosition")]
    public class SC_HS_MoveToPosition : AIStateCreator
    {
        //public Vector3 targetPosition;

        void OnEnable()
        {
            inputParamsType = new AIStateCreatorInputParams.InputParamsType[]
            {
                AIStateCreatorInputParams.InputParamsType.Position1
            };
        }

        public override AIState CreateState(AIController aiController, DecisionContext context, AIStateCreatorInputParams inputParams)
        {
            St_HS_MoveToPosition state = new St_HS_MoveToPosition(aiController, context, inputParams.position1);
            return state;
        }
    }

    public class St_HS_MoveToPosition : AIState
    {
        AIController_HumanoidSoldier aiController;
        EC_HumanoidCharacterController charController;
        Vector3 targetPosition;

        public St_HS_MoveToPosition(AIController aiController, DecisionContext context, Vector3 targetPosition)
        {
            this.aiController = (AIController_HumanoidSoldier)aiController;
            this.charController = this.aiController.characterController;
            this.targetPosition = targetPosition;
        }

        public override void OnStateEnter() 
        {
            charController.MoveTo(targetPosition, true);
            charController.ChangeCharacterStanceToStandingIdle();
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
            //somehow sometme the move to order is ignored:
           // if (!charController.IsMoving())
            //{
                charController.MoveTo(targetPosition, true);
           // }


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

