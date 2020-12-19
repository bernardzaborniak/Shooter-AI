using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


[CustomEditor(typeof(Consideration), true)]
public class ConsiderationEditor : Editor
{
    Texture2D curveVisualisationTexture;
    Consideration targetConsideration;

    private void Awake()
    {
       // SetUp();
        targetConsideration = (Consideration)target;

    }

    /*void SetUp()
    {
        targetConsideration = (Consideration)target;
        curveVisualisationTexture = new Texture2D(targetConsideration.horizontalVisualisationTextureResolution, targetConsideration.verticalVisualisationTextureResolution, TextureFormat.ARGB32, false);
    }*/

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector(); //this methiod draws the deault editor, we can add more custom editors later

        /*

        Keyframe[] keys = new Keyframe[] {new Keyframe(0,0,0,0), new Keyframe(0.5f, 0.5f, 0, 0), new Keyframe(0.6f, 0.7f, 0, 0), new Keyframe(0.7f, 0.9f, 0, 0) };


        //targetConsideration.considerationCurve.GetCurveVisualisationTexture(ref curveVisualisationTexture);
        //GUILayout.Box(curveVisualisationTexture);
        //EditorGUI.DrawPreviewTexture(new Rect(0, 300, 500, 500), curveVisualisationTexture);

        EditorGUILayout.Space(5);
        EditorGUILayout.LabelField("Curve Visualisation", EditorStyles.boldLabel);
        // Saving previous GUI enabled value
        var previousGUIState = GUI.enabled;
        // Disabling edit for property
        GUI.enabled = false;
        // Drawing Property
        //EditorGUILayout.CurveField("Curve Visualisation", new AnimationCurve(keys), GUILayout.Width = );
        EditorGUILayout.CurveField(new AnimationCurve(keys), GUILayout.Height(400), GUILayout.Width(400));

        // Setting old GUI enabled value
        GUI.enabled = previousGUIState;

        /*if (GUILayout.Button("Refresh Curve Visualisation"))
        {
            SetUp();
        }*/
    }
}