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

    [Header("Adjusting Animation Speed")]

    public float turnAnimationRealSpeed;

    public float movementForwardSideDampTime;



    public override void SetUpComponent(GameEntity entity)
    {
        base.SetUpComponent(entity);

        // Convert the strings into hashes to improve performance
        forwardVelocityParamID = Animator.StringToHash(forwardVelocityParam);
        sidewaysVelocityParamID = Animator.StringToHash(sidewaysVelocityParam);
        angularVelocityParamID = Animator.StringToHash(angularVelocityParam);
    }

    public void UpdateAnimation(float currentForwardVelocity, float currentSidewaysVelocity, float angularVelocity)
    {
        animator.SetFloat(forwardVelocityParamID, currentForwardVelocity, movementForwardSideDampTime, Time.deltaTime);
        animator.SetFloat(sidewaysVelocityParamID, currentSidewaysVelocity, movementForwardSideDampTime, Time.deltaTime);


        float agentAngularSpeedNormalized = 0;
        if (angularVelocity > 0)
        {
            agentAngularSpeedNormalized = Utility.Remap(angularVelocity, 0f, turnAnimationRealSpeed, 0, 1);
            if (agentAngularSpeedNormalized < 0)
            {
                agentAngularSpeedNormalized = 0;
            }
        }
        else if (angularVelocity < 0)
        {
            agentAngularSpeedNormalized = Utility.Remap(angularVelocity, 0f, -turnAnimationRealSpeed, 0, -1);
            if (agentAngularSpeedNormalized > 0)
            {
                agentAngularSpeedNormalized = 0;
            }
        }


        //TODO this is only disabled for current test
        //animator.SetFloat(angularVelocityParamID, agentAngularSpeedNormalized);



    }
}
