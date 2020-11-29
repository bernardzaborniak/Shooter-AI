using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(TacticalPointsGeneratorBox))]
public class TacticalPointsGeneratorBoxEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector(); //this methiod draws the deault editor, we can add more custom editors later

        TacticalPointsGeneratorBox myTacticalPointsGeneratorBox = (TacticalPointsGeneratorBox)target;
        //myTacticalPointsGeneratorBox.testInt = EditorGUILayout.IntField("testIntef", myTacticalPointsGeneratorBox.testInt);
        EditorGUILayout.LabelField("TestLabel", "second label value");
        
        if(GUILayout.Button("Generate Points"))
        {
            myTacticalPointsGeneratorBox.Generate();
        }


        if (GUILayout.Button("Bake Cover Ratings"))
        {
            myTacticalPointsGeneratorBox.BakeCoverRatings();
        }

        if (GUILayout.Button("Generate Points & Bake all Ratings "))
        {
            myTacticalPointsGeneratorBox.Generate();
            myTacticalPointsGeneratorBox.BakeCoverRatings();
        }
    }
}
