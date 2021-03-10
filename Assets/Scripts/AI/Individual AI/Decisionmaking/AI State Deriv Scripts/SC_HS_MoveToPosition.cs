using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BenitosAI
{
    [CreateAssetMenu(menuName = "AI/States/Move To Position", fileName = "MoveToPosition")]
    public class SC_HS_MoveToPosition : AIStateCreator
    {
        void OnEnable()
        {
            inputParamsType = new AIStateCreatorInputParams.InputParamsType[]
            {
                AIStateCreatorInputParams.InputParamsType.Position1,
                AIStateCreatorInputParams.InputParamsType.Sprint

            };
        }

        public override AIState CreateState(AIController aiController, DecisionContext context, AIStateCreatorInputParams inputParams)
        {
            St_HS_MoveToPosition state = new St_HS_MoveToPosition(aiController, context, inputParams.position1, inputParams.sprint);
            return state;
        }
    }

    public class St_HS_MoveToPosition : AIState
    {
        AIController_HumanoidSoldier aiController;
        EC_HumanoidCharacterController charController;
        Vector3 targetPosition;
        bool sprint;

        public St_HS_MoveToPosition(AIController aiController, DecisionContext context, Vector3 targetPosition, bool sprint)
        {
            this.aiController = (AIController_HumanoidSoldier)aiController;
            this.charController = this.aiController.characterController;
            this.targetPosition = targetPosition;
            this.sprint = sprint;
        }

        public override void OnStateEnter() 
        {
            charController.MoveTo(targetPosition, sprint);
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
                charController.MoveTo(targetPosition, sprint);
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

