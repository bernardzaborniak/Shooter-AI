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
    public float enterCombatStanceChangeIKWeightsSpeed;
    public float currentChangeIKWeightsSpeed;


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

    [System.Serializable]
    public class IKState
    {
        public string name;
        public float weightsChangeSpeed;
        //public float enterStateWeightChangeSpeed;
        //public float exitStateWeightChangeSpeed;
        public float leftHandIKTargetWeight;
        public float rightHandIKTargetWeight;
        public bool recoilEnabledThroughRightHandIK;

    }



     IKState currentLayer1State;
     IKState currentLayer2State;
     IKState currentLayer3State;


    [Header("Layer 1 IK States")]
    public IKState idleStanceIKState;
    public IKState combatAndCrouchedStanceIKState;

    [Header("Layer 2 IK States")]
    public IKState reloadingIKState;
    public IKState pullingOutWeaponIKState;
    public IKState hidingWeaponIKState;
    public IKState aimingWeaponIKState;

    [Header("Layer 3 IK States")]
    public IKState traversingOffMeshLinkIKState;

    //a higher layered state always overrides the lower layer. if the current 3 layerState is null - the 2and layer is used



    public override void SetUpComponent(GameEntity entity)
    {
        base.SetUpComponent(entity);

        //currentChangeIKWeightsSpeed = defaultChangeIKWeightsSpeed;
        UpdateLayer1TargetWeightsAccordingToEquippedItem();
    }

    public override void UpdateComponent()
    {
        
         IKState currentIKState = null;
        if (currentLayer3State != null)
        {
            currentIKState = currentLayer3State;
        }
        else if (currentLayer2State != null)
        {
            currentIKState = currentLayer2State;
        }
        else if (currentLayer1State != null)
        {
            currentIKState = currentLayer1State;
        }

        float changeSpeed = currentIKState.weightsChangeSpeed * Time.deltaTime;
        //leftHandIK.weight += Mathf.Clamp((currentIKState.leftHandIKTargetWeight - leftHandIK.weight), -changeSpeed, changeSpeed);
        //rightHandIK.weight += Mathf.Clamp((currentIKState.rightHandIKTargetWeight - rightHandIK.weight), -changeSpeed, changeSpeed);

        if (leftHandIK.weight > 0)
        {
            constraintController.SetDesiredLeftIKTarget(HumanoidConstraintController.IKTargetingMode.CustomPosition, currentIKTargetItem.GetLeftHandIKPosition(), currentIKTargetItem.GetLeftHandIKRotation());
        }

        if (rightHandIK.weight > 0)
        { 
            //add a current state check here instead of this //TODO
            if (currentIKState.recoilEnabledThroughRightHandIK)
            {
                //set the positions via constraint manager - they need to be updated in late update
                constraintController.SetDesiredRightIKTarget(HumanoidConstraintController.IKTargetingMode.CustomPosition, aimingWeaponHandPosition.position, aimingWeaponHandPosition.rotation);
            }
            else
            {
                constraintController.SetDesiredRightIKTarget(HumanoidConstraintController.IKTargetingMode.AnimatedHandPosition, Vector3.zero, Quaternion.identity);
            }
        }
        




        /* float changeSpeed = currentChangeIKWeightsSpeed * Time.deltaTime;
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
         }*/


    }

    void UpdateLayer1TargetWeightsAccordingToEquippedItem()
    {
        if (currentIKSettings.idleIKLeft)
        {

            idleStanceIKState.leftHandIKTargetWeight = 1;
        }
        else
        {
            idleStanceIKState.leftHandIKTargetWeight = 0;
        }

        if (currentIKSettings.idleIKRight)
        {
            idleStanceIKState.rightHandIKTargetWeight = 1;
        }
        else
        {
            idleStanceIKState.rightHandIKTargetWeight = 0;
        }


        if (currentIKSettings.combatIKLeft)
        {
            combatAndCrouchedStanceIKState.leftHandIKTargetWeight = 1;
        }
        else
        {
            combatAndCrouchedStanceIKState.leftHandIKTargetWeight = 0;
        }

        if (currentIKSettings.combatIKRight)
        {
            combatAndCrouchedStanceIKState.rightHandIKTargetWeight = 1;
        }
        else
        {
            combatAndCrouchedStanceIKState.rightHandIKTargetWeight = 0;
        }
    }

  

   /* void UpdateDesiredIKWeights()
    {
        if(secondaryIKStance == SecondaryIKStance.None)
        {
            if (primaryIKStance == PrimaryIKStance.Idle)
            {
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
            /*if (primaryIKStance == PrimaryIKStance.Idle)
            {
                SetIKWeightsForIdle();
            }
            else if (primaryIKStance == PrimaryIKStance.CombatStance)
            {
                SetIKWeightsForCombat();
            }*//*
            desiredLeftHandIKRigWeight = 0;
            desiredRightHandIKRigWeight = 0;

        }
        else if (secondaryIKStance == SecondaryIKStance.HidingItem)
        {
            //SetIKWeightsForHidingItem();
            desiredLeftHandIKRigWeight = 0;
            desiredRightHandIKRigWeight = 0;
        }
        else if (secondaryIKStance == SecondaryIKStance.ReloadingWeapon)
        {
            SetIKWeightsForReloadingWeapon();
        }
    }*/

    #region Change States External Orders



    public void OnEnterIdleStance()
    {
        //SetPrimaryIKStance(PrimaryIKStance.Idle);
        // currentChangeIKWeightsSpeed = defaultChangeIKWeightsSpeed;
        //idleStanceIKState

        currentLayer1State = idleStanceIKState;
    }

    public void OnEnterCombatOrCrouchedStance()
    {
        currentLayer1State = combatAndCrouchedStanceIKState;

        // SetPrimaryIKStance(PrimaryIKStance.CombatStance);
        //currentChangeIKWeightsSpeed = enterCombatStanceChangeIKWeightsSpeed;
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

            if (currentIKTargetItem != null)
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

        UpdateLayer1TargetWeightsAccordingToEquippedItem();
    }

    public void OnStartAimingWeapon()
    {
        //SetSecondaryIKStance(SecondaryIKStance.Aiming);
    }

    public void OnStopAimingWeapon()
    {
        /*if (secondaryIKStance == SecondaryIKStance.Aiming)
        {
            SetSecondaryIKStance(SecondaryIKStance.None);
            currentChangeIKWeightsSpeed = defaultChangeIKWeightsSpeed;
        }*/
         
    }

    public void OnStartPullingOutWeapon()
    {
        //SetSecondaryIKStance(SecondaryIKStance.PullingOutItem);
    }

    public void OnStopPullingOutWeapon()
    {
        /*if (secondaryIKStance == SecondaryIKStance.PullingOutItem)
        {
            SetSecondaryIKStance(SecondaryIKStance.None);
        }*/
    }

    public void OnStartHidingWeapon()
    {
        //SetSecondaryIKStance(SecondaryIKStance.HidingItem);
    }

    public void OnStopHidingWeapon()
    {
        /*if (secondaryIKStance == SecondaryIKStance.HidingItem)
        {
            SetSecondaryIKStance(SecondaryIKStance.None);
        }*/
    }

    public void OnStartReloadingWeapon()
    {
        //SetSecondaryIKStance(SecondaryIKStance.ReloadingWeapon);
    }

    public void OnStopReloadingWeapon()
    {
        /*if (secondaryIKStance == SecondaryIKStance.ReloadingWeapon)
        {
            SetSecondaryIKStance(SecondaryIKStance.None);
        }*/
    }

    public void OnStartTraversingOffMeshLink()
    {
        //SetSecondaryIKStance(SecondaryIKStance.TraversingOffMeshLink);
       // delayedOnStopTraversingOffMeshLink = false;
    }

    public void OnStopTraversingOffMeshLink()
    {
        //SetSecondaryIKStance(SecondaryIKStance.None);
    }

    #endregion


    /*
    #region Change States Internal

    void SetPrimaryIKStance(PrimaryIKStance newStance)
    {
        primaryIKStance = newStance;
        UpdateDesiredIKWeights();
    }

    void SetSecondaryIKStance(SecondaryIKStance newStance)
    {
        secondaryIKStance = newStance;
        UpdateDesiredIKWeights();
    }

    #endregion



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
        desiredLeftHandIKRigWeight = 0;
        desiredRightHandIKRigWeight = 0;
    }

   /* void SetIKWeightsForPullingOutItem()
    {
        desiredLeftHandIKRigWeight = 0;
        desiredRightHandIKRigWeight = 0;
    }*/

    /*

    void SetIKWeightsForHidingItem()
    {
        desiredLeftHandIKRigWeight = 0;
        desiredRightHandIKRigWeight = 0;
    }

    void SetIKWeightsForReloadingWeapon()
    {
        desiredLeftHandIKRigWeight = 0;
        desiredRightHandIKRigWeight = 0;
    }*/

}
