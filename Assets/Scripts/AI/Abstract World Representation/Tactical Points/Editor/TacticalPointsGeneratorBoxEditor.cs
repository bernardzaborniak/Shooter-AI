using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;

[CustomEditor(typeof(TacticalPointsGeneratorBox))]
public class TacticalPointsGeneratorBoxEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector(); //this methiod draws the deault editor, we can add more custom editors later

        TacticalPointsGeneratorBox myTacticalPointsGeneratorBox = (TacticalPointsGeneratorBox)target;
        EditorGUILayout.LabelField("TestLabel", "second label value");
        
        if(GUILayout.Button("Generate Points"))
        {
            myTacticalPointsGeneratorBox.Generate();
        }

        if (GUILayout.Button("Delete All Generated Points "))
        {
            myTacticalPointsGeneratorBox.DeleteAllGeneratedPoints();
        }
    }
}
#endif
