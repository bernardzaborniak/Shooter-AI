﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public enum TacticalPointType
{
    OpenFieldPoint,
    CoverPoint,
    CoverPeekPoint
}

//for evaluating rating
public enum QualityOfCoverEvaluationType
{
    Defensive1,
    Moderate1,
    Agressive1,
    Defensive2,
    Moderate2,
    Agressive2,
    Simple
}

[ExecuteInEditMode]
public class TacticalPoint : MonoBehaviour
{
    #region Fields 

    public TacticalPointType tacticalPointType;
    public GameEntity entityUsingThisPoint;
    public GameEntity entityTargetingThisPoint;

    [Tooltip("only used if  type is CoverShootPoint")]
    [ShowWhen("tacticalPointType", TacticalPointType.CoverPoint)]
    public TacticalPoint[] correspondingCoverPeekPoints; //or ShotPositions
    [ShowWhen("tacticalPointType", TacticalPointType.CoverPoint)]
    public Transform coverPeekPointsParent;
    [ShowWhen("tacticalPointType", TacticalPointType.CoverPeekPoint)]
    public TacticalPoint correspondingCoverPoint;

    [Tooltip("not really used yet")]
    public float radius;
   // public int capacity;

    public Renderer pointRenderer;
    public MeshFilter meshFilter;
    public Mesh standingMesh;
    public Mesh crouchedMesh;



    public enum Type
    {
        Standing,
        Crouched
    }

    [Space(5)]
    public Type type;

    // [Space(5)]
    //for now we only use this to test the character controller
    // public int stanceType; //0 is standing, 1 is crouching


    #region Fields for Calculating Rating

    //calculate cover quality algorythm
    Vector3[] raycastDirectionsInLocalSpace =
    {
        new Vector3(0,0,1),
        new Vector3(1,0,1),
        new Vector3(1,0,0),
        new Vector3(1,0,-1),
        new Vector3(0,0,-1),
        new Vector3(-1,0,-1),
        new Vector3(-1,0,0),
        new Vector3(-1,0,1),
    };

    [System.NonSerialized] public PointCastRaysContainer raycastsUsedForGeneratingRating = new PointCastRaysContainer();
    [System.NonSerialized] public PointCoverRating coverRating = new PointCoverRating();

    [SerializeField] int pointReferenceID; //used as reference in the serialized data of the scriptable object the manager communicates to.

    float distanceModeMaxDifferenceToGroup = 1f; //change this to change algorythm results
    class CalculateDistanceModeAlgorythmGroup
    {
        float firstValueInGroup;
        List<float> valuesInGroup;

        public CalculateDistanceModeAlgorythmGroup(float firstValueInGroup)
        {
            this.firstValueInGroup = firstValueInGroup;
            valuesInGroup = new List<float>();
            valuesInGroup.Add(firstValueInGroup);
        }

        public float GetDistanceToGroup(float value)
        {
            //Debug.Log("[[[[[[[get disatcne: " + value + " & " + firstValueInGroup);
            //Debug.Log("[[[[[[[return : " + Mathf.Abs(firstValueInGroup - value));
            return Mathf.Abs(firstValueInGroup - value);
        }

        public bool IsFirstValueInfinity()
        {
            return firstValueInGroup == Mathf.Infinity;
        }

        public void AddValueToGroup(float value)
        {
            valuesInGroup.Add(value);
        }

        public float GetMeanValueOfGroup()
        {
            float allValuesCombined = 0;

            foreach (float value in valuesInGroup)
            {
                allValuesCombined += value;
            }

            return allValuesCombined / (valuesInGroup.Count * 1f);
        }

        public int GetGroupSize()
        {
            return valuesInGroup.Count;
        }

        //for debug only
        public List<float> GetGroupValues()
        {
            return valuesInGroup;
        }

        public float GetGreatestDistance()
        {
            float biggestDistance = 0;

            for (int i = 0; i < valuesInGroup.Count; i++)
            {
                if(valuesInGroup[i] > biggestDistance)
                {
                    biggestDistance = valuesInGroup[i];
                }
            }

            return biggestDistance;
        }

    }

    #endregion

   

    #endregion

    void OnEnable()
    {
        TacticalPointsManager.Instance.AddTacticalPoint(this);
    }

    void OnDisable()
    {
        TacticalPointsManager.Instance.RemoveTacticalPoint(this);
    }

    #region Used In Edit Mode
#if UNITY_EDITOR
    public void UpdateCoverShootPoints()
    {
        if(type == Type.Crouched)
        {
            meshFilter.mesh = crouchedMesh;
        }
        else if(type == Type.Standing)
        {
            meshFilter.mesh = standingMesh;
        }

        //Updates the references of children instantiated in editor by the level Designer
        if (tacticalPointType == TacticalPointType.CoverPoint)
        {
            correspondingCoverPeekPoints = new TacticalPoint[coverPeekPointsParent.childCount];
            for (int i = 0; i < coverPeekPointsParent.childCount; i++)
            {
                TacticalPoint tp = coverPeekPointsParent.GetChild(i).GetComponent<TacticalPoint>();
                if (tp)
                {
                    correspondingCoverPeekPoints[i] = tp;
                    correspondingCoverPeekPoints[i].correspondingCoverPoint = this;
                    EditorUtility.SetDirty(correspondingCoverPeekPoints[i]);

                }
                else
                {
                    Debug.Log("children of coverShootPointsParent on: " + gameObject.name + " is not a tactical point -> fix this");
                }
            }
            EditorUtility.SetDirty(this);
        }
    }
#endif

