﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

/*
[System.Serializable]
public class ConsiderationInputParams
{
    public enum ConsiderationInputType
    {
        Float,
        Int,
        Bool
    }

    public ConsiderationInputType considerationInputType;

}

[CustomPropertyDrawer((typeof(ConsiderationInputParams)))]
public class ConsiderationInputParamsUIE : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        // Using BeginProperty / EndProperty on the parent property means that
        // prefab override logic works on the entire property.
        EditorGUI.BeginProperty(position, label, property);

        // Draw label
        position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

        // Don't make child fields be indented
        var indent = EditorGUI.indentLevel;
        EditorGUI.indentLevel = 0;

        // Calculate rects
        var considerationInputTypeRect = new Rect(position.x, position.y, 70, position.height);
        //var valueDeriv1Rect = new Rect(position.x + 35, position.y, 50, position.height);
        //var valueDeriv2Rect = new Rect(position.x + 90, position.y, position.width - 90, position.height);


        // Draw fields - pass GUIContent.none to each so they are drawn without labels
        EditorGUI.PropertyField(considerationInputTypeRect, property.FindPropertyRelative("considerationInputType"), GUIContent.none); ;
        //EditorGUI.PropertyField(valueBase1Rect, property.FindPropertyRelative("valueDeriv1"), GUIContent.none);
        //EditorGUI.PropertyField(valueBase1Rect, property.FindPropertyRelative("valueDeriv2"), GUIContent.none);


        // Set indent back to what it was
        EditorGUI.indentLevel = indent;

        EditorGUI.EndProperty();
    }
}*/





//[CreateAssetMenu(menuName = "AI/Consideration", fileName = "AI Decision Consideration")]
public class Consideration : ScriptableObject
{
    //public string considerationName;
    //[TextArea]
    public string description;

    // Example Values to make setting up the curves easier for more complex relationships like squared distances
    public float exampleInput1;
    public float exampleInput2;
    public float exampleInput3;
    public float exampleInput4;

    public float exampleOutput1;
    public float exampleOutput2;
    public float exampleOutput3;
    public float exampleOutput4;

    //Inputs needed:

    //character data
    //mine & targets

    //Game Engine Data
    //distance between 
    //elapsed time

    //other systems in the world

    //each of this is predefined -> put them in a dropdown list

    /* public enum ConsiderationInputType
     {
        // Float,
        // Int,
        // Bool
     }

     public ConsiderationInputType considerationInputType;*/

    public CustomCurve considerationCurve;


    private void OnValidate()
    {
        //considerationName = name;

        considerationCurve.UpdateCurveVisualisationKeyframes();
        UpdateExampleValues();
    }

    public void UpdateExampleValues()
    {
        //have 4 example values in 2 rows, upper row is non normalized input
        //lower row is what comes out of the curve after initial normalization etc...
    }

    /* public virtual void SetUpConsideration(AIController aiController)
     {

     }*/

    public virtual float GetConsiderationRating(AIController aiController)
    {
        return 0;
    }



}
