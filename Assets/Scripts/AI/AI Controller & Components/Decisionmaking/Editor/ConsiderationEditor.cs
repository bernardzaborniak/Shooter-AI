using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


[CustomEditor(typeof(Consideration))]
public class ConsiderationEditor : Editor
{
    Texture2D curveVisualisationTexture;
    Consideration targetConsideration;

    private void Awake()
    {
        SetUp();
    }

    void SetUp()
    {
        targetConsideration = (Consideration)target;
        curveVisualisationTexture = new  Texture2D(targetConsideration.horizontalVisualisationTextureResolution, targetConsideration.verticalVisualisationTextureResolution, TextureFormat.ARGB32, false);
    }

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector(); //this methiod draws the deault editor, we can add more custom editors later
        
        targetConsideration.considerationCurve.GetCurveVisualisationTexture(ref curveVisualisationTexture);
        GUILayout.Box(curveVisualisationTexture);

        
        if (GUILayout.Button("Refresh Curve Visualisation"))
        {
            SetUp();
        }
    }
}