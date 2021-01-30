using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoverQualityRemappingTester : MonoBehaviour
{
    //public Transform forwardToTest;
    public Transform[] threatTransforms;
    public Transform[] friendlyTransforms;
    public float distanceToTest;
    public bool crouchTest;
    public QualityOfCoverEvaluationType type;

    public TacticalPoint tPointToTest;

    public enum QualityOfCoverEvaluationType
    {
        Defensive,
        Moderate,
        Agressive
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
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

            DetermineQualityOfCover(type,threatsInfo, friendliesInfo, crouchTest);
        }
    }

    public float DetermineQualityOfCover(QualityOfCoverEvaluationType evaluationType, (Vector3 threatPosition, float distanceToThreat)[] threats, (Vector3 friendlyPosition, float distanceToFriendly)[] friendlies, bool crouching)
    {
        //prepare modifiers
        float endResult = 0;
        float plusPoints = 0;
        float minusPoints = 0;

        PointCoverRating coverRating = tPointToTest.coverRating;

        if (evaluationType == QualityOfCoverEvaluationType.Defensive)
        {
            float crouchedMultiplier;
            float standingMultiplier;

            //float distanceCheckMaxError = 0.5f;


            //always calculate for both standing & crocuhing, just the one not used right now, only gets half the pooints? or 0.33 vs 0.66 rating?
            if (crouching)
            {
                crouchedMultiplier = 0.66f;
                standingMultiplier = 0.33f;
            }
            else
            {
                crouchedMultiplier = 0.33f;
                standingMultiplier = 0.66f;
            }

            float threatsRating = RateThreatsDefensively(threats, coverRating, crouchedMultiplier, standingMultiplier);
            //Threats - add modifiers
            /*for (int i = 0; i < threats.Length; i++)
            {
                Vector3 directionTowardsThreat = threats[i].threatPosition - tPointToTest.GetPointPosition();
                int index = MapWorldSpaceDirectionToCoverRatingDirectionIndex(directionTowardsThreat);

                //crouched Rating
                if (threats[i].distanceToThreat > coverRating.crouchedDistanceRating[index])
                {
                    // coverBetweenPointAndThreat = true;
                    plusPoints += coverRating.crouchedQualityRating[index] * crouchedMultiplier;
                }
                else
                {
                    minusPoints += 1 * crouchedMultiplier; //if there is no cover, this is bad for the rating , also if there is no cover, the quality of the cover which isnt between e and the enemy but behing him doesnt matter
                }

                //standing Rating
                if (threats[i].distanceToThreat > coverRating.standingDistanceRating[index])
                {
                    // coverBetweenPointAndThreat = true;
                    plusPoints += coverRating.standingQualityRating[index] * standingMultiplier;
                }
                else
                {
                    minusPoints += 1 * standingMultiplier; //if there is no cover, this is bad for the rating
                }

            }*/

            float friendlyVisibilityRating = CalculateFriendliesVisibilityRating(friendlies, coverRating, crouchedMultiplier, standingMultiplier);
            //friendlies - add modifiers
            //for friendlies we want to see them when defensive, but modify with 0,5 cause theyre not soo important
            /*for (int i = 0; i < friendlies.Length; i++)
            {
                Vector3 directionTowardsFriendly = friendlies[i].friendlyPosition - tPointToTest.GetPointPosition();
                int index = MapWorldSpaceDirectionToCoverRatingDirectionIndex(directionTowardsFriendly);

                //crouched Rating
                if (friendlies[i].distanceToFriendly > coverRating.crouchedDistanceRating[index])
                {
                    // coverBetweenPointAndThreat = true;
                    minusPoints += coverRating.crouchedQualityRating[index] * crouchedMultiplier * 0.5f;
                }
                else
                {
                    plusPoints += 0.5f * crouchedMultiplier;
                }

                //standing Rating
                if (friendlies[i].distanceToFriendly > coverRating.standingDistanceRating[index])
                {
                    // coverBetweenPointAndThreat = true;
                    minusPoints += coverRating.standingQualityRating[index] * standingMultiplier * 0.5f;
                }
                else
                {
                    plusPoints += 0.5f * standingMultiplier;
                }
            }*/

            //endResult = plusPoints / (plusPoints + minusPoints);
            Debug.Log("threatsRating: " + threatsRating);
            Debug.Log("friendlyVisibilityRating: " + friendlyVisibilityRating);
            endResult = threatsRating * 0.75f + friendlyVisibilityRating*0.25f;
        }
        else if (evaluationType == QualityOfCoverEvaluationType.Moderate)
        {
            float crouchedMultiplier;
            float standingMultiplier;

            float threatsBehindCover = 0;
            float threatsNotBehindCover = 0;

            if (crouching)
            {
                crouchedMultiplier = 0.66f;
                standingMultiplier = 0.33f;
            }
            else
            {
                crouchedMultiplier = 0.33f;
                standingMultiplier = 0.66f;
            }
            //the same of defensive, but if the evaluated tPoint is a cover point, we lso evaluete its shooting points?

            //if cover point, we rate the cover point defensively and the shoting points moderately, else just rate moderately
            if (tPointToTest.tacticalPointType == TacticalPointType.CoverPoint)
            {
                //cover point is rated defensively, but only the crouched cover is rated or 66/33? again
                //shot paoints are rated 50/50 cover crocuh according to moderate formula
                // combine  cover points + shoot points 50/50? how to do this?
            }
            else
            {
                //just do one moderate evalueation with 66/33
                //Threats - add modifiers
                for (int i = 0; i < threats.Length; i++)
                {
                    Vector3 directionTowardsThreat = threats[i].threatPosition - tPointToTest.GetPointPosition();
                    int index = MapWorldSpaceDirectionToCoverRatingDirectionIndex(directionTowardsThreat);

                    //crouched Rating
                    if (threats[i].distanceToThreat > coverRating.crouchedDistanceRating[index])
                    {
                        // coverBetweenPointAndThreat = true;
                        threatsBehindCover += 1 * crouchedMultiplier;
                    }
                    else
                    {
                        threatsNotBehindCover += 1 * crouchedMultiplier;
                       // minusPoints += 1 * crouchedMultiplier; //if there is no cover, this is bad for the rating , also if there is no cover, the quality of the cover which isnt between e and the enemy but behing him doesnt matter
                    }

                    //standing Rating
                    if (threats[i].distanceToThreat > coverRating.standingDistanceRating[index])
                    {
                        // coverBetweenPointAndThreat = true;
                        // plusPoints += coverRating.standingQualityRating[index] * standingMultiplier;
                        threatsBehindCover += 1 * standingMultiplier;
                    }
                    else
                    {
                        //minusPoints += 1 * standingMultiplier; //if there is no cover, this is bad for the rating
                        threatsNotBehindCover += 1 * standingMultiplier;
                    }

                }

                //friendlies - add modifiers
                //for friendlies we want to see them when defensive, but modify with 0,5 cause theyre not soo important
                for (int i = 0; i < friendlies.Length; i++)
                {
                    Vector3 directionTowardsFriendly = friendlies[i].friendlyPosition - tPointToTest.GetPointPosition();
                    int index = MapWorldSpaceDirectionToCoverRatingDirectionIndex(directionTowardsFriendly);

                    //crouched Rating
                    if (friendlies[i].distanceToFriendly > coverRating.crouchedDistanceRating[index])
                    {
                        // coverBetweenPointAndThreat = true;
                        minusPoints += coverRating.crouchedQualityRating[index] * crouchedMultiplier * 0.5f;
                    }
                    else
                    {
                        plusPoints += 0.5f * crouchedMultiplier;
                    }

                    //standing Rating
                    if (friendlies[i].distanceToFriendly > coverRating.standingDistanceRating[index])
                    {
                        // coverBetweenPointAndThreat = true;
                        minusPoints += coverRating.standingQualityRating[index] * standingMultiplier * 0.5f;
                    }
                    else
                    {
                        plusPoints += 0.5f * standingMultiplier;
                    }
                }

                if(threatsNotBehindCover > 0.5f && threatsNotBehindCover < 2f)
                {
                    plusPoints += 3;
                }

                endResult = plusPoints / (plusPoints + minusPoints);
            }
        }
        else if (evaluationType == QualityOfCoverEvaluationType.Agressive)
        {
            //the ost performant analysis? - just checks for the desired position, visible enemy gives plus points

            //if cover point, wo only rate the shooting points agressively, else just rate agressive
            //wants to have vision to as many enemies as possible

            if (tPointToTest.tacticalPointType == TacticalPointType.CoverPoint)
            {
                for (int j = 0; j < tPointToTest.coverShootPoints.Length; j++)
                {
                    coverRating = tPointToTest.coverShootPoints[j].coverRating;

                    for (int i = 0; i < threats.Length; i++)
                    {
                        Vector3 directionTowardsThreat = threats[i].threatPosition - tPointToTest.coverShootPoints[j].GetPointPosition();
                        int index = MapWorldSpaceDirectionToCoverRatingDirectionIndex(directionTowardsThreat);

                        if (crouching)
                        {
                            if (threats[i].distanceToThreat > coverRating.crouchedDistanceRating[index])
                            {
                                // coverBetweenPointAndThreat = true;
                                minusPoints += coverRating.crouchedQualityRating[index];
                            }
                            else
                            {
                                plusPoints += 1; //if there is no cover, this is bad for the rating , also if there is no cover, the quality of the cover which isnt between e and the enemy but behing him doesnt matter
                            }
                        }
                        else
                        {
                            //standing Rating
                            if (threats[i].distanceToThreat > coverRating.standingDistanceRating[index])
                            {
                                // coverBetweenPointAndThreat = true;
                                minusPoints += coverRating.standingQualityRating[index];
                            }
                            else
                            {
                                plusPoints += 1; //if there is no cover, this is bad for the rating
                            }
                        }


                    }
                }
            }
            else
            {
                for (int i = 0; i < threats.Length; i++)
                {
                    Vector3 directionTowardsThreat = threats[i].threatPosition - tPointToTest.GetPointPosition();
                    int index = MapWorldSpaceDirectionToCoverRatingDirectionIndex(directionTowardsThreat);

                    if (crouching)
                    {
                        if (threats[i].distanceToThreat > coverRating.crouchedDistanceRating[index])
                        {
                            // coverBetweenPointAndThreat = true;
                            minusPoints += coverRating.crouchedQualityRating[index];
                        }
                        else
                        {
                            plusPoints += 1; //if there is no cover, this is bad for the rating , also if there is no cover, the quality of the cover which isnt between e and the enemy but behing him doesnt matter
                        }
                    }
                    else
                    {
                        //standing Rating
                        if (threats[i].distanceToThreat > coverRating.standingDistanceRating[index])
                        {
                            // coverBetweenPointAndThreat = true;
                            minusPoints += coverRating.standingQualityRating[index];
                        }
                        else
                        {
                            plusPoints += 1; //if there is no cover, this is bad for the rating
                        }
                    }


                }
            }

            endResult = plusPoints / (plusPoints + minusPoints);
        }

       


        //calculate end result

        
        Debug.Log("endResult: " + endResult);
        return endResult;



        //3. now depending on the type of evaluation, rate the situation accordingly
        //defensive cover, wants to have all threats befind cover and with best quality





        //return 0;
    }

    //void AddPoints(ref plusPoints, ref minusPoints, float distanceToTarget, float coverRatingDistance, plusPointsToAdd, minusPoints)

    int MapWorldSpaceDirectionToCoverRatingDirectionIndex(Vector3 worldSpaceDirection)
    {
        worldSpaceDirection.y = 0;

        //float signedAngle = Vector3.SignedAngle(Vector3.forward, directionTowardsThreat, Vector3.up); //like this or better use the tactical points directions instead?
        float signedAngle = Vector3.SignedAngle(tPointToTest.transform.forward, worldSpaceDirection, tPointToTest.transform.up);

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

        //Debug.Log("signed angle: " + signedAngle);
        //Debug.Log("intex mapped to: " + (int)(signedAngle / 45f));


        return (int)(signedAngle / 45f);
    }

    float RateThreatsDefensively((Vector3 threatPosition, float distanceToThreat)[] threats, PointCoverRating coverRating, float crouchedMultiplier, float standingMultiplier)
    {
        float rating = 0;

        for (int i = 0; i < threats.Length; i++)
        {
            Vector3 directionTowardsThreat = threats[i].threatPosition - tPointToTest.GetPointPosition();
            int index = MapWorldSpaceDirectionToCoverRatingDirectionIndex(directionTowardsThreat);

            //crouched Rating
            if (threats[i].distanceToThreat > coverRating.crouchedDistanceRating[index])
            {
                // coverBetweenPointAndThreat = true;
                //plusPoints += coverRating.crouchedQualityRating[index] * crouchedMultiplier;
                rating += coverRating.crouchedQualityRating[index] * crouchedMultiplier;
            }
            //else
            //{
               // minusPoints += 1 * crouchedMultiplier; //if there is no cover, this is bad for the rating , also if there is no cover, the quality of the cover which isnt between e and the enemy but behing him doesnt matter
            //}

            //standing Rating
            if (threats[i].distanceToThreat > coverRating.standingDistanceRating[index])
            {
                // coverBetweenPointAndThreat = true;
                // plusPoints += coverRating.standingQualityRating[index] * standingMultiplier;
                rating += coverRating.standingQualityRating[index] * standingMultiplier;
            }
            //else
            //{
             //   minusPoints += 1 * standingMultiplier; //if there is no cover, this is bad for the rating
            //}

        }

        return rating / (threats.Length * 1f);
    }

    float CalculateFriendliesVisibilityRating((Vector3 friendlyPosition, float distanceToFriendly)[] friendlies, PointCoverRating coverRating, float crouchedMultiplier, float standingMultiplier)
    {
        float rating = 0;

        for (int i = 0; i < friendlies.Length; i++)
        {
            Vector3 directionTowardsFriendly = friendlies[i].friendlyPosition - tPointToTest.GetPointPosition();
            int index = MapWorldSpaceDirectionToCoverRatingDirectionIndex(directionTowardsFriendly);

            //crouched Rating
            if (friendlies[i].distanceToFriendly < coverRating.crouchedDistanceRating[index])
            {
                // coverBetweenPointAndThreat = true;
                //minusPoints += coverRating.crouchedQualityRating[index] * crouchedMultiplier * 0.5f;
                Debug.Log("friendly crouching + " + crouchedMultiplier);

                rating += 1 * crouchedMultiplier;
            }
            //else
            //{
            //    plusPoints += 0.5f * crouchedMultiplier;
            //}

            //standing Rating
            if (friendlies[i].distanceToFriendly < coverRating.standingDistanceRating[index])
            {
                // coverBetweenPointAndThreat = true;
                //minusPoints += coverRating.standingQualityRating[index] * standingMultiplier * 0.5f;
                Debug.Log("friendly standing + " + standingMultiplier);
                rating += 1 * standingMultiplier;
            }
            //else
            //{
            //    plusPoints += 0.5f * standingMultiplier;
            //}
        }

        return rating/ friendlies.Length;
    }

    //the same for agressive cover & moderate/neutral cover & but also some line of sight rating

}

