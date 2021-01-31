using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoverQualityRemappingTester : MonoBehaviour
{
    public Transform[] threatTransforms;
    public Transform[] friendlyTransforms;
    public bool crouchTest;
    public QualityOfCoverEvaluationType type;

    public TacticalPoint tPointToTest;

    


    void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            //for (int j = 0; j < 100; j++)
            //{
                (Vector3 threatPosition, float distanceToThreat)[] threatsInfo = new (Vector3 threatPosition, float distanceToThreat)[threatTransforms.Length];
                (Vector3 friendlyPosition, float distanceToThreat)[] friendliesInfo = new (Vector3 threatPosition, float distanceToThreat)[friendlyTransforms.Length];

                for (int i = 0; i < threatsInfo.Length; i++)
                {
                    threatsInfo[i] = (threatTransforms[i].position, Vector3.Distance(threatTransforms[i].position, tPointToTest.GetPointPosition()));
                }

                for (int i = 0; i < friendliesInfo.Length; i++)
                {
                    friendliesInfo[i] = (friendlyTransforms[i].position, Vector3.Distance(friendlyTransforms[i].position, tPointToTest.GetPointPosition()));
                }

            tPointToTest.DetermineQualityOfCover(type, threatsInfo, friendliesInfo, crouchTest);
            //}

            
        }
    }

   


}