    public void ResetRotation()
    {
        if (tacticalPointType == TacticalPointType.CoverPoint)
        {
            //save old positions
            Vector3[] oldPositions = new Vector3[correspondingCoverPeekPoints.Length];
            for (int i = 0; i < oldPositions.Length; i++)
            {
                oldPositions[i] = correspondingCoverPeekPoints[i].transform.position;
            }

            //rotate parent
            transform.rotation = Quaternion.identity;

            //restore old posiitons
            for (int i = 0; i < oldPositions.Length; i++)
            {
                correspondingCoverPeekPoints[i].transform.position = oldPositions[i];
                correspondingCoverPeekPoints[i].transform.rotation = Quaternion.identity;
            }
        }
        else
        {
            transform.rotation = Quaternion.identity;
        }
    }

    #endregion

    #region Used For Baking & Setting Up Ratings

#if UNITY_EDITOR
    public void SetPointReferenceID(int id)
    {
        pointReferenceID = id;

        EditorUtility.SetDirty(this);
    }
#endif

    public int GetPointReferenceID()
    {
        return pointReferenceID;
    }

#if UNITY_EDITOR
    public (PointCoverRating pointCoverRating, PointCastRaysContainer pointCastRaysContainer) BakeCoverRatings(ref PointCoverRating pointCoverRating, ref PointCastRaysContainer pointCastRaysContainer, float crouchedHeight, float standingHeight, int raycastFactor, LayerMask raycastLayerMask, float maxRayLength)
    {
        int numberOfRaycastsPerDirection = raycastFactor * raycastFactor;
        pointCastRaysContainer.SetUpRays(numberOfRaycastsPerDirection);

        //p 0 is crouched, p 1 is standing
        // Go through once for standing and once for crouching.
        for (int p = 0; p < 2; p++)
        {
            // Go through every of the 8 directions
            for (int i = 0; i < 8; i++)
            {
                #region Determine start point depending if standing or crouching and on radius

                Vector3 rayStartPoint = Vector3.zero;
                if (p == 0)
                {
                    rayStartPoint = transform.position + new Vector3(0, crouchedHeight, 0) + Random.insideUnitSphere * radius;
                }
                else if (p == 1)
                {
                    rayStartPoint = transform.position + new Vector3(0, standingHeight, 0) + Random.insideUnitSphere * radius;
                }

                #endregion

                #region Cast and Fill raycastsUsedForCurrentDirection with raycasts based on the grid (raycastFactor*raycastFactor)

                // Use A Grid for casting the raycasts.
                float directionGridSize = 45 / raycastFactor;
                int r = 0;
                for (float x = -22.5f; x < 22.5; x = x + directionGridSize)
                {
                    for (float y = -22.5f; y < 22.5; y = y + directionGridSize)
                    {
                        // Take a forward transform, rotate it by the grids y and x offset, later roatte it around y axies - the quaternion order of application is inverse (right to left).
                        Vector3 rotatedRaycastDirectionInLocalSpace = Quaternion.AngleAxis(i * 45, transform.up) * Quaternion.Euler(x + directionGridSize / 2, y + directionGridSize / 2, 0f) * Vector3.forward;

                        // This could be left out if the point would always be aligned to the world forward.
                        Vector3 raycastDirectionInWorldSpace = transform.TransformDirection(rotatedRaycastDirectionInLocalSpace);

                        RaycastHit hit;
                        if (Physics.Raycast(rayStartPoint, raycastDirectionInWorldSpace, out hit, maxRayLength, raycastLayerMask, QueryTriggerInteraction.Ignore))
                        {
                            pointCastRaysContainer.SetRay(p, i, r, new RaycastUsedToGenerateCoverRating(rayStartPoint, hit.point));
                        }
                        else
                        {
                            pointCastRaysContainer.SetRay(p, i, r, new RaycastUsedToGenerateCoverRating(rayStartPoint, rayStartPoint + raycastDirectionInWorldSpace * 100, true));

                        }
                        r++;
                    }
                }

                #endregion

                #region Calculate the DistanceRating

                //Create the sorted array of raycasts
                RaycastUsedToGenerateCoverRating[] raycastUsedSortedByDistance = new RaycastUsedToGenerateCoverRating[numberOfRaycastsPerDirection];
                for (int n = 0; n < numberOfRaycastsPerDirection; n++)
                {
                    raycastUsedSortedByDistance[n] = pointCastRaysContainer.GetRay(p, i, n);
                }

                //Sort the array of raycasts based on distance
                System.Array.Sort(raycastUsedSortedByDistance,
                delegate (RaycastUsedToGenerateCoverRating x, RaycastUsedToGenerateCoverRating y) { return x.distance.CompareTo(y.distance); });


                //The modified mode algorythm  - apparently it doesnt work well with the new cover rating :(
                CalculateDistanceModeAlgorythmGroup currentGroup = null;
                List<CalculateDistanceModeAlgorythmGroup> groupsUsed = new List<CalculateDistanceModeAlgorythmGroup>();

                for (int n = 0; n < numberOfRaycastsPerDirection; n++)
                {
                    if(currentGroup != null)
                    {
                        //check the distance to the currentGroup
                        if(currentGroup.GetDistanceToGroup(raycastUsedSortedByDistance[n].distance) < distanceModeMaxDifferenceToGroup)
                        {
                            if(raycastUsedSortedByDistance[n].distance != Mathf.Infinity)
                            {
                                currentGroup.AddValueToGroup(raycastUsedSortedByDistance[n].distance);
                            }
                        }
                        else
                        {
                            if (currentGroup.IsFirstValueInfinity()) //distance between infinity and infinity is infinity, but we still want to group them together
                            {
                                currentGroup.AddValueToGroup(raycastUsedSortedByDistance[n].distance);
                            }
                            else
                            {
                                currentGroup = null;
                            }
                        }
                    }

                    if(currentGroup == null)
                    {
                        currentGroup = new CalculateDistanceModeAlgorythmGroup(raycastUsedSortedByDistance[n].distance);
                        groupsUsed.Add(currentGroup);
                    }                 
                }


                //Get the biggestGroup & their mean value is the distance we will use
                CalculateDistanceModeAlgorythmGroup biggestGroup = null;
                int biggestGroupSize = 0;
                for (int n = 0; n < groupsUsed.Count; n++)
                {
                    if(groupsUsed[n].GetGroupSize()> biggestGroupSize)
                    {
                        biggestGroupSize = groupsUsed[n].GetGroupSize();
                        biggestGroup = groupsUsed[n];
                    }
                }

                float distanceRating = biggestGroup.GetMeanValueOfGroup();


                #endregion

                #region Calculate the QualityRating
                float qualityRating;

                if (distanceRating == Mathf.Infinity)
                {
                    qualityRating = 0;
                }
                else
                {
                    //check how many raycasts are shorter than the distance - the percentage is the quality rating
                    float maxDistanceFromSelectedDistances = biggestGroup.GetGreatestDistance();
                    int amountOfRacastsShorterThanTheDistanceRating = 0;

                    for (int n = 0; n < numberOfRaycastsPerDirection; n++)
                    {
                        if (raycastUsedSortedByDistance[n].distance <= maxDistanceFromSelectedDistances)
                        {
                            amountOfRacastsShorterThanTheDistanceRating++;
                        }
                        else
                        {
                            break;
                        }
                    }

                    qualityRating = (1f * amountOfRacastsShorterThanTheDistanceRating) / numberOfRaycastsPerDirection;
                }

                


                /* float averageAbsoluteDeviation;
                 float allDeviationsCombined = 0;


                 for (int n = 0; n < numberOfRaycastsPerDirection; n++)
                 {
                     //if (!raycastsUsedForGeneratingRating.GetRay(p, i, n).IsInfinite())
                     if (!pointCastRaysContainer.GetRay(p, i, n).IsInfinite())
                     {
                         // allDeviationsCombined += Mathf.Abs(raycastsUsedForGeneratingRating.GetRay(p, i, n).distance - meanDistance);
                         allDeviationsCombined += Mathf.Abs(pointCastRaysContainer.GetRay(p, i, n).distance - meanDistance);
                     }
                     else
                     {
                         allDeviationsCombined += maxRayLength - meanDistance;
                     }
                 }

                 //averageAbsoluteDeviation = allDeviationsCombined / numberOfRaycastsPerDirection;

                 if (numberOfRaycastsWhichAreNotInfinite == 0)
                 {
                     averageAbsoluteDeviation = maxRayLength;//Mathf.Infinity;
                 }
                 else
                 {
                     //averageAbsoluteDeviation = allDeviationsCombined / numberOfRaycastsWhichAreNotInfinite;
                     averageAbsoluteDeviation = allDeviationsCombined / numberOfRaycastsPerDirection;
                 }

                 float qualityRating = Utility.Remap(averageAbsoluteDeviation, maxRayLength, 0, 0, 1);*/

                #endregion

                #region Set the Rating

                if (p == 0)
                {
                    //pointCoverRating.crouchedDistanceRating[i] = meanDistance;
                    pointCoverRating.crouchedDistanceRating[i] = distanceRating;
                    pointCoverRating.crouchedQualityRating[i] = qualityRating;
                }
                else if (p == 1)
                {
                    //pointCoverRating.standingDistanceRating[i] = meanDistance;
                    pointCoverRating.standingDistanceRating[i] = distanceRating;
                    pointCoverRating.standingQualityRating[i] = qualityRating;
                }

                #endregion
            }
        }

        return (pointCoverRating, pointCastRaysContainer);
    }
#endif

