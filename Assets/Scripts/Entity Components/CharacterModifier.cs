using UnityEngine;

public enum ModifiedValueType
{
    MovementSpeedModifier
}

public enum ModiferType
{
    FixedDuration,
    DeactivateManually
}

[System.Serializable]
public class CharacterModifier
{
    public string name;
    public ModiferType type;
    //public ModifiedValueType valueType;

    public float modifierDuration; //expand this somewhere later to also allow random values
    float nextDeactivateModifierTime;

    public virtual void Activate()
    {
        if(type == ModiferType.FixedDuration)
        {
            nextDeactivateModifierTime = Time.time + modifierDuration;
        }
    }

    public virtual bool ShouldModifierBeDeactivated() //think of a different name for this
    {
        if(type == ModiferType.FixedDuration)
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

    public float walkingSpeedMod;
    public float sprintingSpeedMod;


}