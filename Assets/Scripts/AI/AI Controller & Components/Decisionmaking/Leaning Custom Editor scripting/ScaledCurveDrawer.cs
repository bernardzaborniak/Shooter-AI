using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(ScaledCurve))]
public class ScaledCurveDrawer : PropertyDrawer
{
    const int curveWidth = 50;
    const float min = 0;
    const float max = 1;

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        SerializedProperty scale = property.FindPropertyRelative("scale");
        SerializedProperty curve = property.FindPropertyRelative("curve");

        // Draw scale
        EditorGUI.Slider
            (
                new Rect(position.x, position.y, position.width - curveWidth, position.height),
                scale, min, max, label
            );

        // Draw curve
        int indent = EditorGUI.indentLevel;
        EditorGUI.indentLevel = 0;

        EditorGUI.PropertyField
            (
                new Rect(position.width - curveWidth, position.y, curveWidth, position.height),
                curve, GUIContent.none
            );

        EditorGUI.indentLevel = indent;

    }
}
