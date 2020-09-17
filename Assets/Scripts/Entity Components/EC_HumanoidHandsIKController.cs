using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using System;

public class EC_HumanoidHandsIKController : EntityComponent
{
    [Header("Hand IK")]

    Weapon ikTargetWeapon;

    public float changeIKWeightsSpeed;

    [Tooltip("depending on the specific model skeleton hand orientation?")]
    public Vector3 handIKRotationOffset;

    public TwoBoneIKConstraint leftHandIKConstraint;
    float desiredLeftHandIKRigWeight;
    public Transform leftHandIKTarget;

    public TwoBoneIKConstraint rightHandIKConstraint;
    float desiredRightHandIKRigWeight;
    public Transform rightHandIKTarget;


    [Serializable]
    public class IKSettingsCorrespondingToWeaponInteractionType
    {
        public WeaponInteractionType weaponInteractionType;

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



    public override void SetUpComponent(GameEntity entity)
    {
        base.SetUpComponent(entity);

        currentIKSettings = iKSettingsCorrespondingToWeaponInteractionTypes[0];
        SetIKWeightsForIdle();

        leftHandIKConstraint.weight = 0;
        rightHandIKConstraint.weight = 0;
    }

    public override void UpdateComponent()
    {
        float changeSpeed = changeIKWeightsSpeed * Time.deltaTime;

        leftHandIKConstraint.weight += Mathf.Clamp((desiredLeftHandIKRigWeight - leftHandIKConstraint.weight), -changeSpeed, changeSpeed);
        rightHandIKConstraint.weight += Mathf.Clamp((desiredRightHandIKRigWeight - rightHandIKConstraint.weight), -changeSpeed, changeSpeed);

        if (ikTargetWeapon)
        {
            leftHandIKTarget.position = ikTargetWeapon.leftHandIKPosition.position;
            leftHandIKTarget.rotation = ikTargetWeapon.leftHandIKPosition.rotation * Quaternion.Euler(handIKRotationOffset);

            rightHandIKTarget.position = ikTargetWeapon.rightHandIKPosition.position;
            rightHandIKTarget.rotation = ikTargetWeapon.rightHandIKPosition.rotation * Quaternion.Euler(handIKRotationOffset);
        }
    }


    public void OnChangeWeapon(Weapon newWeapon)
    {
        ikTargetWeapon = newWeapon;

        if (newWeapon == null)
        {
            currentIKSettings = iKSettingsCorrespondingToWeaponInteractionTypes[0];
        }
        else
        {
            for (int i = 0; i < iKSettingsCorrespondingToWeaponInteractionTypes.Length; i++)
            {
                if (iKSettingsCorrespondingToWeaponInteractionTypes[i].weaponInteractionType == newWeapon.weaponInteractionType)
                {
                    currentIKSettings = iKSettingsCorrespondingToWeaponInteractionTypes[i];
                }
            }

        }


        if(iKStance == IKStance.Idle)
        {
            SetIKWeightsForIdle();
        }
        else if(iKStance == IKStance.Combat)
        {
            SetIKWeightsForCombat();
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
        Debug.Log("set for idle");
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
}
