using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UIElements;

// Modifies the desired Right And Left hand IK Weight
public class EC_HumanoidHandsIKController : EntityComponent
{
    //this class needs some refactoring - it isnt very clear, maybe also have an ik stance - disabled?

    #region Fields

    [Header("Hand IK")]

    
    IItemWithIKHandPositions currentIKTargetItem;

    [Tooltip("The IK Targets are being swet through the constraint controller - as they need to be set in LateUpdate")]
    public HumanoidConstraintController constraintController;
    [Space(10)]
    public WhireWhizTwoBoneIK leftHandIK;
    float desiredLeftHandIKRigWeight;
    public WhireWhizTwoBoneIK rightHandIK;
    float desiredRightHandIKRigWeight;

    [Header("IK Weight Change Speeds")]
    [Tooltip("the default speed used for most changes")]
    public float defaultChangeIKWeightsSpeed;


    [Header("For Recoil")]
    public Transform rightHandTransform;

    [Header("Aiming Weapon")]

    [Tooltip("position to which the right hand is being IK'eyd when aiming weapon - weapn is parented to the right hand")]
    public Transform aimingWeaponHandPosition;
    //bool aimingWeapon;

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
        //[Space(10)]
        //public bool aimingIKLeft = false;
        //public bool aimingIKRight = false;
    }
    [Space(10)]
    public IKSettingsCorrespondingToWeaponInteractionType[] iKSettingsCorrespondingToWeaponInteractionTypes;
    [Space(10)]
    public IKSettingsCorrespondingToWeaponInteractionType currentIKSettings;


   // bool IKEnabled;

    enum PrimaryIKStance
    {
       // NoIK,
        Idle,
        CombatStance,
    }
    PrimaryIKStance primaryIKStance;

    enum SecondaryIKStance
    {
        None,
        Aiming,
        TraversingOffMeshLink,
        PullingOutItem,
        HidingItem,
        ReloadingWeapon
    }
    SecondaryIKStance secondaryIKStance;



    #endregion

    public override void SetUpComponent(GameEntity entity)
    {
        base.SetUpComponent(entity);
    }

    public override void UpdateComponent()
    {
        float changeSpeed = defaultChangeIKWeightsSpeed * Time.deltaTime;
        leftHandIK.weight += Mathf.Clamp((desiredLeftHandIKRigWeight - leftHandIK.weight), -changeSpeed, changeSpeed);
        rightHandIK.weight += Mathf.Clamp((desiredRightHandIKRigWeight - rightHandIK.weight), -changeSpeed, changeSpeed);

        //add a current state check here instead of this //TODO
        if (currentIKTargetItem != null)
        {
            //set the positions via constraint manager - they need to be updated in late update
            constraintController.SetDesiredLeftIKTarget(HumanoidConstraintController.IKTargetingMode.CustomPosition, currentIKTargetItem.GetLeftHandIKPosition(), currentIKTargetItem.GetLeftHandIKRotation());
            constraintController.SetDesiredRightIKTarget(HumanoidConstraintController.IKTargetingMode.AnimatedHandPosition, Vector3.zero, Quaternion.identity);
        }
        if (secondaryIKStance == SecondaryIKStance.Aiming)
        {
            constraintController.SetDesiredRightIKTarget(HumanoidConstraintController.IKTargetingMode.CustomPosition, aimingWeaponHandPosition.position, aimingWeaponHandPosition.rotation);
        }
    }

    public void OnChangeItemInHand(Item newItem)
    {
        if (newItem == null)
        {
           currentIKSettings = iKSettingsCorrespondingToWeaponInteractionTypes[0];
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
            }
            else
            {
                currentIKSettings = iKSettingsCorrespondingToWeaponInteractionTypes[0];
            }  
        }
    }

    void UpdateDesiredIKWeights()
    {
        Debug.Log("update weights");
        if(secondaryIKStance == SecondaryIKStance.None)
        {
            if (primaryIKStance == PrimaryIKStance.Idle)
            {
                Debug.Log("update weights 1.1");

                SetIKWeightsForIdle();
            }
            else if (primaryIKStance == PrimaryIKStance.CombatStance)
            {
                SetIKWeightsForCombat();
            }
        }
        else if(secondaryIKStance == SecondaryIKStance.Aiming)
        {
            SetIKWeightsForAiming();
        }
        else if(secondaryIKStance == SecondaryIKStance.TraversingOffMeshLink)
        {
            SetIKWeightsForTraversingOffMeshLink();
        }
        else if(secondaryIKStance == SecondaryIKStance.PullingOutItem)
        {
            //SetIKWeightsForPullingOutItem();
            if (primaryIKStance == PrimaryIKStance.Idle)
            {
                Debug.Log("update weights 4.1");

                SetIKWeightsForIdle();
            }
            else if (primaryIKStance == PrimaryIKStance.CombatStance)
            {
                SetIKWeightsForCombat();
            }
        }
        else if (secondaryIKStance == SecondaryIKStance.HidingItem)
        {
            SetIKWeightsForHidingItem();
        }
        else if (secondaryIKStance == SecondaryIKStance.ReloadingWeapon)
        {
            SetIKWeightsForReloadingWeapon();
        }
    }

    #region Change States External Orders

    public void OnEnterIdleStance()
    {
        SetPrimaryIKStance(PrimaryIKStance.Idle);
    }

    public void OnEnterCombatStance()
   {
        SetPrimaryIKStance(PrimaryIKStance.CombatStance);
   }

    public void OnStartAimingWeapon()
    {
        SetSecondaryIKStance(SecondaryIKStance.Aiming);
    }

    public void OnStopAimingWeapon()
    {
        Debug.Log("on stop aiming  ");

        if (secondaryIKStance == SecondaryIKStance.Aiming)
        {
            Debug.Log("on stop aiming 2  ");

            SetSecondaryIKStance(SecondaryIKStance.None);
        }
         
    }

    public void OnStartPullingOutWeapon()
    {
        Debug.Log("on start pull ");

        SetSecondaryIKStance(SecondaryIKStance.PullingOutItem);
    }

    public void OnStopPullingOutWeapon()
    {
        Debug.Log("on stop pull ");


        if (secondaryIKStance == SecondaryIKStance.PullingOutItem)
        {
            SetSecondaryIKStance(SecondaryIKStance.None);
        }
    }

    public void OnStartHidingWeapon()
    {
        Debug.Log("on start hiding ");

        SetSecondaryIKStance(SecondaryIKStance.HidingItem);
    }

    public void OnStopHidingWeapon()
    {
        Debug.Log("on stop hiding ");

        if (secondaryIKStance == SecondaryIKStance.HidingItem)
        {
            SetSecondaryIKStance(SecondaryIKStance.None);
        }
    }

    public void OnStartReloadingWeapon()
    {
        Debug.Log("on start reload ");

        SetSecondaryIKStance(SecondaryIKStance.ReloadingWeapon);
    }

    public void OnStopReloadingWeapon()
    {
        Debug.Log("on stop reload ");

        if (secondaryIKStance == SecondaryIKStance.ReloadingWeapon)
        {
            SetSecondaryIKStance(SecondaryIKStance.None);
        }
    }

    public void OnStartTraversingOffMeshLink()
    {
        Debug.Log("on start traversing");
        SetSecondaryIKStance(SecondaryIKStance.TraversingOffMeshLink);
       // delayedOnStopTraversingOffMeshLink = false;
    }

    public void OnStopTraversingOffMeshLink()
    {
        Debug.Log("on stop traversing");
        SetSecondaryIKStance(SecondaryIKStance.None);
    }

    #endregion

    #region Change States internal

    void SetPrimaryIKStance(PrimaryIKStance newStance)
    {
        Debug.Log("set primary stance");
        primaryIKStance = newStance;
        UpdateDesiredIKWeights();
    }

    void SetSecondaryIKStance(SecondaryIKStance newStance)
    {
        Debug.Log("set secondary stance");

        secondaryIKStance = newStance;
        UpdateDesiredIKWeights();
    }

    #endregion



    void SetIKWeightsForIdle()
    {
        Debug.Log("set weights for idle");

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
        Debug.Log("set weights for combat");

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
        Debug.Log("set weights for aiming");

        //if (currentIKSettings.aimingIKLeft)
        if (currentIKSettings.combatIKLeft)
        {
            desiredLeftHandIKRigWeight = 1;
        }
        else
        {
            desiredLeftHandIKRigWeight = 0;
        }

        //if (currentIKSettings.aimingIKRight)
        if (currentIKSettings.combatIKRight)
        {
            desiredRightHandIKRigWeight = 1;
        }
        else
        {
            desiredRightHandIKRigWeight = 0;
        }
    }

    void SetIKWeightsForTraversingOffMeshLink()
    {
        Debug.Log("set weights for traversing");
        desiredLeftHandIKRigWeight = 0;
        desiredRightHandIKRigWeight = 0;
    }

   /* void SetIKWeightsForPullingOutItem()
    {
        desiredLeftHandIKRigWeight = 0;
        desiredRightHandIKRigWeight = 0;
    }*/

    void SetIKWeightsForHidingItem()
    {
        Debug.Log("set weights for hiding");

        desiredLeftHandIKRigWeight = 0;
        desiredRightHandIKRigWeight = 0;
    }

    void SetIKWeightsForReloadingWeapon()
    {
        Debug.Log("set weights for reloading");

        desiredLeftHandIKRigWeight = 0;
        desiredRightHandIKRigWeight = 0;
    }

}
