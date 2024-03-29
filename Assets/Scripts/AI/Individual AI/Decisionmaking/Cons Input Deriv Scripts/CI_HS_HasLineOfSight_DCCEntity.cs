using UnityEngine;

namespace BenitosAI
{

    [CreateAssetMenu(menuName = "AI/Consideration Input/Humanoid/Has Line of Sight [DCC_Entity]", fileName = "Has Line of Sight [DCC_Entity]")]
    public class CI_HS_HasLineOfSight_DCCEntity : ConsiderationInput
    {
        //[Tooltip("If the information about the enemy entity is older than x seconds, ignore it")]
        //public float informationFreshnessThreshold = 1f;
        void OnEnable()
        {
            inputParamsType = new ConsiderationInputParams.InputParamsType[]
            {
                ConsiderationInputParams.InputParamsType.LineOfSight
            };
        }

        public override float GetConsiderationInput(DecisionContext decisionContext, ConsiderationInputParams considerationInputParams)
        {
            //get raycast start posiiton & direction -> how? - get the position and direction from the head - position is head and direction is direction from head to target - aim posiiton
            //- check if hitbox entity is target entity

            // get head posiiton from sensing, target from context
            Vector3 headPosition = ((AIController_HumanoidSoldier)decisionContext.aiController).humanSensing.headTransform.position;
            GameEntity targetEntity = ((SensedEntityInfo)decisionContext.target).entity;
           

            RaycastHit hit;
            if(Physics.Raycast(headPosition, ((SensedEntityInfo)decisionContext.target).GetAimPosition()- headPosition, out hit, Mathf.Infinity, considerationInputParams.lineOfSightLayerMask))
            {
                Hitbox hitbox = hit.collider.GetComponent<Hitbox>();
                if (hitbox)
                {
                    if(hitbox.GetGameEntity() == targetEntity)
                    {
                        return 1;
                    }
                }
            }

            //get the correct collider of the unit? - or just check for length? the length from gun to target unit


            return 0;

        }
    }

}
