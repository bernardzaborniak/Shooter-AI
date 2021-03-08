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
        //cause unity property drawer isnt perfect, we use the dicionary, so every item in the list can have its own unfold bool
        //got the code for this from here: https://answers.unity.com/questions/1350095/propertydrawer-foldout-open-all-at-once.html


        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            label = EditorGUI.BeginProperty(position, label, property);

            position.height = 16;

            if (!unfold.ContainsKey(property.propertyPath))
            {
                unfold.Add(property.propertyPath, false);
            }


            //Only draw the triangle without the default label
            unfold[property.propertyPath] = EditorGUI.Foldout(position, unfold[property.propertyPath], ""); //label was the default last param

            //Name of the Decision
            SerializedProperty nameProp = property.FindPropertyRelative("name");
            nameProp.stringValue = EditorGUI.TextField(new Rect(position.x, position.y,position.width/2,position.height), nameProp.stringValue, EditorStyles.label);


            float propertyXPos = position.x;

            //Position the dcc property roughly in the middle.
            position.x = propertyXPos + position.width * 0.5f;
            SerializedProperty dccProp = property.FindPropertyRelative("decisionContextCreator");
            Rect dccRect = new Rect(position.x, position.y, position.width * 0.275f, position.height);
            EditorGUI.ObjectField(dccRect, dccProp.objectReferenceValue, typeof(DecisionContextCreator), false);

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

                // AI State Creator Params
                SerializedProperty aISTateCreatorProp = property.FindPropertyRelative("correspondingAiStateCreator");
                EditorGUI.PropertyField(position, aISTateCreatorProp);

                AIStateCreator aIStateCreator = aISTateCreatorProp.objectReferenceValue as AIStateCreator;
                Debug.Log("aIStateCreator: " + aIStateCreator);

                SerializedProperty aIStateCreatorInputParamsProp = property.FindPropertyRelative("aIStateCreatorInputParams");
                //AIStateCreatorInputParams aIStateCreatorInputParams = aIStateCreatorInputParamsProp.objectReferenceValue as System.Object as AIStateCreatorInputParams; //going through system.object is needed on classes not deriving from monobhaviour or scripbale onjertcs
                //AIStateCreatorInputParams aIStateCreatorInputParams = aIStateCreatorInputParamsProp.objectReferenceValue as AIStateCreatorInputParams; //going through system.object is needed on classes not deriving from monobhaviour or scripbale onjertcs

                //Debug.Log("aIStateCreator.inputParams: " + aIStateCreator.inputParams.inputParamsType);

                // SerializedObject serializedObject = new SerializedObject(aIStateCreator);
                //serializedObject.Update();
                Debug.Log("aIStateCreatorInputParamsProp: " + aIStateCreatorInputParamsProp);
                Debug.Log("aIStateCreator.inputParamsType: " + aIStateCreator.inputParamsType);
                List <UnityEditor.SerializedProperty> aIStateProperties = AIStateCreatorInputParamsSerializationHelper.GetCorrespondingParams(aIStateCreatorInputParamsProp, aIStateCreator.inputParamsType);
                for (int i = 0; i < aIStateProperties.Count; i++)
                {
                    position.y += 18;
                    //Debug.Log("prop: " + aIStateProperties[i]);
                    //EditorGUI.LabelField(position, "labelo");
                    /*if(aIStateProperties[i].propertyType == SerializedPropertyType.Vector3)
                    {
                        Debug.Log("aIStateProperties[i].displayName: " + aIStateProperties[i].displayName + " was vector3");
                        aIStateProperties[i].vector3Value = EditorGUI.Vector3Field(position, aIStateProperties[i].displayName, aIStateProperties[i].vector3Value);

                    }
                    else
                    {
                        EditorGUI.PropertyField(position, aIStateProperties[i]);

                    }*/
                    EditorGUI.PropertyField(position, aIStateProperties[i]);

                }

                //update
                
                //-----------------------------

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

                position.y += 18;
          
                EditorGUIUtility.labelWidth = 0.1f; //this makes sure that things like "element 1" etc are not visible
                EditorGUI.PropertyField(position, property.FindPropertyRelative("considerations"));
            }

            property.serializedObject.ApplyModifiedProperties();
            EditorGUI.EndProperty();
           
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            int height = 16;

            if (unfold.ContainsKey(property.propertyPath))
            {
                if (unfold[property.propertyPath])
                {
                    height += 8 + 18 + 18 +18 + 8 + (int)(EditorGUI.GetPropertyHeight(property.FindPropertyRelative("considerations"),false));

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