    public void UpdateRatings(PointCoverRating pointCoverRating, PointCastRaysContainer pointCastRaysContainer)
    {
        coverRating = pointCoverRating;
        raycastsUsedForGeneratingRating = pointCastRaysContainer;
    }


    public PointCastRaysContainer GetRaycastsUsedForGeneratingRating()
    {
        return raycastsUsedForGeneratingRating;
    }
    #endregion

    #region Used By Game Loop

    public Vector3 GetPointPosition()
    {
        return transform.position;
    }

    public Vector3 GetStandingPosition()
    {
        return transform.position + transform.up * TacticalPointsManager.Instance.standingCoverHeight;
    }

    public Vector3 GetCrouchedPosition()
    {
        return transform.position + transform.up * TacticalPointsManager.Instance.crouchedCoverHeight;
    }


    // Only used if type is CoverShootPoint
    public Vector3 GetPeekPosition()
    {
        return transform.position;
    }

    public bool IsPointUsedByAnEntity()
    {
        return entityUsingThisPoint;
    }


    // Is there room on this point for another soldier?
    public bool IsPointUsedByAnotherEntity(GameEntity myEntity)
    {
        //change this algorythm somehow?
        if (entityUsingThisPoint)
        {
            if(entityUsingThisPoint != myEntity) return true;
        }

        return false;


        /*if(tacticalPointType == TacticalPointType.CoverPoint)
        {
            if (usingEntity)
            {
                return true;
            }
            else
            {
                for (int i = 0; i < coverPeekPoints.Length; i++)
                {
                    if (coverPeekPoints[i].IsPointFull())
                    {
                        return true;
                    }
                }

                return false;

            }
        }
        else if (tacticalPointType == TacticalPointType.CoverPeekPoint)
        {
            if (usingEntity)
            {
                return true;
            }
            else if (coverPointAssignedTo.usingEntity)
            {
                return true;
            }
            else
            {
                for (int i = 0; i < coverPointAssignedTo.coverPeekPoints.Length; i++)
                {
                    // to prevent stack overflow :)
                    if (coverPointAssignedTo.coverPeekPoints[i] != this)
                    {

                    }
                    if (coverPointAssignedTo.coverPeekPoints[i].IsPointFull())
                    {
                        return true;
                    }
                }

                return false;
            }
        }
        else
        {
            if (usingEntity)
            {
                return true;
            }
            else
            {
                return false;
            }
        }*/


    }

