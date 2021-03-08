using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

/*namespace BenitosAI
{
    [CustomPropertyDrawer(typeof(Decision))]
    public class DecisionDrawer : PropertyDrawer
    {
        public Dictionary<string, bool> unfold = new Dictionary<string, bool>();
        //cause unity property drawer isnt perfect, we use the dicionary, so every item in the list can have its own unfold bool
        //got the code for this from here: https://answers.unity.com/questions/1350095/propertydrawer-foldout-open-all-at-once.html

        //string nameValue;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            position.height = 16;

            if (!unfold.ContainsKey(property.propertyPath))
            {
                unfold.Add(property.propertyPath, false);
            }

            unfold[property.propertyPath] = EditorGUI.Foldout(position, unfold[property.propertyPath], label);

            if (unfold[property.propertyPath])
            {
                //Show folded properties
                SerializedProperty nameProp = property.FindPropertyRelative("name");
                
                //EditorGUI.BeginProperty(position, label, nameProp);

                //nameProp.stringValue = EditorGUI.TextField(new Rect(10,25,position.width-20,20), "Name:", nameProp.stringValue);

                //EditorGUI.EndProperty();
                EditorGUILayout.PropertyField(nameProp);
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
    }
}*/


