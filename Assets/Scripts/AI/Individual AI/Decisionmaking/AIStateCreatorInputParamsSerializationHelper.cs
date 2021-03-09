using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
namespace BenitosAI
{
    public static class AIStateCreatorInputParamsSerializationHelper 
    {


        public static List<UnityEditor.SerializedProperty> GetCorrespondingParams(UnityEditor.SerializedProperty inputParamsProp, AIStateCreatorInputParams.InputParamsType[] inputParamsTypes)//AIStateCreator aiStateCreator, ref UnityEditor.SerializedObject serializedObject)
        {
            

            List<UnityEditor.SerializedProperty> paramsList = new List<UnityEditor.SerializedProperty>();

            for (int i = 0; i < inputParamsTypes.Length; i++)
            {
                if (inputParamsTypes[i] == AIStateCreatorInputParams.InputParamsType.GoToTp)
                {
                    paramsList.Add(inputParamsProp.FindPropertyRelative(nameof(AIStateCreatorInputParams.enterTPDistance)));
                    paramsList.Add(inputParamsProp.FindPropertyRelative(nameof(AIStateCreatorInputParams.exitTPDistance)));
                }

                else if (inputParamsTypes[i] == AIStateCreatorInputParams.InputParamsType.Sprint)
                {
                    paramsList.Add(inputParamsProp.FindPropertyRelative(nameof(AIStateCreatorInputParams.sprint)));
                }

                else if (inputParamsTypes[i] == AIStateCreatorInputParams.InputParamsType.CharacterStance)
                {
                    paramsList.Add(inputParamsProp.FindPropertyRelative(nameof(AIStateCreatorInputParams.characterStance)));
                }

                else if (inputParamsTypes[i] == AIStateCreatorInputParams.InputParamsType.HoldWeaponScanForThreat)
                {
                    paramsList.Add(inputParamsProp.FindPropertyRelative(nameof(AIStateCreatorInputParams.minChangeAimDirInterval)));
                    paramsList.Add(inputParamsProp.FindPropertyRelative(nameof(AIStateCreatorInputParams.maxChangeAimDirInterval)));
                }

                else if (inputParamsTypes[i] == AIStateCreatorInputParams.InputParamsType.MaxAimingDeviationAngle)
                {
                    paramsList.Add(inputParamsProp.FindPropertyRelative(nameof(AIStateCreatorInputParams.maxAimingDeviationAngle)));
                }

                else if (inputParamsTypes[i] == AIStateCreatorInputParams.InputParamsType.WeaponID)
                {
                    paramsList.Add(inputParamsProp.FindPropertyRelative(nameof(AIStateCreatorInputParams.weaponID)));
                }

                else if (inputParamsTypes[i] == AIStateCreatorInputParams.InputParamsType.Position1)
                {
                    paramsList.Add(inputParamsProp.FindPropertyRelative(nameof(AIStateCreatorInputParams.position1)));
                }

                else if (inputParamsTypes[i] == AIStateCreatorInputParams.InputParamsType.Position2)
                {
                    paramsList.Add(inputParamsProp.FindPropertyRelative(nameof(AIStateCreatorInputParams.position2)));
                }

                else if (inputParamsTypes[i] == AIStateCreatorInputParams.InputParamsType.LineOfFireCheck)
                {
                    paramsList.Add(inputParamsProp.FindPropertyRelative(nameof(AIStateCreatorInputParams.checkLineOfFireInterval)));
                    paramsList.Add(inputParamsProp.FindPropertyRelative(nameof(AIStateCreatorInputParams.checkLineOfFireLayerMask)));
                }
            }

           
            return paramsList;
        }



    }
}
#endif