    public bool IsPointBeingTargetedByAnotherEntity(GameEntity askingEntity)
    {
        if (entityTargetingThisPoint)
        {
            if (entityTargetingThisPoint != askingEntity) return true;
        }

        return false;
    }

    public bool IsPointBeingTargetedByAnyEntity()
    {
        return entityTargetingThisPoint;
    }

   

    public void OnEntityEntersPoint(GameEntity entity)
    {
        entityUsingThisPoint = entity;

        //also set the other peek or cover points as used
        if(tacticalPointType == TacticalPointType.CoverPoint)
        {
            for (int i = 0; i < correspondingCoverPeekPoints.Length; i++)
            {
                correspondingCoverPeekPoints[i].entityUsingThisPoint = entityUsingThisPoint;
            }
        }
        else if(tacticalPointType == TacticalPointType.CoverPeekPoint)
        {
            if (correspondingCoverPoint == null) Debug.Log("tactical peek point: " + name + " has no coverPointAssignedTo assigned to it! -> fix dat!!!");
            correspondingCoverPoint.entityUsingThisPoint = entityUsingThisPoint;

            for (int i = 0; i < correspondingCoverPoint.correspondingCoverPeekPoints.Length; i++)
            {
                correspondingCoverPoint.correspondingCoverPeekPoints[i].entityUsingThisPoint = entityUsingThisPoint;
            }
        }
    }

    public void OnEntityExitsPoint(GameEntity entity)
    {
        if (entityUsingThisPoint == entity)
        {
            entityUsingThisPoint = null;

            if (tacticalPointType == TacticalPointType.CoverPoint)
            {
                for (int i = 0; i < correspondingCoverPeekPoints.Length; i++)
                {
                    correspondingCoverPeekPoints[i].entityUsingThisPoint = null;
                }
            }
            else if (tacticalPointType == TacticalPointType.CoverPeekPoint)
            {
                correspondingCoverPoint.entityUsingThisPoint = null;

                for (int i = 0; i < correspondingCoverPoint.correspondingCoverPeekPoints.Length; i++)
                {
                    correspondingCoverPoint.correspondingCoverPeekPoints[i].entityUsingThisPoint = null;
                }
            }
        }
    }

