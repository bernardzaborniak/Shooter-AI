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

            //TODO draw input params depending on the selected input paramter enum type?

            // Header
            EditorGUILayout.Space(10);
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.LabelField("Input", labelStyle2);
            EditorGUILayout.Space(5);

            // Input Enum 
            SerializedProperty considerationInputProp = serializedObject.FindProperty("considerationInput");
            EditorGUILayout.PropertyField(considerationInputProp, new GUIContent(""));
            EditorGUILayout.Space(5);

            // Params 

            ConsiderationInput.InputParamsType type;
            if (targetConsideration.considerationInput)
            {
                type = targetConsideration.considerationInput.inputParamsType;

                if (type == ConsiderationInput.InputParamsType.Range)
                {
                    SerializedProperty considerationInputPropMin = serializedObject.FindProperty("min");
                    SerializedProperty considerationInputPropMax = serializedObject.FindProperty("max");

                    EditorGUILayout.PropertyField(considerationInputPropMin);
                    EditorGUILayout.PropertyField(considerationInputPropMax);
                }
                else if (type == ConsiderationInput.InputParamsType.RangeAndDesiredFloatValue)
                {
                    SerializedProperty considerationInputPropMin = serializedObject.FindProperty("min");
                    SerializedProperty considerationInputPropMax = serializedObject.FindProperty("max");

                    EditorGUILayout.PropertyField(considerationInputPropMin);
                    EditorGUILayout.PropertyField(considerationInputPropMax);

                    SerializedProperty considerationInputPropDesFloatVal = serializedObject.FindProperty("desiredFloatValue");
                    EditorGUILayout.PropertyField(considerationInputPropDesFloatVal);
                }
                else if(type == ConsiderationInput.InputParamsType.WeaponID)
                {
                    SerializedProperty weaponIDProp = serializedObject.FindProperty("weaponID");
                    EditorGUILayout.PropertyField(weaponIDProp);
                }
                else if(type == ConsiderationInput.InputParamsType.TPointQualityEvaluationParams)
                {
                    SerializedProperty tPointEvaluationCrouchingProp = serializedObject.FindProperty("tPointEvaluationCrouching");
                    EditorGUILayout.PropertyField(tPointEvaluationCrouchingProp);

                    SerializedProperty tPointEvaluationTypeProp = serializedObject.FindProperty("tPointEvaluationType");
                    EditorGUILayout.PropertyField(tPointEvaluationTypeProp);

                    SerializedProperty tPointEvaluationMaxEnemiesToAcknowledgeWhileRatingProp = serializedObject.FindProperty("tPointEvaluationMaxEnemiesToAcknowledgeWhileRating");
                    EditorGUILayout.PropertyField(tPointEvaluationMaxEnemiesToAcknowledgeWhileRatingProp);

                    SerializedProperty tPointEvaluationMaxFriendliesToAcknowledgeWhileRatingProp = serializedObject.FindProperty("tPointEvaluationMaxFriendliesToAcknowledgeWhileRating");
                    EditorGUILayout.PropertyField(tPointEvaluationMaxFriendliesToAcknowledgeWhileRatingProp);
                }
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

                //set the values
                //exampleOutput1Prop.floatValue = targetConsideration.considerationCurve.GetRemappedValue(exampleInput1Prop.floatValue);
                /* exampleOutput1Prop.floatValue = targetConsideration.considerationCurve.GetRemappedValue(Utility.Remap(exampleInput1Prop.floatValue, targetConsideration.min, targetConsideration.max, 0, 1));
                 exampleOutput2Prop.floatValue = targetConsideration.considerationCurve.GetRemappedValue(Utility.Remap(exampleInput2Prop.floatValue, targetConsideration.min, targetConsideration.max, 0, 1));
                 exampleOutput3Prop.floatValue = targetConsideration.considerationCurve.GetRemappedValue(Utility.Remap(exampleInput3Prop.floatValue, targetConsideration.min, targetConsideration.max, 0, 1));
                 exampleOutput4Prop.floatValue = targetConsideration.considerationCurve.GetRemappedValue(Utility.Remap(exampleInput4Prop.floatValue, targetConsideration.min, targetConsideration.max, 0, 1));
                 exampleOutput5Prop.floatValue = targetConsideration.considerationCurve.GetRemappedValue(Utility.Remap(exampleInput5Prop.floatValue, targetConsideration.min, targetConsideration.max, 0, 1));*/

                exampleOutput1Prop.floatValue = targetConsideration.considerationCurve.GetRemappedValue(exampleInput1Prop.floatValue);
                exampleOutput2Prop.floatValue = targetConsideration.considerationCurve.GetRemappedValue(exampleInput2Prop.floatValue);
                exampleOutput3Prop.floatValue = targetConsideration.considerationCurve.GetRemappedValue(exampleInput3Prop.floatValue);
                exampleOutput4Prop.floatValue = targetConsideration.considerationCurve.GetRemappedValue(exampleInput4Prop.floatValue);
                exampleOutput5Prop.floatValue = targetConsideration.considerationCurve.GetRemappedValue(exampleInput5Prop.floatValue);


                // Readonly Outputs
                EditorGUILayout.BeginHorizontal();
                GUI.enabled = false;
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