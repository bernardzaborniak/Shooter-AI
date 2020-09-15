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
    public string pullOutRifleStateName;
    int pullOutRifleStateNameID;
    public string hideRifleStateName;
    int hideRifleStateNameID;

    public string pullOutPistolStateName;
    int pullOutPistolStateNameID;
    public string hidePistolStateName;
    int hidePistolStateNameID;


    /* ------Animation IDs--------
    
   -----Stance ID's---

    Idle,            0
    Combat,          1
    CrouchingCombat  2

     -----Item In Hand ID's---

    NoItem,            0
    Rifle,             1
    Pistol             2

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

        pullOutRifleStateNameID = Animator.StringToHash(pullOutRifleStateName);
        hideRifleStateNameID = Animator.StringToHash(hideRifleStateName);
        pullOutPistolStateNameID = Animator.StringToHash(pullOutPistolStateName);
        hidePistolStateNameID = Animator.StringToHash(hidePistolStateName);
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

    public void PullOutWeapon(int animationID)
    {
        if (animationID == 1)
        {
            animator.Play(pullOutRifleStateNameID, 2, 0);
        }
        else if (animationID == 2)
        {
            animator.Play(pullOutPistolStateNameID, 2, 0);
        }
    }

    public void HideWeapon(int animationID)
    {
       if(animationID == 1)
       {
            animator.Play(hideRifleStateNameID, 2, 0);
       }
       else if(animationID == 2)
       {
            animator.Play(hidePistolStateNameID, 2, 0);
        }
    }

}
