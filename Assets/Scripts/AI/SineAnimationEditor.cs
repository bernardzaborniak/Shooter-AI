using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(SineAnimation)), CanEditMultipleObjects]
public class SineAnimationEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        if (GUILayout.Button("Randomize Sine Function", EditorStyles.miniButton))
        {
            serializedObject.FindProperty("m_Period").floatValue = Random.Range(0f, 10f);
            serializedObject.FindProperty("m_Amplitude").floatValue = Random.Range(0f, 10f);
            serializedObject.FindProperty("m_PhaseShift").floatValue = Random.Range(0f, 1f);
            serializedObject.ApplyModifiedProperties();
        }
    }
}
