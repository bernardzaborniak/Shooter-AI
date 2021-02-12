using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace BenitosAI
{
    [CreateAssetMenu(menuName = "AI/States/FallBackToPosition", fileName = "FallBackToPosition")]
    public class SC_HS_FallBackToPosition : AIStateCreator
    {
        public float minFallBackDistance;
        public float maxFallBackDistance;
        [Tooltip("the unit advances towards the mean direction where threats are coming from - rotated by a random with this value at maximum on the xz plane")]
        public float maxAngleDeviationFromDirectionToThreats;
        public bool sprint;


        public override AIState CreateState(AIController aiController, DecisionContext context)
        {
            St_HS_FallBackToPosition state = new St_HS_FallBackToPosition(aiController, context, minFallBackDistance, maxFallBackDistance, maxAngleDeviationFromDirectionToThreats, sprint);
            return state;
        }
    }

    public class St_HS_FallBackToPosition : AIState
    {
        AIController_HumanoidSoldier aiController;
        EC_HumanoidCharacterController charController;
        float minFallBackDistance;
        float maxFallBackDistance;
        float maxAngleDeviationFromDirectionToThreats;
        bool sprint;

        public St_HS_FallBackToPosition(AIController aiController, DecisionContext context, float minFallBackDistance, float maxFallBackDistance, float maxAngleDeviationFromDirectionToThreats, bool sprint)
        {
            this.aiController = (AIController_HumanoidSoldier)aiController;
            this.charController = this.aiController.characterController;
            this.minFallBackDistance = minFallBackDistance;
            this.maxFallBackDistance = maxFallBackDistance;
            this.maxAngleDeviationFromDirectionToThreats = maxAngleDeviationFromDirectionToThreats;
            this.sprint = sprint;
        }

        public override void OnStateEnter()
        {
            //find a position to advance to 
            float fallBackDistance = Random.Range(minFallBackDistance, maxFallBackDistance);
            float angleDeviation = Random.Range(-maxAngleDeviationFromDirectionToThreats, maxAngleDeviationFromDirectionToThreats);

            Vector3 fallBackVector = -aiController.blackboard.meanThreatDirection;
            //rotate the vector
            fallBackVector = Quaternion.AngleAxis(angleDeviation, Vector3.up) * fallBackVector;
            //adjust the vector distance
            fallBackVector = fallBackVector * fallBackDistance;

            charController.ChangeCharacterStanceToStandingCombatStance();
            charController.MoveTo(aiController.blackboard.GetMyEntity().transform.position + fallBackVector);

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
