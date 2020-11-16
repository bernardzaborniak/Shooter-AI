using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UIElements;

// Modifies the desired Right And Left hand IK Weight
public class HCC_HumanoidHandsIKController : HumanoidCharacterComponent
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

    [Header("For Recoil")]
    public Transform rightHandTransform;

    [Header("Aiming Weapon")]

    [Tooltip("position to which the right hand is being IK'eyd when aiming weapon - weapn is parented to the right hand")]
    public Transform aimingWeaponHandPosition;

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
    }
    [Space(10)]
    public IKSettingsCorrespondingToWeaponInteractionType[] iKSettingsCorrespondingToWeaponInteractionTypes;
    [Space(10)]
    public IKSettingsCorrespondingToWeaponInteractionType currentIKSettingCorrespondingToWeaponInteractionType;

 

    [System.Serializable]
    public class IKState
    {
        public string name;
       
        public float weightsChangeSpeed;
        public float leftHandIKTargetWeight;
        public float rightHandIKTargetWeight;
        public bool recoilEnabledThroughRightHandIK;

        public bool exitStateAfterDelay;
        public float exitStateTime;

        public virtual float GetCurrentWeightsChangeSpeed()
        {
            return weightsChangeSpeed;
        }

        public virtual float GetLeftHandIKTargetWeight()
        {
            return leftHandIKTargetWeight;
        }
        public virtual float GetRightHandIKTargetWeight()
        {
            return rightHandIKTargetWeight;
        }
    }
    

    IKState currentLayer1State;
    IKState currentLayer2State;
    IKState currentLayer3State;
    IKState currentLayer4State;


    [Header("Layer 1 IK States")]
    public IKState idleStanceIKState;
    public IKState combatAndCrouchedStanceIKState;

    [Header("Layer 2 IK States")]
    public IKState reloadingIKState;
    public IKState pullingOutWeaponIKState;
    public IKState hidingWeaponIKState;

    [Header("Layer 3 IK States")]
    public IKState aimingWeaponIKState;

    [Header("Layer 4 IK States")]
    public IKState traversingOffMeshLinkIKState;

    //a higher layered state always overrides the lower layer. if the current 3 layerState is null - the 2and layer is used

    #endregion

    public override void SetUpComponent(GameEntity entity)
    {
        base.SetUpComponent(entity);

        UpdateLayer1TargetWeightsAccordingToEquippedItem();
        currentLayer1State = idleStanceIKState;
    }

    public override void UpdateComponent()
    {
        IKState currentIKState = null;
        if (currentLayer4State != null)
        {
            currentIKState = currentLayer4State;
        }
        else if (currentLayer3State != null)
        {
            currentIKState = currentLayer3State;
        }
        else if (currentLayer2State != null)
        {
            currentIKState = currentLayer2State;

            if (currentIKState.exitStateAfterDelay)
            {
                if (Time.time > currentIKState.exitStateTime)
                {
                    currentLayer2State = null;
                }
            }
        }
        else if (currentLayer1State != null)
        {
            currentIKState = currentLayer1State;
        }


        float changeSpeed = currentIKState.GetCurrentWeightsChangeSpeed() * Time.deltaTime;
        leftHandIK.weight += Mathf.Clamp((currentIKState.GetLeftHandIKTargetWeight() - leftHandIK.weight), -changeSpeed, changeSpeed);
        rightHandIK.weight += Mathf.Clamp((currentIKState.GetRightHandIKTargetWeight() - rightHandIK.weight), -changeSpeed, changeSpeed);

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
    }

    void UpdateLayer1TargetWeightsAccordingToEquippedItem()
    {
        if (currentIKSettingCorrespondingToWeaponInteractionType.idleIKLeft)
        {

            idleStanceIKState.leftHandIKTargetWeight = 1;
        }
        else
        {
            idleStanceIKState.leftHandIKTargetWeight = 0;
        }

        if (currentIKSettingCorrespondingToWeaponInteractionType.idleIKRight)
        {
            idleStanceIKState.rightHandIKTargetWeight = 1;
        }
        else
        {
            idleStanceIKState.rightHandIKTargetWeight = 0;
        }


        if (currentIKSettingCorrespondingToWeaponInteractionType.combatIKLeft)
        {
            combatAndCrouchedStanceIKState.leftHandIKTargetWeight = 1;
        }
        else
        {
            combatAndCrouchedStanceIKState.leftHandIKTargetWeight = 0;
        }

        if (currentIKSettingCorrespondingToWeaponInteractionType.combatIKRight)
        {
            combatAndCrouchedStanceIKState.rightHandIKTargetWeight = 1;
        }
        else
        {
            combatAndCrouchedStanceIKState.rightHandIKTargetWeight = 0;
        }
    }

    #region Change States External Orders

    public void OnEnterIdleStance()
    {
        currentLayer1State = idleStanceIKState;
    }

    public void OnEnterCombatOrCrouchedStance()
    {
        currentLayer1State = combatAndCrouchedStanceIKState;
    }

    public void OnChangeItemInHand(Item newItem)
    {
        if (newItem == null)
        {
            currentIKSettingCorrespondingToWeaponInteractionType = iKSettingsCorrespondingToWeaponInteractionTypes[0];
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
                        currentIKSettingCorrespondingToWeaponInteractionType = iKSettingsCorrespondingToWeaponInteractionTypes[i];
                    }
                }
            }
            else
            {
                currentIKSettingCorrespondingToWeaponInteractionType = iKSettingsCorrespondingToWeaponInteractionTypes[0];
            }
        }

        UpdateLayer1TargetWeightsAccordingToEquippedItem();
    }

    public void OnStartAimingWeapon()
    {
        currentLayer3State = aimingWeaponIKState;
    }

    public void OnStopAimingWeapon()
    {
        currentLayer3State = null;
    }

    public void OnStartPullingOutWeapon(float timeTillFinished)
    {
        currentLayer2State = pullingOutWeaponIKState;
        pullingOutWeaponIKState.exitStateAfterDelay = true;
        pullingOutWeaponIKState.exitStateTime = Time.time + timeTillFinished-0.1f;
    }

    public void OnStopPullingOutWeapon()
    {
        currentLayer2State = null;
    }

    public void OnStartHidingWeapon(float timeTillFinished)
    {
        currentLayer2State = hidingWeaponIKState;
    }

    public void OnStopHidingWeapon()
    {
        currentLayer2State = null;
    }

    public void OnStartReloadingWeapon()
    {
        currentLayer2State = reloadingIKState;
    }

    public void OnStopReloadingWeapon()
    {
        currentLayer2State = null;
    }

    public void OnStartTraversingOffMeshLink()
    {
        currentLayer4State = traversingOffMeshLinkIKState;
    }

    public void OnStopTraversingOffMeshLink()
    {
        currentLayer4State = null;
    }

    #endregion

}
