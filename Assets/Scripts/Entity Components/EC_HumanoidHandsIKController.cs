using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using UnityEngine.Animations.Rigging;
using System;

public class EC_HumanoidHandsIKController : EntityComponent
{
    [Header("Hand IK")]

    //Gun ikTargetWeapon;
    //public HumanoidConstraintController constraintController;
    IItemWithIKHandPositions currentIKTargetItem;

    public float changeIKWeightsSpeed;

    [Tooltip("depending on the specific model skeleton hand orientation?")]
    public Vector3 handIKRotationOffset;

    float desiredLeftHandIKRigWeight;
    public WhireWhizTwoBoneIK leftHandIK;
    public Transform leftHandIKTarget;

    float desiredRightHandIKRigWeight;
    public WhireWhizTwoBoneIK rightHandIK;
    public Transform rightHandIKTarget;


    [Serializable]
    public class IKSettingsCorrespondingToWeaponInteractionType
    {
        public ItemInteractionType weaponInteractionType;

        public bool idleIKLeft = false;
        public bool idleIKRight = false;
        public bool combatIKLeft = false;
        public bool combatIKRight = false;
    }

    public IKSettingsCorrespondingToWeaponInteractionType[] iKSettingsCorrespondingToWeaponInteractionTypes;

    public IKSettingsCorrespondingToWeaponInteractionType currentIKSettings;


    enum IKStance
    {
        Idle,
        Combat
    }

    IKStance iKStance;

    //overwrite weapon iks
    bool disableIKs; //for now overwriting just means disabling ik's



    public override void SetUpComponent(GameEntity entity)
    {
        base.SetUpComponent(entity);

        DisableIKs();
    }

    public override void UpdateComponent()
    {
        float changeSpeed = changeIKWeightsSpeed * Time.deltaTime;

        leftHandIK.weight += Mathf.Clamp((desiredLeftHandIKRigWeight - leftHandIK.weight), -changeSpeed, changeSpeed);
        rightHandIK.weight += Mathf.Clamp((desiredRightHandIKRigWeight - rightHandIK.weight), -changeSpeed, changeSpeed);

        if (currentIKTargetItem != null)
        {
            leftHandIKTarget.position = currentIKTargetItem.GetLeftHandIKPosition();
            leftHandIKTarget.rotation = currentIKTargetItem.GetLeftHandIKRotation() * Quaternion.Euler(handIKRotationOffset);

            rightHandIKTarget.position = currentIKTargetItem.GetRightHandIKPosition();
            rightHandIKTarget.rotation = currentIKTargetItem.GetRightHandIKRotation() * Quaternion.Euler(handIKRotationOffset);
        }
    }


    public void OnChangeItemInHand(Item newItem)
    {
        if (newItem == null)
        {
           currentIKSettings = iKSettingsCorrespondingToWeaponInteractionTypes[0];
            DisableIKs();
            currentIKTargetItem = null;
        }
        else
        {
            currentIKTargetItem = newItem.GetComponent<IItemWithIKHandPositions>();

            if(currentIKTargetItem != null)
            {
                for (int i = 0; i < iKSettingsCorrespondingToWeaponInteractionTypes.Length; i++)
                {
                    if (iKSettingsCorrespondingToWeaponInteractionTypes[i].weaponInteractionType == newItem.itemInteractionType)
                    {
                        currentIKSettings = iKSettingsCorrespondingToWeaponInteractionTypes[i];
                    }
                }

                ReenableIKs();
            }
            else
            {
                currentIKSettings = iKSettingsCorrespondingToWeaponInteractionTypes[0];
                DisableIKs();
            }  
        } 
    }

    
    public void OnEnterIdleStance()
    {
        iKStance = IKStance.Idle;

        SetIKWeightsForIdle();
    }

    public void OnEnterCombatStance()
    {
        iKStance = IKStance.Combat;

        SetIKWeightsForCombat();
    }

    void SetIKWeightsForIdle()
    {
        if (currentIKSettings.idleIKLeft)
        {
            desiredLeftHandIKRigWeight = 1;
        }
        else
        {
            desiredLeftHandIKRigWeight = 0;
        }

        if (currentIKSettings.idleIKRight)
        {
            desiredRightHandIKRigWeight = 1;
        }
        else
        {
            desiredRightHandIKRigWeight = 0;
        }
    }

    void SetIKWeightsForCombat()
    {
        if (currentIKSettings.combatIKLeft)
        {
            desiredLeftHandIKRigWeight = 1;
        }
        else
        {
            desiredLeftHandIKRigWeight = 0;
        }

        if (currentIKSettings.combatIKRight)
        {
            desiredRightHandIKRigWeight = 1;
        }
        else
        {
            desiredRightHandIKRigWeight = 0;
        }
    }

    public void DisableIKs()
    {
        disableIKs = true;

        desiredLeftHandIKRigWeight = 0;
        desiredRightHandIKRigWeight = 0;
    }

    public void ReenableIKs()
    {
        disableIKs = false;

        if(iKStance == IKStance.Idle)
        {
            SetIKWeightsForIdle();
        }
        else
        {
            SetIKWeightsForCombat();
        } 
    }
}
