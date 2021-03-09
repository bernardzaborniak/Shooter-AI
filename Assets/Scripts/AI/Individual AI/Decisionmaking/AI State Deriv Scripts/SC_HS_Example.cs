using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace BenitosAI
{
    [CreateAssetMenu(menuName = "AI/States/Example", fileName = "Example")]
    public class SC_HS_Example : AIStateCreator
    {
        void OnEnable()
        {
            /*inputParamsType = new AIStateCreatorInputParams.InputParamsType[]
             {
                AIStateCreatorInputParams.InputParamsType.GoToTp,
             };*/
        }
        public override AIState CreateState(AIController aiController, DecisionContext context, AIStateCreatorInputParams inputParams)
        {
            St_HS_Example state = new St_HS_Example(aiController, context);
            return state;
        }
    }

    public class St_HS_Example : AIState
    {
        AIController_HumanoidSoldier aiController;
        EC_HumanoidCharacterController charController;

        public St_HS_Example(AIController aiController, DecisionContext context)
        {
            this.aiController = (AIController_HumanoidSoldier)aiController;
            this.charController = this.aiController.characterController;
        }

        public override void OnStateEnter()
        {

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

        }

        public override bool ShouldStateBeAborted()
        {
            return false;
        }
    }

}

