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

        EditorGUILayout.LabelField("TestLabel", "second label value");
        
        if(GUILayout.Button("Generate Points"))
        {
            ((TacticalPointsGeneratorBox)target).Generate();
        }

        if (GUILayout.Button("Delete All Generated Points "))
        {
            ((TacticalPointsGeneratorBox)target).DeleteAllGeneratedPoints();
        }
    }
}
