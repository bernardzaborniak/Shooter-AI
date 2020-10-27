using UnityEngine;
using UnityEditor;
using System;
using System.Collections;


public enum ModiferType
{
    FixedDuration,
    DeactivateManually
}


[System.Serializable]
public class CharacterModifier
{
    [Header("Character Modifier Base Class")]
    public string name;
    public ModiferType type = ModiferType.DeactivateManually;

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
    [Header("Movement Speed Modifier")]
    public float walkingSpeedMod;
    public float sprintingSpeedMod;


}

/*[CustomPropertyDrawer(typeof(CharacterModifier))]
public class CharacterModifierDrawer : PropertyDrawer
{
    public override VisualElement CreatePropertyGUI(SerializedProperty property)
    {
        // Create property container element.
        var container = new VisualElement();

        // Create property fields.
        var nameField = new PropertyField(property.FindPropertyRelative("name"), "Name");
        var typeField = new PropertyField(property.FindPropertyRelative("type"));
        var modifierDurationField = new PropertyField(property.FindPropertyRelative("modifierDuration"));

        // Add fields to the container.
        container.Add(nameField);
        container.Add(typeField);
         container.Add(modifierDurationField);

        return container;
    }
}*/

/*[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property |
    AttributeTargets.Class | AttributeTargets.Struct, Inherited = true)]
public class ConditionalHideAttribute : PropertyAttribute
{
    //The name of the bool field that will be in control
    public string ConditionalSourceField = "";
    //TRUE = Hide in inspector / FALSE = Disable in inspector 
    public bool HideInInspector = false;

    public ConditionalHideAttribute(string conditionalSourceField)
    {
        this.ConditionalSourceField = conditionalSourceField;
        this.HideInInspector = false;
    }

    public ConditionalHideAttribute(string conditionalSourceField, bool hideInInspector)
    {
        this.ConditionalSourceField = conditionalSourceField;
        this.HideInInspector = hideInInspector;
    }
}

[CustomPropertyDrawer(typeof(ConditionalHideAttribute))]
public class ConditionalHidePropertyDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        ConditionalHideAttribute condHAtt = (ConditionalHideAttribute)attribute;
        bool enabled = GetConditionalHideAttributeResult(condHAtt, property);

        bool wasEnabled = GUI.enabled;
        GUI.enabled = enabled;
        if (!condHAtt.HideInInspector || enabled)
        {
            EditorGUI.PropertyField(position, property, label, true);
        }

        GUI.enabled = wasEnabled;
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        ConditionalHideAttribute condHAtt = (ConditionalHideAttribute)attribute;
        bool enabled = GetConditionalHideAttributeResult(condHAtt, property);

        if (!condHAtt.HideInInspector || enabled)
        {
            return EditorGUI.GetPropertyHeight(property, label);
        }
        else
        {
            return -EditorGUIUtility.standardVerticalSpacing;
        }
    }

    private bool GetConditionalHideAttributeResult(ConditionalHideAttribute condHAtt, SerializedProperty property)
    {
        bool enabled = true;
        string propertyPath = property.propertyPath; //returns the property path of the property we want to apply the attribute to
        string conditionPath = propertyPath.Replace(property.name, condHAtt.ConditionalSourceField); //changes the path to the conditionalsource property path
        SerializedProperty sourcePropertyValue = property.serializedObject.FindProperty(conditionPath);

        if (sourcePropertyValue != null)
        {
            enabled = sourcePropertyValue.boolValue;
        }
        else
        {
            Debug.LogWarning("Attempting to use a ConditionalHideAttribute but no matching SourcePropertyValue found in object: " + condHAtt.ConditionalSourceField);
        }

        return enabled;
    }
}*/