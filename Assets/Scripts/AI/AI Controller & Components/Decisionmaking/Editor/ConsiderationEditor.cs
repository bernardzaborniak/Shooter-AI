using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


[CustomEditor(typeof(Consideration), true)]
public class ConsiderationEditor : Editor
{
    Texture2D curveVisualisationTexture;
    Consideration targetConsideration;

    bool foldoutExampleValues = false;


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
        serializedObject.Update();

        //DrawDefaultInspector(); //this methiod draws the deault editor, we can add more custom editors later

        #region Curve Example Values

       // SerializedProperty nameProp = serializedObject.FindProperty("considerationName");
        SerializedProperty nameProp = serializedObject.FindProperty("m_Name");
        SerializedProperty descriptionProp = serializedObject.FindProperty("description");

        GUIStyle labelStyle = new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleCenter, fontSize = 14, fontStyle = FontStyle.Bold };
        // ----- Disabling & Enabling GUI Elements as a workaround to make the Field read only -----
        //var previousGUIState = GUI.enabled;
        //GUI.enabled = false;
        EditorGUILayout.LabelField(targetConsideration.name, labelStyle);
        EditorGUILayout.Space(10);
        //GUI.enabled = previousGUIState;

        EditorGUILayout.LabelField("Description");
        descriptionProp.stringValue = EditorGUILayout.TextArea(descriptionProp.stringValue, GUILayout.Height(100));
        //EditorGUILayout.PropertyField(descriptionProp, new GUIContent(""), GUILayout.Width(Screen.width),GUILayout.Height(100));


        EditorGUILayout.Space(5);
        foldoutExampleValues = EditorGUILayout.BeginFoldoutHeaderGroup(foldoutExampleValues, "Show Example Curve Values", EditorStyles.foldout);

        if (foldoutExampleValues)
        {
            SerializedProperty exampleInput1Prop = serializedObject.FindProperty("exampleInput1");
            SerializedProperty exampleInput2Prop = serializedObject.FindProperty("exampleInput2");
            SerializedProperty exampleInput3Prop = serializedObject.FindProperty("exampleInput3");
            SerializedProperty exampleInput4Prop = serializedObject.FindProperty("exampleInput4");

            //EditorGUILayout.Begin
            //EditorGUILayout.PropertyField(exampleInput1Prop, GUILayout.Width(20));
            EditorGUILayout.PropertyField(exampleInput1Prop);
            EditorGUILayout.PropertyField(exampleInput2Prop);
            EditorGUILayout.PropertyField(exampleInput3Prop);
            EditorGUILayout.PropertyField(exampleInput4Prop);
        }

        #endregion

        serializedObject.ApplyModifiedProperties();

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