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
    public string normalizedAngularVelocityParam;
    int normalizedAngularVelocityParamID;

    public string stanceParam;
    int stanceParamID;

    public string itemInHandParam;
    int itemInHandParamID;

    [Header("Adjusting Animation Speed")]

    [Tooltip("Used For the turn override layer when aiming with a rifle")]
    public float turnValueNormalizationRealSpeed;
    [Tooltip("Turn animation is only played below this velocity")]
    public float turnAnimationVelocityThreshold;


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
        normalizedAngularVelocityParamID = Animator.StringToHash(normalizedAngularVelocityParam);

        stanceParamID = Animator.StringToHash(stanceParam);
        itemInHandParamID = Animator.StringToHash(itemInHandParam);
    }

    public void UpdateAnimation(float currentForwardVelocity, float currentSidewaysVelocity, float angularVelocity)
    {
       
        animator.SetFloat(sidewaysVelocityParamID, currentSidewaysVelocity);
        animator.SetFloat(forwardVelocityParamID, currentForwardVelocity);


        // Normalize angular Speed between 0 and 1 because that is the value of the 1D Blendtree used by the animator for turning, 0.5 means no turn, 0 is full left turn, 1 is full right turn

        float agentAngularSpeedNormalized = 0;
        if (angularVelocity > 0)
        {
            agentAngularSpeedNormalized = Utility.Remap(angularVelocity, 0f, turnValueNormalizationRealSpeed, 0.5f, 1f);
            if (agentAngularSpeedNormalized > 1f)
            {
                agentAngularSpeedNormalized = 1f;
            }
        }
        else if (angularVelocity < 0)
        {
            agentAngularSpeedNormalized = Utility.Remap(angularVelocity, 0f, -turnValueNormalizationRealSpeed, 0.5f, 0f);
            if (agentAngularSpeedNormalized < 0f)
            {
                agentAngularSpeedNormalized = 0f;
            }
        }
        else
        {
            agentAngularSpeedNormalized = 0.5f;
        }

        animator.SetFloat(normalizedAngularVelocityParamID, agentAngularSpeedNormalized);

        //animator.SetFloat(angularVelocityParamID, angularVelocity);

        float speed = new Vector3(currentForwardVelocity, currentSidewaysVelocity).magnitude;
        float weight = 0;

        if (speed < turnAnimationVelocityThreshold)
        {
            weight = Utility.Remap(speed, 0, turnAnimationVelocityThreshold, 1, 0);
        }
        animator.SetLayerWeight(1, weight);

       
    }


    public void ChangeToIdleStance()
    {
        //animator.SetBool(combatStanceParamID, false);
        animator.SetInteger(stanceParamID, 0);
        //animator.SetLayerWeight(1, 0f);
    }

    public void ChangeToCombatStance()
    {
        //animator.SetBool(combatStanceParamID, true);
        animator.SetInteger(stanceParamID, 1);
        //animator.SetLayerWeight(1, 1f);
    }

    public void ChangeToCrouchedStance()
    {
        //animator.SetBool(combatStanceParamID, false);
        animator.SetInteger(stanceParamID, 2);
        //animator.SetLayerWeight(1, 0f);
    }

    public void ChangeItemInHand(int newItemInHandID)
    {
        Debug.Log("set item in hand: " + newItemInHandID);
        animator.SetInteger(itemInHandParamID, newItemInHandID);
        
    }
}