    public void OnEntityStartsTargetingThisPoint(GameEntity entity)
    {
        entityTargetingThisPoint = entity;

        //also set the other peek or cover points as used
        if (tacticalPointType == TacticalPointType.CoverPoint)
        {
            for (int i = 0; i < correspondingCoverPeekPoints.Length; i++)
            {
                correspondingCoverPeekPoints[i].entityTargetingThisPoint = entityTargetingThisPoint;
            }
        }
        else if (tacticalPointType == TacticalPointType.CoverPeekPoint)
        {
            if (correspondingCoverPoint == null) Debug.Log("tactical peek point: " + name + " has no coverPointAssignedTo assigned to it! -> fix dat!!!");
            correspondingCoverPoint.entityTargetingThisPoint = entityTargetingThisPoint;

            for (int i = 0; i < correspondingCoverPoint.correspondingCoverPeekPoints.Length; i++)
            {
                correspondingCoverPoint.correspondingCoverPeekPoints[i].entityTargetingThisPoint = entityTargetingThisPoint;
            }
        }
    }

    public void OnEntityStopsTargetingThisPoint(GameEntity entity)
    {
        if (entityTargetingThisPoint == entity)
        {
            entityTargetingThisPoint = null;

            if (tacticalPointType == TacticalPointType.CoverPoint)
            {
                for (int i = 0; i < correspondingCoverPeekPoints.Length; i++)
                {
                    correspondingCoverPeekPoints[i].entityTargetingThisPoint = null;
                }
            }
            else if (tacticalPointType == TacticalPointType.CoverPeekPoint)
            {
                correspondingCoverPoint.entityTargetingThisPoint = null;

                for (int i = 0; i < correspondingCoverPoint.correspondingCoverPeekPoints.Length; i++)
                {
                    correspondingCoverPoint.correspondingCoverPeekPoints[i].entityTargetingThisPoint = null;
                }
            }
        }
    }


    #region For Evaluation Cover Quality 


   /* public float DetermineQualityOfCover(QualityOfCoverEvaluationType evaluationType, (Vector3 threatPosition, float distanceToThreat)[] threats, (Vector3 friendlyPosition, float distanceToFriendly)[] friendlies)
    {
        //Prepare Values
        float endResult = 0;

        float[] usedCoverDistanceRating;
        float[] usedCoverQualityRating;
        if (type == Type.Crouched)
        {
            usedCoverDistanceRating = coverRating.crouchedDistanceRating;
            usedCoverQualityRating = coverRating.crouchedQualityRating;
        }
        else
        {
            usedCoverDistanceRating = coverRating.standingDistanceRating;
            usedCoverQualityRating = coverRating.standingQualityRating;
        }

        if (evaluationType == QualityOfCoverEvaluationType.Defensive2)
        {
            //Defensive evaluation: the less enemies in sight, the better + friendlyVisibilityBonus

            float threatsRating = RateThreatsDefensively(threats, usedCoverDistanceRating, usedCoverQualityRating);
            float friendlyVisibilityRating = CalculateFriendliesVisibilityRating(friendlies, usedCoverDistanceRating);

            //Debug.Log("threatsRating: " + threatsRating);
            //Debug.Log("friendlyVisibilityRating: " + friendlyVisibilityRating);
            endResult = threatsRating * 0.8f + friendlyVisibilityRating * 0.2f;
        }
        else if (evaluationType == QualityOfCoverEvaluationType.Moderate2)
        {
            float threatsRating = RateThreatsModerately(threats,usedCoverDistanceRating);
            float friendlyVisibilityRating = CalculateFriendliesVisibilityRating(friendlies, usedCoverDistanceRating);


            endResult = threatsRating * 0.8f + friendlyVisibilityRating * 0.2f;

        }
        else if (evaluationType == QualityOfCoverEvaluationType.Agressive2)
        {
            //Agressive evaluation: the more enemies in sight, the better + friendlyVisibilityBonus


            float threatsRating = RateThreatsAgressively(threats, usedCoverDistanceRating);
            float friendlyVisibilityRating = CalculateFriendliesVisibilityRating(friendlies, usedCoverDistanceRating);

            // Debug.Log("threatsRating: " + threatsRating);
            // Debug.Log("friendlyVisibilityRating: " + friendlyVisibilityRating);
            endResult = threatsRating * 0.85f + friendlyVisibilityRating * 0.15f;
        }

        //Debug.Log("endResult: " + endResult);
        return endResult;
    }

    float RateThreatsAgressively((Vector3 threatPosition, float distanceToThreat)[] threats, float[] usedCoverDistanceRating)
    {
        //the more enemies are not behind cover and visible to be shot, the better
        float rating = 0;
        for (int i = 0; i < threats.Length; i++)
        {
            Vector3 directionTowardsThreat = threats[i].threatPosition - GetPointPosition();
            int index = MapWorldSpaceDirectionToCoverRatingDirectionIndex(directionTowardsThreat);

            if (threats[i].distanceToThreat < usedCoverDistanceRating[index])
            {
                rating += 1;
            }
        }

        //Debug.Log("rate threats agressively rating: " + (rating / (threats.Length * 1f)));
        if (threats.Length > 0)
        {
            rating = rating / (threats.Length * 1f);
        }
        return rating;
    }

    float RateThreatsDefensively((Vector3 threatPosition, float distanceToThreat)[] threats, float[] usedCoverDistanceRating, float[] usedCoverQualityRating)
    {
        //the more enemies are behind cover, the better
        float rating = 0;

        for (int i = 0; i < threats.Length; i++)
        {
            Vector3 directionTowardsThreat = threats[i].threatPosition - GetPointPosition();
            int index = MapWorldSpaceDirectionToCoverRatingDirectionIndex(directionTowardsThreat);

            if (threats[i].distanceToThreat > usedCoverDistanceRating[index])
            {
                float modifiedQuality = usedCoverQualityRating[index]; //we rate the quality a bit higher, to reduce fluctuation
                modifiedQuality += (1 - modifiedQuality) * 0.5f;
                rating += modifiedQuality;
            }
        }

        if (threats.Length > 0)
        {
            rating = rating / (threats.Length * 1f);
        }
        return rating;
    }

    float RateThreatsModerately((Vector3 threatPosition, float distanceToThreat)[] threats, float[] usedCoverDistanceRating)
    {
        //ideally 1-2 enemies are visible
        float rating = 0;

        float threatsBehindCover = 0;
        float threatsNotBehindCover = 0;

        for (int i = 0; i < threats.Length; i++)
        {
            Vector3 directionTowardsThreat = threats[i].threatPosition - GetPointPosition();
            int index = MapWorldSpaceDirectionToCoverRatingDirectionIndex(directionTowardsThreat);

            //crouched Rating
            if (threats[i].distanceToThreat > usedCoverDistanceRating[index])
            {
                // coverBetweenPointAndThreat = true;
                threatsBehindCover += 1;
            }
            else
            {
                threatsNotBehindCover += 1;
            }
        }

        if (threatsNotBehindCover > 0.9f)
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

    float CalculateFriendliesVisibilityRating((Vector3 friendlyPosition, float distanceToFriendly)[] friendlies, float[] usedCoverDistanceRating)
    {
        float rating = 0;

        for (int i = 0; i < friendlies.Length; i++)
        {
            Vector3 directionTowardsFriendly = friendlies[i].friendlyPosition - GetPointPosition();
            int index = MapWorldSpaceDirectionToCoverRatingDirectionIndex(directionTowardsFriendly);

            if (friendlies[i].distanceToFriendly < usedCoverDistanceRating[index])
            {
                rating += 1;
            }
        }

        if (friendlies.Length > 0)
        {
            rating = rating / (friendlies.Length * 1f);
        }
        return rating;
    }*/



