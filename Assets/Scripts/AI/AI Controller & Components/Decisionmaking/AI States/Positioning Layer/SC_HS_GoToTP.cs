using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace BenitosAI
{
    [CreateAssetMenu(menuName = "AI/States/GoToTP", fileName = "GoToTP")]
    public class SC_HS_GoToTP : AIStateCreator
    {
        public bool sprint;
        public EC_HumanoidCharacterController.CharacterStance moveToTPStance;
        public float enterTPDistance;
        public float exitTPDistance;

        public override AIState CreateState(AIController aiController, DecisionContext context)
        {
            St_HS_GoToTP state = new St_HS_GoToTP(aiController, context, moveToTPStance, sprint, enterTPDistance, exitTPDistance);
            return state;
        }
    }

    public class St_HS_GoToTP : AIState
    {
        AIController_HumanoidSoldier aiController;
        EC_HumanoidCharacterController charController;

        bool sprint;
        EC_HumanoidCharacterController.CharacterStance moveToTPStance;

        TacticalPoint targetTP;
        GameEntity myEntity;

        enum State
        {
            MovingToTP,
            InsideTP
        }
        State state;
        float enterTPDistanceSquared;
        float exitTPDistanceSquared;


        public St_HS_GoToTP(AIController aiController, DecisionContext context, EC_HumanoidCharacterController.CharacterStance stance, bool sprint, float enterTPDistance, float exitTPDistance)
        {
            this.aiController = (AIController_HumanoidSoldier)aiController;
            this.charController = this.aiController.characterController;
            this.moveToTPStance = stance;
            this.sprint = sprint;

            targetTP = context.targetTacticalPoint.tacticalPoint;
            myEntity = this.aiController.blackboard.GetMyEntity();

            enterTPDistanceSquared = enterTPDistance * enterTPDistance;
            exitTPDistanceSquared = exitTPDistance * exitTPDistance;
        }

        public override void OnStateEnter()
        {
            if (moveToTPStance == EC_HumanoidCharacterController.CharacterStance.StandingIdle)
            {
                charController.ChangeCharacterStanceToStandingIdle();
            }
            else if (moveToTPStance == EC_HumanoidCharacterController.CharacterStance.StandingCombatStance)
            {
                charController.ChangeCharacterStanceToStandingCombatStance();
            }
            else if (moveToTPStance == EC_HumanoidCharacterController.CharacterStance.Crouching)
            {
                charController.ChangeCharacterStanceToCrouchingStance();
            }

            charController.MoveTo(targetTP.GetPointPosition(), sprint);

            state = State.MovingToTP;
        }

        public override void OnStateExit()
        {
            aiController.OnLeaveTPoint(targetTP);
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

            float distanceToTPSquared = (targetTP.GetPointPosition()- myEntity.transform.position).sqrMagnitude;

            if(state == State.MovingToTP)
            {
                if (!targetTP.IsPointFull())
                {
                    if (distanceToTPSquared < enterTPDistanceSquared)
                    {
                        aiController.OnEnterTPoint(targetTP);

                        //charController.StopMoving();
                        charController.MoveTo(targetTP.GetPointPosition(), false);

                        if (targetTP.type == TacticalPoint.Type.Crouched)
                        {
                            charController.ChangeCharacterStanceToCrouchingStance();
                        }
                        else if (targetTP.type == TacticalPoint.Type.Standing)
                        {
                            charController.ChangeCharacterStanceToStandingCombatStance();
                        }
                        state = State.InsideTP;
                    }
                }          
            }
            else if(state == State.InsideTP)
            {
                if(distanceToTPSquared > exitTPDistanceSquared)
                {
                    aiController.OnLeaveTPoint(targetTP);

                    if (moveToTPStance == EC_HumanoidCharacterController.CharacterStance.StandingIdle)
                    {
                        charController.ChangeCharacterStanceToStandingIdle();
                    }
                    else if (moveToTPStance == EC_HumanoidCharacterController.CharacterStance.StandingCombatStance)
                    {
                        charController.ChangeCharacterStanceToStandingCombatStance();
                    }
                    else if (moveToTPStance == EC_HumanoidCharacterController.CharacterStance.Crouching)
                    {
                        charController.ChangeCharacterStanceToCrouchingStance();
                    }

                    charController.MoveTo(targetTP.GetPointPosition(), sprint);

                    state = State.MovingToTP;
                }
            }
        }

        public override bool ShouldStateBeAborted()
        {
            return false;
        }
    }
}
