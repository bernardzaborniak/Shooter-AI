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
        DeactivateManually
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

    float nextDeactivateModifierTime;


    public virtual void Activate()
    {
        if(type == ModiferType.DeactivateAfterFixedDelay)
        {
            nextDeactivateModifierTime = Time.time + modifierDuration;
        }
        else if(type == ModiferType.DeactivateAfterDelayRandomBetween2)
        {
            nextDeactivateModifierTime = Time.time + Random.Range(modifierMinDuration, modifierMaxDuration);
        }
    }

    public virtual bool ShouldModifierBeDeactivated() //think of a different name for this
    {
        if(type == ModiferType.DeactivateAfterFixedDelay || type == ModiferType.DeactivateAfterDelayRandomBetween2)
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