    //simpler method
    /*public float DetermineQualityOfCoverSimple(Vector3 meanThreatDirection, Vector3 closestEnemyPosition)
    {
        UnityEngine.Profiling.Profiler.BeginSample("DetermineQualityOfCoverSimple");

        float rating = 0;

        if (meanThreatDirection == Vector3.zero) return 0;


        if (tacticalPointType == TacticalPointType.CoverPoint)
        {
            (float distance, float quality) ratingForDirection = GetRatingForDirection(meanThreatDirection);

            if (ratingForDirection.distance < 2)
            {
                if (ratingForDirection.quality > 0.5f) rating = 1;
                else rating = ratingForDirection.quality;
            }
            else
            {
                rating = 0;
            }
        }
        else if(tacticalPointType == TacticalPointType.CoverPeekPoint)
        {
            if (closestEnemyPosition == Vector3.zero) return 0;

            //get rating for the corresponding cover point + the actual peek point we are rating
            (float distance, float quality) ratingForDirectionInCorrespondingCoverPoint = coverPointAssignedTo.GetRatingForDirection(meanThreatDirection);
            (float distance, float quality) ratingForDirection = GetRatingForDirection(meanThreatDirection);

            //corresponding cover point is weighted 0.3
            if (ratingForDirectionInCorrespondingCoverPoint.distance < 1.5)
            {
                rating += 0.3f * ratingForDirectionInCorrespondingCoverPoint.quality;
            }

            //the actual peek point - weighted 0.7
            float distanceToEnemyFromPoint = Vector3.Distance(closestEnemyPosition, transform.position);
            //instead of calculating this distance - use distance to point + measured distance to enemy combined?


            if(distanceToEnemyFromPoint< ratingForDirection.distance)
            {
                rating += 0.7f;
            }
            UnityEngine.Profiling.Profiler.EndSample();

        }


        return rating;
    }*/

    public (float distance, float quality) GetRatingForDirection(Vector3 direction)
    {
        //UnityEngine.Profiling.Profiler.BeginSample("Rate Cover Points: GetRatingForDirection");

        int directionIndex = MapWorldSpaceDirectionToCoverRatingDirectionIndex(direction);

        //UnityEngine.Profiling.Profiler.EndSample();

        if (type == Type.Crouched)
        {
            return (coverRating.crouchedDistanceRating[directionIndex], coverRating.crouchedQualityRating[directionIndex]);
        }
        else
        {
            return (coverRating.standingDistanceRating[directionIndex], coverRating.standingQualityRating[directionIndex]);
        }
    }

