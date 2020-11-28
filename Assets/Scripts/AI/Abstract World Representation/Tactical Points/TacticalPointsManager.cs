﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class TacticalPointsManager : MonoBehaviour
{
    public GameObject openFieldPointPrefab;
    public float standingCoverHeight;
    public float crouchedCoverHeight;
    public float maxSnapDistanceToNavmesh;
    public int distanceRaycastsPerPoint;
    public int qualityRaycastsPerPoint;

    public HashSet<TacticalPointsGeneratorBox> tacticalPointGenerators = new HashSet<TacticalPointsGeneratorBox>();
    public HashSet<TacticalPoint> tacticalPoints = new HashSet<TacticalPoint>();


    public static TacticalPointsManager Instance;

    void OnEnable()   //switched it to OnEnable, cause it also triggers in EditMode unlike Awake
    {
        Instance = this;
    }

    public void AddTacticalPointsGeneratorBox(TacticalPointsGeneratorBox generator)
    {
        tacticalPointGenerators.Add(generator);
    }

    public void RemoveTacticalPointsGeneratorBox(TacticalPointsGeneratorBox generator)
    {
        tacticalPointGenerators.Remove(generator);
    }

    public void AddTacticalPoint(TacticalPoint point)
    {
        tacticalPoints.Add(point);
    }

    public void RemoveTacticalPoint(TacticalPoint point)
    {
        tacticalPoints.Remove(point);
    }

    public void GenerateAll()
    {
        foreach (TacticalPointsGeneratorBox generator in tacticalPointGenerators)
        {
            generator.Generate();
        }
    }

    public void BakeAllCoverDistanceRatings()
    {
        foreach (TacticalPoint point in tacticalPoints)
        {
            point.BakeDistanceCoverRating(crouchedCoverHeight, standingCoverHeight, distanceRaycastsPerPoint);
        }
    }

    public void BakeAllCoverQualityRatings()
    {
        foreach (TacticalPoint point in tacticalPoints)
        {
            point.BakeQualityCoverRating(crouchedCoverHeight, standingCoverHeight, qualityRaycastsPerPoint);
        }
    }
}
