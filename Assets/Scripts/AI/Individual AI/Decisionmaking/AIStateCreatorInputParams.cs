using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BenitosAI
{
    //cuzstom clas inside decision

    [System.Serializable]
    public class AIStateCreatorInputParams //: ScriptableObject //has to be SO so it can be serialized properly
    {
        //move the declaration of this enum somewhere else? - or its good here?
        public enum InputParamsType
        {
            Position,
            Color
        }

        //public InputParamsType inputParamsType;


        // Type.Position
        public Vector3 position1;
        public Vector3 position2;

        // Type.Color
        public Color color;

      


/*#if UNITY_EDITOR
        public List<UnityEditor.SerializedProperty> GetCorrespondingParams(UnityEditor.SerializedProperty inputParamsProp,InputParamsType inputParamsType)//AIStateCreator aiStateCreator, ref UnityEditor.SerializedObject serializedObject)
        {
            

            List<UnityEditor.SerializedProperty> paramsList = new List<UnityEditor.SerializedProperty>();

            if (inputParamsType == InputParamsType.Position)
            {
                //paramsList.Add(serializedObject.FindProperty("inputParams").FindPropertyRelative("position1"));
                //paramsList.Add(serializedObject.FindProperty("inputParams").FindPropertyRelative("position2"));

                paramsList.Add(inputParamsProp.FindPropertyRelative("position1"));
                paramsList.Add(inputParamsProp.FindPropertyRelative("position2"));


            }
            else if (inputParamsType == InputParamsType.Color)
            {
               // paramsList.Add(serializedObject.FindProperty("inputParams").FindPropertyRelative("color"));
                paramsList.Add(inputParamsProp.FindPropertyRelative("color"));

            }
            return paramsList;
        }
        

#endif*/

    }

}

