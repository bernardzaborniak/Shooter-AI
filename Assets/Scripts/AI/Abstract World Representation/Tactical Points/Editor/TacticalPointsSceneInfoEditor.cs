using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(TacticalPointsSceneInfo))]
public class TacticalPointsSceneInfoEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector(); //this methiod draws the deault editor, we can add more custom editors later
        EditorGUILayout.TextField("TestLabel", "testInhalt");

        TacticalPointsSceneInfo targetTacticalPointsSceneInfo = (TacticalPointsSceneInfo)target;
        //myTacticalPointsGeneratorBox.testInt = EditorGUILayout.IntField("testIntef", myTacticalPointsGeneratorBox.testInt);
        
        /*if(targetTacticalPointsSceneInfo.pointRatings == null)
        {
            Debug.Log("the cover ratings are null, rebake!");
        }
        else
        {
            EditorGUILayout.TextField("TestLabel", targetTacticalPointsSceneInfo.pointRatings.Count.ToString());

            foreach (var kvp in targetTacticalPointsSceneInfo.pointRatings)
                GUILayout.Label("Key: " + kvp.Key + " value: " + kvp.Value);

            //foreach (var kvp in raycastUsedPerPoint)
               // GUILayout.Label("Key: " + kvp.Key + " value: " + kvp.Value);
        }*/

        /*if (GUILayout.Button("Generate Points"))
        {
            myTacticalPointsGeneratorBox.Generate();
        }


        /* if (GUILayout.Button("Bake Cover Ratings"))
         {
             myTacticalPointsGeneratorBox.BakeCoverRatings();
         }

         if (GUILayout.Button("Generate Points & Bake all Ratings "))
         {
             myTacticalPointsGeneratorBox.Generate();
             myTacticalPointsGeneratorBox.BakeCoverRatings();
         }

        if (GUILayout.Button("Delete All Generated Points "))
        {
            myTacticalPointsGeneratorBox.DeleteAllGeneratedPoints();
        }*/
    }
}
