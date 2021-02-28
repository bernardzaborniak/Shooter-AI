using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BenitosAI
{
    [CreateAssetMenu(menuName = "AI/States/HoldWeaponScanForThreat", fileName = "HoldWeaponScanForThreat")]
    public class SC_HS_HoldWeaponScanForThreat : AIStateCreator
    {
        public int weaponID;

        [Tooltip("Aiming Spine Deviation Angle")]
        public float maxDeviationAngleFromMovementOrThreatDirection;
        public float minChangeAimDirInterval;
        public float maxChangeAimDirInterval;


        public override AIState CreateState(AIController aiController, DecisionContext context)
        {
            St_HS_HoldWeaponScanForThreat state = new St_HS_HoldWeaponScanForThreat(aiController, context, weaponID, maxDeviationAngleFromMovementOrThreatDirection, minChangeAimDirInterval, maxChangeAimDirInterval);
            return state;
        }
    }

    public class St_HS_HoldWeaponScanForThreat : AIState
    {
        AIController_HumanoidSoldier aiController;
        EC_HumanoidCharacterController charController;
        AIController_Blackboard blackboard;

        int weaponID;

        float maxDeviationAngleFromMovementOrThreatDirection;
        float minChangeAimDieInterval;
        float maxChangeAimDirInterval;
        float nextChangeAimDirTime;

        Vector3 currentAimDir;

        public St_HS_HoldWeaponScanForThreat(AIController aiController, DecisionContext context, int weaponID, float maxDeviationAngleFromMovementOrThreatDirection, float minChangeAimDieInterval, float maxChangeAimDirInterval)
        {
            this.aiController = (AIController_HumanoidSoldier)aiController;
            this.charController = this.aiController.characterController;
            blackboard = this.aiController.blackboard;
            this.weaponID = weaponID;

            this.maxDeviationAngleFromMovementOrThreatDirection = maxDeviationAngleFromMovementOrThreatDirection;
            this.minChangeAimDieInterval = minChangeAimDieInterval;
            this.maxChangeAimDirInterval = maxChangeAimDirInterval;
            nextChangeAimDirTime = Time.time + Random.Range(minChangeAimDieInterval, maxChangeAimDirInterval);

            
        }

        public override void OnStateEnter()
        {
            //charController.MoveTo(targetPosition, true);
            //charController.StopAimingSpine();
            //charController.StopAimingWeapon();

            currentAimDir = CalculateNewAimDir();

        }

        public override void OnStateExit()
        {
            charController.StopAimingSpine();
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
            charController.ChangeSelectedItem(weaponID);
            //Debug.Log("updating state: ");
            /*if (charController.IsMoving())
            {
                charController.StopMoving();
            }*/


            if(Time.time > nextChangeAimDirTime)
            {
                nextChangeAimDirTime = Time.time + Random.Range(minChangeAimDieInterval, maxChangeAimDirInterval);
                currentAimDir = CalculateNewAimDir();
            }

            charController.AimSpineInDirection(currentAimDir);

           
        }

        public override bool ShouldStateBeAborted()
        {
            return false;
        }

        Vector3 CalculateNewAimDir()
        {
            Quaternion randomRot = Quaternion.Euler(Random.Range(-maxDeviationAngleFromMovementOrThreatDirection/2, maxDeviationAngleFromMovementOrThreatDirection/2), Random.Range(-maxDeviationAngleFromMovementOrThreatDirection, maxDeviationAngleFromMovementOrThreatDirection), Random.Range(-maxDeviationAngleFromMovementOrThreatDirection, maxDeviationAngleFromMovementOrThreatDirection));

            if (blackboard.meanThreatDirection != Vector3.zero)
            {
                return  randomRot * blackboard.meanThreatDirection;
            }
            else
            {
                return  charController.movementController.GetCurrentVelocity();
            }


        }
    }

}

