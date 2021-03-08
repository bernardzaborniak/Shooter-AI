using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace BenitosAI
{
    //The sole purpose of this editor is to make the DecisionPropertyDrawer work, it wont work unless the monobehaviour it is serialised in has a custom editor too.

   /* [CustomEditor(typeof(AIController), true)]
    public class AIControllerEditor : Editor
    {
        private static readonly string[] ExcludedFields = new string[] { "decisionLayers" };

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            DrawPropertiesExcluding(serializedObject, ExcludedFields);
            serializedObject.ApplyModifiedProperties();

            EditorGUILayout.Space(5);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("decisionLayers"));


            //Editor innerEditor = Editor.CreateEditor(listItem, typeof(BasicElementEditor));
            //innerEditor.OnInspectorGUI();
        }
    }*/
}

