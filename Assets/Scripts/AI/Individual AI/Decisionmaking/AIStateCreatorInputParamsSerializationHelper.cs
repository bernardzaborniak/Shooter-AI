using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
namespace BenitosAI
{
    public static class AIStateCreatorInputParamsSerializationHelper 
    {


        public static List<UnityEditor.SerializedProperty> GetCorrespondingParams(UnityEditor.SerializedProperty inputParamsProp, AIStateCreatorInputParams.InputParamsType inputParamsType)//AIStateCreator aiStateCreator, ref UnityEditor.SerializedObject serializedObject)
        {


            List<UnityEditor.SerializedProperty> paramsList = new List<UnityEditor.SerializedProperty>();

            if (inputParamsType == AIStateCreatorInputParams.InputParamsType.Position)
            {
                //paramsList.Add(serializedObject.FindProperty("inputParams").FindPropertyRelative("position1"));
                //paramsList.Add(serializedObject.FindProperty("inputParams").FindPropertyRelative("position2"));

                paramsList.Add(inputParamsProp.FindPropertyRelative("position1"));
                paramsList.Add(inputParamsProp.FindPropertyRelative("position2"));


            }
            else if (inputParamsType == AIStateCreatorInputParams.InputParamsType.Color)
            {
                // paramsList.Add(serializedObject.FindProperty("inputParams").FindPropertyRelative("color"));
                paramsList.Add(inputParamsProp.FindPropertyRelative("color"));

            }
            return paramsList;
        }



    }
}
#endif

