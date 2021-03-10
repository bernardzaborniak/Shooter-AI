using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BenitosAI
{

    [CreateAssetMenu(menuName = "AI/Consideration Input/Humanoid/Has Line of Fire without hitting Friendlies", fileName = "Has Line of Fire without hitting Friendlies")]
    public class CI_HS_HasLineOfFireWithoutHittingFriendlies : ConsiderationInput
    {
        //It doesnt check if it has line of fire, as this would be false too often, we also want to allow the unit to shoot if it doent hit
        //what we dont want in any case is for the unit to shoot its friends accidentaly


        //public LayerMask LoFRaycastLayerMask;
        void OnEnable()
        {
            inputParamsType = new ConsiderationInputParams.InputParamsType[]
            {
                ConsiderationInputParams.InputParamsType.LineOfFire
            };
        }

        public override float GetConsiderationInput(DecisionContext decisionContext, ConsiderationInputParams considerationInputParams)
        {
            //get raycast start posiiton & direction -> how? - get the position and direction from the head - position is head and direction is direction from head to target - aim posiiton
            //- check if hitbox entity is target entity
            //this could be problematic with a shotgun with a large firing radius?

            // get head posiiton from sensing, target from context

            //
            AIController_Blackboard blackboard = ((AIController_HumanoidSoldier)decisionContext.aiController).blackboard;
            Transform gunShootPoint = blackboard.GetCurrentUsedGunShootPoint();
            //Vector3 shootDirection = gunShootPoint.forward;

            //sometimes the unit doesnt have a gun in hand, so just return 0
            if (gunShootPoint)
            {
                RaycastHit hit;
                if (Physics.Raycast(gunShootPoint.position, gunShootPoint.forward, out hit, Mathf.Infinity, considerationInputParams.lineOfFireLayerMask))
                {
                    Hitbox hitbox = hit.collider.GetComponent<Hitbox>();
                    if (hitbox)
                    {
                        if (hitbox.GetEntity().teamID == blackboard.GetMyEntity().teamID)
                        {
                            return 0;
                        }
                    }
                }
            }
            else
            {
                return 1;
            }


           // Vector3 headPosition = ((AIController_HumanoidSoldier)decisionContext.aiController).humanSensing.headTransform.position;


           

            //get the correct collider of the unit? - or just check for length? the length from gun to target unit


            return 1;

        }
    }

}

