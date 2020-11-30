using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    public PointCoverRating coverRating;

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

    class UsedRaycast
    {
        public Vector3 start;
        public Vector3 end;
        public float distance;

        public UsedRaycast(Vector3 start, Vector3 end, bool infinity = false)
        {
            this.start = start;
            this.end = end;

            if (infinity)
            {
                distance = Mathf.Infinity;
            }
            else
            {
                distance = Vector3.Distance(end, start);
            }
        }
    }
    HashSet<UsedRaycast> raycastsUsedForGeneratingRating = new HashSet<UsedRaycast>();
    HashSet<UsedRaycast> raycastsUsedForCurrentDirection = new HashSet<UsedRaycast>();


    #region Update Cover Shoot Points inside Editor

#if UNITY_EDITOR

    void Update()
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

    public void BakeCoverRatings(float crouchedHeight, float standingHeight, float raycastFactor, LayerMask raycastLayerMask)
    {
        Debug.Log("Bake Distance");

        //2. cast and save Raycasts
        raycastsUsedForGeneratingRating.Clear();

        //float randomiseRange = 22.5f;

        for (int i = 0; i < 8; i++)
        {

            raycastsUsedForCurrentDirection.Clear();

            Vector3 rayStartPoint = transform.position + new Vector3(0, standingHeight, 0) + Random.insideUnitSphere * radius;

            //Dont randomise the direction -  use a grid instead
            float directionGridSize = 45 / raycastFactor;
            for (float x = -22.5f; x < 22.5; x = x + directionGridSize)
            {
                for (float y = -22.5f; y < 22.5; y = y + directionGridSize)
                {
                    // take a forward transform, rotate it by the grids y and x offset, later roatte it around y axies - the quaternion order of application is inverse (right to left)
                    Vector3 rotatedRaycastDirectionInLocalSpace = Quaternion.AngleAxis(i * 45, transform.up) * Quaternion.Euler(x + directionGridSize / 2, y + directionGridSize / 2, 0f) * Vector3.forward;

                    //this could be left out if the point wouldn not be aligned to the world forward
                    Vector3 raycastDirectionInWorldSpace = transform.TransformDirection(rotatedRaycastDirectionInLocalSpace);

                    RaycastHit hit;
                    if (Physics.Raycast(rayStartPoint, raycastDirectionInWorldSpace, out hit, Mathf.Infinity, raycastLayerMask, QueryTriggerInteraction.Ignore))
                    {
                        raycastsUsedForGeneratingRating.Add(new UsedRaycast(rayStartPoint, hit.point));
                        raycastsUsedForCurrentDirection.Add(new UsedRaycast(rayStartPoint, hit.point));
                    }
                    else
                    {
                        raycastsUsedForGeneratingRating.Add(new UsedRaycast(rayStartPoint, rayStartPoint + raycastDirectionInWorldSpace * 100, true));

                    }
                }
            }



            float allDistancesCombined = 0;
            //go through all the ratings and add the mean value to the cuirrent rating
            foreach (UsedRaycast raycast in raycastsUsedForCurrentDirection)
            {
                allDistancesCombined += raycast.distance;
            }

            float meanDistance;
            if (raycastsUsedForCurrentDirection.Count > 0)
            {
                meanDistance = allDistancesCombined / raycastsUsedForCurrentDirection.Count;
            }
            else
            {
                meanDistance = Mathf.Infinity;
            }
            // Debug.Log("i: " + i + " raycastsUsedForCurrentDirection.Count: " + raycastsUsedForCurrentDirection.Count);
            // Debug.Log("i: " + i + " mean distance: " + meanDistance);

            coverRating.standingDistanceRating[i] = meanDistance;


            //Calculate cover Quality: 
            //Average absolute deviation contributes towards the quality
            float allDeviationsCombined = 0;
            foreach (UsedRaycast raycast in raycastsUsedForCurrentDirection)
            {
                allDeviationsCombined += Mathf.Abs(raycast.distance - meanDistance);
                // Debug.Log("i: " + i + " deviation added : " + Mathf.Abs(raycast.distance - meanDistance));
                //Debug.Log("i: " + i + " deviation after adding: : " + allDeviationsCombined);

            }
            //Debug.Log("i: " + i + " raycastsUsedForCurrentDirection.Count: " + raycastsUsedForCurrentDirection.Count);
            //Debug.Log("i: " + i + " allDeviationsCombined: " + allDeviationsCombined);

            float averageAbsoluteDeviation;
            if (raycastsUsedForCurrentDirection.Count > 0)
            {
                averageAbsoluteDeviation = allDeviationsCombined / raycastsUsedForCurrentDirection.Count;
            }
            else
            {
                averageAbsoluteDeviation = Mathf.Infinity;
            }
            // Debug.Log("i: " + i + " averageAbsoluteDeviation: " + averageAbsoluteDeviation);

            coverRating.standingQualityRating[i] = averageAbsoluteDeviation;
            //try it once with mittlere Abweichung and once with Median der Abweichungen

        }
    }

    private void OnDrawGizmos()
    {
        //Debug.Log("Gizmos: -------  raycastsUsedForGeneratingRating.Count: " + raycastsUsedForGeneratingRating.Count);

        foreach (UsedRaycast item in raycastsUsedForGeneratingRating)
        {
            Gizmos.DrawLine(item.start, item.end);
        }
    }


}
