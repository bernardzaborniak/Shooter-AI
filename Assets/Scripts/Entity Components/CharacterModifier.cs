using UnityEngine;
using System.Collections;
using System.Collections.Generic;


[System.Serializable]
public class CharacterModifier
{
    public enum ModiferType
    {
        DeactivateAfterFixedDelay,
        DeactivateAfterDelayRandomBetween2,
        DeactivateManually,
        DeactivateManuallyWithDelay
    }

    [Header("Character Modifier Base Class")]
    public string name;
    public ModiferType type = ModiferType.DeactivateManually;

    [ConditionalEnumHide("type", 0)]
    public float modifierDuration; 

    [ConditionalEnumHide("type", 1)]
    public float modifierMinDuration;

    [ConditionalEnumHide("type", 1)]
    public float modifierMaxDuration;

    [ConditionalEnumHide("type", 3)]
    [Tooltip("the modifier is deactivated after this small delay after deactivation")]
    public float delayAfterManualDeactivation;
    bool deactivateDelayStarted;

    float nextDeactivateModifierTime;
    [Space(5)]
    public float currentModifierDuration;


    public virtual void Activate()
    {
        if(type == ModiferType.DeactivateAfterFixedDelay)
        {
            currentModifierDuration = modifierDuration;    
        }
        else if(type == ModiferType.DeactivateAfterDelayRandomBetween2)
        {

            currentModifierDuration =  Random.Range(modifierMinDuration, modifierMaxDuration);
        }

        nextDeactivateModifierTime = Time.time + currentModifierDuration;


        if (type == ModiferType.DeactivateManuallyWithDelay)
        {
            deactivateDelayStarted = false;
        }
    }

    //Only for DeactivateManuallyWithDelay
    public virtual bool HasDeactivationDelayPassed()
    {
        if(type == ModiferType.DeactivateManuallyWithDelay)
        {
            if (!deactivateDelayStarted)
            {
                deactivateDelayStarted = true;
                nextDeactivateModifierTime = Time.time + delayAfterManualDeactivation;
                return false;
            }
            else
            {
                if(Time.time> nextDeactivateModifierTime)
                {
                    return true;
                }
            }  
        }
        return true;
    }

    public virtual bool HasModifierTimeRunOut() //think of a different name for this
    {
        if(type == ModiferType.DeactivateAfterFixedDelay || type == ModiferType.DeactivateAfterDelayRandomBetween2 || type == ModiferType.DeactivateManuallyWithDelay && deactivateDelayStarted)
        {
            return (Time.time > nextDeactivateModifierTime);
        }
        else
        {
            return false;
        }
        
    }
}


[System.Serializable]
public class MovementSpeedModifier : CharacterModifier
{
    [Header("Movement Speed Modifier")]
    public float walkingSpeedMod;
    public float sprintingSpeedMod;


}

[System.Serializable]
public class CharacterPreventionModifier : CharacterModifier
{
    public enum CharacterPreventionType
    {
        Stunned,
        JumpingToTraverseOffMeshLink //similar to stunned but allows look at?
    }

    [Header("Character Prevention Modifier")]
    public CharacterPreventionType characterPreventionType;


}

