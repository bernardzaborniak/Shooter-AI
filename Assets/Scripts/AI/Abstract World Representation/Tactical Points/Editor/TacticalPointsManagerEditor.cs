﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(TacticalPointsManager))]
public class TacticalPointsManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector(); //this methiod draws the deault editor, we can add more custom editors later

        TacticalPointsManager myTacticalPointsManager = (TacticalPointsManager)target;

        if (GUILayout.Button("Generate All Points"))
        {
            myTacticalPointsManager.GenerateAll();
        }

        if (GUILayout.Button("Bake All Cover Ratings"))
        {
            myTacticalPointsManager.BakeAllCoverRatings();
        }

        if (GUILayout.Button("Generate Points & Bake all Ratings "))
        {
            myTacticalPointsManager.GenerateAll();
            myTacticalPointsManager.BakeAllCoverRatings();
        }
    }     
}