using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace BenitosAI
{
    //The sole purpose of this Property Drawer is to make the DecisionPropertyDrawer work, it wont work unless the propertyDrawer it is serialised in has a custom editor too.


    /*[CustomPropertyDrawer(typeof(DecisionMaker))]
    public class DecisionMakerDrawer : PropertyDrawer
    {
        public Dictionary<string, bool> unfold = new Dictionary<string, bool>();

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {

            position.height = 16;

            //EditorGUILayout.LabelField("testLabel");
            EditorGUILayout.PropertyField(property.FindPropertyRelative("discardThreshold"));
            EditorGUILayout.PropertyField(property.FindPropertyRelative("decisionMethod"));


            if (!unfold.ContainsKey(property.propertyPath))
            {
                unfold.Add(property.propertyPath, false);
            }

            unfold[property.propertyPath] = EditorGUI.Foldout(position, unfold[property.propertyPath], label);

            if (unfold[property.propertyPath])
            {
                Rect verticalGroup = EditorGUILayout.BeginVertical(EditorStyles.helpBox);

                //EditorGUI.BeginProperty(position, label, property);
                //EditorGUI.indentLevel++;
                Debug.Log("unfolded");

                EditorGUILayout.PropertyField(property.FindPropertyRelative("name"));
                EditorGUILayout.PropertyField(property.FindPropertyRelative("decisionMethod"));
                EditorGUILayout.PropertyField(property.FindPropertyRelative("discardThreshold"));
                EditorGUILayout.LabelField("testLabel");


                //EditorGUI.indentLevel--;
                //EditorGUI.EndProperty();
                
                EditorGUILayout.EndVertical();


            }
            else
               {
                Debug.Log("not unfoldet");
            }
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            if (unfold.ContainsKey(property.propertyPath))
            {
                if (unfold[property.propertyPath])
                {
                    return 16 * 3;
                }
            }

            return 16;

        }
      




        
    }*/
}

