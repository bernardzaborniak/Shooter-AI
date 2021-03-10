using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace BenitosAI
{
    [CreateAssetMenu(menuName = "AI/States/Move To Transform", fileName = "Move To Transform")]
    public class SC_HS_MoveToTransform : AIStateCreator
    {
        void OnEnable()
        {
            inputParamsType = new AIStateCreatorInputParams.InputParamsType[]
            {
                AIStateCreatorInputParams.InputParamsType.Transform1,
                AIStateCreatorInputParams.InputParamsType.Sprint
            };
        }

        public override AIState CreateState(AIController aiController, DecisionContext context, AIStateCreatorInputParams inputParams)
        {
            St_HS_MoveToTransform state = new St_HS_MoveToTransform(aiController, context, inputParams.transform1, inputParams.sprint);
            return state;
        }
    }

    public class St_HS_MoveToTransform: AIState
    {
        AIController_HumanoidSoldier aiController;
        EC_HumanoidCharacterController charController;
        Transform targetTransform;
        bool sprint;

        public St_HS_MoveToTransform(AIController aiController, DecisionContext context, Transform targetTransform, bool sprint)
        {
            this.aiController = (AIController_HumanoidSoldier)aiController;
            this.charController = this.aiController.characterController;
            this.targetTransform = targetTransform;
            this.sprint = sprint;
        }

        public override void OnStateEnter()
        {
            charController.MoveTo(targetTransform.position, sprint);
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
            charController.MoveTo(targetTransform.position, sprint);
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
