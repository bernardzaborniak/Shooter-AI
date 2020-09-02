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
    //public float runAnimationRealSpeed;
    //public float walkAnimationRealSpeed;
    public float turnAnimationRealSpeed;
    //[Tooltip("because this speed gets capped by the movement controller, this acceleration heps smoothing it out, the best if its the same as turning acceleration on Movement COntroller")]
    //public float turnSpeedAcceleration;
    //float angularVelocityLastFrame;


    public override void SetUpComponent(GameEntity entity)
    {
        base.SetUpComponent(entity);

        // Convert the strings into hashes to improve performance
        forwardVelocityParamID = Animator.StringToHash(forwardVelocityParam);
        angularVelocityParamID = Animator.StringToHash(angularVelocityParam);

        //angularVelocityLastFrame = 0;
    }

    //public void UpdateAnimation(float currentVelocity, float runningVelocity, float walkingVelocity, float angularVelocity)
    public void UpdateAnimation(float currentVelocity, float angularVelocity)
    {
        //float animationForwardParam; // 0 is standing, 0.5 is walking, 1 is running


        // We do 2 different remaps, because the distance between walingVelocity and runnningVelocity could be bigger than between standing and walking.
        /*if (currentVelocity > walkingVelocity)
        {
            animationForwardParam = Utility.Remap(currentVelocity, walkingVelocity, runningVelocity, 0.5f, 1);
        }
        else
        {
            animationForwardParam = Utility.Remap(currentVelocity, 0, walkingVelocity, 0, 0.5f);
        }*/

        // Adjust Playback Speed for animations looking realistically for the given speed
        
        
        
       // float agentSpeedNormalized = Utility.Remap(velocity, 0f, runAnimationRealSpeed, 0, 1);
        //if (animationForwardParam < 0) animationForwardParam = 0;

        //Debug.Log("agentSpeedNormalized: " + agentSpeedNormalized);

        //Debug.Log("currentVelocity: " + currentVelocity);
        //animator.SetFloat(forwardVelocityParamID, animationForwardParam);
        //animator.SetFloat(forwardVelocityParamID, 0.5f);
        
        
        animator.SetFloat(forwardVelocityParamID, currentVelocity);
        //animator.SetFloat(forwardVelocityParamID, 0);

        //Debug.Log("angularVelocityRaw: " + angularVelocity);

       // float angularVelocityDelta = Mathf.Clamp(angularVelocity - angularVelocityLastFrame, -turnSpeedAcceleration * Time.deltaTime, turnSpeedAcceleration * Time.deltaTime) ;
        //angularVelocity = angularVelocityLastFrame + angularVelocityDelta;

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

        //angularVelocityLastFrame = angularVelocity;
        //animator.SetFloat(angularVelocityParamID, agentAngularSpeedNormalized, 0.1f, Time.deltaTime);
        animator.SetFloat(angularVelocityParamID, agentAngularSpeedNormalized);
        //Debug.Log("angularVelocity: " + angularVelocity);
        //animator.SetFloat(angularVelocityParamID, 0.5f);
    }
}