/*for (int i = 0; i < threats.Length; i++)
        {
            Debug.Log("-------------Threat num: " + i);
            Vector3 directionTowardsThreat = threats[i].threatPosition - tPointToTest.GetPointPosition();
            int index = MapWorldSpaceDirectionToCoverRatingDirectionIndex(directionTowardsThreat);
            Debug.Log("index: " + index);

            Debug.Log("Crouched------------");
            //crouched Rating
            Debug.Log("threats[i].distanceToThreat: " + threats[i].distanceToThreat);
            Debug.Log(">?");
            Debug.Log("coverRating.crouchedDistanceRating[index]: " + coverRating.crouchedDistanceRating[index]);
            if (threats[i].distanceToThreat > coverRating.crouchedDistanceRating[index])
            {
                // coverBetweenPointAndThreat = true;
                Debug.Log("threat was behind cover -> + " + (coverRating.crouchedQualityRating[index] * crouchedMultiplier) + " plusPoints");
                plusPoints += coverRating.crouchedQualityRating[index] * crouchedMultiplier;
            }
            else
            {
                Debug.Log("threat was not behind cover -> - " + crouchedMultiplier + " minusPoints");
                minusPoints += 1 * crouchedMultiplier; //if there is no cover, this is bad for the rating , also if there is no cover, the quality of the cover which isnt between e and the enemy but behing him doesnt matter
            }

            Debug.Log("Standing------------");
            //standing Rating
            Debug.Log("threats[i].distanceToThreat: " + threats[i].distanceToThreat);
            Debug.Log(">?");
            Debug.Log("coverRating.crouchedDistanceRating[index]: " + coverRating.standingDistanceRating[index]);
            if (threats[i].distanceToThreat > coverRating.standingDistanceRating[index])
            {
                // coverBetweenPointAndThreat = true;
                Debug.Log("threat was behind cover -> + " + (coverRating.standingQualityRating[index] * standingMultiplier) + " plusPoints");
                plusPoints += coverRating.standingQualityRating[index] * standingMultiplier;
            }
            else
            {
                Debug.Log("threat was not behind cover -> - " + standingMultiplier + " minusPoints");
                minusPoints += 1 * standingMultiplier; //if there is no cover, this is bad for the rating
            }

        }


        //friendlies - add modifiers
        //for friendlies we want to see them when defensive, but modify with 0,5 cause theyre not soo important
        for (int i = 0; i < friendlies.Length; i++)
        {
            Debug.Log("-------------Friendly num: " + i);


            Vector3 directionTowardsFriendly = friendlies[i].friendlyPosition - tPointToTest.GetPointPosition();
            int index = MapWorldSpaceDirectionToCoverRatingDirectionIndex(directionTowardsFriendly);
            Debug.Log("index: " + index);

            Debug.Log("Crouched------------");

            Debug.Log("friendlies[i].distanceToFriendly " + friendlies[i].distanceToFriendly);
            Debug.Log(">?");
            Debug.Log("coverRating.crouchedDistanceRating[index]: " + coverRating.crouchedDistanceRating[index]);
            //crouched Rating
            if (friendlies[i].distanceToFriendly > coverRating.crouchedDistanceRating[index])
            {
                // coverBetweenPointAndThreat = true;
                Debug.Log("friendly was behind cover -> - " + (coverRating.crouchedQualityRating[index] * crouchedMultiplier * 0.5f) + " minusPoints");
                minusPoints += coverRating.crouchedQualityRating[index] * crouchedMultiplier * 0.5f;
            }
            else
            {
                Debug.Log("friendly was not behind cover -> + " + (0.5f * crouchedMultiplier) + " plusPoints");
                plusPoints += 0.5f * crouchedMultiplier; 
            }

            Debug.Log("Standing------------");
            Debug.Log("friendlies[i].distanceToFriendly " + friendlies[i].distanceToFriendly);
            Debug.Log(">?");
            Debug.Log("coverRating.crouchedDistanceRating[index]: " + coverRating.standingDistanceRating[index]);
            //standing Rating
            if (friendlies[i].distanceToFriendly > coverRating.standingDistanceRating[index])
            {
                // coverBetweenPointAndThreat = true;
                Debug.Log("friendly was behind cover -> - " + (coverRating.standingQualityRating[index] * standingMultiplier * 0.5f) + " minusPoints");
                minusPoints += coverRating.standingQualityRating[index] * standingMultiplier * 0.5f;
            }
            else
            {
                Debug.Log("friendly was not behind cover -> + " + (0.5f * standingMultiplier) + " plusPoints");
                plusPoints += 0.5f * standingMultiplier; 
            }

        }*/
