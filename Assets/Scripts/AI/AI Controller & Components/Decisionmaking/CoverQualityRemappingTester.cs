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

            DetermineQualityOfCover(type, threatsInfo, friendliesInfo, crouchTest);
        }
    }

    public float DetermineQualityOfCover(QualityOfCoverEvaluationType evaluationType, (Vector3 threatPosition, float distanceToThreat)[] threats, (Vector3 friendlyPosition, float distanceToFriendly)[] friendlies, bool crouching)
    {
        //prepare modifiers
        float endResult = 0;
        float plusPoints = 0;
        float minusPoints = 0;

        float crouchedMultiplier;
        float standingMultiplier;

        PointCoverRating coverRating = tPointToTest.coverRating;

        if (evaluationType == QualityOfCoverEvaluationType.Defensive)
        {
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

            float friendlyVisibilityRating = CalculateFriendliesVisibilityRating(friendlies, coverRating, crouchedMultiplier, standingMultiplier);

            Debug.Log("threatsRating: " + threatsRating);
            Debug.Log("friendlyVisibilityRating: " + friendlyVisibilityRating);
            endResult = threatsRating * 0.75f + friendlyVisibilityRating * 0.25f;
        }
        else if (evaluationType == QualityOfCoverEvaluationType.Moderate)
        {
            

            float threatsRating = 0;
            float threatsRatingCoverShootPoints = 0;
            float friendlyVisibilityRating = 0;
            float friendlyVisibilityRatingCoverShootPoints = 0;

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

                for (int j = 0; j < tPointToTest.coverShootPoints.Length; j++)
                {
                    threatsRatingCoverShootPoints += RateThreatsModerately(threats, tPointToTest.coverShootPoints[j].coverRating, crouchedMultiplier, standingMultiplier);
                    friendlyVisibilityRatingCoverShootPoints += CalculateFriendliesVisibilityRating(friendlies, tPointToTest.coverShootPoints[j].coverRating, crouchedMultiplier, standingMultiplier);
                }

                threatsRatingCoverShootPoints = threatsRatingCoverShootPoints / (tPointToTest.coverShootPoints.Length * 1f);
                friendlyVisibilityRatingCoverShootPoints = friendlyVisibilityRatingCoverShootPoints / (tPointToTest.coverShootPoints.Length * 1f);

                threatsRating = RateThreatsModerately(threats, coverRating, crouchedMultiplier, standingMultiplier)*0.5f + threatsRatingCoverShootPoints*0.5f;
                friendlyVisibilityRating = CalculateFriendliesVisibilityRating(friendlies, coverRating, crouchedMultiplier, standingMultiplier)*0.5f + friendlyVisibilityRatingCoverShootPoints*0.5f;

            }
            else
            {
                threatsRating = RateThreatsModerately(threats, coverRating, crouchedMultiplier, standingMultiplier);
                friendlyVisibilityRating = CalculateFriendliesVisibilityRating(friendlies, coverRating, crouchedMultiplier, standingMultiplier);
            }

            endResult = threatsRating * 0.75f + friendlyVisibilityRating * 0.25f;

        }
        else if (evaluationType == QualityOfCoverEvaluationType.Agressive)
        {
            if (crouching)
            {
                crouchedMultiplier = 0.75f;
                standingMultiplier = 0.25f;
            }
            else
            {
                crouchedMultiplier = 0.25f;
                standingMultiplier = 0.75f;
            }

            float threatsRating = 0;
            float friendlyVisibilityRating = 0;

            //the ost performant analysis? - just checks for the desired position, visible enemy gives plus points
            //if cover point, wo only rate the shooting points agressively, else just rate agressive
            //wants to have vision to as many enemies as possible

            if (tPointToTest.tacticalPointType == TacticalPointType.CoverPoint)
            {
              
                for (int j = 0; j < tPointToTest.coverShootPoints.Length; j++)
                {
                    threatsRating += RateThreatsAgressively(threats, tPointToTest.coverShootPoints[j].coverRating, crouchedMultiplier, standingMultiplier);
                    friendlyVisibilityRating += CalculateFriendliesVisibilityRating(friendlies, tPointToTest.coverShootPoints[j].coverRating, crouchedMultiplier, standingMultiplier);
                }

                //dont divide threatsRating, cause if all of them together have rataing 1, its good enough
                if (threatsRating > 1) threatsRating = 1;
                friendlyVisibilityRating = friendlyVisibilityRating / (tPointToTest.coverShootPoints.Length * 1f);
            }
            else
            {
                 threatsRating = RateThreatsAgressively(threats, coverRating, crouchedMultiplier, standingMultiplier);
                 friendlyVisibilityRating = CalculateFriendliesVisibilityRating(friendlies, coverRating, crouchedMultiplier, standingMultiplier); 
            }

            Debug.Log("threatsRating: " + threatsRating);
            Debug.Log("friendlyVisibilityRating: " + friendlyVisibilityRating);
            endResult = threatsRating * 0.85f + friendlyVisibilityRating * 0.15f;
        }






        Debug.Log("endResult: " + endResult);
        return endResult;
    }


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

    float RateThreatsAgressively((Vector3 threatPosition, float distanceToThreat)[] threats, PointCoverRating coverRating, float crouchedMultiplier, float standingMultiplier)
    {
        //the more enemies are not behind cover and visible to be shot, the better
        float rating = 0;
        Debug.Log("rate threats agressively----------------------------------");
        for (int i = 0; i < threats.Length; i++)
        {
            Vector3 directionTowardsThreat = threats[i].threatPosition - tPointToTest.GetPointPosition();
            int index = MapWorldSpaceDirectionToCoverRatingDirectionIndex(directionTowardsThreat);

            //crouched Rating
            if (threats[i].distanceToThreat < coverRating.crouchedDistanceRating[index])
            {
                rating += 1 * crouchedMultiplier;
                Debug.Log("rating += " + 1 * crouchedMultiplier);
            }

            //standing Rating
            if (threats[i].distanceToThreat < coverRating.standingDistanceRating[index])
            {
                rating += 1 * standingMultiplier;
                Debug.Log("rating += " + 1 * standingMultiplier);

            }
        }

        Debug.Log("rate threats agressively rating: " + (rating / (threats.Length * 1f)));
        return rating / (threats.Length * 1f);
    }

    float RateThreatsDefensively((Vector3 threatPosition, float distanceToThreat)[] threats, PointCoverRating coverRating, float crouchedMultiplier, float standingMultiplier)
    {
        //the more enemies are behind cover, the better
        float rating = 0;

       

        for (int i = 0; i < threats.Length; i++)
        {
            Vector3 directionTowardsThreat = threats[i].threatPosition - tPointToTest.GetPointPosition();
            int index = MapWorldSpaceDirectionToCoverRatingDirectionIndex(directionTowardsThreat);

            //crouched Rating
            if (threats[i].distanceToThreat > coverRating.crouchedDistanceRating[index])
            {
                // coverBetweenPointAndThreat = true;
                rating += coverRating.crouchedQualityRating[index] * crouchedMultiplier;
            }


            //standing Rating
            if (threats[i].distanceToThreat > coverRating.standingDistanceRating[index])
            {
                // coverBetweenPointAndThreat = true;
                rating += coverRating.standingQualityRating[index] * standingMultiplier;
            }
        }

        return rating / (threats.Length * 1f);
    }

    float RateThreatsModerately((Vector3 threatPosition, float distanceToThreat)[] threats, PointCoverRating coverRating, float crouchedMultiplier, float standingMultiplier)
    {
        //ideally 1-2 enemies are visible
        float rating = 0;

        float threatsBehindCover = 0;
        float threatsNotBehindCover = 0;

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
                threatsBehindCover += 1 * standingMultiplier;
            }
            else
            {
                //minusPoints += 1 * standingMultiplier; //if there is no cover, this is bad for the rating
                threatsNotBehindCover += 1 * standingMultiplier;
            }

        }

        if (threatsNotBehindCover > 0.5f)
        {
            if (threatsNotBehindCover < 1.2f)
            {
                rating = 1;
            }
            else if (threatsNotBehindCover < 2.2f)
            {
                rating = 0.6f;
            }
            else if (threatsNotBehindCover < 3.2f)
            {
                rating = 0.3f;
            }
        }
        

        return rating;
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

        return rating / friendlies.Length;
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
