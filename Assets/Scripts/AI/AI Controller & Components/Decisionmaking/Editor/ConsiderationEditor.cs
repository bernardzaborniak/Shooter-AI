using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


[CustomEditor(typeof(Consideration))]
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

        #region header & Description
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

        #endregion


        #region Input

        //TODO draw input params depending on the selected input paramter enum type?

        EditorGUILayout.Space(10);
        SerializedProperty considerationInputProp = serializedObject.FindProperty("considerationInput");
        EditorGUILayout.PropertyField(considerationInputProp);

        #endregion

        #region Curve

        SerializedProperty curveProp = serializedObject.FindProperty("considerationCurve");
        EditorGUILayout.PropertyField(curveProp);

        #endregion

        #region Curve Example Values

        EditorGUILayout.Space(5);
        foldoutExampleValues = EditorGUILayout.BeginFoldoutHeaderGroup(foldoutExampleValues, "Show Example Curve Values", EditorStyles.foldout);

        if (foldoutExampleValues)
        {
            EditorGUILayout.Space(5);

            EditorGUILayout.LabelField("Example Inputs (not normalized)");

           
            SerializedProperty exampleInput1Prop = serializedObject.FindProperty("exampleInput1");
            SerializedProperty exampleInput2Prop = serializedObject.FindProperty("exampleInput2");
            SerializedProperty exampleInput3Prop = serializedObject.FindProperty("exampleInput3");
            SerializedProperty exampleInput4Prop = serializedObject.FindProperty("exampleInput4");

            EditorGUILayout.BeginHorizontal();
            //EditorGUILayout.PropertyField(exampleInput1Prop, GUILayout.Width(20));
            exampleInput1Prop.floatValue = EditorGUILayout.FloatField(exampleInput1Prop.floatValue);
            exampleInput2Prop.floatValue = EditorGUILayout.FloatField(exampleInput2Prop.floatValue);
            exampleInput3Prop.floatValue = EditorGUILayout.FloatField(exampleInput3Prop.floatValue);
            exampleInput4Prop.floatValue = EditorGUILayout.FloatField(exampleInput4Prop.floatValue);
            EditorGUILayout.EndHorizontal();


            EditorGUILayout.LabelField("Example Outputs");

            SerializedProperty exampleOutput1Prop = serializedObject.FindProperty("exampleOutput1");
            SerializedProperty exampleOutput2Prop = serializedObject.FindProperty("exampleOutput2");
            SerializedProperty exampleOutput3Prop = serializedObject.FindProperty("exampleOutput3");
            SerializedProperty exampleOutput4Prop = serializedObject.FindProperty("exampleOutput4");

            //set the values
            exampleOutput1Prop.floatValue = targetConsideration.considerationCurve.GetRemappedValue(exampleInput1Prop.floatValue);
            exampleOutput2Prop.floatValue = targetConsideration.considerationCurve.GetRemappedValue(exampleInput2Prop.floatValue);
            exampleOutput3Prop.floatValue = targetConsideration.considerationCurve.GetRemappedValue(exampleInput3Prop.floatValue);
            exampleOutput4Prop.floatValue = targetConsideration.considerationCurve.GetRemappedValue(exampleInput4Prop.floatValue);


            EditorGUILayout.BeginHorizontal();
            exampleOutput1Prop.floatValue = EditorGUILayout.FloatField(exampleOutput1Prop.floatValue);
            exampleOutput2Prop.floatValue = EditorGUILayout.FloatField(exampleOutput2Prop.floatValue);
            exampleOutput3Prop.floatValue = EditorGUILayout.FloatField(exampleOutput3Prop.floatValue);
            exampleOutput4Prop.floatValue = EditorGUILayout.FloatField(exampleOutput4Prop.floatValue);
            EditorGUILayout.EndHorizontal();


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