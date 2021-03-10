using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BenitosAI
{

    [CreateAssetMenu(menuName = "AI/States/Shoot Weapon At Enemy [DCC_Entity]", fileName = "Shoot Weapon At Enemy [DCC_Entity]")]
    public class SC_HS_ShootWeaponAtEnemy_DCCEntity : AIStateCreator
    {
        void OnEnable()
        {
            inputParamsType = new AIStateCreatorInputParams.InputParamsType[]
            {
                AIStateCreatorInputParams.InputParamsType.WeaponID,
                AIStateCreatorInputParams.InputParamsType.MaxAimingDeviationAngle,
                AIStateCreatorInputParams.InputParamsType.LineOfFireCheck
            };
        }


        public override AIState CreateState(AIController aiController, DecisionContext context, AIStateCreatorInputParams inputParams)
        {
            St_HS_ShootWeaponAtEnemy_DCCEntity state = new St_HS_ShootWeaponAtEnemy_DCCEntity(aiController, context, inputParams.weaponID, inputParams.maxAimingDeviationAngle, inputParams.checkLineOfFireInterval, inputParams.checkLineOfFireLayerMask);
            return state;
        }
    }

    public class St_HS_ShootWeaponAtEnemy_DCCEntity : AIState //AIState_HumanoidSoldier
    {
        AIController_HumanoidSoldier aiController;
        EC_HumanoidCharacterController charController;
        SensedEntityInfo target;
        int weaponID;
        float maxAllowedWeaponAimingErrorAngle;

        EntityActionTag[] actionTags;

        float checkLineOfFireInterval;
        float nextCheckLineOfFireTime;
        bool blockShootingCauseNoLineOfFire = false;
        LayerMask checkLineOfFireLayerMask;


        public St_HS_ShootWeaponAtEnemy_DCCEntity(AIController aiController, DecisionContext context, int weaponID, float maxAllowedWeaponAimingErrorAngle, float checkLineOfFireInterval, LayerMask checkLineOfFireLayerMask)
        {
            this.aiController = (AIController_HumanoidSoldier)aiController;
            this.charController = this.aiController.characterController;
            this.target = (SensedEntityInfo)context.target;
            this.weaponID = weaponID;
            this.maxAllowedWeaponAimingErrorAngle = maxAllowedWeaponAimingErrorAngle;

            actionTags = new EntityActionTag[1];
            actionTags[0] = new EntityActionTag(EntityActionTag.Type.ShootingAtTarget);
            actionTags[0].shootAtTarget = target.entity;

            this.checkLineOfFireInterval = checkLineOfFireInterval;
            this.checkLineOfFireLayerMask = checkLineOfFireLayerMask;

            nextCheckLineOfFireTime = Time.time + Random.Range(0, checkLineOfFireInterval);
        }

        public override void OnStateEnter()
        {

        }

        public override void OnStateExit()
        {
            charController.StopAimingSpine();
            charController.StopAimingWeapon();
        }

        public override EntityActionTag[] GetActionTagsToAddOnStateEnter()
        {
            return actionTags;
        }

        public override EntityActionTag[] GetActionTagsToRemoveOnStateExit()
        {
            return actionTags;
        }

        public override void UpdateState()
        {
            if (target.IsAlive())
            {
                charController.ChangeSelectedItem(weaponID);

                charController.AimSpineAtPosition(target.GetAimPosition());
                charController.AimWeaponAtPosition(target.GetAimPosition());

                if (charController.GetCurrentWeaponAimingErrorAngle(false) < maxAllowedWeaponAimingErrorAngle)
                {
                    //Debug.Log("shot weapon error angle: " + charController.GetCurrentWeaponAimingErrorAngle(false));

                    if (!blockShootingCauseNoLineOfFire)
                    {
                        charController.ShootWeapon();
                    }

                    //check line of fire
                    if (Time.time> nextCheckLineOfFireTime)
                    {
                        nextCheckLineOfFireTime = Time.time + checkLineOfFireInterval;

                        blockShootingCauseNoLineOfFire = false;

                        //If the difference between distance to enemy and raycasted distance turns out to be bigger than 1/3 of the measured distance -> abort.
                        /*RaycastHit hit;
                        //the raycasts starts from roughly the middle of the gun - maybe set a specified point for it later - > this also keeps guns from shooting, when they are inside a wall
                        Vector3 raycastStartPoint = charController.GetCurrentWeaponShootPoint().position + -charController.GetCurrentWeaponShootPoint().forward * 0.3f;
                        if (Physics.Raycast(raycastStartPoint, charController.GetCurrentWeaponShootPoint().forward, out hit, Mathf.Infinity, checkLineOfFireLayerMask))
                        {
                            if(hit.distance < target.lastDistanceMeasured)
                            {
                                if(hit.distance/ target.lastDistanceMeasured < 0.66)
                                {
                                    blockShootingCauseNoLineOfFire = true;
                                    //Debug.Log("aborth shooting weapon cause of line of fire obstruction");
                                }
                            }
                        }*/
                        RaycastHit hit;
                        blockShootingCauseNoLineOfFire = true;
                        Vector3 raycastStartPoint = charController.GetCurrentWeaponShootPoint().position + -charController.GetCurrentWeaponShootPoint().forward * 0.3f;
                        if (Physics.Raycast(raycastStartPoint, target.GetAimPosition() - raycastStartPoint, out hit, Mathf.Infinity, checkLineOfFireLayerMask))
                        {
                            Hitbox hitbox = hit.collider.gameObject.GetComponent<Hitbox>();
                            if (hitbox)
                            {
                                if (hitbox.GetEntity() == target.entity)
                                {
                                    blockShootingCauseNoLineOfFire = false;
                                }
                            }
                        }
                    }
                }
            }
        }

        public override bool ShouldStateBeAborted()
        {
            return !target.IsAlive();
            //return stateShouldBeAbortedCauseNoLineOfFire;
            //return false;
        }
    }
}