    int MapWorldSpaceDirectionToCoverRatingDirectionIndex(Vector3 worldSpaceDirection)
    {
        worldSpaceDirection.y = 0;

        //float signedAngle = Vector3.SignedAngle(Vector3.forward, directionTowardsThreat, Vector3.up); //like this or better use the tactical points directions instead?
        float signedAngle = Vector3.SignedAngle(transform.forward, worldSpaceDirection, transform.up);

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


        return (int)(signedAngle / 45f);
    }




    #endregion

    #region For Evaluation Cover Quality Old

    /* public float DetermineQualityOfCoverOld(QualityOfCoverEvaluationType evaluationType, (Vector3 threatPosition, float distanceToThreat)[] threats, (Vector3 friendlyPosition, float distanceToFriendly)[] friendlies, bool crouching)
     {
         //Prepare Values
         float endResult = 0;
         float crouchedMultiplier;
         float standingMultiplier;

         if (evaluationType == QualityOfCoverEvaluationType.Defensive1)
         {
             //Defensive evaluation: the less enemies in sight, the better + friendlyVisibilityBonus

             //Always calculate for both standing & crocuhing, just the one not used right now, only gets half the pooints, or 0.33 vs 0.66 rating, or similar...
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

             float threatsRating = RateThreatsDefensivelyOld(threats, coverRating, crouchedMultiplier, standingMultiplier);
             float friendlyVisibilityRating = CalculateFriendliesVisibilityRatingOld(friendlies, coverRating, crouchedMultiplier, standingMultiplier);

             //Debug.Log("threatsRating: " + threatsRating);
             //Debug.Log("friendlyVisibilityRating: " + friendlyVisibilityRating);
             endResult = threatsRating * 0.75f + friendlyVisibilityRating * 0.25f;
         }
         else if (evaluationType == QualityOfCoverEvaluationType.Moderate1)
         {
             //Defensive evaluation: if cover point: the less enemies in sight, the better and for every shootPoint of the cover point 1-2 enemies are perfect 
             //When not cover point - 1-2 enemies visible are perfect
             // + friendlyVisibilityBonus

             float threatsRating = 0;
             float friendlyVisibilityRating = 0;
             float threatsRatingCoverShootPoints = 0;
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

             if (tacticalPointType == TacticalPointType.CoverPoint)
             {
                 //cover point is rated defensively, but only the crouched cover is rated or 66/33? again
                 //shot paoints are rated 50/50 cover crocuh according to moderate formula
                 //combine  cover points + shoot points 50/50

                 for (int j = 0; j < coverPeekPoints.Length; j++)
                 {
                     threatsRatingCoverShootPoints += RateThreatsModeratelyOld(threats, coverPeekPoints[j].coverRating, crouchedMultiplier, standingMultiplier);
                     friendlyVisibilityRatingCoverShootPoints += CalculateFriendliesVisibilityRatingOld(friendlies, coverPeekPoints[j].coverRating, crouchedMultiplier, standingMultiplier);
                 }
                 if (coverPeekPoints.Length > 0) //would return nan if divided by 0
                 {
                     threatsRatingCoverShootPoints = threatsRatingCoverShootPoints / (coverPeekPoints.Length * 1f);
                     friendlyVisibilityRatingCoverShootPoints = friendlyVisibilityRatingCoverShootPoints / (coverPeekPoints.Length * 1f);
                 }


                 threatsRating = RateThreatsDefensivelyOld(threats, coverRating, crouchedMultiplier, standingMultiplier) * 0.5f + threatsRatingCoverShootPoints * 0.5f;
                 friendlyVisibilityRating = CalculateFriendliesVisibilityRatingOld(friendlies, coverRating, crouchedMultiplier, standingMultiplier) * 0.5f + friendlyVisibilityRatingCoverShootPoints * 0.5f;

             }
             else
             {
                 threatsRating = RateThreatsModeratelyOld(threats, coverRating, crouchedMultiplier, standingMultiplier);
                 friendlyVisibilityRating = CalculateFriendliesVisibilityRatingOld(friendlies, coverRating, crouchedMultiplier, standingMultiplier);
             }
            // Debug.Log("threatsRating: " + threatsRating);
            // Debug.Log("friendlyVisibilityRating: " + friendlyVisibilityRating);

             endResult = threatsRating * 0.75f + friendlyVisibilityRating * 0.25f;

         }
         else if (evaluationType == QualityOfCoverEvaluationType.Agressive1)
         {
             //Agressive evaluation: the more enemies in sight, the better + friendlyVisibilityBonus

             float threatsRating = 0;
             float friendlyVisibilityRating = 0;

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


             if (tacticalPointType == TacticalPointType.CoverPoint)
             {

                 for (int j = 0; j < coverPeekPoints.Length; j++)
                 {
                     threatsRating += RateThreatsAgressivelyOld(threats, coverPeekPoints[j].coverRating, crouchedMultiplier, standingMultiplier);
                     friendlyVisibilityRating += CalculateFriendliesVisibilityRatingOld(friendlies, coverPeekPoints[j].coverRating, crouchedMultiplier, standingMultiplier);
                 }
                 if (coverPeekPoints.Length > 0)
                 {
                     //Dont divide threatsRating, cause if all of them together have rating 1, its good enough.
                     if (threatsRating > 1) threatsRating = 1;
                     friendlyVisibilityRating = friendlyVisibilityRating / (coverPeekPoints.Length * 1f);
                 }       
             }
             else
             {
                 threatsRating = RateThreatsAgressivelyOld(threats, coverRating, crouchedMultiplier, standingMultiplier);
                 friendlyVisibilityRating = CalculateFriendliesVisibilityRatingOld(friendlies, coverRating, crouchedMultiplier, standingMultiplier);
             }

            // Debug.Log("threatsRating: " + threatsRating);
            // Debug.Log("friendlyVisibilityRating: " + friendlyVisibilityRating);
             endResult = threatsRating * 0.85f + friendlyVisibilityRating * 0.15f;
         }

         //Debug.Log("endResult: " + endResult);
         return endResult;
     }



     float RateThreatsAgressivelyOld((Vector3 threatPosition, float distanceToThreat)[] threats, PointCoverRating coverRating, float crouchedMultiplier, float standingMultiplier)
     {
         //the more enemies are not behind cover and visible to be shot, the better
         float rating = 0;
         //Debug.Log("rate threats agressively----------------------------------");
         for (int i = 0; i < threats.Length; i++)
         {
             Vector3 directionTowardsThreat = threats[i].threatPosition - GetPointPosition();
             int index = MapWorldSpaceDirectionToCoverRatingDirectionIndex(directionTowardsThreat);

             //crouched Rating
             if (threats[i].distanceToThreat < coverRating.crouchedDistanceRating[index])
             {
                 rating += 1 * crouchedMultiplier;
                //Debug.Log("rating += " + 1 * crouchedMultiplier);
             }

             //standing Rating
             if (threats[i].distanceToThreat < coverRating.standingDistanceRating[index])
             {
                 rating += 1 * standingMultiplier;
                 //Debug.Log("rating += " + 1 * standingMultiplier);

             }
         }

         //Debug.Log("rate threats agressively rating: " + (rating / (threats.Length * 1f)));
         if (threats.Length > 0)
         {
             rating = rating / (threats.Length * 1f);
         }
         return rating;
     }

     float RateThreatsDefensivelyOld((Vector3 threatPosition, float distanceToThreat)[] threats, PointCoverRating coverRating, float crouchedMultiplier, float standingMultiplier)
     {
         //the more enemies are behind cover, the better
         float rating = 0;



         for (int i = 0; i < threats.Length; i++)
         {
             Vector3 directionTowardsThreat = threats[i].threatPosition - GetPointPosition();
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

         if (threats.Length > 0)
         {
             rating = rating / (threats.Length * 1f);
         }
         return rating;
     }

     float RateThreatsModeratelyOld((Vector3 threatPosition, float distanceToThreat)[] threats, PointCoverRating coverRating, float crouchedMultiplier, float standingMultiplier)
     {
         //ideally 1-2 enemies are visible
         float rating = 0;

         float threatsBehindCover = 0;
         float threatsNotBehindCover = 0;

         for (int i = 0; i < threats.Length; i++)
         {
             Vector3 directionTowardsThreat = threats[i].threatPosition - GetPointPosition();
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
             }

             //standing Rating
             if (threats[i].distanceToThreat > coverRating.standingDistanceRating[index])
             {
                 // coverBetweenPointAndThreat = true;
                 threatsBehindCover += 1 * standingMultiplier;
             }
             else
             {
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

     float CalculateFriendliesVisibilityRatingOld((Vector3 friendlyPosition, float distanceToFriendly)[] friendlies, PointCoverRating coverRating, float crouchedMultiplier, float standingMultiplier)
     {
         float rating = 0;

         for (int i = 0; i < friendlies.Length; i++)
         {
             Vector3 directionTowardsFriendly = friendlies[i].friendlyPosition - GetPointPosition();
             int index = MapWorldSpaceDirectionToCoverRatingDirectionIndex(directionTowardsFriendly);

             //crouched Rating
             if (friendlies[i].distanceToFriendly < coverRating.crouchedDistanceRating[index])
             {
                 // coverBetweenPointAndThreat = true;
                 //minusPoints += coverRating.crouchedQualityRating[index] * crouchedMultiplier * 0.5f;
                 //Debug.Log("friendly crouching + " + crouchedMultiplier);

                 rating += 1 * crouchedMultiplier;
             }

             //standing Rating
             if (friendlies[i].distanceToFriendly < coverRating.standingDistanceRating[index])
             {
                 // coverBetweenPointAndThreat = true;
                 //minusPoints += coverRating.standingQualityRating[index] * standingMultiplier * 0.5f;
                 //Debug.Log("friendly standing + " + standingMultiplier);
                 rating += 1 * standingMultiplier;
             }
         }

         if (friendlies.Length > 0)
         {
             rating = rating / (friendlies.Length * 1f);
         }
         return rating;
     }*/

    #endregion

    #endregion

}
