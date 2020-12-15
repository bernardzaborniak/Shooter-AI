using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Consideration))]
public class ConsiderationEditor : Editor
{
    int visualisationTextureResolution = 256;
    Texture2D curveVisualisationTexture;
    Consideration targetConsideration;

    private void Awake()
    {
        targetConsideration = (Consideration)target;
        curveVisualisationTexture = new  Texture2D(visualisationTextureResolution, visualisationTextureResolution, TextureFormat.ARGB32, false);
    }

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector(); //this methiod draws the deault editor, we can add more custom editors later
        
        GUILayout.Label("Curve Visualisation");
        targetConsideration.considerationCurve.GetCurveVisualisationTexture(ref curveVisualisationTexture);
        GUILayout.Box(curveVisualisationTexture);

        /*int textureSize = 256;
        Texture2D tex = new Texture2D(textureSize, textureSize, TextureFormat.ARGB32, false);

        for (int y = 0; y < textureSize; y++)
        {
            for (int x = 0; x < textureSize; x++)
            {
                if (y % 5 == 0)
                {
                    tex.SetPixel(x, y, Color.black);

                }
                else
                {
                    tex.SetPixel(x, y, Color.white);
                }
            }
        }

        tex.Apply();*/

        //GUILayout.Label(tex);

        //GUILayout.Box(tex, GUILayout.Width(400), GUILayout.Height(400));

        //GUILayout.Box("testString");



        /*if (GUILayout.Button("Generate All Points"))
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
        }*/
    }
}
