using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(TacticalPointsSceneInfo))]
public class TacticalPointsSceneInfoEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector(); //this methiod draws the deault editor, we can add more custom editors later
    }
}
