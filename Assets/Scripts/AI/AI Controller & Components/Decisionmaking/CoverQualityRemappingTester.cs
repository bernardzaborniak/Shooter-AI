using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoverQualityRemappingTester : MonoBehaviour
{
    public Transform forwardToTest;
    public float distanceToTest;
    public bool crouchTest;

    public TacticalPoint tPointToTest;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            DetermineQualityOfDefensiveCover(forwardToTest.forward, distanceToTest, crouchTest);
        }
    }

    public float DetermineQualityOfDefensiveCover(Vector3 directionTowardsThreat, float distanceToThreat, bool crouching)
    {
        //if(crouching)

        //first determin which index of ratin to use, global transform.forward points towards index 0 -forward points wowards index 4 - indexes go around like a clock .

        //cut the y direction
        directionTowardsThreat.y = 0;

        //float signedAngle = Vector3.SignedAngle(Vector3.forward, directionTowardsThreat, Vector3.up); //like this or better use the tactical points directions instead?
        float signedAngle = Vector3.SignedAngle(tPointToTest.transform.forward, directionTowardsThreat, tPointToTest.transform.up);

        //remap the angle to 360, and shift a little cause 0 index goes in both directions
        if (signedAngle < 0)
        {
            signedAngle += 360f + 22.5f;

            if (signedAngle > 360)
            {
                signedAngle -= 360;
            }
        }
        else
        {
            signedAngle += 22.5f;
        }


        int index = (int)(signedAngle / 45f);
        Debug.Log("signed angle: " + signedAngle);
        Debug.Log("intex mapped to: " + index);
        //return 1;
        return 0;
    }
}
