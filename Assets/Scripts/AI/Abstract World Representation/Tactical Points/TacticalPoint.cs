using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

//using UnityEngine.EventSystems;
//using UnityEngine.Events;


public enum TacticalPointType
{
    OpenFieldPoint,
    CoverPoint,
    CoverShootPoint

}

[ExecuteInEditMode]
public class TacticalPoint : MonoBehaviour
{
    public TacticalPointType tacticalPointType;
    public GameEntity usingEntity;
    [Space(5)]
    [System.NonSerialized] public PointCoverRating coverRating;

    [Tooltip("only used if  type is CoverShootPoint")]
    [ShowWhen("tacticalPointType", TacticalPointType.CoverPoint)]
    public TacticalPoint[] coverShootPoints; //or ShotPositions
    [ShowWhen("tacticalPointType", TacticalPointType.CoverPoint)]
    public Transform coverShootPointsParent;


    [Space(5)]
    //for now we only use this to test the character controller
    public int stanceType; //0 is standing, 1 is crouching

    public float radius;
    public int capacity;

    public Renderer pointRenderer;



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


    [System.NonSerialized] public PointCastRaysContainer raycastsUsedForGeneratingRating;

   // public UnityEvent OnUpdateRating;

    //[HideInInspector]
    [SerializeField] int pointReferenceID; //used as reference in the serialized data of the scriptable object the manager communicates to.


    #region Update Cover Shoot Points inside Editor

#if UNITY_EDITOR

    /*void Update()
    {
        if (!Application.isPlaying)
        {
            if (tacticalPointType == TacticalPointType.CoverPoint)
            {
                coverShootPoints = new TacticalPoint[coverShootPointsParent.childCount];
                for (int i = 0; i < coverShootPointsParent.childCount; i++)
                {
                    TacticalPoint tp = coverShootPointsParent.GetChild(i).GetComponent<TacticalPoint>();
                    if (tp)
                    {
                        coverShootPoints[i] = tp;
                    }
                    else
                    {
                        Debug.Log("children of coverShootPointsParent on: " + gameObject.name + " is not a tactical point -> fix this");
                    }
                }
            }
        }

    }*/

    public void SetPointReferenceID(int id)
    {
        pointReferenceID = id;
        EditorUtility.SetDirty(this);

    }
    public int GetPointReferenceID()
    {
        return pointReferenceID;
    }


    public void UpdateCoverShootPoints()
    {
        if (tacticalPointType == TacticalPointType.CoverPoint)
        {
            coverShootPoints = new TacticalPoint[coverShootPointsParent.childCount];
            for (int i = 0; i < coverShootPointsParent.childCount; i++)
            {
                TacticalPoint tp = coverShootPointsParent.GetChild(i).GetComponent<TacticalPoint>();
                if (tp)
                {
                    coverShootPoints[i] = tp;
                }
                else
                {
                    Debug.Log("children of coverShootPointsParent on: " + gameObject.name + " is not a tactical point -> fix this");
                }
            }
        }
    }

#endif

    #endregion

    public Vector3 GetPostPosition()
    {
        return transform.position;
    }


    // Only used if type is CoverShootPoint
    public Vector3 GetPeekPosition()
    {
        return transform.position;
    }

