using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(TacticalPointsManager))]
public class TacticalPointsManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector(); //this methiod draws the deault editor, we can add more custom editors later


        if (GUILayout.Button("Generate Open Field Points"))
        {
            ((TacticalPointsManager)target).GenerateAll();
        }

        if (GUILayout.Button("Bake Cover Ratings"))
        {
            ((TacticalPointsManager)target).ResetAllPointRotations();
            ((TacticalPointsManager)target).BakeAllCoverRatings();
            
        }

        if (GUILayout.Button("Update Point Ratings "))
        {
            ((TacticalPointsManager)target).UpdatePointRatings();
        }

        if (GUILayout.Button("Reset Point Rotations "))
        {
            ((TacticalPointsManager)target).ResetAllPointRotations();
        }
    }     
}
