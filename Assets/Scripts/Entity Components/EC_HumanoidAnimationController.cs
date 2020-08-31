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
    public string angularVelocityParam;
    int angularVelocityParamID;

    [Header("Adjusting Animation Speed")]
    public float runAnimationRealSpeed;
    public float walkAnimationRealSpeed;
    public float turnAnimationRealSpeed;
    [Tooltip("because this speed gets capped by the movement controller, this acceleration heps smoothing it out, the best if its the same as turning acceleration on Movement COntroller")]
    public float turnSpeedAcceleration;
    float angularVelocityLastFrame;


    public override void SetUpComponent(GameEntity entity)
    {
        // Convert the strings into hashes to improve performance
        forwardVelocityParamID = Animator.StringToHash(forwardVelocityParam);
        angularVelocityParamID = Animator.StringToHash(angularVelocityParam);

        angularVelocityLastFrame = 0;
    }

    public void UpdateAnimation(float velocity, float angularVelocity)
    {
        //float agentSpeedNormalized = Utility.Remap(velocity, 0.2f, runAnimationRealSpeed, 0, 1);
        float agentSpeedNormalized = Utility.Remap(velocity, 0f, runAnimationRealSpeed, 0, 1);
        if (agentSpeedNormalized < 0) agentSpeedNormalized = 0;

        //Debug.Log("agentSpeedNormalized: " + agentSpeedNormalized);


        animator.SetFloat(forwardVelocityParamID, agentSpeedNormalized);

        //Debug.Log("angularVelocityRaw: " + angularVelocity);

        float angularVelocityDelta = Mathf.Clamp(angularVelocity - angularVelocityLastFrame, -turnSpeedAcceleration * Time.deltaTime, turnSpeedAcceleration * Time.deltaTime) ;
        angularVelocity = angularVelocityLastFrame + angularVelocityDelta;

        //Debug.Log("angularVelocitysmoothed: " + angularVelocity);

        float agentAngularSpeedNormalized = 0;
        if (angularVelocity > 0)
        {
            //agentAngularSpeedNormalized = Utility.Remap(angularVelocity, 0.2f, turnAnimationRealSpeed, 0, 1);
            agentAngularSpeedNormalized = Utility.Remap(angularVelocity, 0f, turnAnimationRealSpeed, 0, 1);
            if (agentAngularSpeedNormalized < 0)
            {
                agentAngularSpeedNormalized = 0;
            }
        }
        else if (angularVelocity < 0)
        {
            //agentAngularSpeedNormalized = Utility.Remap(angularVelocity, -0.2f, -turnAnimationRealSpeed, 0, -1);
            agentAngularSpeedNormalized = Utility.Remap(angularVelocity, 0f, -turnAnimationRealSpeed, 0, -1);
            if (agentAngularSpeedNormalized > 0)
            {
                agentAngularSpeedNormalized = 0;
            }
        }

        //Debug.Log("agentAngularSpeedNormalized: " + agentAngularSpeedNormalized);

        angularVelocityLastFrame = angularVelocity;
        animator.SetFloat(angularVelocityParamID, agentAngularSpeedNormalized, 0.1f, Time.deltaTime);
    }
}
