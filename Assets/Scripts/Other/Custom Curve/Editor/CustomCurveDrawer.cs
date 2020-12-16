using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(CustomCurve))]
public class CustomCurveDrawer : PropertyDrawer
{
    Color curveColor = new Color(0f, 0f, 1f);

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {

        Rect verticalGroup = EditorGUILayout.BeginVertical(EditorStyles.helpBox);

        // ------ Header & Type ------
        EditorGUILayout.LabelField("Curve Options", EditorStyles.boldLabel);

        SerializedProperty curveTypeProp = property.FindPropertyRelative("curveType");
        EditorGUILayout.PropertyField(curveTypeProp);

        EditorGUILayout.Space(5);

        #region Draw Curve Params according to Enum

        // ------ Parameters according to enum Value ------

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

        #region Draw Curve Visualisation

        EditorGUILayout.Space(10);


        // ------  Get Visualisation Keyframes and connect them via correct tangents ----------

        SerializedProperty curveVisualisationPositionsProp = property.FindPropertyRelative("curveVisualisationPositions");
        int keyNumber = curveVisualisationPositionsProp.arraySize;
        Keyframe[] keys = new Keyframe[keyNumber];

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
                // nextPos = curveVisualisationPositionsProp.GetArrayElementAtIndex(i).vector2Value + (curveVisualisationPositionsProp.GetArrayElementAtIndex(i).vector2Value - curveVisualisationPositionsProp.GetArrayElementAtIndex(i - 1).vector2Value);

            }


            SerializedProperty prop = curveVisualisationPositionsProp.GetArrayElementAtIndex(i);

            Vector2 keyPos = prop.vector2Value;
            Vector2 inTangent = previousPos - keyPos;
            Vector2 outTangent = nextPos - keyPos;

            keys[i] = new Keyframe(prop.vector2Value.x, prop.vector2Value.y, inTangent.y/inTangent.x, outTangent.y / outTangent.x);
        }


        // ----- Disabling & Enabling GUI Elements as a workaround to make the curve read only -----

        var previousGUIState = GUI.enabled;
        GUI.enabled = false;
        EditorGUILayout.CurveField(new AnimationCurve(keys), curveColor, new Rect(0,0,1,1), GUILayout.Height(Screen.width *0.9f), GUILayout.Width(Screen.width * 0.9f));
        GUI.enabled = previousGUIState;


        // ---------------- Draw grid & Numbers above the Curve --------------------
        
        Rect gridRect = GUILayoutUtility.GetLastRect();  
        Color thickLinesColor = new Color(0, 0, 0, 0.3f);
        float thickLinesThickness = 1.3f;
        Color thinLinesColor = new Color(0, 0, 0, 0.1f);
        float thinLinesThickness = 1f;

        //Draw thick middle lines
        EditorGUI.DrawRect(new Rect(gridRect.x + gridRect.width / 2, gridRect.y +1, thickLinesThickness, gridRect.height-1), thickLinesColor);         //vertical
        EditorGUI.DrawRect(new Rect(gridRect.x, gridRect.y + gridRect.height/2, gridRect.width, thickLinesThickness), thickLinesColor);          //horizontal

        //10 thin vertical lines
        for (int i = 0; i < 10; i++)
        {
            EditorGUI.DrawRect(new Rect(gridRect.x + (gridRect.width / 10)*i, gridRect.y + 1, thinLinesThickness, gridRect.height - 1), thinLinesColor);
        }

        //10 thin horizontal lines
        for (int i = 0; i < 10; i++)
        {
            EditorGUI.DrawRect(new Rect(gridRect.x, gridRect.y + (gridRect.height / 10)*i, gridRect.width, thinLinesThickness), thinLinesColor);
        }

        // Draw Input & Output Text
        GUIStyle axiesStyle = new GUIStyle(EditorStyles.label);
        axiesStyle.normal.textColor = Color.white;
        axiesStyle.fontSize = 15;

        //Draw Output Axies
        Rect outputRect = new Rect(gridRect.position.x, gridRect.position.y + gridRect.height/2 + 20, 50, 30);
        EditorGUIUtility.RotateAroundPivot(-90f, outputRect.position);
        EditorGUI.LabelField(outputRect, "Output", axiesStyle);
        EditorGUIUtility.RotateAroundPivot(90f, outputRect.position);

        //Draw Input Axies
        Rect inputRect = new Rect(gridRect.position.x + gridRect.width / 2 - 20, gridRect.position.y + gridRect.height - 30, 50, 30);
        EditorGUI.LabelField(inputRect, "Input", axiesStyle);

        //Draw 0s and 1s
        axiesStyle.fontSize = 17;

        Rect zeroRect = new Rect(gridRect.position.x + 8, gridRect.position.y + gridRect.height - 30, 50, 30);
        EditorGUI.LabelField(zeroRect, "0", axiesStyle);

        Rect oneRect1 = new Rect(gridRect.position.x + 8, gridRect.position.y, 50, 30);
        EditorGUI.LabelField(oneRect1, "1", axiesStyle);

        Rect oneRect2 = new Rect(gridRect.position.x + gridRect.width - 20, gridRect.position.y + gridRect.height - 30, 50, 30);
        EditorGUI.LabelField(oneRect2, "1", axiesStyle);

        #endregion

        EditorGUILayout.EndVertical();

    }
}
