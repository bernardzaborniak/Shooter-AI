using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace BenitosAI
{
   /*
    [CustomPropertyDrawer(typeof(DecisionWrapper))]
    public class DecisionWrapperDrawer : PropertyDrawer
    {
       // bool foldout = false;
        //string name;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            // Set the name of the object to the name of the referenced decision
            SerializedProperty nameProp = property.FindPropertyRelative("name");
            SerializedProperty decisionProp = property.FindPropertyRelative("decision");
            if (decisionProp.objectReferenceValue)
            {
                string nameToSet = decisionProp.objectReferenceValue.ToString();
                char[] chars = nameToSet.ToCharArray();

                nameToSet = "";
                for (int i = 0; i < chars.Length; i++)
                {
                    if(chars[i] == '(')
                    {
                        break;
                    }
                    else
                    {
                        nameToSet += chars[i];
                    }
                }


                nameProp.stringValue = nameToSet;
            }
         


            EditorGUI.PropertyField(position, property, label, true);

            //EditorGUI.BeginProperty(position, label, property);


            /*property.isExpanded = EditorGUI.BeginFoldoutHeaderGroup(position, property.isExpanded, "foldout header");

            if (property.isExpanded)
            {
                EditorGUI.TextField(new Rect(position.x+10, position.y+20, position.width - 20, 20), "test text");
                EditorGUI.TextField(new Rect(position.x + 10, position.y + 40, position.width - 20, 20), "test text2");
            }

            EditorGUI.EndFoldoutHeaderGroup();

            //EditorGUI.EndProperty();*/



            //EditorGUILayout.LabelField("DecisionWrapperDrawer", EditorStyles.boldLabel);

            //SerializedProperty nameProp = property.FindPropertyRelative("decision");
            //name = nameProp.stringValue;
            //foldout = EditorGUILayout.BeginFoldoutHeaderGroup(foldout, "name",EditorStyles.boldLabel);

            //if (foldout)
            //{
            //SerializedProperty decisionProp = property.FindPropertyRelative("decision");
            //  EditorGUILayout.PropertyField(decisionProp);

            //  SerializedProperty weigtProp = property.FindPropertyRelative("weigt");
            //  EditorGUILayout.PropertyField(weigtProp);
            //}

      /*  }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            //return base.GetPropertyHeight(property, label);
            if (property.isExpanded)
            {
                return 60f;

            }
            else
            {
                return base.GetPropertyHeight(property, label);
            }

        }
    }*/
}
