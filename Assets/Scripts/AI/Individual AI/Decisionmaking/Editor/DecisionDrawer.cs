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
        //public Dictionary<string, bool> hasMomentumUnfold = new Dictionary<string, bool>();

        //cause unity property drawer isnt perfect, we use the dicionary, so every item in the list can have its own unfold bool
        //got the code for this from here: https://answers.unity.com/questions/1350095/propertydrawer-foldout-open-all-at-once.html

        //string nameValue;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            label = EditorGUI.BeginProperty(position, label, property);

            position.height = 16;

            if (!unfold.ContainsKey(property.propertyPath))
            {
                unfold.Add(property.propertyPath, false);
            }


            //position.width *= 0.5f;
            //EditorGUIUtility.labelWidth = 14
            unfold[property.propertyPath] = EditorGUI.Foldout(position, unfold[property.propertyPath], ""); //label was the default last param
            //position.width *= 2;
            // position.width *= 0.5f;
            EditorGUI.LabelField(new Rect(position.x, position.y,position.width/2,position.height), property.displayName, EditorStyles.label);
           // position.width *= 2;

            position.x += position.width * 0.6f;
            EditorGUI.LabelField(position, new GUIContent("DCC"));
            position.x -= position.width * 0.6f;

            position.x += position.width * 0.8f;
            EditorGUI.LabelField(position, new GUIContent("34"));
            position.x -= position.width * 0.8f;

            if (unfold[property.propertyPath])
            {
                //Show folded properties
                //SerializedProperty nameProp = property.FindPropertyRelative("name");

                //EditorGUI.BeginProperty(position, label, nameProp);

                //nameProp.stringValue = EditorGUI.TextField(new Rect(10,25,position.width-20,20), "Name:", nameProp.stringValue);

                //EditorGUI.EndProperty();
                // EditorGUILayout.PropertyField(nameProp);

                position.y += 18 + 8;

                //EditorGUI.LabelField(position, new GUIContent("DCC 1"));
                EditorGUI.PropertyField(position, property.FindPropertyRelative("correspondingAiStateCreator"));

                position.y += 18;
                //EditorGUI.LabelField(position, new GUIContent("DCC 2"));
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
                EditorGUI.PropertyField(position, property.FindPropertyRelative("considerations"));
                //EditorGUI.PropertyField(position, property.FindPropertyRelative("hasMomentum"));

                //label = EditorGUI.BeginProperty(position, label, property);

                //Rect contentPosition = EditorGUI.PrefixLabel(position, label);
                // Rect contentPosition = EditorGUI.PrefixLabel(position, new GUIContent("45");

                //EditorGUI.EndProperty();

            }


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


