using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;

[System.Serializable]
public class CharacterModifierCreator
{
    //is a blueprint, which creates new modifiers
    public enum ModiferDurationType
    {
        DeactivateAfterFixedDelay,
        DeactivateAfterDelayRandomBetween2,
        DeactivateManually,
    }

    [Header("Character Modifier Base Class")]
    public string name;
    public ModiferDurationType modifierDurationType = ModiferDurationType.DeactivateManually;

    [ConditionalEnumHide("modifierDurationType", 0)]
    public float modifierDuration; 

    [ConditionalEnumHide("modifierDurationType", 1)]
    public float modifierMinDuration;

    [ConditionalEnumHide("modifierDurationType", 1)]
    public float modifierMaxDuration;

    public enum ModifierType
    {
        CharacterMovementSpeedModifier,
        CharacterPreventionModifier
    }
    [Space(10)]
    public ModifierType modifierType;


    // only for MovementSpeedModifier
    [ConditionalEnumHide("modifierType", 0)]
    public float walkingSpeedMod;
    [ConditionalEnumHide("modifierType", 0)]
    public float sprintingSpeedMod;

    [ConditionalEnumHide("modifierType", 1)]
    public ActiveCharacterPreventionModifier.CharacterPreventionType characterPreventionType;

    //float nextDeactivateModifierTime;

    //[Space(5)]
    //public float currentModifierDuration;

    //int hashCode;
    public ActiveCharacterModifier CreateAndActivateNewModifier()
    {
        float currentModifierDuration = -1;
        if (modifierDurationType == ModiferDurationType.DeactivateAfterFixedDelay)
        {
            currentModifierDuration = modifierDuration;
        }
        else if (modifierDurationType == ModiferDurationType.DeactivateAfterDelayRandomBetween2)
        {

            currentModifierDuration = Random.Range(modifierMinDuration, modifierMaxDuration);
        }

        if(modifierType == ModifierType.CharacterMovementSpeedModifier)
        {
            return new ActiveCharacterMovementSpeedModifier(this, currentModifierDuration);

        }
        else if(modifierType == ModifierType.CharacterPreventionModifier)
        {
            return new ActiveCharacterPreventionModifier(this, currentModifierDuration);

        }
        return null;
    }


    /*public virtual void Activate()
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

    }*/


    /*public virtual bool HasModifierTimeRunOut() //think of a different name for this
    {
        if(type != ModiferType.DeactivateManually)
        {
            return (Time.time > nextDeactivateModifierTime);
        }
        else
        {
            return false;
        }
        
    }*/

    /*public void SetHashCode()
    {
        hashCode = base.GetHashCode();
    }

    public override int GetHashCode()
    {
        return hashCode;
    }*/
}

public class ActiveCharacterModifier //: IEqualityComparer<ActiveCharacterModifier>
{
    public CharacterModifierCreator creator;
    public CharacterModifierCreator.ModiferDurationType modifierDurationType;

    public float currentModifierDuration;
    float nextDeactivateModifierTime;

    int hashCode;


    public  ActiveCharacterModifier(CharacterModifierCreator creator, float currentModifierDuration)
    {
        this.creator = creator;
        modifierDurationType = creator.modifierDurationType;
        this.currentModifierDuration = currentModifierDuration;
        nextDeactivateModifierTime = Time.time + currentModifierDuration;

        hashCode = creator.GetHashCode();
    }

    public virtual bool HasModifierTimeRunOut() 
    {
        if (modifierDurationType != CharacterModifierCreator.ModiferDurationType.DeactivateManually)
        {
            return (Time.time > nextDeactivateModifierTime);
        }
        else
        {
            return false;
        }

    }
    public override int GetHashCode()
    {
        Debug.Log("return active override code for getHashCode");
        //return obj.creator.GetHashCode();
        return hashCode;
    }

    public override bool Equals(object obj)
    {
        Debug.Log("override equals");

        if (obj is ActiveCharacterModifier)
        {
            if((obj as ActiveCharacterModifier).creator == creator)
            {
                return true;
            }
        }
        return false;
    }

    /*public virtual int GetHashCode(ActiveCharacterModifier obj) 
    {
        Debug.Log("return active override code for getHashCode");
        //return obj.creator.GetHashCode();
        return obj.hashCode;
    }
    public virtual bool Equals(ActiveCharacterModifier obj1, ActiveCharacterModifier obj2) 
    {
        Debug.Log("equals override");

        if (obj1.creator == obj2.creator)
        {
            return true;
        }
        else
        {
            return false;
        }
    }*/
}

public class ActiveCharacterMovementSpeedModifier : ActiveCharacterModifier//, IEqualityComparer<ActiveCharacterModifier>
{
    //[Header("Movement Speed Modifier")]
    public float walkingSpeedMod;
    public float sprintingSpeedMod;

    public ActiveCharacterMovementSpeedModifier(CharacterModifierCreator creator, float currentModifierDuration): base(creator, currentModifierDuration)
    {
        walkingSpeedMod = creator.walkingSpeedMod;
        sprintingSpeedMod = creator.sprintingSpeedMod;
    }

    
    
    /*public override int GetHashCode(ActiveCharacterModifier obj)
    {
        Debug.Log("return active override code for getHashCode");
        return obj.creator.GetHashCode();
    }
    public override bool Equals(ActiveCharacterModifier obj1, ActiveCharacterModifier obj2)
    {
        Debug.Log("equals override");

        if (obj1.creator == obj2.creator)
        {
            return true;
        }
        else
        {
            return false;
        }
    }*/

}

public class ActiveCharacterPreventionModifier : ActiveCharacterModifier//, IEqualityComparer<ActiveCharacterModifier>
{
    public enum CharacterPreventionType
    {
        Stunned,
        JumpingToTraverseOffMeshLink //similar to stunned but allows look at?
    }

    public CharacterPreventionType characterPreventionType;

    public ActiveCharacterPreventionModifier(CharacterModifierCreator creator, float currentModifierDuration) : base(creator, currentModifierDuration)
    {
        characterPreventionType = creator.characterPreventionType;
    }

   /* public override int GetHashCode(ActiveCharacterModifier obj)
    {
        Debug.Log("return active override code for getHashCode");
        return obj.creator.GetHashCode();
    }
    public override bool Equals(ActiveCharacterModifier obj1, ActiveCharacterModifier obj2)
    {
        Debug.Log("equals override");

        if (obj1.creator == obj2.creator)
        {
            return true;
        }
        else
        {
            return false;
        }
    }*/
    
}