    // Is there room on this point for another soldier?
    public bool IsPointFull()
    {
        if (usingEntity)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public void OnEntityEntersPoint(GameEntity entity)
    {
        usingEntity = entity;
    }

    public void OnEntityExitsPoint(GameEntity entity)
    {
        if (usingEntity == entity)
        {
            usingEntity = null;
        }
    }

    void OnEnable()
    {
        TacticalPointsManager.Instance.AddTacticalPoint(this);
    }

    void OnDisable()
    {
        TacticalPointsManager.Instance.RemoveTacticalPoint(this);
    }

    public (PointCoverRating pointCoverRating, PointCastRaysContainer pointCastRaysContainer) BakeCoverRatings(ref PointCoverRating pointCoverRating, ref PointCastRaysContainer pointCastRaysContainer, float crouchedHeight, float standingHeight, int raycastFactor, LayerMask raycastLayerMask, float maxRayLength)
    {
        //PointCoverRating pointCoverRating = new PointCoverRating();
        //PointCastRaysContainer pointCastRaysContainer = new PointCastRaysContainer();


        int numberOfRaycastsPerDirection = raycastFactor * raycastFactor;

        //raycastsUsedForGeneratingRating.SetUpRays(numberOfRaycastsPerDirection);
        pointCastRaysContainer.SetUpRays(numberOfRaycastsPerDirection);


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
                            //raycastsUsedForGeneratingRating.SetRay(p, i, r, new RaycastUsedToGenerateCoverRating(rayStartPoint, hit.point));
                            pointCastRaysContainer.SetRay(p, i, r, new RaycastUsedToGenerateCoverRating(rayStartPoint, hit.point));
                        }
                        else
                        {
                            //raycastsUsedForGeneratingRating.SetRay(p, i, r, new RaycastUsedToGenerateCoverRating(rayStartPoint, rayStartPoint + raycastDirectionInWorldSpace * distanceValueForInfinity, true));
                            //raycastsUsedForGeneratingRating.SetRay(p, i, r, new RaycastUsedToGenerateCoverRating(rayStartPoint, rayStartPoint + raycastDirectionInWorldSpace * distanceValueForInfinity));

                            //raycastsUsedForGeneratingRating.SetRay(p, i, r, new RaycastUsedToGenerateCoverRating(rayStartPoint, rayStartPoint + raycastDirectionInWorldSpace * 100, true));
                            pointCastRaysContainer.SetRay(p, i, r, new RaycastUsedToGenerateCoverRating(rayStartPoint, rayStartPoint + raycastDirectionInWorldSpace * 100, true));

                        }
                        r++;
                    }
                }

                #endregion

                #region Calculate the meanDistance -> DistanceRating

                float meanDistance;
                float allDistancesCombined = 0;
                int numberOfRaycastsWhichAreNotInfinite = 0;
                //go through all the ratings and add the mean value to the cuirrent rating

                for (int n = 0; n < numberOfRaycastsPerDirection; n++)
                {
                     //if (!raycastsUsedForGeneratingRating.GetRay(p,i,n).IsInfinite())
                     if (!pointCastRaysContainer.GetRay(p,i,n).IsInfinite())
                     {
                         numberOfRaycastsWhichAreNotInfinite++;
                         //allDistancesCombined += raycastsUsedForGeneratingRating.GetRay(p, i, n).distance;
                         allDistancesCombined += pointCastRaysContainer.GetRay(p, i, n).distance;
                     }
                    //allDistancesCombined += raycastsUsedForGeneratingRating.GetRay(p, i, n).distance;

                }

                //meanDistance = allDistancesCombined / numberOfRaycastsPerDirection;

                if (numberOfRaycastsWhichAreNotInfinite == 0)
                {
                    meanDistance = Mathf.Infinity;
                }
                else
                {
                    meanDistance = allDistancesCombined / numberOfRaycastsWhichAreNotInfinite;
                }

                #endregion

                #region Calculate the average absolute deviation -> QualityRating

                float averageAbsoluteDeviation;
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

                float qualityRating = Utility.Remap(averageAbsoluteDeviation,maxRayLength,0, 0, 1);

                #endregion

                //TODO try it once with mittlere Abweichung and once with Median der Abweichungen

                #region Set the Rating

                if (p == 0)
                {
                    //coverRating.crouchedDistanceRating[i] = meanDistance;
                    pointCoverRating.crouchedDistanceRating[i] = meanDistance;
                    //coverRating.crouchedQualityRating[i] = averageAbsoluteDeviation;
                    //coverRating.crouchedQualityRating[i] = qualityRating;
                    pointCoverRating.crouchedQualityRating[i] = qualityRating;
                }
                else if (p == 1)
                {
                    // coverRating.standingDistanceRating[i] = meanDistance;
                    pointCoverRating.standingDistanceRating[i] = meanDistance;
                    //coverRating.standingQualityRating[i] = averageAbsoluteDeviation;
                    //coverRating.standingQualityRating[i] = qualityRating;
                    pointCoverRating.standingQualityRating[i] = qualityRating;
                }

                #endregion
            }
        }

        //EditorUtility.SetDirty(this);
        

        return (pointCoverRating, pointCastRaysContainer);
    }

    public void UpdateRatings(PointCoverRating pointCoverRating, PointCastRaysContainer pointCastRaysContainer)
    {
        Debug.Log("Tactical Point  UpdatePointRatings");

        coverRating = pointCoverRating;
        raycastsUsedForGeneratingRating = pointCastRaysContainer;

        //OnUpdateRating.Invoke();
    }

    public PointCastRaysContainer GetRaycastsUsedForGeneratingRating()
    {
        return raycastsUsedForGeneratingRating;
    }

    public void ResetRotation()
    {
        if(tacticalPointType == TacticalPointType.CoverPoint)
        {
            //save old positions
            Vector3[] oldPositions = new Vector3[coverShootPoints.Length];
            for (int i = 0; i < oldPositions.Length; i++)
            {
                oldPositions[i] = coverShootPoints[i].transform.position;
            }

            //rotate parent
            transform.rotation = Quaternion.identity;

            //restore old posiitons
            for (int i = 0; i < oldPositions.Length; i++)
            {
                coverShootPoints[i].transform.position = oldPositions[i];
                coverShootPoints[i].transform.rotation = Quaternion.identity;
            }
        }
        else
        {
            transform.rotation = Quaternion.identity;
        }
    }

}
