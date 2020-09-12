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
    public string angularVelocityParam;
    int angularVelocityParamID;

    public string combatStanceParam;
    public int combatStanceParamID;

    [Header("Adjusting Animation Speed")]

    [Tooltip("Used For the turn override layer when aiming with a rifle")]
    public float turnValueNormalizationRealSpeed;
    //[Tooltip("If angular velocity reaches this value, the animation also reaches its maximum value")]
    //public float idleTurnAngularSpeedAnimationTreshold;



    public override void SetUpComponent(GameEntity entity)
    {
        base.SetUpComponent(entity);

        // Convert the strings into hashes to improve performance
        forwardVelocityParamID = Animator.StringToHash(forwardVelocityParam);
        sidewaysVelocityParamID = Animator.StringToHash(sidewaysVelocityParam);
        angularVelocityParamID = Animator.StringToHash(angularVelocityParam);
        normalizedAngularVelocityParamID = Animator.StringToHash(normalizedAngularVelocityParam);

        combatStanceParamID = Animator.StringToHash(combatStanceParam);
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

        //float turn2 = Utility.Remap(agentAngularSpeedNormalized, 0f, 1f, -1f, 1f);
        animator.SetFloat(angularVelocityParamID, angularVelocity);

    }

    public void ChangeFromIdleToCombatStance()
    {
        animator.SetBool(combatStanceParamID, true);
        animator.SetLayerWeight(1, 1f);
    }

    public void ChangeFromCombatToIdleStance()
    {
        animator.SetBool(combatStanceParamID, false);
        animator.SetLayerWeight(1, 0f);
    }
}
