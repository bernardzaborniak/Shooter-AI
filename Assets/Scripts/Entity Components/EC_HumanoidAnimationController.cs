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
    [Tooltip("How long is the original animaiton in seconds?")]
    public float pullOutRifleAnimationLength;
    [Tooltip("How long is the original animaiton in seconds?")]
    public float hideRifleAnimationLength;
    [Tooltip("How long is the original animaiton in seconds?")]
    public float pullOutPistolAnimationLength;
    [Tooltip("How long is the original animaiton in seconds?")]
    public float hidePistolAnimationLength;


    // pull out / hide weapon speed & offset adjusters
    public string pullOutWeaponSpeedMultiplierParam;
    int pullOutWeaponSpeedMultiplierParamID;
    public string pullOutWeaponStartOffset;
    int pullOutWeaponStartOffsetID;

    public string hideWeaponSpeedMultiplierParam;
    int hideWeaponSpeedMultiplierParamID;
    public string hideWeaponStartOffset;
    int hideWeaponStartOffsetID;

    [Header("Reload Weapon")]
    public string reloadWeaponParam;
    int reloadWeaponParamID;

    public string reloadWeaponSpeedMultiplierParam;
    int reloadWeaponSpeedMultiplierParamID;

    public float reloadRifleStandingAnimationLength;
    public float reloadRifleCrouchingAnimationLength;

    [Header("Throw Grenade")]
    public string throwGrenadeParam;
    int throwGrenadeParamID;

    public string throwGrenadeSpeedMultiplierParam;
    int throwGrenadeSpeedMultiplierParamID;

    public float throwingGrenadeAnimationLength;

    [Header("Flinch")]
    public string flinchParam;
    int flinchParamID;
    public string flinchIDParam;
    int flinchIDParamID;
    [Tooltip("What is the maximum value the flinchID can have - how many different flinch animations do we have?")]
    public int numberOdFlinchAnimationVariations;

    [Header("Stagger")]
    public string staggerParam;
    int staggerParamID;
    public string staggerIDParam;
    int staggerIDParamID;
    [Tooltip("What is the maximum value the staggerID can have - how many different stagger animations do we have?")]
    public int numberOdStaggerAnimationVariations;





    /* ------Animation ID's--------
    
   -----Stance ID's---

    Idle,            0
    Combat,          1
    CrouchingCombat  2

     -----Item In Hand ID's---

    NoItem,            0
    Rifle,             1
    Pistol             2
    Grenade            3

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
        weaponInteractionStateParamID = Animator.StringToHash(weaponInteractionStateParam);

        // pull out / hide weapon speed & offset adjusters
        pullOutWeaponSpeedMultiplierParamID = Animator.StringToHash(pullOutWeaponSpeedMultiplierParam);
        pullOutWeaponStartOffsetID = Animator.StringToHash(pullOutWeaponStartOffset);

        hideWeaponSpeedMultiplierParamID = Animator.StringToHash(hideWeaponSpeedMultiplierParam);
        hideWeaponStartOffsetID = Animator.StringToHash(hideWeaponStartOffset);

        //reloading wepaon
        reloadWeaponParamID = Animator.StringToHash(reloadWeaponParam);
        reloadWeaponSpeedMultiplierParamID = Animator.StringToHash(reloadWeaponSpeedMultiplierParam);

        //throwing grenade
        throwGrenadeParamID = Animator.StringToHash(throwGrenadeParam);
        throwGrenadeSpeedMultiplierParamID = Animator.StringToHash(throwGrenadeSpeedMultiplierParam);

        //flinch
        flinchParamID = Animator.StringToHash(flinchParam);
        flinchIDParamID = Animator.StringToHash(flinchIDParam);

        //stagger
        staggerParamID = Animator.StringToHash(staggerParam);
        staggerIDParamID = Animator.StringToHash(staggerIDParam);
    }

    public override void UpdateComponent()
    {
        if (Input.GetKeyDown(KeyCode.B))
        {
            Stagger();
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
        animator.SetInteger(itemInHandParamID, newItemInHandID);       
    }

    public void ChangeWeaponInteractionState(int weaponinteractionState)
    {
        animator.SetInteger(weaponInteractionStateParamID, weaponinteractionState);
    }

    public void AdjustPullOutAnimationSpeedAndOffset(float animationDuration, float animationOffset)
    {
        int weaponAnimationID = animator.GetInteger(itemInHandParamID);

        if (weaponAnimationID == 1)
        {
            animator.SetFloat(pullOutWeaponSpeedMultiplierParamID, pullOutRifleAnimationLength / animationDuration);

        }
        else if (weaponAnimationID == 2)
        {
            animator.SetFloat(pullOutWeaponSpeedMultiplierParamID, pullOutPistolAnimationLength / animationDuration);
        }

        animator.SetFloat(pullOutWeaponStartOffsetID, animationOffset);
    }


    public void AdjustHideAnimationSpeedAndOffset(float animationDuration, float animationOffset)
    {
        int weaponAnimationID = animator.GetInteger(itemInHandParamID);

        if (weaponAnimationID == 1)
        {
            animator.SetFloat(hideWeaponSpeedMultiplierParamID, hideRifleAnimationLength / animationDuration);
        }
        else if (weaponAnimationID == 2)
        {
            animator.SetFloat(hideWeaponSpeedMultiplierParamID, hidePistolAnimationLength / animationDuration);
        }

        animator.SetFloat(hideWeaponStartOffsetID, animationOffset);
    }

    public void StartReloadingWeapon(float animationDuration)
    {

        //modify speed based on stance
        int stanceID = animator.GetInteger(stanceParamID);

        if(stanceID == 2)
        {
            animator.SetFloat(reloadWeaponSpeedMultiplierParamID, reloadRifleCrouchingAnimationLength / animationDuration);
        }
        else
        {
            animator.SetFloat(reloadWeaponSpeedMultiplierParamID, reloadRifleStandingAnimationLength / animationDuration);
        }

        //set bool true
        animator.SetBool(reloadWeaponParamID, true);
    }

    public void AbortReloadingWeapon()
    {
        animator.SetBool(reloadWeaponParamID, false);
    }

    public void StartThrowingGrenade(float animationDuration)
    {
        Debug.Log("start throwing anim");
        animator.SetBool(throwGrenadeParamID, true);

        animator.SetFloat(throwGrenadeSpeedMultiplierParamID, throwingGrenadeAnimationLength / animationDuration);
    }

    public void AbortThrowingGrenade()
    {
        Debug.Log("Stop throwing anim");
        animator.SetBool(throwGrenadeParamID, false);
    }

    public void Flinch()
    {
        //select a random flinch Animation
        animator.SetInteger(flinchIDParamID, Random.Range(0, numberOdFlinchAnimationVariations));

        animator.SetTrigger(flinchParamID);
    }

    public void Stagger()
    {
        animator.SetInteger(staggerIDParamID, Random.Range(0, numberOdStaggerAnimationVariations));
        animator.SetTrigger(staggerParamID);
    }
}
