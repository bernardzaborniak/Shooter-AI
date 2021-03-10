using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace BenitosAI
{
    public static class ConsiderationInputParamsSerializationHelper
    {
        public static List<SerializedProperty> GetCorrespondingParams(SerializedProperty inputParamsProp, ConsiderationInputParams.InputParamsType[] inputParamsTypes)
        {


            List<SerializedProperty> paramsList = new List<SerializedProperty>();

            for (int i = 0; i < inputParamsTypes.Length; i++)
            {
                if (inputParamsTypes[i] == ConsiderationInputParams.InputParamsType.Range)
                {
                    // Using typeof() instead of the name strings makes sure we dont have to change this code when renaming the variables.
                    paramsList.Add(inputParamsProp.FindPropertyRelative(nameof(ConsiderationInputParams.min)));
                    paramsList.Add(inputParamsProp.FindPropertyRelative(nameof(ConsiderationInputParams.max)));
                }

                else if (inputParamsTypes[i] == ConsiderationInputParams.InputParamsType.DesiredFloatValue)
                {
                    paramsList.Add(inputParamsProp.FindPropertyRelative(nameof(ConsiderationInputParams.desiredFloatValue)));
                }

                else if (inputParamsTypes[i] == ConsiderationInputParams.InputParamsType.WeaponID)
                {
                    paramsList.Add(inputParamsProp.FindPropertyRelative(nameof(ConsiderationInputParams.weaponID)));
                }

                else if (inputParamsTypes[i] == ConsiderationInputParams.InputParamsType.Direction)
                {
                    paramsList.Add(inputParamsProp.FindPropertyRelative(nameof(ConsiderationInputParams.direction)));
                }

                else if (inputParamsTypes[i] == ConsiderationInputParams.InputParamsType.LineOfFire)
                {
                    paramsList.Add(inputParamsProp.FindPropertyRelative(nameof(ConsiderationInputParams.lineOfFireLayerMask)));
                }

                else if (inputParamsTypes[i] == ConsiderationInputParams.InputParamsType.LineOfSight)
                {
                    paramsList.Add(inputParamsProp.FindPropertyRelative(nameof(ConsiderationInputParams.lineOfSightLayerMask)));
                }

                else if (inputParamsTypes[i] == ConsiderationInputParams.InputParamsType.InformationFreshness)
                {
                    paramsList.Add(inputParamsProp.FindPropertyRelative(nameof(ConsiderationInputParams.informationFreshnessThreshold)));
                }

            }
            return paramsList;
        }
    }
}
