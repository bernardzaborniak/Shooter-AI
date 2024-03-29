using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace BenitosAI
{
    [CustomPropertyDrawer(typeof(Decision))]
    public class DecisionDrawer : PropertyDrawer
    {
        public Dictionary<string, bool> unfold = new Dictionary<string, bool>();
        //Cause unity PropertyDrawer isnt perfect for foldouts, we use the dicionary, so every item in the list can have its own unfold bool.
        //Got the code for this from here: https://answers.unity.com/questions/1350095/propertydrawer-foldout-open-all-at-once.html


        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            //label = EditorGUI.BeginProperty(position, label, property);
            EditorGUI.BeginProperty(position, label, property);

            // Update unfold Dictionary
            if (!unfold.ContainsKey(property.propertyPath))
            {
                unfold.Add(property.propertyPath, false);
            }

            // -------------- Decision Values visible in fold------------------
            position.height = 16;

            //Only draw the triangle without the default label
            unfold[property.propertyPath] = EditorGUI.Foldout(position, unfold[property.propertyPath], ""); //label was the default last param

            //Name of the Decision
            SerializedProperty nameProp = property.FindPropertyRelative("name");
            Rect nameRect = new Rect(position.x, position.y, position.width * 0.525f, position.height);
            nameProp.stringValue = EditorGUI.TextField(nameRect, nameProp.stringValue);

            //Save the default x value for later
            float propertyXPos = position.x; 

            //Position the dcc property roughly in the middle.
            position.x = propertyXPos + position.width * 0.55f;
            SerializedProperty dccProp = property.FindPropertyRelative("decisionContextCreator");
            Rect dccRect = new Rect(position.x, position.y, position.width * 0.225f, position.height);
            dccProp.objectReferenceValue = EditorGUI.ObjectField(dccRect, dccProp.objectReferenceValue, typeof(DecisionContextCreator), false);

            //Name the Weight property
            position.x = propertyXPos + position.width * 0.8f;
            EditorGUI.LabelField(position, new GUIContent("weight"));

            //Position the Weight property completely to the right
            position.x = propertyXPos + position.width * 0.9f;
            Rect weightRect = new Rect(position.x, position.y, position.width * 0.1f, position.height);
            SerializedProperty weightProp = property.FindPropertyRelative("weight");
            weightProp.floatValue = EditorGUI.FloatField(weightRect, weightProp.floatValue);

            //Go back to the default x value
            position.x = propertyXPos;


            if (unfold[property.propertyPath])
            {
                position.y += 18 + 8;

                //----------------- AI State Creator ----------------
                SerializedProperty aISTateCreatorProp = property.FindPropertyRelative("correspondingAiStateCreator");
                EditorGUI.PropertyField(position, aISTateCreatorProp);
                
                SerializedProperty aIStateCreatorInputParamsProp = property.FindPropertyRelative("aIStateCreatorInputParams");

                //If there is an AI state Creator assigned -> show its properties
                AIStateCreator aIStateCreator = aISTateCreatorProp.objectReferenceValue as AIStateCreator;
                if (aIStateCreator != null)
                {
                    List<UnityEditor.SerializedProperty> aIStateProperties = AIStateCreatorInputParamsSerializationHelper.GetCorrespondingParams(aIStateCreatorInputParamsProp, aIStateCreator.inputParamsType);
                    EditorGUI.indentLevel += 1;
                    for (int i = 0; i < aIStateProperties.Count; i++)
                    {
                        position.y += 18;
                        EditorGUI.PropertyField(position, aIStateProperties[i]);

                    }
                    EditorGUI.indentLevel -= 1;
                }

                
                //------------------ Momentum --------------
                position.y += 18+6;
                SerializedProperty hasMomentumProp = property.FindPropertyRelative("hasMomentum");
                hasMomentumProp.boolValue = EditorGUI.Toggle(position, hasMomentumProp.displayName, hasMomentumProp.boolValue);

                if (hasMomentumProp.boolValue)
                {
                    EditorGUI.indentLevel += 1;
                    position.y += 18;
                    EditorGUI.PropertyField(position, property.FindPropertyRelative("momentumSelectedBonus"));
                    position.y += 18;
                    EditorGUI.PropertyField(position, property.FindPropertyRelative("momentumDecayRate"));
                    EditorGUI.indentLevel -= 1;

                }

                //-------------------- Considerations --------------
                position.y += 18;
                EditorGUIUtility.labelWidth = 0.1f; //this makes sure that we draw the nice defualt unity list, but things like "element 1" etc are not visible.
                EditorGUI.PropertyField(position, property.FindPropertyRelative("considerations"));
            }


            property.serializedObject.ApplyModifiedProperties(); //Im not sure if its really necessary
            EditorGUI.EndProperty();    
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            int height = 16;

            if (unfold.ContainsKey(property.propertyPath))
            {
                if (unfold[property.propertyPath])
                {
                    height += 8 + 18 + 18 + 18 + 8 + 8; //The strange addition is to simplify adjustment  Default unity line is 16px, margin is 2 -> 18 per line

                    //AI STate creator params height:
                    SerializedProperty aISTateCreatorProp = property.FindPropertyRelative("correspondingAiStateCreator");
                    SerializedProperty aIStateCreatorInputParamsProp = property.FindPropertyRelative("aIStateCreatorInputParams");
                    AIStateCreator aIStateCreator = aISTateCreatorProp.objectReferenceValue as AIStateCreator;
                    if (aIStateCreator != null)
                    {
                        List<UnityEditor.SerializedProperty> aIStateProperties = AIStateCreatorInputParamsSerializationHelper.GetCorrespondingParams(aIStateCreatorInputParamsProp, aIStateCreator.inputParamsType);
                        for (int i = 0; i < aIStateProperties.Count; i++)
                        {
                            height += 18;

                        }
                    }

                    //Conideration list height:
                    height += (int)(EditorGUI.GetPropertyHeight(property.FindPropertyRelative("considerations"),true));  

                    if (property.FindPropertyRelative("hasMomentum").boolValue)
                    {
                        height += 18 + 18;
                    }
                }
            }

            return height;
        }  
    }
}


