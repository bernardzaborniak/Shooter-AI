using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace BenitosAI
{
    [CreateAssetMenu(menuName = "AI/States/AdvanceToPosition", fileName = "AdvanceToPosition")]
    public class SC_HS_AdvanceToPosition : AIStateCreator
    {
        //public Vector3 targetPosition;
        public float minAdvanceDistance;
        public float maxAdvanceDistance;
        [Tooltip("the unit advances towards the mean direction where threats are coming from - rotated by a random with this value at maximum on the xz plane")]
        public float maxAngleDeviationFromDirectionToThreats;

        void OnEnable()
        {
            inputParamsType = AIStateCreatorInputParams.InputParamsType.Position;
        }

        public override AIState CreateState(AIController aiController, DecisionContext context)
        {
            St_HS_AdvanceToPosition state = new St_HS_AdvanceToPosition(aiController, context, minAdvanceDistance, maxAdvanceDistance, maxAngleDeviationFromDirectionToThreats);//, targetPosition);
            return state;
        }
    }

    public class St_HS_AdvanceToPosition : AIState
    {
        AIController_HumanoidSoldier aiController;
        EC_HumanoidCharacterController charController;
        float minAdvanceDistance;
        float maxAdvanceDistance;
        float maxAngleDeviationFromDirectionToThreats;
       // Vector3 targetPosition;

        public St_HS_AdvanceToPosition(AIController aiController, DecisionContext context, float minAdvanceDistance, float maxAdvanceDistance, float maxAngleDeviationFromDirectionToThreats)//, Vector3 targetPosition)
        {
            this.aiController = (AIController_HumanoidSoldier)aiController;
            this.charController = this.aiController.characterController;
            this.minAdvanceDistance = minAdvanceDistance;
            this.maxAdvanceDistance = maxAdvanceDistance;
            this.maxAngleDeviationFromDirectionToThreats = maxAngleDeviationFromDirectionToThreats;
            //this.targetPosition = targetPosition;
        }

        public override void OnStateEnter()
        {
            //find a position to advance to 
            float advancingDistance = Random.Range(minAdvanceDistance, maxAdvanceDistance);
            float angleDeviation = Random.Range(-maxAngleDeviationFromDirectionToThreats, maxAngleDeviationFromDirectionToThreats);

            Vector3 advancingVector = aiController.blackboard.meanThreatDirection;
            //rotate the vector
            advancingVector = Quaternion.AngleAxis(angleDeviation, Vector3.up) * advancingVector;
            //adjust the vector distance
            advancingVector = advancingVector * advancingDistance;

            charController.ChangeCharacterStanceToStandingCombatStance();
            charController.MoveTo(aiController.blackboard.GetMyEntity().transform.position + advancingVector);

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

        }

        public override bool ShouldStateBeAborted()
        {
            return false;
        }
    }
}