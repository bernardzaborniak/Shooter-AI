using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(CustomCurve))]
public class CustomCurveDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {

        int indent = EditorGUI.indentLevel;
        EditorGUI.indentLevel = 1;

        #region Curve Params

        SerializedProperty curveTypeProp = property.FindPropertyRelative("curveType");

        EditorGUILayout.LabelField("Curve Params", EditorStyles.boldLabel);

        EditorGUILayout.PropertyField(curveTypeProp);
        
        EditorGUILayout.Space(5);


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
        }

        #endregion

        #region Curve Visualisation

        EditorGUILayout.Space(5);
        EditorGUILayout.LabelField("Curve Visualisation", EditorStyles.boldLabel);

        //Keyframe[] keys = new Keyframe[] { new Keyframe(0, 0, 0, 0), new Keyframe(0.5f, 0.5f, 0, 0), new Keyframe(0.6f, 0.7f, 0, 0), new Keyframe(0.7f, 0.9f, 0, 0) };

        SerializedProperty curveVisualisationPositionsProp = property.FindPropertyRelative("curveVisualisationPositions");
        int keyNumber = curveVisualisationPositionsProp.arraySize;
        Keyframe[] keys = new Keyframe[keyNumber];
        //Vector3 previousPosition; //stored to get the curve tangents right
        //Vector3 nextPosition; //stored to get the curve tangents right


        //Debug.Log("Draw keys 2 -------------------------------------------------------------------");
        for (int i = 0; i < keyNumber; i++)
        {
            Vector2 previousPos;
            if (i != 0)
            {
                previousPos = curveVisualisationPositionsProp.GetArrayElementAtIndex(i - 1).vector2Value;
            }
            else
            {
                previousPos = curveVisualisationPositionsProp.GetArrayElementAtIndex(0).vector2Value;
            }

            Vector2 nextPos;
            if (i+1 != keyNumber)
            {
                nextPos = curveVisualisationPositionsProp.GetArrayElementAtIndex(i + 1).vector2Value;
            }
            else
            {
                nextPos = curveVisualisationPositionsProp.GetArrayElementAtIndex(keyNumber-1).vector2Value;
            }


            SerializedProperty prop = curveVisualisationPositionsProp.GetArrayElementAtIndex(i);

            Vector2 keyPos = prop.vector2Value;
            //Debug.Log("keyPos 2: " + keyPos);
            Vector2 inTangent = previousPos - keyPos;
            Vector2 outTangent = nextPos - keyPos;

            keys[i] = new Keyframe(prop.vector2Value.x, prop.vector2Value.y, inTangent.y/inTangent.x, outTangent.y / outTangent.x);
        }
        
        //Vector2[] keyPositions = curveVisualisationPositionsProp.
        //Keyframe[] keys = new Keyframe[] { new Keyframe(0, 0, 0, 0), new Keyframe(0.5f, 0.5f, 0, 0), new Keyframe(0.6f, 0.7f, 0, 0), new Keyframe(0.7f, 0.9f, 0, 0) };

        //TODO reenable the readonly elements here

        // Saving previous GUI enabled value
        //var previousGUIState = GUI.enabled;
        // Disabling edit for property
        //GUI.enabled = false;
        // Drawing Property
        EditorGUILayout.CurveField(new AnimationCurve(keys),Color.blue,new Rect(0,0,1,1), GUILayout.Height(400), GUILayout.Width(400));

        // Setting old GUI enabled value
        //GUI.enabled = previousGUIState;

        #endregion



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
