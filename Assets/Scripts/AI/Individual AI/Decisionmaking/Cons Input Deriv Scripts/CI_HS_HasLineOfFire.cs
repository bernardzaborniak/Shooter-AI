using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace BenitosAI
{


    [CreateAssetMenu(menuName = "AI/ConsiderationInput/Humanoid/HasLineofFire", fileName = "HasLineofFire")]
    public class CI_HS_HasLineOfFire : ConsiderationInput
    {   
        public LayerMask checkLineOfFireLayerMask;

        public override float GetConsiderationInput(DecisionContext decisionContext, Consideration consideration)
        {
            //get raycast start posiiton & direction -> how? - get the position and direction from the head - position is head and direction is direction from head to target - aim posiiton
            //- check if hitbox entity is target entity

            RaycastHit hit;
            //the raycasts starts from roughly the middle of the gun - maybe set a specified point for it later - > this also keeps guns from shooting, when they are inside a wall
            //Vector3 raycastStartPoint = charController.GetCurrentWeaponShootPoint().position + -charController.GetCurrentWeaponShootPoint().forward * 0.3f;
            Transform weaponShootPointTransform = ((AIController_HumanoidSoldier)decisionContext.aiController).characterController.GetCurrentWeaponShootPoint();
            //Vector3 raycastStartPoint = weaponShootPointTransform.position + -weaponShootPointTransform.forward * 0.3f;
            Vector3 raycastStartPoint = weaponShootPointTransform.position;

            if (Physics.Raycast(raycastStartPoint, decisionContext.targetEntity.GetAimPosition()- raycastStartPoint, out hit, Mathf.Infinity, checkLineOfFireLayerMask))
            {
                Hitbox hitbox = hit.collider.gameObject.GetComponent<Hitbox>();
                if (hitbox)
                {
                    if(hitbox.GetEntity() == decisionContext.targetEntity.entity)
                    {
                        return 1;
                    }
                }

               /* Debug.Log("hit.distance: " + hit.distance);
                Debug.Log("decisionContext.targetEntity.lastDistanceMeasured: " + decisionContext.targetEntity.lastDistanceMeasured);
                Debug.Log("hit : " + hit.collider.gameObject.name);

                if (hit.distance < decisionContext.targetEntity.lastDistanceMeasured)
                {
                    //if (hit.distance / decisionContext.targetEntity.lastDistanceMeasured < 0.66)
                    if (decisionContext.targetEntity.lastDistanceMeasured - hit.distance > 0.3f)
                    {
                        Debug.Log("return 0");
                        return 0;
                    }
                }*/
            }


            return 0;

        }
    }

}
