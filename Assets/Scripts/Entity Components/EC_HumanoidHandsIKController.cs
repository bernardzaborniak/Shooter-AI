using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

// Modifies the desired Right And Left hand IK Weight
public class EC_HumanoidHandsIKController : EntityComponent
{
    //this calss needs some refactoring - it isnt very clear, maybe also have an ik stance - disabled?

    #region Fields

    [Header("Hand IK")]
    public float changeIKWeightsSpeed;
    IItemWithIKHandPositions currentIKTargetItem;
    [Tooltip("depending on the specific model skeleton hand orientation?")]
    public Vector3 handIKRotationOffset;

    [Space(10)]
    public WhireWhizTwoBoneIK leftHandIK;
    public Transform leftHandIKTarget;
    float desiredLeftHandIKRigWeight;

    [Space(10)]
    public WhireWhizTwoBoneIK rightHandIK;
    public Transform rightHandIKTarget;
    float desiredRightHandIKRigWeight;

    [Header("Aiming Weapon")]

    [Tooltip("position to which the right hand is being IK'eyd when aiming weapon - weapn is parented to the right hand")]
    public Transform aimingWeaponHandPosition;
    bool aimingWeapon;

    [Serializable]
    public class IKSettingsCorrespondingToWeaponInteractionType
    {
        public ItemInteractionType weaponInteractionType;
        [Space(10)]
        public bool idleIKLeft = false;
        public bool idleIKRight = false;
        [Space(10)]
        public bool combatIKLeft = false;
        public bool combatIKRight = false;
        [Space(10)]
        public bool aimingIKLeft = false;
        public bool aimingIKRight = false;
    }
    [Space(10)]
    public IKSettingsCorrespondingToWeaponInteractionType[] iKSettingsCorrespondingToWeaponInteractionTypes;
    [Space(10)]
    public IKSettingsCorrespondingToWeaponInteractionType currentIKSettings;


    enum IKStance
    {
        Idle,
        CombatStance,
    }

 
    IKStance iKStance;

    //overwrite weapon iks
    bool disableIKs; //for now overwriting just means disabling ik's

    #endregion

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

           // rightHandIKTarget.position = currentIKTargetItem.GetRightHandIKPosition();
            //rightHandIKTarget.rotation = currentIKTargetItem.GetRightHandIKRotation() * Quaternion.Euler(handIKRotationOffset);
        }

        if (aimingWeapon)
        {
            rightHandIKTarget.position = aimingWeaponHandPosition.position;
            rightHandIKTarget.rotation = aimingWeaponHandPosition.rotation * Quaternion.Euler(handIKRotationOffset);
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
        iKStance = IKStance.CombatStance;

        SetIKWeightsForCombat();
   }

    public void OnStartAimingWeapon()
    {
        aimingWeapon = true;

        ReenableIKs();
    }

    public void OnStopAimingWeapon()
    {
        aimingWeapon = false;

        ReenableIKs();
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

    void SetIKWeightsForAiming()
    {
        if (currentIKSettings.aimingIKLeft)
        {
            desiredLeftHandIKRigWeight = 1;
        }
        else
        {
            desiredLeftHandIKRigWeight = 0;
        }

        if (currentIKSettings.aimingIKRight)
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

        if (aimingWeapon)
        {
            SetIKWeightsForAiming();
        }
        else
        {
            if (iKStance == IKStance.Idle)
            {
                SetIKWeightsForIdle();
            }
            else
            {
                SetIKWeightsForCombat();
            }
        }

        
    }
}
