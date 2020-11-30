﻿using System.Collections;
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

    public class UsedRaycast
    {
        public Vector3 start;
        public Vector3 end;
        public float distance;

        bool infinite;

        public UsedRaycast(Vector3 start, Vector3 end, bool infinite = false)
        {
            this.start = start;
            this.end = end;

            this.infinite = infinite;

            if (infinite)
            {
                distance = Mathf.Infinity;
            }
            else
            {
                distance = Vector3.Distance(end, start);
            }
        }

        public bool IsInfinite()
        {
            return infinite;
        }
    }

    //TODO rework the way raycasts are saved for better visualisation
    //this should be an array of 8 - if this would be the case we can leave out the next hashset? - no or yes- just have an infinity if statement in the mean calculation

    //public HashSet<UsedRaycast> raycastsUsedForGeneratingRating = new HashSet<UsedRaycast>(); 
    //HashSet<UsedRaycast> raycastsUsedForCurrentDirection = new HashSet<UsedRaycast>();
    [SerializeField] UsedRaycast[][][] raycastsUsedForGeneratingRating; //first array is for standing/crouching, second is for the 8 directions, third is for the raycast per directions


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

    public void BakeCoverRatings(float crouchedHeight, float standingHeight, int raycastFactor, LayerMask raycastLayerMask)
    {
        // Instantiate the array
        int numberOfRaycastsPerDirection = raycastFactor * raycastFactor;

        raycastsUsedForGeneratingRating = new UsedRaycast[2][][];
        for (int i = 0; i < 2; i++)
        {
            raycastsUsedForGeneratingRating[i] = new UsedRaycast[8][];
            for (int j = 0; j < 8; j++)
            {
                raycastsUsedForGeneratingRating[i][j] = new UsedRaycast[numberOfRaycastsPerDirection];
            }
        }
        

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
                        if (Physics.Raycast(rayStartPoint, raycastDirectionInWorldSpace, out hit, Mathf.Infinity, raycastLayerMask, QueryTriggerInteraction.Ignore))
                        {
                            raycastsUsedForGeneratingRating[p][i][r] = new UsedRaycast(rayStartPoint, hit.point);
                        }
                        else
                        {
                            raycastsUsedForGeneratingRating[p][i][r] = new UsedRaycast(rayStartPoint, rayStartPoint + raycastDirectionInWorldSpace * 100, true);
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

                for (int n = 0; n < raycastsUsedForGeneratingRating[p][i].Length; n++)
                {
                    if (!raycastsUsedForGeneratingRating[p][i][n].IsInfinite())
                    {
                        numberOfRaycastsWhichAreNotInfinite++;
                        allDistancesCombined += raycastsUsedForGeneratingRating[p][i][n].distance;
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

                for (int n = 0; n < raycastsUsedForGeneratingRating[p][i].Length; n++)
                {
                    if (!raycastsUsedForGeneratingRating[p][i][n].IsInfinite())
                    {
                        allDeviationsCombined += Mathf.Abs(raycastsUsedForGeneratingRating[p][i][n].distance - meanDistance);
                    }
                }

                if(numberOfRaycastsWhichAreNotInfinite == 0)
                {
                    averageAbsoluteDeviation = Mathf.Infinity;
                }
                else
                {
                    averageAbsoluteDeviation = allDeviationsCombined / numberOfRaycastsWhichAreNotInfinite;
                }

                #endregion

                //TODO try it once with mittlere Abweichung and once with Median der Abweichungen

                #region Set the Rating

                if (p == 0)
                {
                    coverRating.crouchedDistanceRating[i] = meanDistance;
                    coverRating.crouchedQualityRating[i] = averageAbsoluteDeviation;
                }
                else if (p == 1)
                {
                    coverRating.standingDistanceRating[i] = meanDistance;
                    coverRating.standingQualityRating[i] = averageAbsoluteDeviation;
                }

                #endregion
            }
        }

        //var so = new SerializedObject(coverRating);
        coverRating.ApplyModifiedProperties(); //? something like this?
    }

    public UsedRaycast[][][] GetRaycastsUsedForGeneratingRating()
    {
        return raycastsUsedForGeneratingRating;
    }

    private void OnDrawGizmos()
    {
        //Debug.Log("Gizmos: -------  raycastsUsedForGeneratingRating.Count: " + raycastsUsedForGeneratingRating.Count);

        /*if (raycastsUsedForGeneratingRating != null)
        {
            for (int i = 0; i < 2; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    foreach (UsedRaycast usedRaycast in raycastsUsedForGeneratingRating[i][j])
                    {
                        Gizmos.DrawLine(usedRaycast.start, usedRaycast.end);
                    }
                }
            }

        }*/
        
    }


}
