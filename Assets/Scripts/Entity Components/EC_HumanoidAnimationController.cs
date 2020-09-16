using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EC_HumanoidAnimationController : EntityComponent
{
    [Header("References")]
    public Animator animator;

    [Header("Anim Params")]
    public string forwardVelocityParam;
    int forwardVelocityParamID;
    public string sidewaysVelocityParam;
    int sidewaysVelocityParamID;
    public string angularVelocityParam;
    int angularVelocityParamID;

    public string stanceParam;
    int stanceParamID;

    public string itemInHandParam;
    int itemInHandParamID;

    [Header("Adjusting Animtion Layers etc..")]
    [Tooltip("Turn animation override layer is only played below this velocity")]
    public float turnAnimationVelocityThreshold;

    [Header("Changing Weapons")]

    public string weaponInteractionStateParam;
    int  weaponInteractionStateParamID;


    // pull out / hide weapon states
   // public string pullOutRifleStateName;
   // int pullOutRifleStateNameID;
    [Tooltip("How long is the original animaiton in seconds?")]
    public float pullOutRifleAnimationLength;
    //public string hideRifleStateName;
    //int hideRifleStateNameID;
    [Tooltip("How long is the original animaiton in seconds?")]
    public float hideRifleAnimationLength;

    //public string pullOutPistolStateName;
    //int pullOutPistolStateNameID;
    [Tooltip("How long is the original animaiton in seconds?")]
    public float pullOutPistolAnimationLength;
   // public string hidePistolStateName;
    //int hidePistolStateNameID;
    [Tooltip("How long is the original animaiton in seconds?")]
    public float hidePistolAnimationLength;

    /*public string pullOutWeaponParam;
    int pullOutWeaponParamID;
    public string hideWeaponParam;
    int hideWeaponParamID;

    public string noOverrideState;
    int noOverrideStateID;*/



    // pull out / hide weapon speed & offset adjusters
   
    public string pullOutWeaponSpeedMultiplierParam;
    int pullOutWeaponSpeedMultiplierParamID;
    public string pullOutWeaponStartOffset;
    int pullOutWeaponStartOffsetID;

    public string hideWeaponSpeedMultiplierParam;
    int hideWeaponSpeedMultiplierParamID;
    public string hideWeaponStartOffset;
    int hideWeaponStartOffsetID;


    /* ------Animation ID's--------
    
   -----Stance ID's---

    Idle,            0
    Combat,          1
    CrouchingCombat  2

     -----Item In Hand ID's---

    NoItem,            0
    Rifle,             1
    Pistol             2

    ---WeaponInteractionState ID's-----
    
     Idle,              0
     PullingOutWeapon,  1
     HidingWeapon       2
    

    */


    public override void SetUpComponent(GameEntity entity)
    {
        base.SetUpComponent(entity);

        // Convert the strings into hashes to improve performance
        forwardVelocityParamID = Animator.StringToHash(forwardVelocityParam);
        sidewaysVelocityParamID = Animator.StringToHash(sidewaysVelocityParam);
        angularVelocityParamID = Animator.StringToHash(angularVelocityParam);

        stanceParamID = Animator.StringToHash(stanceParam);
        itemInHandParamID = Animator.StringToHash(itemInHandParam);

        // pull out / hide weapon states
        /* pullOutRifleStateNameID = Animator.StringToHash(pullOutRifleStateName);
         hideRifleStateNameID = Animator.StringToHash(hideRifleStateName);
         pullOutPistolStateNameID = Animator.StringToHash(pullOutPistolStateName);
         hidePistolStateNameID = Animator.StringToHash(hidePistolStateName);*/

        //pullOutWeaponParamID = Animator.StringToHash(pullOutWeaponParam);
        //hideWeaponParamID = Animator.StringToHash(hideWeaponParam);
        weaponInteractionStateParamID = Animator.StringToHash(weaponInteractionStateParam);

        // pull out / hide weapon speed & offset adjusters
        pullOutWeaponSpeedMultiplierParamID = Animator.StringToHash(pullOutWeaponSpeedMultiplierParam);
        pullOutWeaponStartOffsetID = Animator.StringToHash(pullOutWeaponStartOffset);

        hideWeaponSpeedMultiplierParamID = Animator.StringToHash(hideWeaponSpeedMultiplierParam);
        hideWeaponStartOffsetID = Animator.StringToHash(hideWeaponStartOffset);

        //noOverrideStateID = Animator.StringToHash(noOverrideState);
    }

    public override void UpdateComponent()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            //animator.SetBool("Pulling Out Weapon", true);
            
        }
        if (Input.GetKeyDown(KeyCode.Z))
        {
            //animator.SetBool("Hiding Weapon", true);
        }
    }

    public void UpdateLocomotionAnimation(float velocity, float forwardVelocity, float sidewaysVelocity, float angularVelocity)
    {  
        animator.SetFloat(sidewaysVelocityParamID, sidewaysVelocity);
        animator.SetFloat(forwardVelocityParamID, forwardVelocity);
        animator.SetFloat(angularVelocityParamID, angularVelocity);

        // Set Turn layer weight
        float weight = 0;

        if (velocity < turnAnimationVelocityThreshold)
        {
            weight = Utility.Remap(velocity, 0, turnAnimationVelocityThreshold, 1, 0);
        }
        animator.SetLayerWeight(1, weight);      
    }

    public void ChangeToIdleStance()
    {
        animator.SetInteger(stanceParamID, 0);
    }

    public void ChangeToCombatStance()
    {
        animator.SetInteger(stanceParamID, 1);
    }

    public void ChangeToCrouchedStance()
    {
        animator.SetInteger(stanceParamID, 2);
    }

    public void ChangeItemInHand(int newItemInHandID)
    {
        //Debug.Log("set item in hand: " + newItemInHandID);
        animator.SetInteger(itemInHandParamID, newItemInHandID);       
    }

    public void ChangeWeaponInteractionState(int weaponinteractionState)
    {
        //adjust the speeds and offsets


        animator.SetInteger(weaponInteractionStateParamID, weaponinteractionState);
    }

    public void AdjustPullOutAnimationSpeedAndOffset(float animationDuration, float animationOffset)
    {
        Debug.Log("adjust speed: " + animationDuration + " " + animationOffset);
        int weaponAnimationID = animator.GetInteger(itemInHandParamID);

        if (weaponAnimationID == 1)
        {
            // animator.Play(pullOutRifleStateNameID, 2, 0);
            animator.SetFloat(pullOutWeaponSpeedMultiplierParamID, pullOutRifleAnimationLength / animationDuration);
            Debug.Log("multiplier: " + (pullOutRifleAnimationLength / animationDuration));

        }
        else if (weaponAnimationID == 2)
        {
            //animator.Play(pullOutPistolStateNameID, 2, 0);
            animator.SetFloat(pullOutWeaponSpeedMultiplierParamID, pullOutPistolAnimationLength / animationDuration);
        }

        animator.SetFloat(pullOutWeaponStartOffsetID, animationOffset);
    }


    public void AdjustHideAnimationSpeedAndOffset(float animationDuration, float animationOffset)
    {
        int weaponAnimationID = animator.GetInteger(itemInHandParamID);

        if (weaponAnimationID == 1)
        {
            // animator.Play(pullOutRifleStateNameID, 2, 0);
            animator.SetFloat(hideWeaponSpeedMultiplierParamID, hideRifleAnimationLength / animationDuration);
            Debug.Log("multiplier: " + (pullOutRifleAnimationLength / animationDuration));

        }
        else if (weaponAnimationID == 2)
        {
            //animator.Play(pullOutPistolStateNameID, 2, 0);
            animator.SetFloat(hideWeaponSpeedMultiplierParamID, hidePistolAnimationLength / animationDuration);
        }

        animator.SetFloat(hideWeaponStartOffsetID, animationOffset);
    }

    /*public void OStartPullingOutWeapon(int animationID, float animationDuration)
    {

        Debug.Log("----------------------pull out start anim-----------------------------------");

        int weaponAnimationID = animator.GetInteger(itemInHandParamID); // this instead? could reduce problems and make it cleaner?

        if (animationID == 1)
        {
           // animator.Play(pullOutRifleStateNameID, 2, 0);
            animator.SetFloat(pullOutWeaponSpeedMultiplierParamID, pullOutRifleAnimationLength / animationDuration);
            Debug.Log("multiplier: " + (pullOutRifleAnimationLength / animationDuration));

        }
        else if (animationID == 2)
        {
            //animator.Play(pullOutPistolStateNameID, 2, 0);
            animator.SetFloat(pullOutWeaponSpeedMultiplierParamID, pullOutPistolAnimationLength / animationDuration);
        }

        //animator.SetBool(pullOutWeaponParamID, true);
        Debug.Log("pullOutWeaponParamID: " + true);
    }

    public void FinishPullingOutWeapon()
    {
        //animator.Play(noOverrideStateID, 2, 0);
        animator.SetBool(pullOutWeaponParamID, false);
        Debug.Log("pullOutWeaponParamID: " + false);
    }

    public void StartHidingWeapon(int animationID, float animationDuration, float offset)
    {
       animator.SetFloat(hideWeaponStartOffsetID, offset);

       if(animationID == 1)
       {
           // animator.Play(hideRifleStateNameID, 2, 0);
            animator.SetFloat(hideWeaponSpeedMultiplierParamID, hideRifleAnimationLength / animationDuration);
       }
       else if(animationID == 2)
       {
            //animator.Play(hidePistolStateNameID, 2, 0);
            animator.SetFloat(hideWeaponSpeedMultiplierParamID, hidePistolAnimationLength / animationDuration);
       }

        animator.SetBool(hideWeaponParamID, true);
        Debug.Log("hideWeaponParamID: " + true);

    }

    public void FinishHidingWeapon()
    {
        //animator.Play(noOverrideStateID, 2, 0);
        animator.SetBool(hideWeaponParamID, false);
        Debug.Log("hideWeaponParamID: " + false);
    }*/

}
