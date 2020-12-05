using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


public enum TacticalPointType
{
    OpenFieldPoint,
    CoverPoint,
    CoverShootPoint
}

[ExecuteInEditMode]
public class TacticalPoint : MonoBehaviour
{
    #region Fields 

    public TacticalPointType tacticalPointType;
    public GameEntity usingEntity;

    [Tooltip("only used if  type is CoverShootPoint")]
    [ShowWhen("tacticalPointType", TacticalPointType.CoverPoint)]
    public TacticalPoint[] coverShootPoints; //or ShotPositions
    [ShowWhen("tacticalPointType", TacticalPointType.CoverPoint)]
    public Transform coverShootPointsParent;

    public float radius;
    public int capacity;

    public Renderer pointRenderer;

    [Space(5)]
    //for now we only use this to test the character controller
    public int stanceType; //0 is standing, 1 is crouching


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

    public void UpdateCoverShootPoints()
    {
        //Updates the references of children instantiated in editor by the level Designer
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

    public void ResetRotation()
    {
        if (tacticalPointType == TacticalPointType.CoverPoint)
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

    #endregion

    #region Used For Baking & Setting Up Ratings

    public void SetPointReferenceID(int id)
    {
        pointReferenceID = id;
        EditorUtility.SetDirty(this);

    }
    public int GetPointReferenceID()
    {
        return pointReferenceID;
    }

    public (PointCoverRating pointCoverRating, PointCastRaysContainer pointCastRaysContainer) BakeCoverRatings(ref PointCoverRating pointCoverRating, ref PointCastRaysContainer pointCastRaysContainer, float crouchedHeight, float standingHeight, int raycastFactor, LayerMask raycastLayerMask, float maxRayLength)
    {
        int numberOfRaycastsPerDirection = raycastFactor * raycastFactor;
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

                #region Calculate the meanDistance -> DistanceRating

                float meanDistance;
                float allDistancesCombined = 0;
                int numberOfRaycastsWhichAreNotInfinite = 0;

                for (int n = 0; n < numberOfRaycastsPerDirection; n++)
                {
                    if (!pointCastRaysContainer.GetRay(p, i, n).IsInfinite())
                    {
                        numberOfRaycastsWhichAreNotInfinite++;
                        allDistancesCombined += pointCastRaysContainer.GetRay(p, i, n).distance;
                    }

                }


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

                float qualityRating = Utility.Remap(averageAbsoluteDeviation, maxRayLength, 0, 0, 1);

                #endregion

                #region Set the Rating

                if (p == 0)
                {
                    pointCoverRating.crouchedDistanceRating[i] = meanDistance;
                    pointCoverRating.crouchedQualityRating[i] = qualityRating;
                }
                else if (p == 1)
                {
                    pointCoverRating.standingDistanceRating[i] = meanDistance;
                    pointCoverRating.standingQualityRating[i] = qualityRating;
                }

                #endregion
            }
        }

        return (pointCoverRating, pointCastRaysContainer);
    }

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

    #endregion

}
