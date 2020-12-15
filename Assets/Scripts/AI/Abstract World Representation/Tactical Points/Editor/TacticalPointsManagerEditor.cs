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


        if (GUILayout.Button("Generate All Points"))
        {
            ((TacticalPointsManager)target).GenerateAll();
        }

        if (GUILayout.Button("Bake All Cover Ratings"))
        {
            ((TacticalPointsManager)target).ResetAllPointRotations();
            ((TacticalPointsManager)target).BakeAllCoverRatings();
            
        }

        if (GUILayout.Button("Update Point Ratings "))
        {
            ((TacticalPointsManager)target).UpdatePointRatings();
        }

        if (GUILayout.Button("Reset All Point Rotations "))
        {
            ((TacticalPointsManager)target).ResetAllPointRotations();
        }
    }     
}
