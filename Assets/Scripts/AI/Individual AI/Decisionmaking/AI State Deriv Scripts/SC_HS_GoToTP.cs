using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace BenitosAI
{
    [CreateAssetMenu(menuName = "AI/States/Go to TP", fileName = "Go to TP")]
    public class SC_HS_GoToTP : AIStateCreator
    {
       // public bool sprint;
       // public EC_HumanoidCharacterController.CharacterStance moveToTPStance;
       // public float enterTPDistance;
       // public float exitTPDistance;


        void OnEnable()
        {
            inputParamsType =  new AIStateCreatorInputParams.InputParamsType[]
            {
                AIStateCreatorInputParams.InputParamsType.GoToTp,
                AIStateCreatorInputParams.InputParamsType.Sprint,
                AIStateCreatorInputParams.InputParamsType.CharacterStance
            };
        }

        public override AIState CreateState(AIController aiController, DecisionContext context, AIStateCreatorInputParams inputParams)
        {
         
           // inputParams.inputParamsType = AIStateCreatorInputParams.InputParamsType.Position;

            St_HS_GoToTP state = new St_HS_GoToTP(aiController, context, inputParams.characterStance, inputParams.sprint, inputParams.enterTPDistance, inputParams.exitTPDistance);
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

            targetTP = context.targetTacticalPointInfo.tPoint;
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
            aiController.OnStartTargetingTPoint(targetTP);
        }

        public override void OnStateExit()
        {
            aiController.OnLeaveTPoint(targetTP);
            aiController.OnStopTargetingTPoint(targetTP);

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
                if (!targetTP.IsPointUsedByAnotherEntity(myEntity))
                {
                    if (distanceToTPSquared < enterTPDistanceSquared)
                    {
                        aiController.OnEnterTPoint(targetTP);
                        aiController.OnStopTargetingTPoint(targetTP);

                        //charController.StopMoving();
                        //charController.MoveTo(targetTP.GetPointPosition(), false);

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

                    charController.MoveTo(targetTP.GetPointPosition(), false); //sometimes the mvoe order is ignored?

                }
            }
            else if(state == State.InsideTP)
            {
                if(distanceToTPSquared > exitTPDistanceSquared)
                {
                    aiController.OnLeaveTPoint(targetTP);
                    aiController.OnStartTargetingTPoint(targetTP);


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
