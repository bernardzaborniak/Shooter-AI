using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace BenitosAI
{

    [CustomEditor(typeof(Consideration))]
    public class ConsiderationEditor : Editor
    {
        Texture2D curveVisualisationTexture;
        Consideration targetConsideration;

        bool foldoutExampleValues = false;


        private void Awake()
        {
            targetConsideration = (Consideration)target;
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            GUIStyle labelStyle1 = new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleCenter, fontSize = 14, fontStyle = FontStyle.Bold };
            GUIStyle labelStyle2 = new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleCenter, fontSize = 12, fontStyle = FontStyle.Bold };

            #region Header & Description

            SerializedProperty nameProp = serializedObject.FindProperty("m_Name");
            SerializedProperty descriptionProp = serializedObject.FindProperty("description");

            // Header
            EditorGUILayout.LabelField(targetConsideration.name, labelStyle1);
            EditorGUILayout.Space(10);

            // Description
            EditorGUILayout.LabelField("Description");
            descriptionProp.stringValue = EditorGUILayout.TextArea(descriptionProp.stringValue, GUILayout.Height(100));

            #endregion

            #region Input

            // Input Header
            EditorGUILayout.Space(10);
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.LabelField("Input", labelStyle2);
            EditorGUILayout.Space(5);

            //Consideration Input Params
            SerializedProperty considerationInputProp = serializedObject.FindProperty("considerationInput");
            EditorGUILayout.PropertyField(considerationInputProp, new GUIContent(""));
            EditorGUILayout.Space(5);

            // Params 
            SerializedProperty considerationInputInputParamsProp = serializedObject.FindProperty("considerationInputParams");

            //If there is an ConsiderationInput assigned -> show its properties
            ConsiderationInput considerationInput = considerationInputProp.objectReferenceValue as ConsiderationInput;
            if (considerationInput != null)
            {
                List<UnityEditor.SerializedProperty> considerationInputProperties = ConsiderationInputParamsSerializationHelper.GetCorrespondingParams(considerationInputInputParamsProp, considerationInput.inputParamsType);
                EditorGUI.indentLevel += 1;
                for (int i = 0; i < considerationInputProperties.Count; i++)
                {
                    EditorGUILayout.PropertyField(considerationInputProperties[i]);

                }
                EditorGUI.indentLevel -= 1;
            }

            EditorGUILayout.Space(10);

            EditorGUILayout.EndVertical();
            #endregion

            #region Curve
            SerializedProperty curveProp = serializedObject.FindProperty("considerationCurve");
            EditorGUILayout.PropertyField(curveProp);

            #endregion

            #region Curve Example Values

            EditorGUILayout.Space(5);
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);

            foldoutExampleValues = EditorGUILayout.BeginFoldoutHeaderGroup(foldoutExampleValues, "Show Example Curve Values", EditorStyles.foldout);

            if (foldoutExampleValues)
            {
                EditorGUILayout.Space(5);

                EditorGUILayout.LabelField("Example Inputs (normalized)");


                SerializedProperty exampleInput1Prop = serializedObject.FindProperty("exampleInput1");
                SerializedProperty exampleInput2Prop = serializedObject.FindProperty("exampleInput2");
                SerializedProperty exampleInput3Prop = serializedObject.FindProperty("exampleInput3");
                SerializedProperty exampleInput4Prop = serializedObject.FindProperty("exampleInput4");
                SerializedProperty exampleInput5Prop = serializedObject.FindProperty("exampleInput5");

                EditorGUILayout.BeginHorizontal();
                //EditorGUILayout.PropertyField(exampleInput1Prop, GUILayout.Width(20));
                exampleInput1Prop.floatValue = EditorGUILayout.FloatField(exampleInput1Prop.floatValue);
                exampleInput2Prop.floatValue = EditorGUILayout.FloatField(exampleInput2Prop.floatValue);
                exampleInput3Prop.floatValue = EditorGUILayout.FloatField(exampleInput3Prop.floatValue);
                exampleInput4Prop.floatValue = EditorGUILayout.FloatField(exampleInput4Prop.floatValue);
                exampleInput5Prop.floatValue = EditorGUILayout.FloatField(exampleInput5Prop.floatValue);
                EditorGUILayout.EndHorizontal();

              


                EditorGUILayout.LabelField("Example Outputs");

                SerializedProperty exampleOutput1Prop = serializedObject.FindProperty("exampleOutput1");
                SerializedProperty exampleOutput2Prop = serializedObject.FindProperty("exampleOutput2");
                SerializedProperty exampleOutput3Prop = serializedObject.FindProperty("exampleOutput3");
                SerializedProperty exampleOutput4Prop = serializedObject.FindProperty("exampleOutput4");
                SerializedProperty exampleOutput5Prop = serializedObject.FindProperty("exampleOutput5");

                exampleOutput1Prop.floatValue = targetConsideration.considerationCurve.GetRemappedValue(exampleInput1Prop.floatValue);
                exampleOutput2Prop.floatValue = targetConsideration.considerationCurve.GetRemappedValue(exampleInput2Prop.floatValue);
                exampleOutput3Prop.floatValue = targetConsideration.considerationCurve.GetRemappedValue(exampleInput3Prop.floatValue);
                exampleOutput4Prop.floatValue = targetConsideration.considerationCurve.GetRemappedValue(exampleInput4Prop.floatValue);
                exampleOutput5Prop.floatValue = targetConsideration.considerationCurve.GetRemappedValue(exampleInput5Prop.floatValue);

                // Readonly Outputs
                EditorGUILayout.BeginHorizontal();
                GUI.enabled = false; //workaround to make them readonly
                EditorGUILayout.TextField(exampleOutput1Prop.floatValue.ToString("F2"));
                EditorGUILayout.TextField(exampleOutput2Prop.floatValue.ToString("F2"));
                EditorGUILayout.TextField(exampleOutput3Prop.floatValue.ToString("F2"));
                EditorGUILayout.TextField(exampleOutput4Prop.floatValue.ToString("F2"));
                EditorGUILayout.TextField(exampleOutput5Prop.floatValue.ToString("F2"));
                GUI.enabled = true;
                EditorGUILayout.EndHorizontal();
            }

            EditorGUILayout.EndVertical();

            #endregion

            serializedObject.ApplyModifiedProperties();
        }
    }

}