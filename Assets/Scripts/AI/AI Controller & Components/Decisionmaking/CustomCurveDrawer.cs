using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(CustomCurve))]
public class CustomCurveDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        SerializedProperty curveTypeProp = property.FindPropertyRelative("curveType");

        //EditorGUI.LabelField(new Rect(position.x, position.y, position.width, position.height), "Curve Params", EditorStyles.boldLabel);
        EditorGUILayout.LabelField("Curve Params", EditorStyles.boldLabel);

        EditorGUILayout.PropertyField(curveTypeProp);
        
        EditorGUILayout.Space(5);

        int indent = EditorGUI.indentLevel;
        EditorGUI.indentLevel = 1;

        //if(curveType == Curve)
        int curveType = curveTypeProp.enumValueIndex;

        if(curveType == 0)
        {
            SerializedProperty bi_thresholdProp = property.FindPropertyRelative("bi_threshold");
            SerializedProperty bi_inverse = property.FindPropertyRelative("bi_inverse");

            //EditorGUILayout.PropertyField(bi_thresholdProp);
            EditorGUILayout.Slider(bi_thresholdProp, 0f, 1f);
            EditorGUILayout.PropertyField(bi_inverse);
        }
        else if(curveType == 1)
        {
            SerializedProperty lin_Slope = property.FindPropertyRelative("lin_Slope");
            SerializedProperty lin_Shift = property.FindPropertyRelative("lin_Shift");

            EditorGUILayout.PropertyField(lin_Slope);
            EditorGUILayout.PropertyField(lin_Shift);
        }
        else if (curveType == 2)
        {
            SerializedProperty q_Slope = property.FindPropertyRelative("q_Slope");
            SerializedProperty q_Exponent = property.FindPropertyRelative("q_Exponent");
            SerializedProperty q_VertShift = property.FindPropertyRelative("q_VertShift");
            SerializedProperty q_HorizShift = property.FindPropertyRelative("q_HorizShift");

            EditorGUILayout.PropertyField(q_Slope);
            EditorGUILayout.PropertyField(q_Exponent);
            EditorGUILayout.PropertyField(q_VertShift);
            EditorGUILayout.PropertyField(q_HorizShift);
        }
        else if (curveType == 3)
        {
            SerializedProperty logistic_Slope = property.FindPropertyRelative("logistic_Slope");
            SerializedProperty logistic_XShift = property.FindPropertyRelative("logistic_XShift");
            SerializedProperty logistic_YShift = property.FindPropertyRelative("logistic_YShift");
            SerializedProperty logistic_YScalar = property.FindPropertyRelative("logistic_YScalar");

            EditorGUILayout.PropertyField(logistic_Slope);
            EditorGUILayout.PropertyField(logistic_XShift);
            EditorGUILayout.PropertyField(logistic_YShift);
            EditorGUILayout.PropertyField(logistic_YScalar);
        }
        else if (curveType == 4)
        {
            SerializedProperty logit_A = property.FindPropertyRelative("logit_A");
            SerializedProperty logit_B = property.FindPropertyRelative("logit_B");

            EditorGUILayout.Slider(logit_A, -0.3f, 0.3f);
            EditorGUILayout.Slider(logit_B, -0.3f, 0.3f);
           // EditorGUILayout.PropertyField(logit_A);
           // EditorGUILayout.PropertyField(logit_B);
        }

        EditorGUI.indentLevel = indent;

        //EditorGUI.indentLevel = 0;

        //EditorGUI.PropertyField(new Rect(position.x, position.y+50, position.width, position.height), curveType, GUIContent.none);

        // Draw scale
        /*EditorGUI.Slider
            (
                new Rect(position.x, position.y, position.width - curveWidth, position.height),
                scale, min, max, label
            );

        // Draw curve
        int indent = EditorGUI.indentLevel;
        EditorGUI.indentLevel = 1;

        EditorGUI.PropertyField
            (
                new Rect(position.width - curveWidth, position.y, curveWidth, position.height),
                curve, GUIContent.none
            );

        EditorGUI.indentLevel = indent;*/

    }
}
